using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using AGooday.AgPay.Payment.Api.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.PayOrder
{
    /// <summary>
    /// 关闭订单
    /// </summary>
    [ApiController]
    [Route("api/pay")]
    public class CloseOrderController : ApiControllerBase
    {
        private readonly ILogger<CloseOrderController> _logger;
        private readonly PayOrderService _payOrderService;
        private readonly IChannelServiceFactory<IPayOrderCloseService> _payOrderCloseServiceFactory;

        public CloseOrderController(ILogger<CloseOrderController> logger,
            PayOrderService payOrderService,
            IChannelServiceFactory<IPayOrderCloseService> payOrderCloseServiceFactory,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(requestKit, configContextQueryService)
        {
            _logger = logger;
            _payOrderService = payOrderService;
            _payOrderCloseServiceFactory = payOrderCloseServiceFactory;
        }

        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("close")]
        [PermissionAuth(PermCode.PAY.API_PAY_ORDER_CLOSE)]
        public async Task<ApiRes> CloseOrderAsync()
        {
            //获取参数 & 验签
            ClosePayOrderRQ rq = await this.GetRQByWithMchSignAsync<ClosePayOrderRQ>();

            if (StringUtil.IsAllNullOrEmpty(rq.MchOrderNo, rq.PayOrderId))
            {
                throw new BizException("mchOrderNo 和 payOrderId 不能同时为空");
            }

            PayOrderDto payOrder = await _payOrderService.QueryMchOrderAsync(rq.MchNo, rq.PayOrderId, rq.MchOrderNo);
            if (payOrder == null)
            {
                throw new BizException("订单不存在");
            }

            if (payOrder.State != (sbyte)PayOrderState.STATE_INIT && payOrder.State != (sbyte)PayOrderState.STATE_ING)
            {
                throw new BizException("当前订单不可关闭");
            }

            ClosePayOrderRS bizRes = new ClosePayOrderRS();

            // 订单生成状态  直接修改订单状态
            if (payOrder.State == (sbyte)PayOrderState.STATE_INIT)
            {
                await _payOrderService.UpdateIng2CloseAsync(payOrder.PayOrderId);
                bizRes.ChannelRetMsg = ChannelRetMsg.ConfirmSuccess(null);
                return ApiRes.OkWithSign(bizRes, rq.SignType, (await _configContextQueryService.QueryMchAppAsync(rq.MchNo, rq.AppId)).AppSecret);
            }

            try
            {
                string payOrderId = payOrder.PayOrderId;

                //查询支付接口是否存在
                IPayOrderCloseService closeService = _payOrderCloseServiceFactory.GetService(payOrder.IfCode);

                // 支付通道接口实现不存在
                if (closeService == null)
                {
                    _logger.LogError("{IfCode} interface not exists!", payOrder.IfCode);
                    //_logger.LogError($"{payOrder.IfCode} interface not exists!");
                    return null;
                }

                //查询出商户应用的配置信息
                MchAppConfigContext mchAppConfigContext = await _configContextQueryService.QueryMchInfoAndAppInfoAsync(payOrder.MchNo, payOrder.AppId);

                ChannelRetMsg channelRetMsg = await closeService.CloseAsync(payOrder, mchAppConfigContext);
                if (channelRetMsg == null)
                {
                    _logger.LogError("channelRetMsg is null");
                    return null;
                }

                _logger.LogInformation("关闭订单[{payOrderId}]结果为: {channelRetMsg}", payOrderId, channelRetMsg);
                //_logger.LogInformation($"关闭订单[{payOrderId}]结果为: {channelRetMsg}");

                // 关闭订单 成功
                if (channelRetMsg.ChannelState == ChannelState.CONFIRM_SUCCESS)
                {
                    await _payOrderService.UpdateIng2CloseAsync(payOrderId);
                }
                else
                {
                    return ApiRes.CustomFail(channelRetMsg.ChannelErrMsg);
                }

                bizRes.ChannelRetMsg = channelRetMsg;
            }
            catch (Exception e)
            {
                // 关闭订单异常
                _logger.LogError(e, "error payOrderId={PayOrderId}", payOrder.PayOrderId);
                //_logger.LogError(e, $"error payOrderId={payOrder.PayOrderId}");
                return null;
            }

            return ApiRes.OkWithSign(bizRes, rq.SignType, (await _configContextQueryService.QueryMchAppAsync(rq.MchNo, rq.AppId)).AppSecret);
        }
    }
}
