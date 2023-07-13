using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.Services;
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

            // 业务处理
            JObject reqParams = new JObject();
            reqParams.Add("transactionAmount", payOrder.Amount);
            reqParams.Add("transactionCurrencyCode", "156");
            reqParams.Add("merchantOrderId", payOrder.PayOrderId);
            reqParams.Add("merchantRemark", payOrder.Subject);
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
            reqParams.Add("deviceType", "11");

            /* 经度 长度⇐10。
             * 实体类终端（设备类型非01,11,12,13）：经纬度与基站信息cust2字段必传其一；
             * 非实体类终端（设备类型为01,11,12,13）：经纬度与ip字段必传其一。
             * 格式： 1 位正负号+3 位整数+1 位小数点+5 位小数；
             * 对于正负号：+表示东经， -表示西经。例如-121.48352
             */
            // reqParams.Add("longitude", "");
            /* 纬度 长度⇐10。
             * 实体类终端（设备类型非01,11,12,13）：经纬度与基站信息cust2字段必传其一；
             * 非实体类终端（设备类型为01,11,12,13）：经纬度与ip字段必传其一。
             * 格式： 1 位正负号 + 2 位整数 + 1 位小数点 + 6 位小数；
             * 对于正负号：+表示北纬， -表示南纬。例如 + 31.221345或 - 03.561345
             */
            // reqParams.Add("latitude", "");
            // 基站信息 注：实体类终端（设备类型非01,11,12,13）如无经纬度，该字段必送
            // reqParams.Add("cust2", "");
            // 终端设备IP地址 长度⇐64, 非实体类终端（设备类型为01,11,12,13）如无经纬度，该字段必送；格式如：“ip”:“172.20.11.089”
            reqParams.Add("ip", payOrder.ClientIp);

            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            // 发送请求
            JObject resJSON = PackageParamAndReq("/v6/poslink/transaction/pay", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string errCode = resJSON.GetValue("errCode").ToString(); // 错误代码
            string errInfo = resJSON.GetValue("errInfo").ToString(); // 错误说明
            try
            {
                switch (errCode)
                {
                    case "0000":
                    case "00":
                    case "SUCCESS":
                    default:
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                        channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                        break;
                }
            }
            catch (Exception e)
            {
                channelRetMsg.ChannelErrCode = errCode;
                channelRetMsg.ChannelErrMsg = errInfo;
            }
            res.ChannelRetMsg = channelRetMsg;
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
