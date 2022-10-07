using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params;
using AGooday.AgPay.Common.Enumerator;

namespace AGooday.AgPay.Payment.Api.Models
{
    public class MchAppConfigContext
    {
        /** 商户信息缓存 */
        public string MchNo { get; set; }
        public string AppId { get; set; }
        public byte MchType { get; set; }
        public MchInfoDto MchInfo { get; set; }
        public MchAppDto MchApp { get; set; }

        /** 商户支付配置信息缓存,  <接口代码, 支付参数>  */
        public Dictionary<string, NormalMchParams> NormalMchParamsMap { get; set; } = new Dictionary<string, NormalMchParams>();
        public Dictionary<string, IsvSubMchParams> IsvSubMchParamsMap { get; set; } = new Dictionary<string, IsvSubMchParams>();

        /** 放置所属服务商的信息 **/
        public IsvConfigContext IsvConfigContext { get; set; }

        /** 缓存 Paypal 对象 **/
        public PaypalWrapper PaypalWrapper { get; set; }

        /** 缓存支付宝client 对象 **/
        public AlipayClientWrapper AlipayClientWrapper { get; set; }

        /** 缓存 wxServiceWrapper 对象 **/
        public WxServiceWrapper WxServiceWrapper { get; set; }

        /** 获取普通商户配置信息 **/
        public NormalMchParams GetNormalMchParamsByIfCode(String ifCode)
        {
            NormalMchParamsMap.TryGetValue(ifCode, out NormalMchParams normalMchParams);
            return normalMchParams;
        }

        /** 获取isv配置信息 **/
        public T GetNormalMchParamsByIfCode<T>(String ifCode) where T : NormalMchParams
        {
            NormalMchParamsMap.TryGetValue(ifCode, out NormalMchParams normalMchParams);
            return (T)normalMchParams;
        }

        /** 获取特约商户配置信息 **/
        public IsvSubMchParams GetIsvSubMchParamsByIfCode(String ifCode)
        {
            IsvSubMchParamsMap.TryGetValue(ifCode, out IsvSubMchParams isvSubMchParams);
            return isvSubMchParams;
        }

        /** 获取isv配置信息 **/
        public T GetIsvsubMchParamsByIfCode<T>(String ifCode) where T : IsvSubMchParams
        {
            IsvSubMchParamsMap.TryGetValue(ifCode, out IsvSubMchParams isvSubMchParams);
            return (T)isvSubMchParams;
        }

        /** 是否为 服务商特约商户 **/
        public bool IsIsvsubMch()
        {
            return this.MchType == (byte)MchInfoType.TYPE_ISVSUB;
        }

        public AlipayClientWrapper GetAlipayClientWrapper()
        {
            return IsIsvsubMch() ? this.IsvConfigContext.AlipayClientWrapper : this.AlipayClientWrapper;
        }

        public WxServiceWrapper GetWxServiceWrapper()
        {
            return IsIsvsubMch() ? this.IsvConfigContext.WxServiceWrapper : this.WxServiceWrapper;
        }

        public PaypalWrapper GetPaypalWrapper()
        {
            return this.PaypalWrapper;
        }
    }
}
