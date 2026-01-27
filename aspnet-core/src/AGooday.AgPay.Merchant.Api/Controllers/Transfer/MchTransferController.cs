using AGooday.AgPay.AopSdk;
using AGooday.AgPay.AopSdk.Exceptions;
using AGooday.AgPay.AopSdk.Models;
using AGooday.AgPay.AopSdk.Request;
using AGooday.AgPay.AopSdk.Response;
using AGooday.AgPay.Application.Config;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Base.Api.Attributes;
using AGooday.AgPay.Base.Api.Authorization;
using AGooday.AgPay.Base.Api.Controllers;
using AGooday.AgPay.Base.Api.Models;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Cache.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Merchant.Api.Controllers.Transfer
{
    [Route("api/mchTransfers")]
    [ApiController, Authorize, NoLog]
    public class MchTransferController : CommonController
    {
        private readonly IPayInterfaceConfigService _payIfConfigService;
        private readonly IPayInterfaceDefineService _payIfDefineService;
        private readonly ISysConfigService _sysConfigService;
        private readonly IMchAppService _mchAppService;

        public MchTransferController(ILogger<MchTransferController> logger,
            ICacheService cacheService,
            IAuthService authService,
            IPayInterfaceConfigService payIfConfigService,
            IPayInterfaceDefineService payIfDefineService,
            IMchAppService mchAppService,
            ISysConfigService sysConfigService)
            : base(logger, cacheService, authService)
        {
            _payIfConfigService = payIfConfigService;
            _payIfDefineService = payIfDefineService;
            _mchAppService = mchAppService;
            _sysConfigService = sysConfigService;
        }

        /// <summary>
        /// 查询商户对应应用下支持的支付通道
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet, Route("ifCodes/{appId}")]
        [PermissionAuth(PermCode.MCH.ENT_MCH_TRANSFER_IF_CODE_LIST)]
        public async Task<ApiRes> IfCodeListAsync(string appId)
        {
            var ifCodes = (await _payIfConfigService.GetByInfoIdAsync(CS.INFO_TYPE.MCH_APP, appId)).Select(s => s.IfCode);
            if (ifCodes is null)
                return ApiRes.Ok(ifCodes);
            var result = await _payIfDefineService.GetByIfCodesAsync(ifCodes);
            return ApiRes.Ok(result);
        }

        [HttpGet, Route("channelUserId")]
        [PermissionAuth(PermCode.MCH.ENT_MCH_TRANSFER_CHANNEL_USER, PermCode.MCH.ENT_DIVISION_RECEIVER_ADD)]
        public async Task<ApiRes> ChannelUserIdAsync(string appId, string ifCode, string extParam)
        {
            var mchApp = await _mchAppService.GetByIdAsync(appId);
            if (mchApp == null || mchApp.State != CS.PUB_USABLE || !mchApp.MchNo.Equals(await GetCurrentMchNoAsync()))
            {
                throw new BizException("商户应用不存在或不可用");
            }
            var param = new JObject();
            param.Add("mchNo", await GetCurrentMchNoAsync());
            param.Add("appId", appId);
            param.Add("ifCode", ifCode);
            param.Add("extParam", extParam);
            param.Add("reqTime", DateTimeOffset.Now.ToUnixTimeSeconds());
            param.Add("version", "1.0");
            param.Add("signType", "MD5");

            DBApplicationConfig dbApplicationConfig = _sysConfigService.GetDBApplicationConfig();

            param.Add("redirectUrl", $"{dbApplicationConfig.MchSiteUrl}/api/anon/channelUserIdCallback");

            param.Add("sign", AgPayUtil.GetSign(param, mchApp.AppSecret));

            var url = URLUtil.AppendUrlQuery($"{dbApplicationConfig.PaySiteUrl}/api/channelUserId/jump", param);
            return ApiRes.Ok(url);
        }

        [HttpPost, Route("doTransfer")]
        [PermissionAuth(PermCode.MCH.ENT_MCH_PAY_TEST_DO)]
        public async Task<ApiRes> DoTransferAsync(TransferOrderModel transferOrder)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TransferOrderModel, TransferOrderCreateReqModel>());
            var mapper = config.CreateMapper();
            var model = mapper.Map<TransferOrderModel, TransferOrderCreateReqModel>(transferOrder);
            var mchApp = await _mchAppService.GetByIdAsync(model.AppId);
            if (mchApp == null || mchApp.State != CS.PUB_USABLE || !mchApp.MchNo.Equals(await GetCurrentMchNoAsync()))
            {
                throw new BizException("商户应用不存在或不可用");
            }
            TransferOrderCreateRequest request = new TransferOrderCreateRequest();
            model.MchNo = await GetCurrentMchNoAsync();
            model.AppId = mchApp.AppId;
            model.Currency = "CNY";
            model.Amount = AmountUtil.ConvertDollar2Cent(transferOrder.Amount);
            request.SetBizModel(model);

            var agPayClient = new AgPayClient(_sysConfigService.GetDBApplicationConfig().PaySiteUrl, mchApp.AppSecret);

            try
            {
                TransferOrderCreateResponse response = await agPayClient.ExecuteAsync(request);
                if (response.Code != 0)
                {
                    throw new BizException(response.Msg);
                }
                return ApiRes.Ok(response.Get());
            }
            catch (AgPayException e)
            {
                throw new BizException(e.Message);
            }
        }
    }
}
