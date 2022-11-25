using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params;

namespace AGooday.AgPay.Payment.Api.Models
{
    /// <summary>
    /// Isv支付参数信息 放置到内存， 避免多次查询操作
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

        /// <summary>
        /// 商户支付配置信息缓存
        /// </summary>
        public Dictionary<string, IsvParams> IsvParamsMap { get; set; } = new Dictionary<string, IsvParams>();

        /// <summary>
        /// 缓存支付宝client 对象
        /// </summary>
        public AlipayClientWrapper AlipayClientWrapper { get; set; }

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
    }
}
