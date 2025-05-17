using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params;
using AGooday.AgPay.Application.Params.AliPay;
using AGooday.AgPay.Application.Params.PpPay;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Components.Third.Models;

namespace AGooday.AgPay.Components.Third.Services
{
    public class ConfigContextQueryService
    {
        private readonly ConfigContextService _configContextService;
        private readonly IMchStoreService _mchStoreService;
        private readonly IMchAppService _mchAppService;
        private readonly IMchInfoService _mchInfoService;
        private readonly IAgentInfoService _agentInfoService;
        private readonly IIsvInfoService _isvInfoService;
        private readonly IPayWayService _payWayService;
        private readonly IPayInterfaceConfigService _payInterfaceConfigService;

        public ConfigContextQueryService(ConfigContextService configContextService,
            IMchStoreService mchStoreService,
            IMchAppService mchAppService,
            IMchInfoService mchInfoService,
            IAgentInfoService agentInfoService,
            IIsvInfoService isvInfoService,
            IPayWayService payWayService,
            IPayInterfaceConfigService payInterfaceConfigService)
        {
            _mchStoreService = mchStoreService;
            _mchAppService = mchAppService;
            _mchInfoService = mchInfoService;
            _agentInfoService = agentInfoService;
            _isvInfoService = isvInfoService;
            _payWayService = payWayService;
            _payInterfaceConfigService = payInterfaceConfigService;
            _configContextService = configContextService;
        }

        private static bool IsCache() => SysConfigService.IS_USE_CACHE;

        public async Task<MchAppDto> QueryMchAppAsync(string mchNo, string mchAppId)
        {
            if (IsCache())
            {
                return (await _configContextService.GetMchAppConfigContextAsync(mchNo, mchAppId)).MchApp;
            }

            return await _mchAppService.GetByIdAsNoTrackingAsync(mchAppId, mchNo);
        }

        public async Task<MchStoreDto> QueryMchStoreAsync(string mchNo, long? storeId)
        {
            if (!storeId.HasValue)
            {
                return null;
            }

            if (IsCache())
            {
                return (await _configContextService.GetMchInfoConfigContextAsync(mchNo)).GetMchStore(storeId.Value);
            }

            return await _mchStoreService.GetByIdAsNoTrackingAsync(storeId.Value, mchNo);
        }

        public async Task<MchAppConfigContext> QueryMchInfoAndAppInfoAsync(string mchNo, string mchAppId)
        {
            if (IsCache())
            {
                return await _configContextService.GetMchAppConfigContextAsync(mchNo, mchAppId);
            }

            var mchInfo = await _mchInfoService.GetByIdAsNoTrackingAsync(mchNo);
            var mchApp = await QueryMchAppAsync(mchNo, mchAppId);

            if (mchInfo == null || mchApp == null)
            {
                return null;
            }

            MchAppConfigContext result = new MchAppConfigContext();
            result.MchInfo = mchInfo;
            result.MchNo = mchNo;
            result.MchType = mchInfo.Type;

            result.MchApp = mchApp;
            result.AppId = mchAppId;

            return result;
        }

        public async Task<AgentInfoDto> QueryAgentInfoAsync(MchAppConfigContext configContext)
        {
            if (IsCache())
            {
                return configContext.AgentConfigContext?.AgentInfo;
            }

            return await _agentInfoService.GetByIdAsNoTrackingAsync(configContext.MchInfo.AgentNo);
        }

        public async Task<IsvInfoDto> QueryIsvInfoAsync(MchAppConfigContext configContext)
        {
            if (IsCache())
            {
                return configContext.IsvConfigContext?.IsvInfo;
            }

            return await _isvInfoService.GetByIdAsNoTrackingAsync(configContext.MchInfo.IsvNo);
        }

        public async Task<NormalMchParams> QueryNormalMchParamsAsync(string mchNo, string mchAppId, string ifCode)
        {
            if (IsCache())
            {
                return (await _configContextService.GetMchAppConfigContextAsync(mchNo, mchAppId)).GetNormalMchParamsByIfCode(ifCode);
            }

            // 查询商户的所有支持的参数配置
            var payInterfaceConfig = await _payInterfaceConfigService.GetByInfoIdAndIfCodeAsync(CS.INFO_TYPE.MCH_APP, mchAppId, ifCode);

            if (payInterfaceConfig == null || payInterfaceConfig.State != CS.YES)
            {
                return null;
            }

            return NormalMchParams.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
        }

        public async Task<IsvSubMchParams> QueryIsvSubMchParamsAsync(string mchNo, string mchAppId, string ifCode)
        {
            if (IsCache())
            {
                return (await _configContextService.GetMchAppConfigContextAsync(mchNo, mchAppId)).GetIsvsubMchParamsByIfCode<IsvSubMchParams>(ifCode);
            }

            // 查询商户的所有支持的参数配置
            var payInterfaceConfig = await _payInterfaceConfigService.GetByInfoIdAndIfCodeAsync(CS.INFO_TYPE.MCH_APP, mchAppId, ifCode);

            if (payInterfaceConfig == null || payInterfaceConfig.State != CS.YES)
            {
                return null;
            }

            return IsvSubMchParams.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
        }

        public async Task<IsvParams> QueryIsvParamsAsync(string isvNo, string ifCode)
        {
            if (IsCache())
            {
                IsvConfigContext isvConfigContext = await _configContextService.GetIsvConfigContextAsync(isvNo);
                return isvConfigContext == null ? null : isvConfigContext.GetIsvParamsByIfCode(ifCode);
            }

            // 查询商户的所有支持的参数配置
            var payInterfaceConfig = await _payInterfaceConfigService.GetByInfoIdAndIfCodeAsync(CS.INFO_TYPE.ISV, isvNo, ifCode);

            if (payInterfaceConfig == null || payInterfaceConfig.State != CS.YES)
            {
                return null;
            }

            return IsvParams.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
        }

        public async Task<NormalMchOauth2Params> QueryNormalMchOauth2ParamsAsync(string mchNo, string mchAppId, string ifCode)
        {
            if (IsCache())
            {
                return (await _configContextService.GetMchAppConfigContextAsync(mchNo, mchAppId)).GetNormalMchOauth2ParamsByInfoId(mchAppId);
            }

            // 查询商户的所有支持的参数配置
            var payInterfaceConfig = await _payInterfaceConfigService.GetByInfoIdAndIfCodeAsync(CS.INFO_TYPE.MCH_APP_OAUTH2, mchAppId, ifCode);

            if (payInterfaceConfig == null || payInterfaceConfig.State != CS.YES)
            {
                return null;
            }

            return NormalMchOauth2Params.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
        }

        public async Task<IsvSubMchOauth2Params> QueryIsvSubMchOauth2ParamsAsync(string mchNo, string mchAppId, string ifCode)
        {
            if (IsCache())
            {
                return (await _configContextService.GetMchAppConfigContextAsync(mchNo, mchAppId)).GetIsvsubMchOauth2ParamsByInfoId<IsvSubMchOauth2Params>(mchAppId);
            }

            // 查询商户的所有支持的参数配置
            var payInterfaceConfig = await _payInterfaceConfigService.GetByInfoIdAndIfCodeAsync(CS.INFO_TYPE.MCH_APP_OAUTH2, mchAppId, ifCode);

            if (payInterfaceConfig == null || payInterfaceConfig.State != CS.YES)
            {
                return null;
            }

            return IsvSubMchOauth2Params.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
        }

        public async Task<PayInterfaceConfigDto> QueryIsvPayIfConfigAsync(string isvNo, string ifCode)
        {
            if (IsCache())
            {
                IsvConfigContext isvConfigContext = await _configContextService.GetIsvConfigContextAsync(isvNo);
                return isvConfigContext == null ? null : isvConfigContext.GetIsvPayIfConfigByIfCode(ifCode);
            }

            // 查询商户的所有支持的参数配置
            var payInterfaceConfig = await _payInterfaceConfigService.GetByInfoIdAndIfCodeAsync(CS.INFO_TYPE.ISV, isvNo, ifCode);

            if (payInterfaceConfig == null || payInterfaceConfig.State != CS.YES)
            {
                return null;
            }

            return payInterfaceConfig;
        }

        public async Task<IsvOauth2Params> QueryIsvOauth2ParamsAsync(string isvNo, string infoId, string ifCode)
        {
            if (IsCache())
            {
                IsvConfigContext isvConfigContext = await _configContextService.GetIsvConfigContextAsync(isvNo);
                return isvConfigContext == null ? null : isvConfigContext.GetIsvOauth2ParamsByIfCodeAndInfoId(ifCode + (infoId ?? isvNo));
            }

            // 查询商户的所有支持的参数配置
            var payInterfaceConfig = await _payInterfaceConfigService.GetByInfoIdAndIfCodeAsync(CS.INFO_TYPE.ISV_OAUTH2, (infoId ?? isvNo), ifCode);

            if (payInterfaceConfig == null || payInterfaceConfig.State != CS.YES)
            {
                return null;
            }

            return IsvOauth2Params.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
        }

        public async Task<AliPayClientWrapper> GetAlipayClientWrapperAsync(MchAppConfigContext mchAppConfigContext)
        {
            if (IsCache())
            {
                return (await _configContextService.GetMchAppConfigContextAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId)).GetAlipayClientWrapper();
            }

            if (mchAppConfigContext.IsIsvSubMch())
            {
                AliPayIsvParams alipayParams = (AliPayIsvParams)await QueryIsvParamsAsync(mchAppConfigContext.MchInfo.IsvNo, CS.IF_CODE.ALIPAY);
                return AliPayClientWrapper.BuildAlipayClientWrapper(alipayParams);
            }
            else
            {
                AliPayNormalMchParams alipayParams = (AliPayNormalMchParams)await QueryNormalMchParamsAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, CS.IF_CODE.ALIPAY);
                return AliPayClientWrapper.BuildAlipayClientWrapper(alipayParams);
            }
        }

        public async Task<WxServiceWrapper> GetWxServiceWrapperAsync(MchAppConfigContext mchAppConfigContext, string apiVersion = null)
        {
            if (IsCache() && apiVersion == null)
            {
                return (await _configContextService.GetMchAppConfigContextAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId)).GetWxServiceWrapper();
            }

            if (mchAppConfigContext.IsIsvSubMch())
            {
                WxPayIsvParams wxParams = (WxPayIsvParams)await QueryIsvParamsAsync(mchAppConfigContext.MchInfo.IsvNo, CS.IF_CODE.WXPAY);
                return WxServiceWrapper.BuildWxServiceWrapper(wxParams, apiVersion);
            }
            else
            {
                WxPayNormalMchParams wxParams = (WxPayNormalMchParams)await QueryNormalMchParamsAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, CS.IF_CODE.WXPAY);
                return WxServiceWrapper.BuildWxServiceWrapper(wxParams, apiVersion);
            }
        }

        public async Task<PayPalWrapper> GetPaypalWrapperAsync(MchAppConfigContext mchAppConfigContext)
        {
            if (IsCache())
            {
                return (await _configContextService.GetMchAppConfigContextAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId)).GetPaypalWrapper();
            }
            PpPayNormalMchParams ppPayNormalMchParams = (PpPayNormalMchParams)await QueryNormalMchParamsAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, CS.IF_CODE.PPPAY); ;
            return PayPalWrapper.BuildPaypalWrapper(ppPayNormalMchParams);
        }

        public async Task<string> GetWayTypeByWayCodeAsync(string wayCode)
        {
            var wayType = await _payWayService.GetWayTypeByWayCodeAsync(wayCode);
            return wayType;
        }
    }
}
