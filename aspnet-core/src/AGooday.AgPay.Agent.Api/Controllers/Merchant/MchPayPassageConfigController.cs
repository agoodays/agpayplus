using AGooday.AgPay.Agent.Api.Attributes;
using AGooday.AgPay.Agent.Api.Authorization;
using AGooday.AgPay.Agent.Api.Models;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Cache.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AGooday.AgPay.Agent.Api.Controllers.Merchant
{
    /// <summary>
    /// 商户支付通道管理类
    /// </summary>
    [Route("api/mch/payPassages")]
    [ApiController, Authorize]
    public class MchPayPassageConfigController : CommonController
    {
        private readonly IMchPayPassageService _mchPayPassageService;
        private readonly IPayWayService _payWayService;
        private readonly IMchAppService _mchAppService;
        private readonly IMchInfoService _mchInfoService;

        public MchPayPassageConfigController(ILogger<MchPayPassageConfigController> logger,
            ICacheService cacheService,
            IAuthService authService,
            IMchPayPassageService mchPayPassageServic,
            IPayWayService payWayService,
            IMchAppService mchAppService,
            IMchInfoService mchInfoService)
            : base(logger, cacheService, authService)
        {
            _mchPayPassageService = mchPayPassageServic;
            _payWayService = payWayService;
            _mchAppService = mchAppService;
            _mchInfoService = mchInfoService;
        }

        /// <summary>
        /// 查询应用支付接口配置列表
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.AGENT.ENT_MCH_PAY_PASSAGE_LIST)]
        public async Task<ApiPageRes<MchPayPassagePayWayDto>> ListAsync(string appId, [FromQuery] PayWayQueryDto dto)
        {
            var payWays = await _payWayService.GetPaginatedDataAsync<MchPayPassagePayWayDto>(dto);
            if (payWays?.Count > 0)
            {
                // 支付方式代码集合
                var wayCodes = payWays.Select(s => s.WayCode).ToList();

                // 应用支付通道集合
                var mchPayPassages = _mchPayPassageService.GetByAppIdAndWayCodesAsNoTracking(appId, wayCodes);

                foreach (var payWay in payWays)
                {
                    payWay.PassageState = CS.NO;
                    foreach (var mchPayPassage in mchPayPassages)
                    {
                        if (payWay.WayCode.Equals(mchPayPassage.WayCode) && mchPayPassage.State == CS.YES)
                        {
                            payWay.PassageState = CS.YES;
                            break;
                        }
                    }
                }
            }
            return ApiPageRes<MchPayPassagePayWayDto>.Pages(payWays);
        }

        /// <summary>
        /// 根据appId、支付方式查询可用的支付接口列表
        /// </summary>
        /// <param name="isvNo"></param>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        [HttpGet, Route("availablePayInterface/{appId}/{wayCode}"), NoLog]
        [PermissionAuth(PermCode.AGENT.ENT_MCH_PAY_PASSAGE_CONFIG)]
        public async Task<ApiRes> AvailablePayInterfaceAsync(string appId, string wayCode, int pageNumber, int pageSize)
        {
            var mchApp = await _mchAppService.GetByIdAsync(appId);
            if (mchApp == null || mchApp.State != CS.YES)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            var mchInfo = await _mchInfoService.GetByIdAsync(mchApp.MchNo);
            if (mchInfo == null || mchInfo.State != CS.YES)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            // 根据支付方式查询可用支付接口列表
            var result = await _mchPayPassageService.SelectAvailablePayInterfaceListAsync(wayCode, appId, CS.INFO_TYPE.MCH_APP, mchInfo.Type, pageNumber, pageSize);
            return ApiPageRes<AvailablePayInterfaceDto>.Pages(result);
        }

        /// <summary>
        /// 应用支付通道配置
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("更新商户支付通道")]
        [PermissionAuth(PermCode.AGENT.ENT_MCH_PAY_PASSAGE_ADD)]
        public async Task<ApiRes> SaveOrUpdateAsync(ReqParamsModel model)
        {
            try
            {
                var s = Request.Body;
                List<MchPayPassageDto> mchPayPassages = JsonConvert.DeserializeObject<List<MchPayPassageDto>>(model.ReqParams);
                if (!(mchPayPassages?.Count > 0))
                {
                    throw new BizException("操作失败");
                }
                var mchApp = await _mchAppService.GetByIdAsync(mchPayPassages.First().AppId);
                if (mchApp == null || mchApp.State != CS.YES)
                {
                    return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
                }
                await _mchPayPassageService.SaveOrUpdateBatchSelfAsync(mchPayPassages, mchApp.MchNo);
                return ApiRes.Ok();
            }
            catch (Exception e)
            {
                return ApiRes.Fail(ApiCode.SYSTEM_ERROR, e.Message);
            }
        }
    }
}
