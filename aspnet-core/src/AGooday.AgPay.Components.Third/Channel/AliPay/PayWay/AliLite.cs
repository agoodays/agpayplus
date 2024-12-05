using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.AliPay.Kits;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;

namespace AGooday.AgPay.Components.Third.Channel.AliPay.PayWay
{
    /// <summary>
    /// 支付宝 小程序支付
    /// </summary>
    public class AliLite : AliPayPaymentService
    {
        public AliLite(ILogger<AliLite> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            AliLiteOrderRQ bizRQ = (AliLiteOrderRQ)rq;

            AlipayTradeCreateRequest req = new AlipayTradeCreateRequest();
            AlipayTradeCreateModel model = new AlipayTradeCreateModel();
            model.OutTradeNo = payOrder.PayOrderId;
            model.Subject = payOrder.Subject; //订单标题
            model.Body = payOrder.Body; //订单描述信息
            model.TotalAmount = AmountUtil.ConvertCent2Dollar(payOrder.Amount);  //支付金额
            model.TimeExpire = payOrder.ExpiredTime?.ToString("yyyy-MM-dd HH:mm:ss");
            model.BuyerId = bizRQ.GetChannelUserId();
            model.ProductCode = "JSAPI_PAY";
            req.SetNotifyUrl(GetNotifyUrl()); // 设置异步通知地址
            req.SetBizModel(model);

            //统一放置 isv接口必传信息
            AliPayKit.PutApiIsvInfo(mchAppConfigContext, req, model);

            //调起支付宝 （如果异常， 将直接跑出   ChannelException ）
            AlipayTradeCreateResponse alipayResp = _configContextQueryService.GetAlipayClientWrapper(mchAppConfigContext).Execute(req);

            // 构造函数响应数据
            AliLiteOrderRS res = ApiResBuilder.BuildSuccess<AliLiteOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            //放置 响应数据
            channelRetMsg.ChannelAttach = alipayResp.Body;

            // ↓↓↓↓↓↓ 调起接口成功后业务判断务必谨慎！！ 避免因代码编写bug，导致不能正确返回订单状态信息  ↓↓↓↓↓↓

            res.AlipayTradeNo = alipayResp.TradeNo;

            channelRetMsg.ChannelOrderId = alipayResp.TradeNo;
            //业务处理成功
            if (!alipayResp.IsError)
            {
                channelRetMsg.ChannelState = ChannelState.WAITING;
            }
            else
            {
                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                channelRetMsg.ChannelErrCode = AliPayKit.AppendErrCode(alipayResp.Code, alipayResp.SubCode);
                channelRetMsg.ChannelErrMsg = AliPayKit.AppendErrMsg(alipayResp.Msg, alipayResp.SubMsg);
            }
            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            AliLiteOrderRQ bizRQ = (AliLiteOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.GetChannelUserId()))
            {
                throw new BizException("[buyerUserId]不可为空");
            }

            return null;
        }
    }
}
