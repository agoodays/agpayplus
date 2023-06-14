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
using static SKIT.FlurlHttpClient.Wechat.Api.Models.ComponentTCBBatchCreateContainerServiceVersionRequest.Types;

namespace AGooday.AgPay.Payment.Api.Channel.SxfPay.PayWay
{
    public class AliBar : SxfPayPaymentService
    {
        public AliBar(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【随行付条码(alipay)支付】";
            AliBarOrderRQ bizRQ = (AliBarOrderRQ)rq;
            // 构造函数响应数据
            AliBarOrderRS res = ApiResBuilder.BuildSuccess<AliBarOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            JObject reqParams = new JObject();
            reqParams.Add("authCode", bizRQ.AuthCode.Trim()); //授权码 通过扫码枪/声波获取设备获取的支付宝/微信/银联付款码

            // 发送请求
            JObject resJSON = PackageParamAndReq("/order/reverseScan", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string code = resJSON.GetValue("code").ToString(); //请求响应码
            string msg = resJSON.GetValue("msg").ToString(); //响应信息
            try
            {
                if ("0000".Equals(code))
                {
                    string bizCode = resJSON.GetValue("bizCode").ToString(); //业务响应码
                    string bizMsg = resJSON.GetValue("bizMsg").ToString(); //业务响应信息
                    if ("0000".Equals(bizCode))
                    {
                        string tranSts = resJSON.GetValue("tranSts").ToString();
                        switch (tranSts)
                        {
                            case "SUCCESS":
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                                break;
                            case "FAIL":
                                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                                break;
                            case "PAYING":
                                channelRetMsg.ChannelState = ChannelState.WAITING;
                                channelRetMsg.IsNeedQuery = true; // 开启轮询查单;
                                break;
                        }
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
            AliBarOrderRQ bizRQ = (AliBarOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.AuthCode))
            {
                throw new BizException("用户支付条码[authCode]不可为空");
            }

            return null;
        }
    }
}
