using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.YsfPay.PayWay
{
    public class WxBar : YsfPayPaymentService
    {
        /// <summary>
        /// 云闪付 微信 条码支付
        /// </summary>
        /// <param name="serviceProvider"></param>
        public WxBar(ILogger<WxBar> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override async Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【云闪付条码(wechat)支付】";
            WxBarOrderRQ bizRQ = (WxBarOrderRQ)rq;
            // 构造函数响应数据
            WxBarOrderRS res = ApiResBuilder.BuildSuccess<WxBarOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            JObject reqParams = new JObject();
            reqParams.Add("authCode", bizRQ.AuthCode.Trim()); //付款码： 用户 APP 展示的付款条码或二维码

            // 云闪付 bar 统一参数赋值
            BarParamsSet(reqParams, payOrder);

            //客户端IP
            reqParams.Add("termInfo", JsonConvert.SerializeObject(new { ip = !string.IsNullOrWhiteSpace(payOrder.ClientIp) ? payOrder.ClientIp : "127.0.0.1" })); //终端信息

            // 发送请求
            JObject resJSON = await PackageParamAndReqAsync("/gateway/api/pay/micropay", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string respCode = resJSON.GetValue("respCode").ToString(); //应答码
            string respMsg = resJSON.GetValue("respMsg").ToString(); //应答信息
            try
            {
                //00-交易成功， 02-用户支付中 , 12-交易重复， 需要发起查询处理    其他认为失败
                if ("00".Equals(respCode))
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                    res.PayData = resJSON.GetValue("payData").ToString();
                }
                else if ("02".Equals(respCode) || "12".Equals(respCode) || "99".Equals(respCode))
                {
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                    channelRetMsg.IsNeedQuery = true; // 开启轮询查单
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = respCode;
                    channelRetMsg.ChannelErrMsg = respMsg;
                }
            }
            catch (Exception)
            {
                channelRetMsg.ChannelErrCode = respCode;
                channelRetMsg.ChannelErrMsg = respMsg;
            }
            return res;
        }

        public override Task<string> PreCheckAsync(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            WxBarOrderRQ bizRQ = (WxBarOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.AuthCode))
            {
                throw new BizException("用户支付条码[authCode]不可为空");
            }

            return Task.FromResult<string>(null);
        }
    }
}
