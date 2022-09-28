using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AGooday.AgPay.Common.Exceptions;

namespace AGooday.AgPay.Manager.Api.Controllers.Merchant
{
    /// <summary>
    /// 商户支付通道管理类
    /// </summary>
    [Route("/api/mch/payPassages")]
    [ApiController]
    public class MchPayPassageConfigController : ControllerBase
    {
        private readonly ILogger<MchPayInterfaceConfigController> _logger;
        private readonly IMchPayPassageService _mchPayPassageService;
        private readonly IPayWayService _payWayService;
        private readonly IMchAppService _mchAppService;
        private readonly IMchInfoService _mchInfoService;

        public MchPayPassageConfigController(ILogger<MchPayInterfaceConfigController> logger,
            IMchPayPassageService mchPayPassageServic,
            IPayWayService payWayService,
            IMchAppService mchAppService,
            IMchInfoService mchInfoService)
        {
            _logger = logger;
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
        [HttpGet]
        [Route("")]
        public ApiRes List(string appId, [FromQuery] PayWayQueryDto dto)
        {
            var payWays = _payWayService.GetPaginatedData<MchPayPassagePayWayDto>(dto);
            if (payWays?.Count() > 0)
            {
                // 支付方式代码集合
                var wayCodes = payWays.Select(s => s.WayCode).ToList();

                // 应用支付通道集合
                var mchPayPassages = _mchPayPassageService.GetAll(appId, wayCodes);

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
            return ApiRes.Ok(new { Records = payWays.ToList(), Total = payWays.TotalCount, Current = payWays.PageIndex, HasNext = payWays.HasNext });
        }

        /// <summary>
        /// 根据appId、支付方式查询可用的支付接口列表
        /// </summary>
        /// <param name="isvNo"></param>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("availablePayInterface/{appId}/{wayCode}")]
        public ApiRes AvailablePayInterface(string appId, string wayCode)
        {
            var mchApp = _mchAppService.GetById(appId);
            if (mchApp == null || mchApp.State != CS.YES)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            var mchInfo = _mchInfoService.GetById(mchApp.MchNo);
            if (mchInfo == null || mchInfo.State != CS.YES)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            // 根据支付方式查询可用支付接口列表
            var result = _mchPayPassageService.SelectAvailablePayInterfaceList(wayCode, appId, CS.INFO_TYPE_MCH_APP, mchInfo.Type);
            return ApiRes.Ok(result);
        }

        /// <summary>
        /// 应用支付通道配置
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public ApiRes SaveOrUpdate(List<MchPayPassageDto> mchPayPassages)
        {
            if (!(mchPayPassages?.Count() > 0))
            {
                throw new BizException("操作失败");
            }
            var mchApp = _mchAppService.GetById(mchPayPassages.First().AppId);
            if (mchApp == null || mchApp.State != CS.YES)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            _mchPayPassageService.SaveOrUpdateBatchSelf(mchPayPassages, mchApp.MchNo);
            return ApiRes.Ok();
        }
    }
}
