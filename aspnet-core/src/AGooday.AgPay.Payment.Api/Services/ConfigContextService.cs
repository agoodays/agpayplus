using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.AliPay;
using AGooday.AgPay.Application.Params.PpPay;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Application.Params;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Models;
using System;
using System.Collections.Generic;
using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Payment.Api.Services
{
    public class ConfigContextService
    {
        /** <商户ID, 商户配置项>  **/
        private static Dictionary<string, MchInfoConfigContext> mchInfoConfigContextMap = new Dictionary<string, MchInfoConfigContext>();

        /** <应用ID, 商户配置上下文>  **/
        private static Dictionary<string, MchAppConfigContext> mchAppConfigContextMap = new Dictionary<string, MchAppConfigContext>();

        /** <服务商号, 服务商配置上下文>  **/
        private static Dictionary<string, IsvConfigContext> isvConfigContextMap = new Dictionary<string, IsvConfigContext>();

        private readonly IMchAppService _mchAppService;
        private readonly IMchInfoService _mchInfoService;
        private readonly IIsvInfoService _isvInfoService;
        private readonly IPayInterfaceConfigService _payInterfaceConfigService;

        public ConfigContextService(IMchAppService mchAppService, IMchInfoService mchInfoService, IIsvInfoService isvInfoService, IPayInterfaceConfigService payInterfaceConfigService)
        {
            _mchAppService = mchAppService;
            _mchInfoService = mchInfoService;
            _isvInfoService = isvInfoService;
            _payInterfaceConfigService = payInterfaceConfigService;
        }

        /** 获取 [商户配置信息] **/
        public MchInfoConfigContext GetMchInfoConfigContext(string mchNo)
        {
            mchInfoConfigContextMap.TryGetValue(mchNo, out MchInfoConfigContext _mchInfoConfigContext);

            //无此数据， 需要初始化
            if (_mchInfoConfigContext == null)
            {
                InitMchInfoConfigContext(mchNo);
            }

            mchInfoConfigContextMap.TryGetValue(mchNo, out _mchInfoConfigContext);
            return _mchInfoConfigContext;
        }

        /** 获取 [商户应用支付参数配置信息] **/
        public MchAppConfigContext GetMchAppConfigContext(string mchNo, string appId)
        {
            mchAppConfigContextMap.TryGetValue(mchNo, out MchAppConfigContext _mchAppConfigContext);

            //无此数据， 需要初始化
            if (_mchAppConfigContext == null)
            {
                InitMchAppConfigContext(mchNo, appId);
            }

            mchAppConfigContextMap.TryGetValue(mchNo, out _mchAppConfigContext);
            return _mchAppConfigContext;
        }

        /** 获取 [ISV支付参数配置信息] **/
        public IsvConfigContext GetIsvConfigContext(string isvNo)
        {
            isvConfigContextMap.TryGetValue(isvNo, out IsvConfigContext _isvConfigContext);

            //无此数据， 需要初始化
            if (_isvConfigContext == null)
            {
                InitIsvConfigContext(isvNo);
            }
            isvConfigContextMap.TryGetValue(isvNo, out _isvConfigContext);
            return _isvConfigContext;
        }

        /** 初始化 [商户配置信息] **/
        public void InitMchInfoConfigContext(string mchNo)
        {
            // 当前系统不进行缓存
            if (!IsCache())
            {
                return;
            }

            //商户主体信息
            var mchInfo = _mchInfoService.GetById(mchNo);
            // 查询不到商户主体， 可能已经删除
            if (mchInfo == null)
            {
                mchInfoConfigContextMap.TryGetValue(mchNo, out MchInfoConfigContext _mchInfoConfigContext);

                // 删除所有的商户应用
                if (_mchInfoConfigContext != null)
                {
                    foreach (var item in _mchInfoConfigContext.AppMap)
                    {
                        mchAppConfigContextMap.Remove(item.Key);
                    }
                }

                mchInfoConfigContextMap.Remove(mchNo);
                return;
            }

            MchInfoConfigContext mchInfoConfigContext = new MchInfoConfigContext();

            // 设置商户信息
            mchInfoConfigContext.MchNo = mchInfo.MchNo;
            mchInfoConfigContext.MchType = mchInfo.Type;
            mchInfoConfigContext.MchInfo = mchInfo;
            _mchAppService.GetAll().Where(w => w.MchNo.Equals(mchNo)).ToList().ForEach(mchApp =>
            {
                //1. 更新商户内appId集合
                mchInfoConfigContext.PutMchApp(mchApp);

                mchAppConfigContextMap.TryGetValue(mchApp.AppId, out MchAppConfigContext mchAppConfigContext);
                if (mchAppConfigContext != null)
                {
                    mchAppConfigContext.MchApp = mchApp;
                    mchAppConfigContext.MchNo = mchInfo.MchNo;
                    mchAppConfigContext.MchType = mchInfo.Type;
                    mchAppConfigContext.MchInfo = mchInfo;
                }
            });

            mchInfoConfigContextMap.Add(mchNo, mchInfoConfigContext);
        }

        /** 初始化 [商户应用支付参数配置信息] **/
        public void InitMchAppConfigContext(string mchNo, string appId)
        {
            // 当前系统不进行缓存
            if (!IsCache())
            {
                return;
            }

            // 获取商户的配置信息
            MchInfoConfigContext mchInfoConfigContext = GetMchInfoConfigContext(mchNo);
            // 商户信息不存在
            if (mchInfoConfigContext == null)
            {
                return;
            }

            // 查询商户应用信息主体
            var dbMchApp = _mchAppService.GetById(appId);

            //DB已经删除
            if (dbMchApp == null)
            {
                mchAppConfigContextMap.Remove(appId);  //清除缓存信息
                mchInfoConfigContext.AppMap.Remove(appId); //清除主体信息中的appId
                return;
            }


            // 商户应用mchNo 与参数不匹配
            if (!dbMchApp.MchNo.Equals(mchNo))
            {
                return;
            }

            //更新商户信息主体中的商户应用
            mchInfoConfigContext.PutMchApp(dbMchApp);

            //商户主体信息
            var mchInfo = mchInfoConfigContext.MchInfo;
            MchAppConfigContext mchAppConfigContext = new MchAppConfigContext();

            // 设置商户信息
            mchAppConfigContext.AppId = appId;
            mchAppConfigContext.MchNo = mchInfo.MchNo;
            mchAppConfigContext.MchType = mchInfo.Type;
            mchAppConfigContext.MchInfo = mchInfo;
            mchAppConfigContext.MchApp = dbMchApp;

            // 查询商户的所有支持的参数配置
            var allConfigList = _payInterfaceConfigService.GetByInfoId(CS.INFO_TYPE_MCH_APP, appId);

            // 普通商户
            if (mchInfo.Type == CS.MCH_TYPE_NORMAL)
            {
                foreach (var payInterfaceConfig in allConfigList)
                {
                    mchAppConfigContext.NormalMchParamsMap.Add(
                        payInterfaceConfig.IfCode,
                        NormalMchParams.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams)
                    );
                }

                //放置alipay client

                AliPayNormalMchParams alipayParams = mchAppConfigContext.GetNormalMchParamsByIfCode<AliPayNormalMchParams>(CS.IF_CODE.ALIPAY);
                if (alipayParams != null)
                {
                    mchAppConfigContext.AlipayClientWrapper = AlipayClientWrapper.BuildAlipayClientWrapper(alipayParams);
                }

                //放置 wxJavaService
                WxPayNormalMchParams wxpayParams = mchAppConfigContext.GetNormalMchParamsByIfCode<WxPayNormalMchParams>(CS.IF_CODE.WXPAY);
                if (wxpayParams != null)
                {
                    mchAppConfigContext.WxServiceWrapper = WxServiceWrapper.BuildWxServiceWrapper(wxpayParams);
                }

                //放置 paypal client
                PpPayNormalMchParams ppPayMchParams = mchAppConfigContext.GetNormalMchParamsByIfCode<PpPayNormalMchParams>(CS.IF_CODE.PPPAY);
                if (ppPayMchParams != null)
                {
                    mchAppConfigContext.PaypalWrapper = PaypalWrapper.BuildPaypalWrapper(ppPayMchParams);
                }


            }
            else
            {
                //服务商模式商户
                foreach (var payInterfaceConfig in allConfigList)
                {
                    mchAppConfigContext.IsvSubMchParamsMap.Add(
                            payInterfaceConfig.IfCode,
                            IsvSubMchParams.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams)
                    );
                }

                //放置 当前商户的 服务商信息
                mchAppConfigContext.IsvConfigContext = GetIsvConfigContext(mchInfo.IsvNo);

            }

            mchAppConfigContextMap.Add(appId, mchAppConfigContext);
        }

        /** 初始化 [ISV支付参数配置信息]  **/
        public void InitIsvConfigContext(string isvNo)
        {

            if (!IsCache())
            { // 当前系统不进行缓存
                return;
            }

            //查询出所有商户的配置信息并更新
            var mchNoList = _mchInfoService.GetAll().Where(w => w.IsvNo.Equals(isvNo)).Select(s => s.MchNo);

            // 查询出所有 所属当前服务商的所有应用集合
            IEnumerable<string> mchAppIdList = new List<string>();
            if (mchNoList?.Count() > 0)
            {
                mchAppIdList = _mchAppService.GetAll().Where(w => mchNoList.Contains(w.MchNo)).Select(s => s.AppId);
            }

            IsvConfigContext isvConfigContext = new IsvConfigContext();
            var isvInfo = _isvInfoService.GetById(isvNo);
            if (isvInfo == null)
            {

                foreach (var appId in mchAppIdList)
                {
                    //将更新已存在缓存的商户配置信息 （每个商户下存储的为同一个 服务商配置的对象指针）
                    mchAppConfigContextMap.TryGetValue(appId, out MchAppConfigContext mchAppConfigContext);
                    if (mchAppConfigContext != null)
                    {
                        mchAppConfigContext.IsvConfigContext = null;
                    }
                }

                isvConfigContextMap.Remove(isvNo); // 服务商有商户不可删除， 此处不再更新商户下的配置信息
                return;
            }

            // 设置商户信息
            isvConfigContext.IsvNo = isvInfo.IsvNo;
            isvConfigContext.IsvInfo = isvInfo;

            // 查询商户的所有支持的参数配置
            var allConfigList = _payInterfaceConfigService.GetByInfoId(CS.INFO_TYPE_ISV, isvNo);

            foreach (var payInterfaceConfig in allConfigList)
            {
                isvConfigContext.IsvParamsMap.Add(
                        payInterfaceConfig.IfCode,
                        IsvParams.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams)
                );
            }

            //放置alipay client
            AliPayIsvParams alipayParams = isvConfigContext.GetIsvParamsByIfCode<AliPayIsvParams>(CS.IF_CODE.ALIPAY);
            if (alipayParams != null)
            {
                isvConfigContext.AlipayClientWrapper = AlipayClientWrapper.BuildAlipayClientWrapper(alipayParams);
            }

            //放置 wxJavaService
            WxPayIsvParams wxpayParams = isvConfigContext.GetIsvParamsByIfCode<WxPayIsvParams>(CS.IF_CODE.WXPAY);
            if (wxpayParams != null)
            {
                isvConfigContext.WxServiceWrapper = WxServiceWrapper.BuildWxServiceWrapper(wxpayParams);
            }

            isvConfigContextMap.Add(isvNo, isvConfigContext);

            //查询出所有商户的配置信息并更新
            foreach (string appId in mchAppIdList)
            {
                //将更新已存在缓存的商户配置信息 （每个商户下存储的为同一个 服务商配置的对象指针）
                mchAppConfigContextMap.TryGetValue(appId, out MchAppConfigContext mchAppConfigContext);
                if (mchAppConfigContext != null)
                {
                    mchAppConfigContext.IsvConfigContext = isvConfigContext;
                }
            }
        }

        private bool IsCache()
        {
            return SysConfigService.IS_USE_CACHE;
        }
    }
}
