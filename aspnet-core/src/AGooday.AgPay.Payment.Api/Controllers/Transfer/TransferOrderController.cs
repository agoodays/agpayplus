using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.Exceptions;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.Transfer;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using AGooday.AgPay.Payment.Api.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.Transfer
{
    /// <summary>
    /// 转账接口
    /// </summary>
    [ApiController]
    public class TransferOrderController : ApiControllerBase
    {
        private readonly ILogger<TransferOrderController> _logger;
        private readonly IChannelServiceFactory<ITransferService> _transferServiceFactory;
        private readonly ITransferOrderService _transferOrderService;
        private readonly IPayInterfaceConfigService _payInterfaceConfigService;
        private readonly PayMchNotifyService _payMchNotifyService;

        public TransferOrderController(ILogger<TransferOrderController> logger,
            IChannelServiceFactory<ITransferService> transferServiceFactory,
            ITransferOrderService transferOrderService,
            IPayInterfaceConfigService payInterfaceConfigService,
            PayMchNotifyService payMchNotifyService,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(requestKit, configContextQueryService)
        {
            _logger = logger;
            _transferServiceFactory = transferServiceFactory;
            _transferOrderService = transferOrderService;
            _payInterfaceConfigService = payInterfaceConfigService;
            _payMchNotifyService = payMchNotifyService;
        }

        /// <summary>
        /// 转账
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("api/transferOrder")]
        [PermissionAuth(PermCode.PAY.API_TRANS_ORDER)]
        public async Task<ApiRes> TransferOrderAsync()
        {
            TransferOrderDto transferOrder = null;

            //获取参数 & 验签
            TransferOrderRQ bizRQ = await this.GetRQByWithMchSignAsync<TransferOrderRQ>();

            try
            {
                string mchNo = bizRQ.MchNo;
                string appId = bizRQ.AppId;
                string ifCode = bizRQ.IfCode;

                // 商户订单号是否重复
                if (await _transferOrderService.IsExistOrderByMchOrderNoAsync(mchNo, bizRQ.MchOrderNo))
                {
                    throw new BizException($"商户订单[{bizRQ.MchOrderNo}]已存在");
                }

                if (!string.IsNullOrWhiteSpace(bizRQ.NotifyUrl) && !StringUtil.IsAvailableUrl(bizRQ.NotifyUrl))
                {
                    throw new BizException("异步通知地址协议仅支持http:// 或 https:// !");
                }

                // 商户配置信息
                MchAppConfigContext mchAppConfigContext = await _configContextQueryService.QueryMchInfoAndAppInfoAsync(mchNo, appId);
                if (mchAppConfigContext == null)
                {
                    throw new BizException("获取商户应用信息失败");
                }

                //MchInfoDto mchInfo = mchAppConfigContext.MchInfo;
                MchAppDto mchApp = mchAppConfigContext.MchApp;

                // 是否已正确配置
                if (!await _payInterfaceConfigService.MchAppHasAvailableIfCodeAsync(appId, ifCode))
                {
                    throw new BizException("应用未开通此接口配置!");
                }

                ITransferService transferService = _transferServiceFactory.GetService(ifCode);
                if (transferService == null)
                {
                    throw new BizException("无此转账通道接口");
                }

                if (!transferService.IsSupport(bizRQ.EntryType))
                {
                    throw new BizException("该接口不支持该入账方式");
                }

                transferOrder = await this.GenTransferOrderAsync(bizRQ, mchAppConfigContext, ifCode);

                //预先校验
                string errMsg = transferService.PreCheck(bizRQ, transferOrder);
                if (!string.IsNullOrEmpty(errMsg))
                {
                    throw new BizException(errMsg);
                }

                // 入库
                await _transferOrderService.AddAsync(transferOrder);

                // 调起上游接口
                ChannelRetMsg channelRetMsg = await transferService.TransferAsync(bizRQ, transferOrder, mchAppConfigContext);

                //处理退款单状态
                await this.ProcessChannelMsgAsync(channelRetMsg, transferOrder);

                TransferOrderRS bizRes = TransferOrderRS.BuildByRecord(transferOrder);
                return ApiRes.OkWithSign(bizRes, bizRQ.SignType, mchApp.AppSecret);

            }
            catch (BizException e)
            {
                return ApiRes.CustomFail(e.Message);
            }
            catch (ChannelException e)
            {
                //处理上游返回数据
                await this.ProcessChannelMsgAsync(e.ChannelRetMsg, transferOrder);

                if (e.ChannelRetMsg.ChannelState == ChannelState.SYS_ERROR)
                {
                    return ApiRes.CustomFail(e.Message);
                }

                TransferOrderRS bizRes = TransferOrderRS.BuildByRecord(transferOrder);
                return ApiRes.OkWithSign(bizRes, bizRQ.SignType, (await _configContextQueryService.QueryMchAppAsync(bizRQ.MchNo, bizRQ.AppId)).AppSecret);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "系统异常: {Message}", e.Message);
                //_logger.LogError(e, $"系统异常: {e.Message}");
                return ApiRes.CustomFail("系统异常");
            }
        }

        private async Task<TransferOrderDto> GenTransferOrderAsync(TransferOrderRQ rq, MchAppConfigContext configContext, string ifCode)
        {
            MchInfoDto mchInfo = configContext.MchInfo;
            MchAppDto mchApp = configContext.MchApp;
            AgentInfoDto agentInfo = await _configContextQueryService.QueryAgentInfoAsync(configContext);
            IsvInfoDto isvInfo = await _configContextQueryService.QueryIsvInfoAsync(configContext);
            return this.GenTransferOrder(rq, mchInfo, mchApp, agentInfo, isvInfo, ifCode);
        }

        private TransferOrderDto GenTransferOrder(TransferOrderRQ rq, MchInfoDto mchInfo, MchAppDto mchApp, AgentInfoDto agentInfo, IsvInfoDto isvInfo, string ifCode)
        {
            TransferOrderDto transferOrder = new TransferOrderDto();
            transferOrder.TransferId = SeqUtil.GenTransferId(); //生成转账订单号
            transferOrder.MchNo = mchInfo.MchNo; //商户号
            transferOrder.MchName = mchInfo.MchName; //商户名称（简称）
            transferOrder.MchShortName = mchInfo.MchShortName; //商户简称
            transferOrder.AgentNo = mchInfo.AgentNo; //代理商号
            transferOrder.AgentName = agentInfo?.AgentName; //代理商名称
            transferOrder.AgentShortName = agentInfo?.AgentShortName; //代理商简称
            transferOrder.IsvNo = mchInfo.IsvNo; //服务商号
            transferOrder.IsvName = isvInfo?.IsvName; //服务商名称
            transferOrder.IsvShortName = isvInfo?.IsvShortName; //服务商简称
            transferOrder.AppId = mchApp.AppId; //商户应用appId
            transferOrder.MchType = mchInfo.Type; //商户类型
            transferOrder.MchOrderNo = rq.MchOrderNo; //商户订单号
            transferOrder.IfCode = ifCode; //接口代码
            transferOrder.EntryType = rq.EntryType; //入账方式
            transferOrder.Amount = rq.Amount; //订单金额
            transferOrder.Currency = rq.Currency; //币种
            transferOrder.ClientIp = StringUtil.DefaultIfEmpty(rq.ClientIp, GetClientIp()); //客户端IP
            transferOrder.State = (byte)TransferOrderState.STATE_INIT; //订单状态, 默认订单生成状态
            transferOrder.AccountNo = rq.AccountNo; //收款账号
            transferOrder.AccountName = rq.AccountName; //账户姓名
            transferOrder.BankName = rq.BankName; //银行名称
            transferOrder.TransferDesc = rq.TransferDesc; //转账备注
            transferOrder.ExtParam = rq.ExtParam; //商户扩展参数
            transferOrder.NotifyUrl = rq.NotifyUrl; //异步通知地址
            transferOrder.CreatedAt = DateTime.Now; //订单创建时间
            return transferOrder;
        }

        /// <summary>
        /// 处理返回的渠道信息，并更新订单状态
        /// transferOrder将对部分信息进行 赋值操作。
        /// </summary>
        /// <param name="channelRetMsg"></param>
        /// <param name="transferOrder"></param>
        /// <exception cref="BizException"></exception>
        private async Task ProcessChannelMsgAsync(ChannelRetMsg channelRetMsg, TransferOrderDto transferOrder)
        {
            // 对象为空 || 上游返回状态为空， 则无需操作
            if (channelRetMsg == null || channelRetMsg.ChannelState == null)
            {
                return;
            }

            // string transferId = transferOrder.TransferId;

            // 明确成功
            if (ChannelState.CONFIRM_SUCCESS == channelRetMsg.ChannelState)
            {
                await this.UpdateInitOrderStateThrowExceptionAsync((byte)TransferOrderState.STATE_SUCCESS, transferOrder, channelRetMsg);
                await _payMchNotifyService.TransferOrderNotifyAsync(transferOrder);
            }
            // 明确失败
            else if (ChannelState.CONFIRM_FAIL == channelRetMsg.ChannelState)
            {
                await this.UpdateInitOrderStateThrowExceptionAsync((byte)TransferOrderState.STATE_FAIL, transferOrder, channelRetMsg);
                await _payMchNotifyService.TransferOrderNotifyAsync(transferOrder);
            }
            // 上游处理中 || 未知 || 上游接口返回异常  订单为支付中状态
            else if (ChannelState.WAITING == channelRetMsg.ChannelState ||
                    ChannelState.UNKNOWN == channelRetMsg.ChannelState ||
                    ChannelState.API_RET_ERROR == channelRetMsg.ChannelState)
            {
                await this.UpdateInitOrderStateThrowExceptionAsync((byte)TransferOrderState.STATE_ING, transferOrder, channelRetMsg);
            }
            // 系统异常：  订单不再处理。  为： 生成状态
            else if (ChannelState.SYS_ERROR == channelRetMsg.ChannelState)
            {
            }
            else
            {
                throw new BizException("ChannelState 返回异常！");
            }
        }

        /// <summary>
        /// 更新订单状态 --》 订单生成--》 其他状态  (向外抛出异常)
        /// </summary>
        /// <param name="orderState"></param>
        /// <param name="transferOrder"></param>
        /// <param name="channelRetMsg"></param>
        /// <exception cref="BizException"></exception>
        private async Task UpdateInitOrderStateThrowExceptionAsync(byte orderState, TransferOrderDto transferOrder, ChannelRetMsg channelRetMsg)
        {
            transferOrder.State = orderState;
            transferOrder.ChannelOrderNo = channelRetMsg.ChannelOrderId;
            transferOrder.ErrCode = channelRetMsg.ChannelErrCode;
            transferOrder.ErrMsg = channelRetMsg.ChannelErrMsg;

            bool isSuccess = await _transferOrderService.UpdateInit2IngAsync(transferOrder.TransferId);
            if (!isSuccess)
            {
                throw new BizException("更新转账订单异常!");
            }

            isSuccess = await _transferOrderService.UpdateIng2SuccessOrFailAsync(transferOrder.TransferId, transferOrder.State,
                    channelRetMsg.ChannelOrderId, channelRetMsg.ChannelErrCode, channelRetMsg.ChannelErrMsg);
            if (!isSuccess)
            {
                throw new BizException("更新转账订单异常!");
            }
        }
    }
}
