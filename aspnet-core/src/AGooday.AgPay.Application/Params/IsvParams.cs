using AGooday.AgPay.Application.Params.AliPay;
using AGooday.AgPay.Application.Params.SxfPay;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Application.Params.YsfPay;
using AGooday.AgPay.Common.Constants;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params
{
    /// <summary>
    /// 抽象类 isv参数定义
    /// </summary>
    public abstract class IsvParams
    {
        public static IsvParams Factory(string ifCode, string paramsStr)
        {
            if (CS.IF_CODE.WXPAY.Equals(ifCode))
            {
                return JsonConvert.DeserializeObject<WxPayIsvParams>(paramsStr);
            }
            else if (CS.IF_CODE.ALIPAY.Equals(ifCode))
            {
                return JsonConvert.DeserializeObject<AliPayIsvParams>(paramsStr);
            }
            else if (CS.IF_CODE.YSFPAY.Equals(ifCode))
            {
                return JsonConvert.DeserializeObject<YsfPayIsvParams>(paramsStr);
            }
            else if (CS.IF_CODE.SXFPAY.Equals(ifCode))
            {
                return JsonConvert.DeserializeObject<SxfPayIsvParams>(paramsStr);
            }
            return null;
        }

        /// <summary>
        /// 敏感数据脱敏
        /// </summary>
        /// <returns></returns>
        public abstract string DeSenData();
    }
}
