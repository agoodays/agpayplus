using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params;
using AGooday.AgPay.Common.Enumerator;

namespace AGooday.AgPay.Payment.Api.Models
{
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
        public AlipayClientWrapper AlipayClientWrapper { get; set; }

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
        /// 获取isv配置信息
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
        /// 获取isv配置信息
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
        ///  是否为 服务商特约商户
        /// </summary>
        /// <returns></returns>
        public bool IsIsvSubMch()
        {
            return this.MchType == (byte)MchInfoType.TYPE_ISVSUB;
        }

        public AlipayClientWrapper GetAlipayClientWrapper()
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
