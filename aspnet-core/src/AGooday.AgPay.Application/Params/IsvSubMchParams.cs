using AGooday.AgPay.Application.Params.AliPay;
using AGooday.AgPay.Application.Params.SxfPay;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Application.Params.YsfPay;
using AGooday.AgPay.Common.Constants;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params
{
    /// <summary>
    /// 抽象类 特约商户参数定义
    /// </summary>
    public abstract class IsvSubMchParams
    {
        public static IsvSubMchParams Factory(string ifCode, string paramsStr)
        {
            if (CS.IF_CODE.WXPAY.Equals(ifCode))
            {
                return JsonConvert.DeserializeObject<WxPayIsvSubMchParams>(paramsStr);
            }
            else if (CS.IF_CODE.ALIPAY.Equals(ifCode))
            {
                return JsonConvert.DeserializeObject<AliPayIsvSubMchParams>(paramsStr);
            }
            else if (CS.IF_CODE.YSFPAY.Equals(ifCode))
            {
                return JsonConvert.DeserializeObject<YsfPayIsvSubMchParams>(paramsStr);
            }
            else if (CS.IF_CODE.SXFPAY.Equals(ifCode))
            {
                return JsonConvert.DeserializeObject<SxfPayIsvSubMchParams>(paramsStr);
            }
            return null;
        }
    }
}
