using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Payment.Api.Utils;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.UmsPay.PayWay
{
    public class AliBar : UmsPayPaymentService
    {
        /// <summary>
        /// 银联商务 支付宝 条码支付
        /// </summary>
        public AliBar(IServiceProvider serviceProvider, 
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService) 
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【银联商务条码(alipay)支付】";
            AliBarOrderRQ bizRQ = (AliBarOrderRQ)rq;
            // 构造函数响应数据
            AliBarOrderRS res = ApiResBuilder.BuildSuccess<AliBarOrderRS>();

            JObject reqParams = new JObject();
            /* 支付方式
             * E_CASH – 电子现金
             * SOUNDWAVE – 声波
             * NFC – NFC
             * CODE_SCAN – 扫码
             * MANUAL – 手输
             * FACE_SCAN – 扫脸
             */
            reqParams.Add("payMode", "CODE_SCAN");
            reqParams.Add("payCode", bizRQ.AuthCode.Trim()); //授权码 通过扫码枪/声波获取设备获取的支付宝/微信/银联付款码

            // 业务处理

            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            AliBarOrderRQ bizRQ = (AliBarOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.AuthCode))
            {
                throw new BizException("用户支付条码[authCode]不可为空");
            }

            return null;
        }
    }
}
