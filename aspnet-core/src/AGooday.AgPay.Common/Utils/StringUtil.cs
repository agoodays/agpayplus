using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Common.Utils
{
    public class StringUtil
    {
        /// <summary>
        /// 对字符加星号处理：除前面几位和后面几位外，其他的字符以星号代替
        /// </summary>
        /// <param name="content">传入的字符串</param>
        /// <param name="frontNum"></param>
        /// <param name="endNum"></param>
        /// <param name="starNum"></param>
        /// <returns></returns>
        public static string Str2Star(string content, int frontNum, int endNum, int starNum)
        {
            if (frontNum >= content.Length || frontNum < 0)
            {
                return content;
            }
            if (endNum >= content.Length || endNum < 0)
            {
                return content;
            }
            if (frontNum + endNum >= content.Length)
            {
                return content;
            }
            string starStr = "";
            for (int i = 0; i < starNum; i++)
            {
                starStr = starStr + "*";
            }
            return content.Substring(0, frontNum) + starStr
                    + content.Substring(content.Length - endNum, content.Length);
        }
    }
}
