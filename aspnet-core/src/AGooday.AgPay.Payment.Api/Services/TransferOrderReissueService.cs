using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Payment.Api.Channel;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;

namespace AGooday.AgPay.Payment.Api.Services
{
    public class TransferOrderReissueService
    {
        private readonly ConfigContextQueryService configContextQueryService;
        private readonly ITransferOrderService transferOrderService;
        private readonly PayMchNotifyService payMchNotifyService;
        private readonly Func<string, ITransferService> transferServiceFactory;
        private readonly ILogger<PayOrderProcessService> _logger;

        public TransferOrderReissueService(ConfigContextQueryService configContextQueryService,
            ITransferOrderService transferOrderService,
            PayMchNotifyService payMchNotifyService,
            Func<string, ITransferService> transferServiceFactory,
            ILogger<PayOrderProcessService> logger)
        {
            this.configContextQueryService = configContextQueryService;
            this.transferOrderService = transferOrderService;
            this.payMchNotifyService = payMchNotifyService;
            this.transferServiceFactory = transferServiceFactory;
            _logger = logger;
        }

        /// <summary>
        /// 处理转账订单
        /// </summary>
        /// <param name="transferOrder"></param>
        /// <returns></returns>
        public ChannelRetMsg ProcessOrder(TransferOrderDto transferOrder)
        {
            try
            {
                string transferId = transferOrder.TransferId;

                // 查询转账接口是否存在
                ITransferService transferService = transferServiceFactory(transferOrder.IfCode);

                // 支付通道转账接口实现不存在
                if (transferService == null)
                {
                    _logger.LogError($"{transferOrder.IfCode} interface not exists!");
                    return null;
                }

                // 查询出商户应用的配置信息
                MchAppConfigContext mchAppConfigContext = configContextQueryService.QueryMchInfoAndAppInfo(transferOrder.MchNo, transferOrder.AppId);

                ChannelRetMsg channelRetMsg = transferService.Query(transferOrder, mchAppConfigContext);
                if (channelRetMsg == null)
                {
                    _logger.LogError("channelRetMsg is null");
                    return null;
                }

                _logger.LogInformation($"补单[{transferId}]查询结果为：{channelRetMsg}", transferId);

                // 查询成功
                if (channelRetMsg.ChannelState == ChannelState.CONFIRM_SUCCESS)
                {
                    // 转账成功
                    transferOrderService.UpdateIng2Success(transferId, channelRetMsg.ChannelOrderId);
                    payMchNotifyService.TransferOrderNotify(transferOrderService.GetById(transferId));

                }
                else if (channelRetMsg.ChannelState == ChannelState.CONFIRM_FAIL)
                {
                    // 转账失败
                    transferOrderService.UpdateIng2Fail(transferId, channelRetMsg.ChannelOrderId, channelRetMsg.ChannelUserId, channelRetMsg.ChannelErrCode);
                    payMchNotifyService.TransferOrderNotify(transferOrderService.GetById(transferId));
                }

                return channelRetMsg;

            }
            catch (Exception e)
            {
                //继续下一次迭代查询
                _logger.LogError(e, $"error transferId = {transferOrder.TransferId}");
                return null;
            }
        }
    }
}
