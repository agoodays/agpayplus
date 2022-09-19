using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AGooday.AgPay.Common.Utils
{
    public sealed class RegUtil
    {
        public const string REG_MOBILE = "^1\\d{10}$"; //判断是否是手机号
	    public const string REG_ALIPAY_USER_ID = "^2088\\d{12}$"; //判断是支付宝用户Id 以2088开头的纯16位数字
        public static bool IsMobile(string str)
        {
            Regex regex = new Regex(REG_MOBILE);
            return regex.IsMatch(str);
        }
    }
}
