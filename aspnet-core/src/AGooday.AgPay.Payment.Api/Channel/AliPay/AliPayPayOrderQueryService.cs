using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Channel.AliPay.Kits;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;

namespace AGooday.AgPay.Payment.Api.Channel.AliPay
{
    /// <summary>
    /// 支付宝 查单接口实现类
    /// </summary>
    public class AliPayPayOrderQueryService : IPayOrderQueryService
    {
        protected readonly ConfigContextQueryService _configContextQueryService;

        public AliPayPayOrderQueryService(ConfigContextQueryService configContextQueryService)
        {
            _configContextQueryService = configContextQueryService;
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.ALIPAY;
        }

        public ChannelRetMsg Query(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            AlipayTradeQueryRequest req = new AlipayTradeQueryRequest();

            // 商户订单号，商户网站订单系统中唯一订单号，必填
            AlipayTradeQueryModel model = new AlipayTradeQueryModel();
            model.OutTradeNo = payOrder.PayOrderId;
            req.SetBizModel(model);

            //通用字段
            AliPayKit.PutApiIsvInfo(mchAppConfigContext, req, model);

            AlipayTradeQueryResponse resp = _configContextQueryService.GetAlipayClientWrapper(mchAppConfigContext).Execute(req);
            string result = resp.TradeStatus;

            if ("TRADE_SUCCESS".Equals(result))
            {
                return ChannelRetMsg.ConfirmSuccess(resp.TradeNo);  //支付成功
            }
            else if ("WAIT_BUYER_PAY".Equals(result))
            {
                return ChannelRetMsg.Waiting(); //支付中
            }
            return ChannelRetMsg.Waiting(); //支付中
        }
    }
}
