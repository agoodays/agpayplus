using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Payment.Api.Channel;
using AGooday.AgPay.Payment.Api.RQRS.Msg;

namespace AGooday.AgPay.Payment.Api.Services
{
    /// <summary>
    /// 业务： 支付订单分账处理逻辑
    /// </summary>
    public class PayOrderDivisionProcessService
    {
        private readonly ILogger<PayOrderDivisionProcessService> log;
        private readonly IMQSender mqSender;
        protected readonly Func<string, IDivisionService> _divisionServiceFactory;
        private readonly IPayOrderService payOrderService;
        private readonly IMchDivisionReceiverService mchDivisionReceiverService;
        private readonly IMchDivisionReceiverGroupService mchDivisionReceiverGroupService;
        private readonly IPayOrderDivisionRecordService payOrderDivisionRecordService;
        private readonly ConfigContextQueryService configContextQueryService;

        public PayOrderDivisionProcessService(ILogger<PayOrderDivisionProcessService> logger,
            IMQSender mqSender,
            IPayOrderService payOrderService,
            IMchDivisionReceiverService mchDivisionReceiverService,
            IMchDivisionReceiverGroupService mchDivisionReceiverGroupService,
            IPayOrderDivisionRecordService payOrderDivisionRecordService,
            ConfigContextQueryService configContextQueryService)
        {
            log = logger;
            this.mqSender = mqSender;
            this.payOrderService = payOrderService;
            this.mchDivisionReceiverService = mchDivisionReceiverService;
            this.mchDivisionReceiverGroupService = mchDivisionReceiverGroupService;
            this.payOrderDivisionRecordService = payOrderDivisionRecordService;
            this.configContextQueryService = configContextQueryService;
        }

        /// <summary>
        /// 处理分账，
        /// 1. 向外抛异常： 系统检查没有通过 / 系统级别异常
        /// 2 若正常调起接口将返回渠道侧响应结果
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <param name="useSysAutoDivisionReceivers"></param>
        /// <param name="receiverList"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        public ChannelRetMsg ProcessPayOrderDivision(string payOrderId, byte? useSysAutoDivisionReceivers, List<PayOrderDivisionMQ.CustomerDivisionReceiver> receiverList, bool? isResend)
        {
            // 是否重发分账接口（ 当分账失败， 列表允许再次发送请求 ）
            isResend ??= false;

            string logPrefix = $"订单[{payOrderId}]执行分账";

            //查询订单信息
            PayOrderDto payOrder = payOrderService.GetById(payOrderId);

            if (payOrder == null)
            {
                log.LogError($"{logPrefix}，订单不存在");
                throw new BizException("订单不存在");
            }

            // 订单不是成功状态 || 分账状态不正确
            if (payOrder.State != (byte)PayOrderState.STATE_SUCCESS || (payOrder.DivisionState != (byte)PayOrderDivision.DIVISION_STATE_WAIT_TASK && payOrder.DivisionState != (byte)PayOrderDivision.DIVISION_STATE_UNHAPPEN))
            {
                log.LogError($"{logPrefix}, 订单状态或分账状态不正确");
                throw new BizException("订单状态或分账状态不正确");
            }

            //更新订单为： 分账任务处理中
            payOrder.DivisionState = (byte)PayOrderDivision.DIVISION_STATE_ING;
            bool updPayOrder = payOrderService.Update(payOrder);
            if (!updPayOrder)
            {
                log.LogError($"{logPrefix}, 更新支付订单为分账处理中异常！");
                throw new BizException("更新支付订单为分账处理中异常");
            }
            // 所有的分账列表
            List<PayOrderDivisionRecordDto> recordList = null;

            // 重发通知，可直接查库
            if (isResend.Value)
            {
                // 根据payOrderId && 待分账（ 重试时将更新为待分账状态 ）  ， 此处不可查询出分账成功的订单。
                recordList = payOrderDivisionRecordService.GetByPayOrderId(payOrderId)
                    .Where(w => w.State == (byte)PayOrderDivisionRecordState.STATE_WAIT).ToList();
            }
            else
            {
                // 查询&过滤 所有的分账接收对象
                List<MchDivisionReceiverDto> allReceiver = this.QueryReceiver(useSysAutoDivisionReceivers, payOrder, receiverList);

                //得到全部分账比例 (所有待分账账号的分账比例总和)
                var allDivisionProfit = Decimal.Zero;
                foreach (MchDivisionReceiverDto receiver in allReceiver)
                {
                    allDivisionProfit = Decimal.Add(allDivisionProfit, receiver.DivisionProfit.Value);
                }

                //计算分账金额 = 商家实际入账金额
                long payOrderDivisionAmount = payOrderService.CalMchIncomeAmount(payOrder);

                //剩余待分账金额 (用作最后一个分账账号的 计算， 避免出现分账金额超出最大) [结果向下取整 ， 避免出现金额溢出的情况。 ]
                long subDivisionAmount = AmountUtil.CalPercentageFee(payOrderDivisionAmount, allDivisionProfit, MidpointRounding.ToNegativeInfinity);

                recordList = new List<PayOrderDivisionRecordDto>();

                //计算订单分账金额, 并插入到记录表

                string batchOrderId = SeqUtil.GenDivisionBatchId();

                foreach (MchDivisionReceiverDto receiver in allReceiver)
                {
                    PayOrderDivisionRecordDto record = GenRecord(batchOrderId, payOrder, receiver, payOrderDivisionAmount, subDivisionAmount);

                    //剩余金额
                    subDivisionAmount = subDivisionAmount - record.CalDivisionAmount;

                    //入库保存
                    payOrderDivisionRecordService.Add(record);
                    recordList.Add(record);
                }
            }

            ChannelRetMsg channelRetMsg = null;

            try
            {
                //调用渠道侧分账接口
                IDivisionService divisionService = _divisionServiceFactory(payOrder.IfCode);
                if (divisionService == null)
                {
                    throw new BizException("通道无此分账接口");
                }

                channelRetMsg = divisionService.SingleDivision(payOrder, recordList, configContextQueryService.QueryMchInfoAndAppInfo(payOrder.MchNo, payOrder.AppId));

                // 确认分账成功 ( 明确分账成功 )
                if (channelRetMsg.ChannelState == ChannelState.CONFIRM_SUCCESS)
                {
                    //分账成功
                    payOrderDivisionRecordService.UpdateRecordSuccessOrFail(recordList, (byte)PayOrderDivisionRecordState.STATE_SUCCESS, channelRetMsg.ChannelOrderId, channelRetMsg.ChannelOriginResponse);
                }
                // 分账失败 ( 明确分账成功 )
                else if (channelRetMsg.ChannelState == ChannelState.CONFIRM_FAIL)
                {
                    //分账失败
                    payOrderDivisionRecordService.UpdateRecordSuccessOrFail(recordList, (byte)PayOrderDivisionRecordState.STATE_FAIL, channelRetMsg.ChannelOrderId, channelRetMsg.ChannelErrMsg);
                }
                // 已受理
                else if (channelRetMsg.ChannelState == ChannelState.WAITING)
                {
                    payOrderDivisionRecordService.UpdateRecordSuccessOrFail(recordList, (byte)PayOrderDivisionRecordState.STATE_ACCEPT, channelRetMsg.ChannelOrderId, channelRetMsg.ChannelErrMsg);
                }
            }
            catch (Exception e)
            {
                log.LogError(e, $"{logPrefix}, 调用分账接口异常");
                payOrderDivisionRecordService.UpdateRecordSuccessOrFail(recordList, (byte)PayOrderDivisionRecordState.STATE_FAIL, null, $"系统异常：{e.Message}");

                channelRetMsg = ChannelRetMsg.ConfirmFail(null, null, e.Message);
            }

            //更新 支付订单主表状态  分账任务已结束。
            payOrder.DivisionState = (byte)PayOrderDivision.DIVISION_STATE_FINISH;
            payOrder.DivisionLastTime = DateTime.Now;
            if (payOrder.DivisionState.Equals((byte)PayOrderDivision.DIVISION_STATE_ING))
                payOrderService.Update(payOrder);
            return channelRetMsg;
        }
        /// <summary>
        /// 生成对象信息
        /// </summary>
        /// <param name="batchOrderId"></param>
        /// <param name="payOrder"></param>
        /// <param name="receiver"></param>
        /// <param name="payOrderDivisionAmount"></param>
        /// <param name="subDivisionAmount"></param>
        /// <returns></returns>
        private PayOrderDivisionRecordDto GenRecord(string batchOrderId, PayOrderDto payOrder, MchDivisionReceiverDto receiver, long payOrderDivisionAmount, long subDivisionAmount)
        {
            PayOrderDivisionRecordDto record = new PayOrderDivisionRecordDto();
            record.MchNo = payOrder.MchNo;
            record.IsvNo = payOrder.IsvNo;
            record.AppId = payOrder.AppId;
            record.MchName = payOrder.MchName;
            record.MchType = payOrder.MchType;
            record.IfCode = payOrder.IfCode;
            record.PayOrderId = payOrder.PayOrderId;
            record.PayOrderChannelOrderNo = payOrder.ChannelOrderNo; //支付订单渠道订单号
            record.PayOrderAmount = payOrder.Amount; //订单金额
            record.PayOrderDivisionAmount = payOrderDivisionAmount; // 订单计算分账金额
            record.BatchOrderId = batchOrderId; //系统分账批次号
            record.State = (byte)PayOrderDivisionRecordState.STATE_WAIT; //状态: 待分账
            record.ReceiverId = receiver.ReceiverId;
            record.ReceiverGroupId = receiver.ReceiverGroupId;
            record.ReceiverAlias = receiver.ReceiverAlias;
            record.AccType = receiver.AccType;
            record.AccNo = receiver.AccNo;
            record.AccName = receiver.AccName;
            record.RelationType = receiver.RelationType;
            record.RelationTypeName = receiver.RelationTypeName;
            record.DivisionProfit = receiver.DivisionProfit;

            if (subDivisionAmount <= 0)
            {
                record.CalDivisionAmount = 0L;
            }
            else
            {
                //计算的分账金额
                record.CalDivisionAmount = AmountUtil.CalPercentageFee(record.PayOrderDivisionAmount, record.DivisionProfit.Value);
                if (record.CalDivisionAmount > subDivisionAmount)
                {
                    // 分账金额超过剩余总金额时： 将按照剩余金额进行分账。
                    record.CalDivisionAmount = subDivisionAmount;
                }
            }

            return record;
        }

        private List<MchDivisionReceiverDto> QueryReceiver(byte? useSysAutoDivisionReceivers, PayOrderDto payOrder, List<PayOrderDivisionMQ.CustomerDivisionReceiver> customerDivisionReceiverList)
        {
            // 查询全部分账列表
            MchDivisionReceiverQueryDto queryWrapper = new MchDivisionReceiverQueryDto();

            queryWrapper.MchNo = payOrder.MchNo; //mchNo
            queryWrapper.AppId = payOrder.AppId; //appId
            queryWrapper.IfCode = payOrder.IfCode; //ifCode
            queryWrapper.State = CS.PUB_USABLE; // 可用状态的账号
            queryWrapper.PageNumber = 0;

            // 自动分账组的账号
            if (useSysAutoDivisionReceivers == CS.YES)
            {
                var groups = mchDivisionReceiverGroupService.GetByMchNo(payOrder.MchNo)
                    .Where(w => w.AutoDivisionFlag.Equals(CS.YES));

                if (!groups.Any())
                {
                    return new List<MchDivisionReceiverDto>();
                }

                queryWrapper.ReceiverGroupId = groups.First().ReceiverGroupId;
            }

            //全部分账账号
            List<MchDivisionReceiverDto> allMchReceiver = mchDivisionReceiverService.GetPaginatedData(queryWrapper);
            if (!allMchReceiver.Any())
            {
                return allMchReceiver;
            }

            //自动分账组
            if (useSysAutoDivisionReceivers == CS.YES)
            {
                return allMchReceiver;
            }

            //以下为 自定义列表

            // 自定义列表未定义
            if (customerDivisionReceiverList == null || !customerDivisionReceiverList.Any())
            {
                return new List<MchDivisionReceiverDto>();
            }

            // 过滤账号
            List<MchDivisionReceiverDto> filterMchReceiver = new List<MchDivisionReceiverDto>();

            foreach (MchDivisionReceiverDto mchDivisionReceiver in allMchReceiver)
            {
                foreach (PayOrderDivisionMQ.CustomerDivisionReceiver customerDivisionReceiver in customerDivisionReceiverList)
                {

                    // 查询匹配相同的项目
                    if (mchDivisionReceiver.ReceiverId.Equals(customerDivisionReceiver.ReceiverId) ||
                        mchDivisionReceiver.ReceiverGroupId.Equals(customerDivisionReceiver.ReceiverGroupId))
                    {
                        // 重新对分账比例赋值
                        if (customerDivisionReceiver.DivisionProfit != null)
                        {
                            mchDivisionReceiver.DivisionProfit = customerDivisionReceiver.DivisionProfit;
                        }
                        filterMchReceiver.Add(mchDivisionReceiver);
                    }
                }
            }

            return filterMchReceiver;
        }
    }
}
