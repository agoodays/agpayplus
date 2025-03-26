using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;

namespace AGooday.AgPay.Components.Third.Services
{
    public class TransferOrderReissueService
    {
        private readonly ILogger<PayOrderProcessService> _logger;
        private readonly ITransferOrderService _transferOrderService;
        private readonly IChannelServiceFactory<ITransferService> _transferServiceFactory;
        private readonly PayMchNotifyService _payMchNotifyService;
        private readonly ConfigContextQueryService _configContextQueryService;

        public TransferOrderReissueService(ILogger<PayOrderProcessService> logger,
            ITransferOrderService transferOrderService,
            IChannelServiceFactory<ITransferService> transferServiceFactory,
            PayMchNotifyService payMchNotifyService,
            ConfigContextQueryService configContextQueryService)
        {
            _logger = logger;
            _transferOrderService = transferOrderService;
            _transferServiceFactory = transferServiceFactory;
            _payMchNotifyService = payMchNotifyService;
            _configContextQueryService = configContextQueryService;
        }

        /// <summary>
        /// 处理转账订单
        /// </summary>
        /// <param name="transferOrder"></param>
        /// <returns></returns>
        public async Task<ChannelRetMsg> ProcessOrderAsync(TransferOrderDto transferOrder)
        {
            try
            {
                string transferId = transferOrder.TransferId;

                // 查询转账接口是否存在
                ITransferService transferService = _transferServiceFactory.GetService(transferOrder.IfCode);

                // 支付通道转账接口实现不存在
                if (transferService == null)
                {
                    _logger.LogError("{IfCode} interface not exists!", transferOrder.IfCode);
                    //_logger.LogError($"{transferOrder.IfCode} interface not exists!");
                    return null;
                }

                // 查询出商户应用的配置信息
                MchAppConfigContext mchAppConfigContext = await _configContextQueryService.QueryMchInfoAndAppInfoAsync(transferOrder.MchNo, transferOrder.AppId);

                ChannelRetMsg channelRetMsg = await transferService.QueryAsync(transferOrder, mchAppConfigContext);
                if (channelRetMsg == null)
                {
                    _logger.LogError("channelRetMsg is null");
                    return null;
                }

                _logger.LogInformation("补单[{transferId}]查询结果为：{channelRetMsg}", transferId, channelRetMsg);
                //_logger.LogInformation($"补单[{transferId}]查询结果为：{channelRetMsg}");

                // 查询成功
                if (channelRetMsg.ChannelState == ChannelState.CONFIRM_SUCCESS)
                {
                    // 转账成功
                    await _transferOrderService.UpdateIng2SuccessAsync(transferId, channelRetMsg.ChannelOrderId);
                    await _payMchNotifyService.TransferOrderNotifyAsync(await _transferOrderService.GetByIdAsync(transferId));

                }
                else if (channelRetMsg.ChannelState == ChannelState.CONFIRM_FAIL)
                {
                    // 转账失败
                    await _transferOrderService.UpdateIng2FailAsync(transferId, channelRetMsg.ChannelOrderId, channelRetMsg.ChannelErrCode, channelRetMsg.ChannelErrMsg);
                    await _payMchNotifyService.TransferOrderNotifyAsync(await _transferOrderService.GetByIdAsync(transferId));
                }

                return channelRetMsg;

            }
            catch (Exception e)
            {
                //继续下一次迭代查询
                _logger.LogError(e, "error transferId = {TransferId}", transferOrder.TransferId);
                //_logger.LogError(e, $"error transferId = {transferOrder.TransferId}");
                return null;
            }
        }
    }
}
