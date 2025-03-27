using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using Newtonsoft.Json;

namespace AGooday.AgPay.Components.Third.Services
{
    /// <summary>
    /// 查询上游订单， &  补单服务实现类
    /// </summary>
    public class ChannelOrderReissueService
    {
        private readonly ILogger<ChannelOrderReissueService> _logger;
        private readonly IPayOrderService _payOrderService;
        private readonly IChannelServiceFactory<IRefundService> _refundServiceFactory;
        private readonly IChannelServiceFactory<IPayOrderQueryService> _payOrderQueryServiceFactory;
        private readonly ConfigContextQueryService _configContextQueryService;
        private readonly PayOrderProcessService _payOrderProcessService;
        private readonly RefundOrderProcessService _refundOrderProcessService;

        public ChannelOrderReissueService(ILogger<ChannelOrderReissueService> logger,
            IChannelServiceFactory<IPayOrderQueryService> payOrderQueryServiceFactory,
            IChannelServiceFactory<IRefundService> refundServiceFactory,
            ConfigContextQueryService configContextQueryService,
            PayOrderProcessService payOrderProcessService,
            RefundOrderProcessService refundOrderProcessService,
            IPayOrderService payOrderService)
        {
            _logger = logger;
            _payOrderService = payOrderService;
            _refundServiceFactory = refundServiceFactory;
            _payOrderQueryServiceFactory = payOrderQueryServiceFactory;
            _configContextQueryService = configContextQueryService;
            _payOrderProcessService = payOrderProcessService;
            _refundOrderProcessService = refundOrderProcessService;
        }

        /// <summary>
        /// 处理支付订单
        /// </summary>
        /// <param name="payOrder"></param>
        /// <returns></returns>
        public async Task<ChannelRetMsg> ProcessPayOrderAsync(PayOrderDto payOrder)
        {
            try
            {
                string payOrderId = payOrder.PayOrderId;

                //查询支付接口是否存在
                IPayOrderQueryService queryService = _payOrderQueryServiceFactory.GetService(payOrder.IfCode);

                // 支付通道接口实现不存在
                if (queryService == null)
                {
                    _logger.LogError("{IfCode} interface not exists!", payOrder.IfCode);
                    //_logger.LogError($"{payOrder.IfCode} interface not exists!");
                    return null;
                }

                //查询出商户应用的配置信息
                MchAppConfigContext mchAppConfigContext = await _configContextQueryService.QueryMchInfoAndAppInfoAsync(payOrder.MchNo, payOrder.AppId);

                ChannelRetMsg channelRetMsg = await queryService.QueryAsync(payOrder, mchAppConfigContext);
                if (channelRetMsg == null)
                {
                    _logger.LogError("channelRetMsg is null");
                    return null;
                }

                _logger.LogInformation("补单[{payOrderId}] 查询结果为: {channelRetMsg}", payOrderId, JsonConvert.SerializeObject(channelRetMsg));
                //_logger.LogInformation($"补单[{payOrderId}] 查询结果为: {JsonConvert.SerializeObject(channelRetMsg)}");

                // 查询成功
                if (channelRetMsg.ChannelState == ChannelState.CONFIRM_SUCCESS)
                {
                    if (await _payOrderService.UpdateIng2SuccessAsync(payOrderId, channelRetMsg.ChannelMchNo, channelRetMsg.ChannelIsvNo, channelRetMsg.ChannelOrderId, channelRetMsg.ChannelUserId, channelRetMsg.PlatformOrderId, channelRetMsg.PlatformMchOrderId))
                    {
                        //订单支付成功，其他业务逻辑
                        await _payOrderProcessService.ConfirmSuccessAsync(payOrder);
                    }
                }
                //确认失败
                else if (channelRetMsg.ChannelState == ChannelState.CONFIRM_FAIL)
                {
                    //1. 更新支付订单表为失败状态
                    await _payOrderService.UpdateIng2FailAsync(payOrderId, channelRetMsg.ChannelMchNo, channelRetMsg.ChannelIsvNo, channelRetMsg.ChannelOrderId, channelRetMsg.ChannelUserId, channelRetMsg.PlatformOrderId, channelRetMsg.PlatformMchOrderId, channelRetMsg.ChannelErrCode, channelRetMsg.ChannelErrMsg);
                }

                return channelRetMsg;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error payOrderId={PayOrderId}", payOrder.PayOrderId);
                //_logger.LogError(e, $"error payOrderId={payOrder.PayOrderId}");
                return null;
            }
        }
        /// <summary>
        /// 处理退款订单
        /// </summary>
        /// <param name="refundOrder"></param>
        /// <returns></returns>
        public async Task<ChannelRetMsg> ProcessRefundOrderAsync(RefundOrderDto refundOrder)
        {
            try
            {
                string refundOrderId = refundOrder.RefundOrderId;

                //查询支付接口是否存在
                IRefundService queryService = _refundServiceFactory.GetService(refundOrder.IfCode);

                // 支付通道接口实现不存在
                if (queryService == null)
                {
                    _logger.LogError("退款补单: {IfCode} interface not exists!", refundOrder.IfCode);
                    //_logger.LogError($"退款补单：{refundOrder.IfCode} interface not exists!");
                    return null;
                }

                //查询出商户应用的配置信息
                MchAppConfigContext mchAppConfigContext = await _configContextQueryService.QueryMchInfoAndAppInfoAsync(refundOrder.MchNo, refundOrder.AppId);

                ChannelRetMsg channelRetMsg = await queryService.QueryAsync(refundOrder, mchAppConfigContext);
                if (channelRetMsg == null)
                {
                    _logger.LogError("退款补单: channelRetMsg is null");
                    return null;
                }

                _logger.LogInformation("退款补单: [{refundOrderId}] 查询结果为: {channelRetMsg}", refundOrderId, JsonConvert.SerializeObject(channelRetMsg));
                //_logger.LogInformation($"退款补单: [{refundOrderId}] 查询结果为: {JsonConvert.SerializeObject(channelRetMsg)}");
                // 根据渠道返回结果，处理退款订单
                await _refundOrderProcessService.HandleRefundOrder4ChannelAsync(channelRetMsg, refundOrder);

                return channelRetMsg;
            }
            catch (Exception e)
            {
                //继续下一次迭代查询
                _logger.LogError(e, "退款补单: error refundOrderId={RefundOrderId}", refundOrder.RefundOrderId);
                //_logger.LogError(e, $"退款补单: error refundOrderId={refundOrder.RefundOrderId}");
                return null;
            }
        }
    }
}
