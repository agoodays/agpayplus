using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Payment.Api.Authorization;
using AGooday.AgPay.Payment.Api.Channel;
using AGooday.AgPay.Payment.Api.Controllers.PayOrder;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Controllers.Qr
{
    /// <summary>
    /// 商户获取渠道用户ID接口
    /// </summary>
    [Route("api/channelUserId")]
    [ApiController]
    public class ChannelUserIdController : AbstractPayOrderController
    {
        private readonly Func<string, IChannelUserService> _channelUserServiceFactory;

        public ChannelUserIdController(ILogger<ChannelUserIdController> logger, 
            Func<string, IChannelUserService> channelUserServiceFactory,
            Func<string, IPaymentService> paymentServiceFactory,
            PayOrderProcessService payOrderProcessService,
            IMchPayPassageService mchPayPassageService,
            IPayRateConfigService payRateConfigService,
            IPayWayService payWayService,
            IPayOrderService payOrderService,
            IPayOrderProfitService payOrderProfitService,
            ISysConfigService sysConfigService,
            IMQSender mqSender,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, paymentServiceFactory, payOrderProcessService, mchPayPassageService, payRateConfigService, payWayService, payOrderService, payOrderProfitService, sysConfigService, mqSender, requestKit, configContextQueryService)
        {
            _channelUserServiceFactory = channelUserServiceFactory;
        }

        /// <summary>
        /// 重定向到微信地址
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("jump")]
        [PermissionAuth(PermCode.PAY.API_CHANNEL_USER)]
        public ActionResult Jump()
        {
            //获取请求数据
            ChannelUserIdRQ rq = GetRQByWithMchSign<ChannelUserIdRQ>();
            string ifCode = "AUTO".Equals(rq.IfCode, StringComparison.OrdinalIgnoreCase) ? GetIfCodeByUA() : rq.IfCode;

            IChannelUserService channelUserService = _channelUserServiceFactory(ifCode);

            if (channelUserService == null)
            {
                throw new BizException("不支持的客户端");
            }

            if (StringUtil.IsAvailableUrl(rq.RedirectUrl))
            {
                throw new BizException("跳转地址有误！");
            }

            JObject jsonObject = new JObject();
            jsonObject.Add("mchNo", rq.MchNo);
            jsonObject.Add("appId", rq.AppId);
            jsonObject.Add("extParam", rq.ExtParam);
            jsonObject.Add("ifCode", ifCode);
            jsonObject.Add("redirectUrl", rq.RedirectUrl);

            //回调地址
            string callbackUrl = _sysConfigService.GetDBApplicationConfig().GenMchChannelUserIdApiOauth2RedirectUrlEncode(jsonObject);

            //获取商户配置信息
            MchAppConfigContext mchAppConfigContext = _configContextQueryService.QueryMchInfoAndAppInfo(rq.MchNo, rq.AppId);
            string redirectUrl = channelUserService.BuildUserRedirectUrl(callbackUrl, mchAppConfigContext);

            return Redirect(redirectUrl);
        }

        /// <summary>
        /// 回调地址
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("oauth2Callback/{aesData}")]
        public ActionResult Oauth2Callback(string aesData)
        {
            JObject callbackData = JObject.Parse(AgPayUtil.AesDecode(aesData));

            string mchNo = callbackData.GetValue("mchNo").ToString();
            string appId = callbackData.GetValue("appId").ToString();
            string ifCode = callbackData.GetValue("ifCode").ToString();
            string extParam = callbackData.GetValue("extParam").ToString();
            string redirectUrl = callbackData.GetValue("redirectUrl").ToString();

            // 获取接口
            IChannelUserService channelUserService = _channelUserServiceFactory(ifCode);

            if (channelUserService == null)
            {
                throw new BizException("不支持的客户端");
            }

            //获取商户配置信息
            MchAppConfigContext mchAppConfigContext = _configContextQueryService.QueryMchInfoAndAppInfo(mchNo, appId);

            //获取渠道用户ID
            string channelUserId = channelUserService.GetChannelUserId(GetReqParamJson(), mchAppConfigContext);

            //同步跳转
            JObject appendParams = new JObject();
            appendParams.Add("appId", appId);
            appendParams.Add("channelUserId", channelUserId);
            appendParams.Add("extParam", extParam);
            return Redirect(URLUtil.AppendUrlQuery(redirectUrl, appendParams));
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true), Route("param")]
        public async Task<ActionResult> ParamAsync()
        {
            var param = await GetReqParamToJsonAsync();
            return Ok(param);
        }

        /// <summary>
        /// 根据UA获取支付接口
        /// </summary>
        /// <returns></returns>
        private string GetIfCodeByUA()
        {
            string ua = Request.Headers.UserAgent.FirstOrDefault();

            // 无法识别扫码客户端
            if (string.IsNullOrEmpty(ua))
            {
                return null;
            }

            if (ua.Contains("AlipayClient"))
            {
                return CS.IF_CODE.ALIPAY;  //支付宝服务窗支付
            }
            else if (ua.Contains("MicroMessenger"))
            {
                return CS.IF_CODE.WXPAY;
            }
            return null;
        }
    }
}
