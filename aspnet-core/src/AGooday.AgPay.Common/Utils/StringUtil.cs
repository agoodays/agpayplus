using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Common.Utils
{
    public static class  StringUtil
    {
        public static string ToHex(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString().ToLower();
            }
            return hexString;
        }

        public static byte[] UnHex(string hex)
        {
            byte[] inputByteArray = new byte[hex.Length / 2];
            for (int x = 0; x < hex.Length / 2; x++)
            {
                int i = (Convert.ToInt32(hex.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            return inputByteArray;
        }

        public static string ToHex(string s)
        {
            return ToHex(s, Encoding.UTF8.ToString(), false);
        }

        /// 从字符串转换到16进制表示的字符串
        /// 编码,如"utf-8","gb2312"
        /// 是否每字符用逗号分隔
        public static string ToHex(string s, string charset, bool fenge)
        {
            if ((s.Length % 2) != 0)
            {
                s += " ";//空格
                         //throw new ArgumentException("s is not valid chinese string!");
            }
            System.Text.Encoding chs = System.Text.Encoding.GetEncoding(charset);
            byte[] bytes = chs.GetBytes(s);
            string str = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                str += string.Format("{0:X}", bytes[i]);
                if (fenge && (i != bytes.Length - 1))
                {
                    str += string.Format("{0}", ",");
                }
            }
            return str.ToLower();
        }

        /// 从16进制转换成utf编码的字符串
        /// 编码,如"utf-8","gb2312"
        public static string UnHex(string hex, string charset)
        {
            if (hex == null)
                throw new ArgumentNullException("hex");
            hex = hex.Replace(",", "");
            hex = hex.Replace("\n", "");
            hex = hex.Replace("\\", "");
            hex = hex.Replace(" ", "");
            if (hex.Length % 2 != 0)
            {
                hex += "20";//空格
                throw new ArgumentException("hex is not a valid number!", "hex");
            }
            // 需要将 hex 转换成 byte 数组。
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    // 每两个字符是一个 byte。
                    bytes[i] = byte.Parse(hex.Substring(i * 2, 2),
                    System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    // Rethrow an exception with custom message.
                    throw new ArgumentException("hex is not a valid hex number!", "hex");
                }
            }
            System.Text.Encoding chs = System.Text.Encoding.GetEncoding(charset);
            return chs.GetString(bytes);
        }
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
            return content.SubstringUp(0, frontNum) + starStr
                    + content.SubstringUp(content.Length - endNum, content.Length);
        }

        public static string SubstringUp(this string value, int beginIndex, int endIndex)
        {
            int length = value.Length;
            CheckBoundsBeginEnd(beginIndex, endIndex, length);
            int subLen = endIndex - beginIndex;
            if (beginIndex == 0 && endIndex == length)
            {
                return value;
            }
            return value.Substring(beginIndex, endIndex - beginIndex);
        }

        public static void CheckBoundsBeginEnd(int begin, int end, int length)
        {
            if (begin < 0 || begin > end || end > length)
            {
                throw new ArgumentException("begin " + begin + ", end " + end + ", length " + length);
            }
        }

        public static bool IsAvailableUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            return url.StartsWith("http://") || url.StartsWith("https://");
        }

        public static T DefaultIfEmpty<T>(T value, T defaultValue) => value == null ? defaultValue : value;
        public static string DefaultString(string str) => DefaultString(str, "");
        public static string DefaultString(string str, string defaultStr) => string.IsNullOrWhiteSpace(str) ? defaultStr : str;
    }
}
