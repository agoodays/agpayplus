using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.UmsPay.PayWay
{
    /// <summary>
    /// 银联商务 云闪付 条码支付
    /// </summary>
    public class YsfBar : UmsPayPaymentService
    {
        public YsfBar(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【银联商务条码(unionpay)支付】";
            YsfBarOrderRQ bizRQ = (YsfBarOrderRQ)rq;
            // 构造函数响应数据
            YsfBarOrderRS res = ApiResBuilder.BuildSuccess<YsfBarOrderRS>();

            // 业务处理
            JObject reqParams = new JObject();
            reqParams.Add("payCode", bizRQ.AuthCode.Trim()); //授权码 通过扫码枪/声波获取设备获取的支付宝/微信/银联付款码
            // 银联商务 bar 统一参数赋值
            BarParamsSet(reqParams, payOrder, GetNotifyUrl());

            var channelRetMsg = UmsBar(reqParams, logPrefix, mchAppConfigContext);
            res.ChannelRetMsg = channelRetMsg;
            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            YsfBarOrderRQ bizRQ = (YsfBarOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.AuthCode))
            {
                throw new BizException("用户支付条码[authCode]不可为空");
            }

            return null;
        }
    }
}
