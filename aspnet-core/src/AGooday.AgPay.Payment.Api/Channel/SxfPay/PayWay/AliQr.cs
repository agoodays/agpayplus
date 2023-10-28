using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.SxfPay.Enumerator;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.SxfPay.PayWay
{
    /// <summary>
    /// 随行付 支付宝 二维码支付
    /// </summary>
    public class AliQr : SxfPayPaymentService
    {
        public AliQr(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【随行付(alipay)二维码支付】";
            AliQrOrderRQ bizRQ = (AliQrOrderRQ)rq;
            JObject reqParams = new JObject();
            AliQrOrderRS res = ApiResBuilder.BuildSuccess<AliQrOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 请求参数赋值
            SxfPublicParams(reqParams, payOrder);
            string payType = SxfPayEnum.GetPayType(payOrder.WayCode);
            /*支付渠道，枚举值
            取值范围：
            WECHAT 微信
            ALIPAY 支付宝
            UNIONPAY 银联*/
            reqParams.Add("payType", payType);
            reqParams.Add("notifyUrl", GetNotifyUrl()); //支付结果通知地址不上送则交易成功后，无异步交易结果通知

            // 发送请求
            JObject resJSON = PackageParamAndReq("/order/activeScan", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string code = resJSON.GetValue("code").ToString(); //请求响应码
            string msg = resJSON.GetValue("msg").ToString(); //响应信息
            string orgId = resJSON.GetValue("orgId").ToString(); //天阙平台机构编号
            channelRetMsg.ChannelIsvNo = orgId;
            try
            {
                if ("0000".Equals(code))
                {
                    var respData = resJSON.GetValue("respData").ToObject<JObject>();
                    string bizCode = respData.GetValue("bizCode").ToString(); //业务响应码
                    string bizMsg = respData.GetValue("bizMsg").ToString(); //业务响应信息
                    if ("0000".Equals(bizCode))
                    {
                        string mno = respData.GetValue("mno").ToString();//商户编号
                        string uuid = respData.GetValue("uuid").ToString();//天阙平台订单号
                        /*落单号
                        仅供退款使用
                        消费者账单中的条形码订单号*/
                        respData.TryGetString("sxfUuid", out string sxfUuid);
                        string payUrl = respData.GetValue("payUrl").ToString();
                        //二维码地址
                        if (CS.PAY_DATA_TYPE.CODE_IMG_URL.Equals(bizRQ.PayDataType))
                        {
                            res.CodeImgUrl = _sysConfigService.GetDBApplicationConfig().GenScanImgUrl(payUrl);
                        }
                        else
                        {
                            //默认都为跳转地址方式
                            res.CodeUrl = payUrl;
                        }
                        channelRetMsg.ChannelOrderId = uuid;
                        channelRetMsg.ChannelState = ChannelState.WAITING;
                    }
                    else
                    {
                        channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                        channelRetMsg.ChannelErrCode = bizCode;
                        channelRetMsg.ChannelErrMsg = bizMsg;
                    }
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                    channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                }
            }
            catch (Exception e)
            {
                channelRetMsg.ChannelErrCode = code;
                channelRetMsg.ChannelErrMsg = msg;
            }
            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            return null;
        }
    }
}
