using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Payment.Api.Authorization;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Division;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AGooday.AgPay.Payment.Api.Controllers.Division
{
    /// <summary>
    /// 发起分账请求
    /// </summary>
    [ApiController]
    public class PayOrderDivisionExecController : ApiControllerBase
    {
        private readonly ILogger<PayOrderDivisionExecController> _logger;
        private readonly IPayOrderService _payOrderService;
        private readonly IMchDivisionReceiverService _mchDivisionReceiverService;
        private readonly IMchDivisionReceiverGroupService _mchDivisionReceiverGroupService;
        private readonly PayOrderDivisionProcessService _payOrderDivisionProcessService;

        public PayOrderDivisionExecController(ILogger<PayOrderDivisionExecController> logger,
            IPayOrderService payOrderService,
            IMchDivisionReceiverService mchDivisionReceiverService,
            IMchDivisionReceiverGroupService mchDivisionReceiverGroupService,
            PayOrderDivisionProcessService payOrderDivisionProcessService,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(requestKit, configContextQueryService)
        {
            _logger = logger;
            _payOrderService = payOrderService;
            _mchDivisionReceiverService = mchDivisionReceiverService;
            _mchDivisionReceiverGroupService = mchDivisionReceiverGroupService;
            _payOrderDivisionProcessService = payOrderDivisionProcessService;
        }

        /// <summary>
        /// 分账执行
        /// </summary>
        /// <param name="bizRQ"></param>
        /// <returns></returns>
        [HttpPost, Route("api/division/exec")]
        [PermissionAuth(PermCode.PAY.API_DIVISION_EXEC)]
        public ApiRes Exec()
        {
            //获取参数 & 验签
            PayOrderDivisionExecRQ bizRQ = GetRQByWithMchSign<PayOrderDivisionExecRQ>();

            try
            {
                if (StringUtil.IsAllNullOrWhiteSpace(bizRQ.MchOrderNo) && string.IsNullOrWhiteSpace(bizRQ.PayOrderId))
                {
                    throw new BizException("mchOrderNo 和 payOrderId不能同时为空");
                }

                PayOrderDto payOrder = _payOrderService.QueryMchOrder(bizRQ.MchNo, bizRQ.PayOrderId, bizRQ.MchOrderNo);
                if (payOrder == null)
                {
                    throw new BizException("订单不存在");
                }

                if (payOrder.State != (byte)PayOrderState.STATE_SUCCESS || payOrder.DivisionState != (byte)PayOrderDivisionState.DIVISION_STATE_UNHAPPEN || payOrder.DivisionMode != (byte)PayOrderDivisionMode.DIVISION_MODE_MANUAL)
                {
                    throw new BizException("当前订单状态不支持分账");
                }

                List<PayOrderDivisionMQ.CustomerDivisionReceiver> receiverList = null;

                //不使用默认分组， 需要转换每个账号信息
                if (bizRQ.UseSysAutoDivisionReceivers != CS.YES && !string.IsNullOrWhiteSpace(bizRQ.Receivers))
                {
                    receiverList = JsonConvert.DeserializeObject<List<PayOrderDivisionMQ.CustomerDivisionReceiver>>(bizRQ.Receivers);
                }

                // 验证账号是否合法
                this.CheckReceiverList(receiverList, payOrder.IfCode, bizRQ.MchNo, bizRQ.AppId);

                // 商户配置信息
                MchAppConfigContext mchAppConfigContext = _configContextQueryService.QueryMchInfoAndAppInfo(bizRQ.MchNo, bizRQ.AppId);
                if (mchAppConfigContext == null)
                {
                    throw new BizException("获取商户应用信息失败");
                }

                //处理分账请求
                ChannelRetMsg channelRetMsg = _payOrderDivisionProcessService.ProcessPayOrderDivision(bizRQ.PayOrderId, bizRQ.UseSysAutoDivisionReceivers, receiverList,false);

                PayOrderDivisionExecRS bizRS = new PayOrderDivisionExecRS();
                bizRS.State = (byte)(channelRetMsg.ChannelState == ChannelState.CONFIRM_SUCCESS ? PayOrderDivisionRecordState.STATE_SUCCESS : PayOrderDivisionRecordState.STATE_FAIL);
                bizRS.ChannelBatchOrderId = channelRetMsg.ChannelOrderId;
                bizRS.ErrCode = channelRetMsg.ChannelErrCode;
                bizRS.ErrMsg = channelRetMsg.ChannelErrMsg;
                return ApiRes.OkWithSign(bizRS, bizRQ.SignType, mchAppConfigContext.MchApp.AppSecret);
            }
            catch (BizException e)
            {
                return ApiRes.CustomFail(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e,$"系统异常：{e.Message}");
                return ApiRes.CustomFail("系统异常");
            }
        }

        /// <summary>
        /// 检验账号是否合法
        /// </summary>
        /// <param name="receiverList"></param>
        /// <param name="ifCode"></param>
        /// <param name="mchNo"></param>
        /// <param name="appId"></param>
        /// <exception cref="BizException"></exception>
        private void CheckReceiverList(List<PayOrderDivisionMQ.CustomerDivisionReceiver> receiverList, string ifCode, string mchNo, string appId)
        {
            if (receiverList == null || receiverList.Count == 0)
            {
                return;
            }

            var receiverIdSet = new HashSet<long>();
            var receiverGroupIdSet = new HashSet<long>();

            foreach (var receiver in receiverList)
            {
                if (receiver.ReceiverId.HasValue)
                {
                    receiverIdSet.Add(receiver.ReceiverId.Value);
                }

                if (receiver.ReceiverGroupId.HasValue)
                {
                    receiverGroupIdSet.Add(receiver.ReceiverGroupId.Value);
                }

                if (receiver.ReceiverId == null && receiver.ReceiverGroupId == null)
                {
                    throw new BizException("分账用户组： receiverId 和 与receiverGroupId 必填一项");
                }

                if (receiver.DivisionProfit.HasValue)
                {
                    if (receiver.DivisionProfit.Value.CompareTo(Decimal.Zero) <= 0)
                    {
                        throw new BizException($"分账用户receiverId=[{receiver.ReceiverId}], receiverGroupId=[{receiver.ReceiverGroupId}] 分账比例不得小于0%");
                    }

                    if (receiver.DivisionProfit.Value.CompareTo(Decimal.One) > 0)
                    {
                        throw new BizException($"分账用户receiverId=[{receiver.ReceiverId}], receiverGroupId=[{receiver.ReceiverGroupId}] 分账比例不得高于100%");
                    }
                }
            }

            if (!receiverIdSet.IsNotEmptyOrNull())
            {
                int receiverCount = _mchDivisionReceiverService.GetCount(receiverIdSet, mchNo, appId, ifCode);

                if (receiverCount != receiverIdSet.Count)
                {
                    throw new BizException("分账[用户]中包含不存在或渠道不可用账号，请更改");
                }
            }

            if (!receiverGroupIdSet.IsNotEmptyOrNull())
            {
                int receiverGroupCount = _mchDivisionReceiverService.GetCount(receiverGroupIdSet, mchNo);

                if (receiverGroupCount != receiverGroupIdSet.Count)
                {
                    throw new BizException("分账[账号组]中包含不存在或不可用组，请更改");
                }
            }
        }
    }
}
