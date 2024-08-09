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

        public ConfigContextQueryService(IMchStoreService mchStoreService,
            IMchAppService mchAppService,
            IMchInfoService mchInfoService,
            IAgentInfoService agentInfoService,
            IIsvInfoService isvInfoService,
            IPayWayService payWayService,
            IPayInterfaceConfigService payInterfaceConfigService,
            ConfigContextService configContextService)
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

        public MchAppDto QueryMchApp(string mchNo, string mchAppId)
        {
            if (IsCache())
            {
                return _configContextService.GetMchAppConfigContext(mchNo, mchAppId).MchApp;
            }

            return _mchAppService.GetById(mchAppId, mchNo);
        }

        public MchStoreDto QueryMchStore(string mchNo, long? storeId)
        {
            if (!storeId.HasValue)
            {
                return null;
            }

            if (IsCache())
            {
                return _configContextService.GetMchInfoConfigContext(mchNo).GetMchStore(storeId.Value);
            }

            return _mchStoreService.GetById(storeId.Value, mchNo);
        }

        public MchAppConfigContext QueryMchInfoAndAppInfo(string mchNo, string mchAppId)
        {
            if (IsCache())
            {
                return _configContextService.GetMchAppConfigContext(mchNo, mchAppId);
            }

            var mchInfo = _mchInfoService.GetById(mchNo);
            var mchApp = QueryMchApp(mchNo, mchAppId);

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

        public AgentInfoDto QueryAgentInfo(MchAppConfigContext configContext)
        {
            if (IsCache())
            {
                return configContext.AgentConfigContext?.AgentInfo;
            }

            return _agentInfoService.GetById(configContext.MchInfo.AgentNo);
        }

        public IsvInfoDto QueryIsvInfo(MchAppConfigContext configContext)
        {
            if (IsCache())
            {
                return configContext.IsvConfigContext?.IsvInfo;
            }

            return _isvInfoService.GetById(configContext.MchInfo.IsvNo);
        }

        public NormalMchParams QueryNormalMchParams(string mchNo, string mchAppId, string ifCode)
        {
            if (IsCache())
            {
                return _configContextService.GetMchAppConfigContext(mchNo, mchAppId).GetNormalMchParamsByIfCode(ifCode);
            }

            // 查询商户的所有支持的参数配置
            var payInterfaceConfig = _payInterfaceConfigService.GetByInfoIdAndIfCode(CS.INFO_TYPE.MCH_APP, mchAppId, ifCode);

            if (payInterfaceConfig == null || payInterfaceConfig.State != CS.YES)
            {
                return null;
            }

            return NormalMchParams.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
        }

        public IsvSubMchParams QueryIsvSubMchParams(string mchNo, string mchAppId, string ifCode)
        {
            if (IsCache())
            {
                return _configContextService.GetMchAppConfigContext(mchNo, mchAppId).GetIsvsubMchParamsByIfCode<IsvSubMchParams>(ifCode);
            }

            // 查询商户的所有支持的参数配置
            var payInterfaceConfig = _payInterfaceConfigService.GetByInfoIdAndIfCode(CS.INFO_TYPE.MCH_APP, mchAppId, ifCode);

            if (payInterfaceConfig == null || payInterfaceConfig.State != CS.YES)
            {
                return null;
            }

            return IsvSubMchParams.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
        }

        public IsvParams QueryIsvParams(string isvNo, string ifCode)
        {
            if (IsCache())
            {
                IsvConfigContext isvConfigContext = _configContextService.GetIsvConfigContext(isvNo);
                return isvConfigContext == null ? null : isvConfigContext.GetIsvParamsByIfCode(ifCode);
            }

            // 查询商户的所有支持的参数配置
            var payInterfaceConfig = _payInterfaceConfigService.GetByInfoIdAndIfCode(CS.INFO_TYPE.ISV, isvNo, ifCode);

            if (payInterfaceConfig == null || payInterfaceConfig.State != CS.YES)
            {
                return null;
            }

            return IsvParams.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
        }

        public NormalMchOauth2Params QueryNormalMchOauth2Params(string mchNo, string mchAppId, string ifCode)
        {
            if (IsCache())
            {
                return _configContextService.GetMchAppConfigContext(mchNo, mchAppId).GetNormalMchOauth2ParamsByInfoId(mchAppId);
            }

            // 查询商户的所有支持的参数配置
            var payInterfaceConfig = _payInterfaceConfigService.GetByInfoIdAndIfCode(CS.INFO_TYPE.MCH_APP_OAUTH2, mchAppId, ifCode);

            if (payInterfaceConfig == null || payInterfaceConfig.State != CS.YES)
            {
                return null;
            }

            return NormalMchOauth2Params.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
        }

        public IsvSubMchOauth2Params QueryIsvSubMchOauth2Params(string mchNo, string mchAppId, string ifCode)
        {
            if (IsCache())
            {
                return _configContextService.GetMchAppConfigContext(mchNo, mchAppId).GetIsvsubMchOauth2ParamsByInfoId<IsvSubMchOauth2Params>(mchAppId);
            }

            // 查询商户的所有支持的参数配置
            var payInterfaceConfig = _payInterfaceConfigService.GetByInfoIdAndIfCode(CS.INFO_TYPE.MCH_APP_OAUTH2, mchAppId, ifCode);

            if (payInterfaceConfig == null || payInterfaceConfig.State != CS.YES)
            {
                return null;
            }

            return IsvSubMchOauth2Params.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
        }

        public PayInterfaceConfigDto QueryIsvPayIfConfig(string isvNo, string ifCode)
        {
            if (IsCache())
            {
                IsvConfigContext isvConfigContext = _configContextService.GetIsvConfigContext(isvNo);
                return isvConfigContext == null ? null : isvConfigContext.GetIsvPayIfConfigByIfCode(ifCode);
            }

            // 查询商户的所有支持的参数配置
            var payInterfaceConfig = _payInterfaceConfigService.GetByInfoIdAndIfCode(CS.INFO_TYPE.ISV, isvNo, ifCode);

            if (payInterfaceConfig == null || payInterfaceConfig.State != CS.YES)
            {
                return null;
            }

            return payInterfaceConfig;
        }

        public IsvOauth2Params QueryIsvOauth2Params(string isvNo, string infoId, string ifCode)
        {
            if (IsCache())
            {
                IsvConfigContext isvConfigContext = _configContextService.GetIsvConfigContext(isvNo);
                return isvConfigContext == null ? null : isvConfigContext.GetIsvOauth2ParamsByIfCodeAndInfoId(ifCode + (infoId ?? isvNo));
            }

            // 查询商户的所有支持的参数配置
            var payInterfaceConfig = _payInterfaceConfigService.GetByInfoIdAndIfCode(CS.INFO_TYPE.ISV_OAUTH2, (infoId ?? isvNo), ifCode);

            if (payInterfaceConfig == null || payInterfaceConfig.State != CS.YES)
            {
                return null;
            }

            return IsvOauth2Params.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
        }

        public AliPayClientWrapper GetAlipayClientWrapper(MchAppConfigContext mchAppConfigContext)
        {
            if (IsCache())
            {
                return _configContextService.GetMchAppConfigContext(mchAppConfigContext.MchNo, mchAppConfigContext.AppId).GetAlipayClientWrapper();
            }

            if (mchAppConfigContext.IsIsvSubMch())
            {
                AliPayIsvParams alipayParams = (AliPayIsvParams)QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, CS.IF_CODE.ALIPAY);
                return AliPayClientWrapper.BuildAlipayClientWrapper(alipayParams);
            }
            else
            {
                AliPayNormalMchParams alipayParams = (AliPayNormalMchParams)QueryNormalMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, CS.IF_CODE.ALIPAY);
                return AliPayClientWrapper.BuildAlipayClientWrapper(alipayParams);
            }
        }

        public WxServiceWrapper GetWxServiceWrapper(MchAppConfigContext mchAppConfigContext)
        {
            if (IsCache())
            {
                return _configContextService.GetMchAppConfigContext(mchAppConfigContext.MchNo, mchAppConfigContext.AppId).GetWxServiceWrapper();
            }

            if (mchAppConfigContext.IsIsvSubMch())
            {
                WxPayIsvParams wxParams = (WxPayIsvParams)QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, CS.IF_CODE.WXPAY);
                return WxServiceWrapper.BuildWxServiceWrapper(wxParams);
            }
            else
            {
                WxPayNormalMchParams wxParams = (WxPayNormalMchParams)QueryNormalMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, CS.IF_CODE.WXPAY);
                return WxServiceWrapper.BuildWxServiceWrapper(wxParams);
            }
        }

        public PayPalWrapper GetPaypalWrapper(MchAppConfigContext mchAppConfigContext)
        {
            if (IsCache())
            {
                return _configContextService.GetMchAppConfigContext(mchAppConfigContext.MchNo, mchAppConfigContext.AppId).GetPaypalWrapper();
            }
            PpPayNormalMchParams ppPayNormalMchParams = (PpPayNormalMchParams)QueryNormalMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, CS.IF_CODE.PPPAY); ;
            return PayPalWrapper.BuildPaypalWrapper(ppPayNormalMchParams);
        }

        public string GetWayTypeByWayCode(string wayCode)
        {
            var wayType = _payWayService.GetWayTypeByWayCode(wayCode);
            return wayType;
        }
    }
}
