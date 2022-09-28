using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AGooday.AgPay.Common.Utils
{
    public class AgPayUtil
    {
        public static string AES_KEY = "4ChT08phkz59hquD795X7w==";

        public static string AesEncode(string data)
        {
            return EnDecryptUtil.AESEncryptToHex(data, AES_KEY);
        }
        public static string AesDecode(string data)
        {
            return EnDecryptUtil.AESDecryptUnHex(data, AES_KEY);
        }

        /// <summary>
        /// 校验微信/支付宝二维码是否符合规范， 并根据支付类型返回对应的支付方式
        /// </summary>
        /// <param name="barCode">条码</param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        public static string GetPayWayCodeByBarCode(string barCode)
        {
            if (string.IsNullOrWhiteSpace(barCode))
            {
                throw new BizException("条码为空");
            }
            //微信 ： 用户付款码条形码规则：18位纯数字，以10、11、12、13、14、15开头
            //文档： https://pay.weixin.qq.com/wiki/doc/api/micropay.php?chapter=5_1
            if (barCode.Length == 18 && new Regex(@"^(10|11|12|13|14|15|16|17|18|19)(.*)").Match(barCode).Success)
            {
                return CS.PAY_WAY_CODE.WX_BAR;
            }
            //支付宝： 25~30开头的长度为16~24位的数字
            //文档： https://docs.open.alipay.com/api_1/alipay.trade.pay/
            else if (barCode.Length >= 16 && barCode.Length <= 24 && new Regex(@"^(25|26|27|28|29|30)(.*)").Match(barCode).Success)
            {
                return CS.PAY_WAY_CODE.ALI_BAR;
            }
            //云闪付： 二维码标准： 19位 + 62开头
            //文档：https://wenku.baidu.com/view/b2eddcd09a89680203d8ce2f0066f5335a8167fa.html
            else if (barCode.Length == 19 && new Regex(@"^(62)(.*)").Match(barCode).Success)
            {
                return CS.PAY_WAY_CODE.YSF_BAR;
            }
            else
            {
                //暂时不支持的条码类型
                throw new BizException("不支持的条码");
            }
        }
    }
}
