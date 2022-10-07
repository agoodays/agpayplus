using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params;

namespace AGooday.AgPay.Payment.Api.Models
{
    /// <summary>
    /// Isv支付参数信息 放置到内存， 避免多次查询操作
    /// </summary>
    public class IsvConfigContext
    {
        /** isv信息缓存 */
        public string IsvNo { get; set; }
        public IsvInfoDto IsvInfo { get; set; }

        /** 商户支付配置信息缓存 */
        public Dictionary<string, IsvParams> IsvParamsMap { get; set; } = new Dictionary<string, IsvParams>();

        /** 缓存支付宝client 对象 **/
        public AlipayClientWrapper AlipayClientWrapper { get; set; }

        /** 缓存 wxServiceWrapper 对象 **/
        public WxServiceWrapper WxServiceWrapper { get; set; }

        /** 获取isv配置信息 **/
        public IsvParams GetIsvParamsByIfCode(string ifCode)
        {
            IsvParamsMap.TryGetValue(ifCode, out IsvParams isvParams);
            return isvParams;
        }

        /** 获取isv配置信息 **/
        public T GetIsvParamsByIfCode<T>(string ifCode) where T : IsvParams
        {
            IsvParamsMap.TryGetValue(ifCode, out IsvParams isvParams);
            return (T)isvParams;
        }
    }
}
