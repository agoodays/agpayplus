using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Application.Params;

namespace AGooday.AgPay.Manager.Api.Controllers.Merchant
{
    /// <summary>
    /// 服务商支付接口管理类
    /// </summary>
    [Route("/api/mch/payConfigs")]
    [ApiController]
    public class MchPayInterfaceConfigController : ControllerBase
    {
        private readonly ILogger<MchPayInterfaceConfigController> _logger;
        private readonly IPayInterfaceConfigService _payIfConfigService;
        private readonly IMchAppService _mchAppService;
        private readonly IMchInfoService _mchInfoService;

        public MchPayInterfaceConfigController(ILogger<MchPayInterfaceConfigController> logger,
            IPayInterfaceConfigService payIfConfigService,
            IMchAppService mchAppService,
            IMchInfoService mchInfoService)
        {
            _logger = logger;
            _payIfConfigService = payIfConfigService;
            _mchAppService = mchAppService;
            _mchInfoService = mchInfoService;
        }

        /// <summary>
        /// 查询应用支付接口配置列表
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public ApiRes List(string appId)
        {
            var data = _payIfConfigService.SelectAllPayIfConfigListByAppId(appId);
            return ApiRes.Ok(data);
        }

        /// <summary>
        /// 根据 appId、接口类型 获取应用参数配置
        /// </summary>
        /// <param name="isvNo"></param>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{appId}/{ifCode}")]
        public ApiRes GetByAppId(string appId, string ifCode)
        {
            var payInterfaceConfig = _payIfConfigService.GetByInfoIdAndIfCode(CS.INFO_TYPE_MCH_APP, appId, ifCode);
            if (payInterfaceConfig != null)
            {
                // 费率转换为百分比数值
                payInterfaceConfig.IfRate = payInterfaceConfig.IfRate * 100;

                // 敏感数据脱敏
                if (!string.IsNullOrWhiteSpace(payInterfaceConfig.IfParams))
                {
                    var mchApp = _mchAppService.GetById(appId);
                    var mchInfo = _mchInfoService.GetById(mchApp.MchNo);

                    // 普通商户的支付参数执行数据脱敏
                    if (mchInfo.Type == CS.MCH_TYPE_NORMAL)
                    {
                        NormalMchParams mchParams = NormalMchParams.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
                        if (mchParams != null)
                        {
                            payInterfaceConfig.IfParams = mchParams.DeSenData();
                        }
                    }
                }
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 应用支付接口配置
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public ApiRes SaveOrUpdate(PayInterfaceConfigDto dto)
        {
            return ApiRes.Ok();
        }

        [HttpGet]
        [Route("alipayIsvsubMchAuthUrls/{mchAppId}")]
        public ApiRes QueryAlipayIsvsubMchAuthUrl(string mchAppId)
        {
            return ApiRes.Ok(new { authUrl = "", authQrImgUrl = "" });
        }
    }
}
