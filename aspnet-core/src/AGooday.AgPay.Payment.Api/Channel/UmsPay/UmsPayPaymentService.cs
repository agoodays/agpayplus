using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.UmsPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Payment.Api.Channel.UmsPay.Utils;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.Services;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Channel.UmsPay
{
    /// <summary>
    /// 银联商务下单
    /// </summary>
    public class UmsPayPaymentService : AbstractPaymentService
    {
        private readonly ILog log = LogManager.GetLogger(typeof(UmsPayPaymentService));

        public UmsPayPaymentService(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.UMSPAY;
        }

        public override bool IsSupport(string wayCode)
        {
            return true;
        }

        public override AbstractRS Pay(UnifiedOrderRQ bizRQ, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            throw new NotImplementedException();
        }

        public override string PreCheck(UnifiedOrderRQ bizRQ, PayOrderDto payOrder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取正式环境/沙箱HOST地址
        /// </summary>
        /// <param name="isvParams"></param>
        /// <returns></returns>
        public static string GetUmsPayHost4env(UmsPayIsvParams isvParams)
        {
            return CS.YES == isvParams.Sandbox ? UmsPayConfig.SANDBOX_SERVER_URL : UmsPayConfig.PROD_SERVER_URL;
        }

        /// <summary>
        /// 封装参数 & 统一请求
        /// </summary>
        /// <param name="apiUri"></param>
        /// <param name="reqParams"></param>
        /// <param name="logPrefix"></param>
        /// <param name="mchAppConfigContext"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        public JObject PackageParamAndReq(string apiUri, JObject reqParams, string logPrefix, MchAppConfigContext mchAppConfigContext, bool isBarPay = false)
        {
            UmsPayIsvParams isvParams = (UmsPayIsvParams)_configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

            if (string.IsNullOrWhiteSpace(isvParams?.AppId) || string.IsNullOrWhiteSpace(isvParams?.AppKey))
            {
                log.Error($"服务商配置为空：isvParams：{JsonConvert.SerializeObject(isvParams)}");
                throw new BizException("服务商配置为空。");
            }

            UmsPayIsvSubMchParams isvsubMchParams = (UmsPayIsvSubMchParams)_configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());

            reqParams.Add(isBarPay ? "merchantCode" : "mid", isvsubMchParams.Mid); //商户号
            reqParams.Add(isBarPay ? "terminalCode" : "tid", isvsubMchParams.Tid); //终端号

            // 调起上游接口
            log.Info($"{logPrefix} reqJSON={reqParams}");
            string resText = UmsHttpUtil.DoPostJson(GetUmsPayHost4env(isvParams) + apiUri, isvParams.AppId, isvParams.AppKey, reqParams);
            log.Info($"{logPrefix} resJSON={resText}");

            if (string.IsNullOrWhiteSpace(resText))
            {
                return null;
            }

            var resParams = JObject.Parse(resText);
            return resParams;
        }
    }
}
