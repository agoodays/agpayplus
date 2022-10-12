
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Utils;
using Aop.Api.Request;
using Aop.Api.Domain;
using Aop.Api.Response;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Payment.Api.Channel.AliPay;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using Aop.Api.Util;
using Newtonsoft.Json;

namespace AGooday.AgPay.Payment.Api.Channel.YsfPay.PayWay
{
    public class AliBar : YsfPayPaymentService
    {
        /// <summary>
        /// 支付宝 条码支付
        /// </summary>
        /// <param name="serviceProvider"></param>
        public AliBar(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【云闪付条码(alipay)支付】";
            AliBarOrderRQ bizRQ = (AliBarOrderRQ)rq;
            // 构造函数响应数据
            AliBarOrderRS res = ApiResBuilder.BuildSuccess<AliBarOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            JObject reqParams = new JObject();
            reqParams.Add("authCode", bizRQ.AuthCode.Trim()); //付款码： 用户 APP 展示的付款条码或二维码
            // 云闪付 bar 统一参数赋值
            BarParamsSet(reqParams, payOrder);

            //客户端IP
            reqParams.Add("termInfo", JsonConvert.SerializeObject(new { ip = !string.IsNullOrWhiteSpace(payOrder.ClientIp) ? payOrder.ClientIp : "127.0.0.1" })); //终端信息
            // 发送请求
            JObject resJSON = PackageParamAndReq("/gateway/api/pay/micropay", reqParams, logPrefix, mchAppConfigContext);
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
            catch (Exception e)
            {
                channelRetMsg.ChannelErrCode = respCode;
                channelRetMsg.ChannelErrMsg = respMsg;
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
