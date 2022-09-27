using AGooday.AgPay.Application.Params.AliPay;
using AGooday.AgPay.Application.Params.PpPay;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Application.Params.XxPay;
using AGooday.AgPay.Common.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Params
{
    /// <summary>
    /// 抽象类 普通商户参数定义
    /// </summary>
    public abstract class NormalMchParams
    {
        public static NormalMchParams Factory(string ifCode, string paramsStr)
        {
            if (CS.IF_CODE.WXPAY.Equals(ifCode))
            {
                return JsonConvert.DeserializeObject<WxPayNormalMchParams>(paramsStr);
            }
            else if (CS.IF_CODE.ALIPAY.Equals(ifCode))
            {
                return JsonConvert.DeserializeObject<AliPayNormalMchParams>(paramsStr);
            }
            else if (CS.IF_CODE.XXPAY.Equals(ifCode))
            {
                return JsonConvert.DeserializeObject<XxPayNormalMchParams>(paramsStr);
            }
            else if (CS.IF_CODE.PPPAY.Equals(ifCode))
            {
                return JsonConvert.DeserializeObject<PpPayNormalMchParams>(paramsStr);
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
