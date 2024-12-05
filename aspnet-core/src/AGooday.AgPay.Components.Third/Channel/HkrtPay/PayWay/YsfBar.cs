using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.HkrtPay.PayWay
{
    /// <summary>
    /// 海科融通 云闪付 条码支付
    /// </summary>
    public class YsfBar : HkrtPayPaymentService
    {
        public YsfBar(ILogger<YsfBar> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【海科融通条码(unionpay)支付】";
            YsfBarOrderRQ bizRQ = (YsfBarOrderRQ)rq;
            // 构造函数响应数据
            YsfBarOrderRS res = ApiResBuilder.BuildSuccess<YsfBarOrderRS>();

            JObject reqParams = new JObject();
            reqParams.Add("auth_code", bizRQ.AuthCode.Trim()); //授权码 通过扫码枪/声波获取设备获取的支付宝/微信/银联付款码
            // 海科融通 bar 统一参数赋值
            BarParamsSet(reqParams, payOrder, GetNotifyUrl());

            var channelRetMsg = HkrtBar(reqParams, logPrefix, mchAppConfigContext);
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
