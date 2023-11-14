using AGooday.AgPay.Application.Params.AliPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.Services;
using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;

namespace AGooday.AgPay.Payment.Api.Channel.AliPay.Kits
{
    /// <summary>
    /// 【支付宝】支付通道工具包
    /// </summary>
    public class AliPayKit
    {
        public static IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// 放置 isv特殊信息
        /// </summary>
        /// <param name="mchAppConfigContext"></param>
        /// <param name="req"></param>
        /// <param name="model"></param>
        public static void PutApiIsvInfo<T>(MchAppConfigContext mchAppConfigContext, IAopRequest<T> req, AopObject model) where T : AopResponse
        {
            //不是特约商户， 无需放置此值
            if (!mchAppConfigContext.IsIsvSubMch())
            {
                return;
            }

            ConfigContextQueryService configContextQueryService = ServiceProvider.GetService<ConfigContextQueryService>();

            // 获取支付参数
            AliPayIsvParams isvParams = (AliPayIsvParams)configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, CS.IF_CODE.ALIPAY);
            AliPayIsvSubMchParams isvsubMchParams = (AliPayIsvSubMchParams)configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, CS.IF_CODE.ALIPAY);

            // 子商户信息
            if (req is AlipayTradePayRequest)
            {
                ((AlipayTradePayRequest)req).PutOtherTextParam("app_auth_token", isvsubMchParams.AppAuthToken);
            }
            else if (typeof(AlipayTradeAppPayRequest).IsInstanceOfType(req))
            {
                ((AlipayTradeAppPayRequest)req).PutOtherTextParam("app_auth_token", isvsubMchParams.AppAuthToken);
            }
            else if (typeof(AlipayTradeCreateRequest).IsInstanceOfType(req))
            {
                ((AlipayTradeCreateRequest)req).PutOtherTextParam("app_auth_token", isvsubMchParams.AppAuthToken);
            }
            else if (typeof(AlipayTradePagePayRequest).IsInstanceOfType(req))
            {
                ((AlipayTradePagePayRequest)req).PutOtherTextParam("app_auth_token", isvsubMchParams.AppAuthToken);
            }
            else if (typeof(AlipayTradePrecreateRequest).IsInstanceOfType(req))
            {
                ((AlipayTradePrecreateRequest)req).PutOtherTextParam("app_auth_token", isvsubMchParams.AppAuthToken);
            }
            else if (typeof(AlipayTradeWapPayRequest).IsInstanceOfType(req))
            {
                ((AlipayTradeWapPayRequest)req).PutOtherTextParam("app_auth_token", isvsubMchParams.AppAuthToken);
            }
            else if (typeof(AlipayTradeQueryRequest).IsInstanceOfType(req))
            {
                ((AlipayTradeQueryRequest)req).PutOtherTextParam("app_auth_token", isvsubMchParams.AppAuthToken);
            }
            else if (typeof(AlipayTradeRefundRequest).IsInstanceOfType(req))
            {
                ((AlipayTradeRefundRequest)req).PutOtherTextParam("app_auth_token", isvsubMchParams.AppAuthToken);
            }
            else if (typeof(AlipayTradeFastpayRefundQueryRequest).IsInstanceOfType(req))
            {
                ((AlipayTradeFastpayRefundQueryRequest)req).PutOtherTextParam("app_auth_token", isvsubMchParams.AppAuthToken);
            }
            else if (typeof(AlipayFundTransToaccountTransferRequest).IsInstanceOfType(req))
            {
                ((AlipayFundTransToaccountTransferRequest)req).PutOtherTextParam("app_auth_token", isvsubMchParams.AppAuthToken);
            }
            else if (typeof(AlipayTradeRoyaltyRelationBindRequest).IsInstanceOfType(req))
            {
                ((AlipayTradeRoyaltyRelationBindRequest)req).PutOtherTextParam("app_auth_token", isvsubMchParams.AppAuthToken);
            }
            else if (typeof(AlipayTradeOrderSettleRequest).IsInstanceOfType(req))
            {
                ((AlipayTradeOrderSettleRequest)req).PutOtherTextParam("app_auth_token", isvsubMchParams.AppAuthToken);
            }
            else if (typeof(AlipayTradeCloseRequest).IsInstanceOfType(req))
            {
                ((AlipayTradeCloseRequest)req).PutOtherTextParam("app_auth_token", isvsubMchParams.AppAuthToken);
            }
            else if (typeof(AlipayTradeOrderSettleQueryRequest).IsInstanceOfType(req))
            {
                ((AlipayTradeOrderSettleQueryRequest)req).PutOtherTextParam("app_auth_token", isvsubMchParams.AppAuthToken);
            }

            // 服务商信息
            ExtendParams extendParams = new ExtendParams();
            extendParams.SysServiceProviderId = isvParams.Pid;

            if (typeof(AlipayTradePayModel).IsInstanceOfType(model))
            {
                ((AlipayTradePayModel)model).ExtendParams = extendParams;
            }
            else if (typeof(AlipayTradeAppPayModel).IsInstanceOfType(model))
            {
                ((AlipayTradeAppPayModel)model).ExtendParams = extendParams;
            }
            else if (typeof(AlipayTradeCreateModel).IsInstanceOfType(model))
            {
                ((AlipayTradeCreateModel)model).ExtendParams = extendParams;
            }
            else if (typeof(AlipayTradePagePayModel).IsInstanceOfType(model))
            {
                ((AlipayTradePagePayModel)model).ExtendParams = extendParams;
            }
            else if (typeof(AlipayTradePrecreateModel).IsInstanceOfType(model))
            {
                ((AlipayTradePrecreateModel)model).ExtendParams = extendParams;
            }
            else if (typeof(AlipayTradeWapPayModel).IsInstanceOfType(model))
            {
                ((AlipayTradeWapPayModel)model).ExtendParams = extendParams;
            }
        }

        public static string AppendErrCode(string code, string subCode)
        {
            return !string.IsNullOrWhiteSpace(subCode) ? subCode : code; //优先： subCode
        }

        public static string AppendErrMsg(string msg, string subMsg)
        {

            string result = null;
            if (StringUtil.IsAllNotNullOrWhiteSpace(msg, subMsg))
            {
                result = msg + "【" + subMsg + "】";
            }
            else
            {
                result = !string.IsNullOrWhiteSpace(subMsg) ? subMsg : msg;
            }
            return result.Length > 253 ? $"{result.Substring(0, 253)}..." : result;
        }
    }
}
