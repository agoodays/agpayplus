using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.AllinPay.PayWay
{
    /// <summary>
    /// 通联 微信 Native
    /// </summary>
    public class WxNative : AllinPayPaymentService
    {
        public WxNative(ILogger<WxNative> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【通联(wechat)二维码支付】";
            WxNativeOrderRQ bizRQ = (WxNativeOrderRQ)rq;
            JObject reqParams = new JObject();
            WxNativeOrderRS res = ApiResBuilder.BuildSuccess<WxNativeOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 请求参数赋值
            UnifiedParamsSet(reqParams, payOrder, GetNotifyUrl(), GetReturnUrl());

            // 发送请求
            JObject resJSON = PackageParamAndReq("/apiweb/unitorder/pay", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string code = resJSON.GetValue("retcode").ToString(); //请求响应码
            string msg = resJSON.GetValue("retmsg").ToString(); //响应信息
            reqParams.TryGetString("cusid", out string cusid); // 商户号
            channelRetMsg.ChannelMchNo = cusid;
            //channelRetMsg.ChannelIsvNo = orgid;
            try
            {
                if ("SUCCESS".Equals(code))
                {
                    string trxid = resJSON.GetValue("trxid").ToString();
                    string payUrl = resJSON.GetValue("payinfo").ToString();
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
                    channelRetMsg.ChannelOrderId = trxid;
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                    channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                }
            }
            catch (Exception)
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
