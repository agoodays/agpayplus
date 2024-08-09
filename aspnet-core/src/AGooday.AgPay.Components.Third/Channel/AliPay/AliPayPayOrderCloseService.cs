using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Components.Third.Channel.AliPay.Kits;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;

namespace AGooday.AgPay.Components.Third.Channel.AliPay
{
    /// <summary>
    /// 支付宝 关闭订单接口实现类
    /// </summary>
    public class AliPayPayOrderCloseService : IPayOrderCloseService
    {
        protected readonly ConfigContextQueryService _configContextQueryService;

        public AliPayPayOrderCloseService(ConfigContextQueryService configContextQueryService)
        {
            _configContextQueryService = configContextQueryService;
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.ALIPAY;
        }

        public ChannelRetMsg Close(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            AlipayTradeCloseRequest req = new AlipayTradeCloseRequest();

            // 商户订单号，商户网站订单系统中唯一订单号，必填
            AlipayTradeCloseModel model = new AlipayTradeCloseModel();
            model.OutTradeNo = payOrder.PayOrderId;
            req.SetBizModel(model);

            //通用字段
            AliPayKit.PutApiIsvInfo(mchAppConfigContext, req, model);

            AlipayTradeCloseResponse resp = _configContextQueryService.GetAlipayClientWrapper(mchAppConfigContext).Execute(req);

            // 返回状态成功
            if (!resp.IsError)
            {
                return ChannelRetMsg.ConfirmSuccess(resp.TradeNo);
            }
            else
            {
                return ChannelRetMsg.SysError(resp.SubMsg);
            }
        }
    }
}
