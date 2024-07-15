using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params;
using AGooday.AgPay.Common.Enumerator;

namespace AGooday.AgPay.Payment.Api.Models
{
    /// <summary>
    /// 商户应用支付参数信息
    /// 放置到内存， 避免多次查询操作
    /// </summary>
    public class MchAppConfigContext
    {
        #region 商户信息缓存
        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 商户类型: 1-普通商户, 2-特约商户(服务商模式)
        /// </summary>
        public byte MchType { get; set; }
        /// <summary>
        /// 商户信息
        /// </summary>
        public MchInfoDto MchInfo { get; set; }
        /// <summary>
        /// 商户应用信息
        /// </summary>
        public MchAppDto MchApp { get; set; }
        #endregion

        #region 商户支付配置信息缓存,  <接口代码, 支付参数>
        /// <summary>
        /// 普通商户支付配置信息缓存
        /// </summary>
        public Dictionary<string, NormalMchParams> NormalMchParamsMap { get; set; } = new Dictionary<string, NormalMchParams>();
        /// <summary>
        /// 特约商户支付配置信息缓存
        /// </summary>
        public Dictionary<string, IsvSubMchParams> IsvSubMchParamsMap { get; set; } = new Dictionary<string, IsvSubMchParams>();
        #endregion

        #region 商户Oauth2配置信息缓存,  <接口代码, 支付参数>
        /// <summary>
        /// 普通商户Oauth2配置信息缓存
        /// </summary>
        public Dictionary<string, NormalMchOauth2Params> NormalMchOauth2ParamsMap { get; set; } = new Dictionary<string, NormalMchOauth2Params>();
        /// <summary>
        /// 特约商户Oauth2配置信息缓存
        /// </summary>
        public Dictionary<string, IsvSubMchOauth2Params> IsvSubMchOauth2ParamsMap { get; set; } = new Dictionary<string, IsvSubMchOauth2Params>();
        #endregion

        /// <summary>
        /// 放置所属代理商的信息
        /// </summary>
        public AgentConfigContext AgentConfigContext { get; set; }

        /// <summary>
        /// 放置所属服务商的信息
        /// </summary>
        public IsvConfigContext IsvConfigContext { get; set; }

        /// <summary>
        /// 缓存 Paypal 对象
        /// </summary>
        public PayPalWrapper PaypalWrapper { get; set; }

        /// <summary>
        /// 缓存支付宝client 对象
        /// </summary>
        public AliPayClientWrapper AlipayClientWrapper { get; set; }

        /// <summary>
        /// 缓存 wxServiceWrapper 对象
        /// </summary>
        public WxServiceWrapper WxServiceWrapper { get; set; }

        /// <summary>
        /// 获取普通商户配置信息
        /// </summary>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        public NormalMchParams GetNormalMchParamsByIfCode(string ifCode)
        {
            NormalMchParamsMap.TryGetValue(ifCode, out NormalMchParams normalMchParams);
            return normalMchParams;
        }

        /// <summary>
        /// 获取普通商户配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        public T GetNormalMchParamsByIfCode<T>(string ifCode) where T : NormalMchParams
        {
            NormalMchParamsMap.TryGetValue(ifCode, out NormalMchParams normalMchParams);
            return (T)normalMchParams;
        }

        /// <summary>
        /// 获取特约商户配置信息
        /// </summary>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        public IsvSubMchParams GetIsvSubMchParamsByIfCode(string ifCode)
        {
            IsvSubMchParamsMap.TryGetValue(ifCode, out IsvSubMchParams isvSubMchParams);
            return isvSubMchParams;
        }

        /// <summary>
        /// 获取特约商户配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        public T GetIsvsubMchParamsByIfCode<T>(string ifCode) where T : IsvSubMchParams
        {
            IsvSubMchParamsMap.TryGetValue(ifCode, out IsvSubMchParams isvSubMchParams);
            return (T)isvSubMchParams;
        }

        /// <summary>
        /// 获取普通商户Oauth2配置信息
        /// </summary>
        /// <param name="infoId"></param>
        /// <returns></returns>
        public NormalMchOauth2Params GetNormalMchOauth2ParamsByInfoId(string infoId)
        {
            NormalMchOauth2ParamsMap.TryGetValue(infoId, out NormalMchOauth2Params normalMchOauth2Params);
            return normalMchOauth2Params;
        }

        /// <summary>
        /// 获取普通商户Oauth2配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="infoId"></param>
        /// <returns></returns>
        public T GetNormalMchOauth2ParamsByInfoId<T>(string infoId) where T : NormalMchOauth2Params
        {
            NormalMchOauth2ParamsMap.TryGetValue(infoId, out NormalMchOauth2Params normalMchOauth2Params);
            return (T)normalMchOauth2Params;
        }

        /// <summary>
        /// 获取特约商户Oauth2配置信息
        /// </summary>
        /// <param name="infoId"></param>
        /// <returns></returns>
        public IsvSubMchOauth2Params GetIsvSubMchOauth2ParamsByInfoId(string infoId)
        {
            IsvSubMchOauth2ParamsMap.TryGetValue(infoId, out IsvSubMchOauth2Params isvSubMchOauth2Params);
            return isvSubMchOauth2Params;
        }

        /// <summary>
        /// 获取特约商户Oauth2配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="infoId"></param>
        /// <returns></returns>
        public T GetIsvsubMchOauth2ParamsByInfoId<T>(string infoId) where T : IsvSubMchOauth2Params
        {
            IsvSubMchOauth2ParamsMap.TryGetValue(infoId, out IsvSubMchOauth2Params isvSubMchOauth2Params);
            return (T)isvSubMchOauth2Params;
        }

        /// <summary>
        ///  是否为服务商特约商户
        /// </summary>
        /// <returns></returns>
        public bool IsIsvSubMch()
        {
            return this.MchType == (byte)MchInfoType.TYPE_ISVSUB;
        }

        public AliPayClientWrapper GetAlipayClientWrapper()
        {
            return IsIsvSubMch() ? this.IsvConfigContext.AlipayClientWrapper : this.AlipayClientWrapper;
        }

        public WxServiceWrapper GetWxServiceWrapper()
        {
            return IsIsvSubMch() ? this.IsvConfigContext.WxServiceWrapper : this.WxServiceWrapper;
        }

        public PayPalWrapper GetPaypalWrapper()
        {
            return this.PaypalWrapper;
        }
    }
}
