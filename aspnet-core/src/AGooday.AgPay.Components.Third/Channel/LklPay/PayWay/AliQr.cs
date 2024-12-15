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

namespace AGooday.AgPay.Components.Third.Channel.LklPay.PayWay
{
    /// <summary>
    /// 拉卡拉 支付宝 二维码支付
    /// </summary>
    public class AliQr : LklPayPaymentService
    {
        public AliQr(ILogger<AliQr> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override async Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【拉卡拉(alipay)二维码支付】";
            AliQrOrderRQ bizRQ = (AliQrOrderRQ)rq;
            JObject reqParams = new JObject();
            AliQrOrderRS res = ApiResBuilder.BuildSuccess<AliQrOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 请求参数赋值
            UnifiedParamsSet(reqParams, payOrder, GetNotifyUrl(), GetReturnUrl());

            // 发送请求
            JObject resJSON = await PackageParamAndReqAsync("/api/v3/labs/trans/preorder", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string code = resJSON?.GetValue("code").ToString(); //请求响应码
            string msg = resJSON?.GetValue("msg").ToString(); //响应信息
            try
            {
                if ("BBS00000".Equals(code))
                {
                    var respData = resJSON.GetValue("resp_data")?.ToObject<JObject>();
                    respData.TryGetString("merchant_no", out string merchantNo);//全局流水号
                    respData.TryGetString("trade_no", out string tradeNo);//全局流水号
                    var accRespFields = respData.GetValue("acc_resp_fields")?.ToObject<JObject>();
                    var payUrl = accRespFields?.GetValue("code").ToString();
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
                    channelRetMsg.ChannelOrderId = tradeNo;
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                }
                else if ("BBS11112".Equals(code) || "BBS11105".Equals(code) || "BBS10000".Equals(code))
                {
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                    channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = code;
                    channelRetMsg.ChannelErrMsg = msg;
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
