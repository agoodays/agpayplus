using AGooday.AgPay.Application.Config;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Manager.Api.Controllers.Merchant
{
    [Route("api/mchChannel")]
    [ApiController, Authorize, NoLog]
    public class MchChannelController : CommonController
    {
        private readonly ISysConfigService _sysConfigService;
        private readonly IMchAppService _mchAppService;

        public MchChannelController(ILogger<MchChannelController> logger,
            IMchAppService mchAppService,
            ISysConfigService sysConfigService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            _sysConfigService = sysConfigService;
            _mchAppService = mchAppService;
        }

        [HttpGet, Route("channelUserId")]
        [PermissionAuth(PermCode.MGR.ENT_DIVISION_RECEIVER_ADD)]
        public ApiRes ChannelUserId(string appId, string ifCode, string extParam)
        {
            var mchApp = _mchAppService.GetById(appId);
            if (mchApp == null || mchApp.State != CS.PUB_USABLE)
            {
                throw new BizException("商户应用不存在或不可用");
            }
            var param = new JObject();
            param.Add("mchNo", mchApp.MchNo);
            param.Add("appId", appId);
            param.Add("ifCode", ifCode);
            param.Add("extParam", extParam);
            param.Add("reqTime", DateTimeOffset.Now.ToUnixTimeSeconds());
            param.Add("version", "1.0");
            param.Add("signType", "MD5");

            DBApplicationConfig dbApplicationConfig = _sysConfigService.GetDBApplicationConfig();

            param.Add("redirectUrl", $"{dbApplicationConfig.MgrSiteUrl}/api/anon/channelUserIdCallback");

            param.Add("sign", AgPayUtil.GetSign(param, mchApp.AppSecret));

            var url = URLUtil.AppendUrlQuery($"{dbApplicationConfig.PaySiteUrl}/api/channelUserId/jump", param);
            return ApiRes.Ok(url);
        }
    }
}
