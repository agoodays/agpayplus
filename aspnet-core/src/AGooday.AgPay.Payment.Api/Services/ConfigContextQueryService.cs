using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params;
using AGooday.AgPay.Application.Params.AliPay;
using AGooday.AgPay.Application.Params.PpPay;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Models;

namespace AGooday.AgPay.Payment.Api.Services
{
    public class ConfigContextQueryService
    {
        private readonly ConfigContextService _configContextService;
        private readonly IMchAppService _mchAppService;
        private readonly IMchInfoService _mchInfoService;
        private readonly IIsvInfoService _isvInfoService;
        private readonly IPayInterfaceConfigService _payInterfaceConfigService;

        public ConfigContextQueryService(IMchAppService mchAppService, IMchInfoService mchInfoService, IIsvInfoService isvInfoService, IPayInterfaceConfigService payInterfaceConfigService, ConfigContextService configContextService)
        {
            _mchAppService = mchAppService;
            _mchInfoService = mchInfoService;
            _isvInfoService = isvInfoService;
            _payInterfaceConfigService = payInterfaceConfigService;
            _configContextService = configContextService;
        }

        private bool IsCache()
        {
            return SysConfigService.IS_USE_CACHE;
        }

        public MchAppDto QueryMchApp(string mchNo, string mchAppId)
        {
            if (IsCache())
            {
                return _configContextService.GetMchAppConfigContext(mchNo, mchAppId).MchApp;
            }

            return _mchAppService.GetById(mchAppId, mchNo);
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

        public NormalMchParams QueryNormalMchParams(string mchNo, string mchAppId, string ifCode)
        {
            if (IsCache())
            {
                return _configContextService.GetMchAppConfigContext(mchNo, mchAppId).GetNormalMchParamsByIfCode(ifCode);
            }

            // 查询商户的所有支持的参数配置
            var payInterfaceConfig = _payInterfaceConfigService.GetByInfoIdAndIfCode(CS.INFO_TYPE_MCH_APP, mchAppId, ifCode);

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
            var payInterfaceConfig = _payInterfaceConfigService.GetByInfoIdAndIfCode(CS.INFO_TYPE_MCH_APP, mchAppId, ifCode);

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
            var payInterfaceConfig = _payInterfaceConfigService.GetByInfoIdAndIfCode(CS.INFO_TYPE_ISV, isvNo, ifCode);

            if (payInterfaceConfig == null || payInterfaceConfig.State != CS.YES)
            {
                return null;
            }

            return IsvParams.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
        }

        public AlipayClientWrapper GetAlipayClientWrapper(MchAppConfigContext mchAppConfigContext)
        {
            if (IsCache())
            {
                return _configContextService.GetMchAppConfigContext(mchAppConfigContext.MchNo, mchAppConfigContext.AppId).GetAlipayClientWrapper();
            }

            if (mchAppConfigContext.IsIsvSubMch())
            {
                AliPayIsvParams alipayParams = (AliPayIsvParams)QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, CS.IF_CODE.ALIPAY);
                return AlipayClientWrapper.BuildAlipayClientWrapper(alipayParams);
            }
            else
            {
                AliPayNormalMchParams alipayParams = (AliPayNormalMchParams)QueryNormalMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, CS.IF_CODE.ALIPAY);
                return AlipayClientWrapper.BuildAlipayClientWrapper(alipayParams);
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
    }
}
