using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params;

namespace AGooday.AgPay.Components.Third.Models
{
    /// <summary>
    /// Isv支付参数信息
    /// 放置到内存， 避免多次查询操作
    /// </summary>
    public class IsvConfigContext
    {
        #region isv信息缓存
        /// <summary>
        /// 服务商号
        /// </summary>
        public string IsvNo { get; set; }
        /// <summary>
        /// 服务商信息
        /// </summary>
        public IsvInfoDto IsvInfo { get; set; }
        #endregion

        #region 服务商支付配置信息缓存,  <接口代码, 支付参数>
        /// <summary>
        /// 服务商支付配置信息缓存
        /// </summary>
        public Dictionary<string, IsvParams> IsvParamsMap { get; set; } = new Dictionary<string, IsvParams>();
        #endregion

        #region 服务商If配置信息缓存,  <接口代码, If配置信息>
        /// <summary>
        /// 服务商Oauth2
        /// </summary>
        public Dictionary<string, PayInterfaceConfigDto> IsvPayIfConfigMap { get; set; } = new Dictionary<string, PayInterfaceConfigDto>();
        #endregion

        #region 服务商Oauth2配置信息缓存,  <接口代码+InfoId, 支付参数>
        /// <summary>
        /// 服务商Oauth2配置信息缓存,  <InfoId, 支付参数>
        /// </summary>
        public Dictionary<string, IsvOauth2Params> IsvOauth2ParamsMap { get; set; } = new Dictionary<string, IsvOauth2Params>();
        #endregion

        /// <summary>
        /// 缓存支付宝client 对象
        /// </summary>
        public AliPayClientWrapper AlipayClientWrapper { get; set; }

        /// <summary>
        /// 缓存 wxServiceWrapper 对象
        /// </summary>
        public WxServiceWrapper WxServiceWrapper { get; set; }

        /// <summary>
        /// 获取isv配置信息
        /// </summary>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        public IsvParams GetIsvParamsByIfCode(string ifCode)
        {
            IsvParamsMap.TryGetValue(ifCode, out IsvParams isvParams);
            return isvParams;
        }

        /// <summary>
        /// 获取isv配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        public T GetIsvParamsByIfCode<T>(string ifCode) where T : IsvParams
        {
            IsvParamsMap.TryGetValue(ifCode, out IsvParams isvParams);
            return (T)isvParams;
        }

        /// <summary>
        /// 获取isv If配置信息
        /// </summary>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        public PayInterfaceConfigDto GetIsvPayIfConfigByIfCode(string ifCode)
        {
            IsvPayIfConfigMap.TryGetValue(ifCode, out PayInterfaceConfigDto isvOauth2Params);
            return isvOauth2Params;
        }

        /// <summary>
        /// 获取isv Oauth2配置信息
        /// </summary>
        /// <param name="infoId"></param>
        /// <returns></returns>
        public IsvOauth2Params GetIsvOauth2ParamsByIfCodeAndInfoId(string ifCodeAndInfoId)
        {
            IsvOauth2ParamsMap.TryGetValue(ifCodeAndInfoId, out IsvOauth2Params isvOauth2Params);
            return isvOauth2Params;
        }

        /// <summary>
        /// 获取isv Oauth2配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="infoId"></param>
        /// <returns></returns>
        public T GetIsvOauth2ParamsByIfCodeAndInfoId<T>(string ifCodeAndInfoId) where T : IsvOauth2Params
        {
            IsvOauth2ParamsMap.TryGetValue(ifCodeAndInfoId, out IsvOauth2Params isvOauth2Params);
            return (T)isvOauth2Params;
        }
    }
}
