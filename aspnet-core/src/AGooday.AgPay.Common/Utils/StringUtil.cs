using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;

namespace AGooday.AgPay.Common.Utils
{
    public static class StringUtil
    {
        public static string ToHex(byte[] bytes, bool isUpper = false)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString(isUpper ? "X2" : "x2"));
            }
            return sb.ToString();
        }

        public static byte[] UnHex(string hex)
        {
            int len = hex.Length / 2;
            byte[] bytes = new byte[len];
            for (int i = 0; i < len; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return bytes;
        }

        //public static string ToHex(byte[] bytes) // 0xae00cf => "AE00CF "
        //{
        //    string hexString = string.Empty;
        //    if (bytes != null)
        //    {
        //        StringBuilder strB = new StringBuilder();

        //        for (int i = 0; i < bytes.Length; i++)
        //        {
        //            strB.Append(bytes[i].ToString("X2"));
        //        }
        //        hexString = strB.ToString().ToLower();
        //    }
        //    return hexString;
        //}

        //public static byte[] UnHex(string hex)
        //{
        //    byte[] inputByteArray = new byte[hex.Length / 2];
        //    for (int x = 0; x < hex.Length / 2; x++)
        //    {
        //        int i = (Convert.ToInt32(hex.Substring(x * 2, 2), 16));
        //        inputByteArray[x] = (byte)i;
        //    }
        //    return inputByteArray;
        //}

        public static string ToHex(string s) => ToHex(s, Encoding.UTF8.ToString(), false);

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
            ArgumentNullException.ThrowIfNull(hex);
            hex = hex.Replace(",", "");
            hex = hex.Replace("\n", "");
            hex = hex.Replace("\\", "");
            hex = hex.Replace(" ", "");
            if (hex.Length % 2 != 0)
            {
                //hex += "20";//空格
                throw new ArgumentException("hex is not a valid number!", nameof(hex));
            }
            // 需要将 hex 转换成 byte 数组。
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    // 每两个字符是一个 byte。
                    bytes[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
                }
                catch
                {
                    // Rethrow an exception with custom message.
                    throw new ArgumentException("hex is not a valid hex number!", nameof(hex));
                }
            }
            Encoding chs = Encoding.GetEncoding(charset);
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
                starStr += "*";
            }
            return content.SubstringUp(0, frontNum) + starStr
                    + content.SubstringUp(content.Length - endNum, content.Length);
        }

        /// <summary>
        /// 合并两个json字符串
        /// key相同，则后者覆盖前者的值
        /// key不同，则合并至前者
        /// </summary>
        /// <param name="originStr"></param>
        /// <param name="mergeStr"></param>
        /// <returns>合并后的json字符串</returns>
        public static string Merge(string originStr, string mergeStr)
        {
            if (string.IsNullOrWhiteSpace(originStr) || string.IsNullOrWhiteSpace(mergeStr))
            {
                return null;
            }

            JObject originJSON = JObject.Parse(originStr);
            JObject mergeJSON = JObject.Parse(mergeStr);

            if (originJSON == null || mergeJSON == null)
            {
                return null;
            }

            originJSON.Merge(mergeJSON, new JsonMergeSettings() { MergeArrayHandling = MergeArrayHandling.Replace });
            //return originJSON.ToString();
            return JsonConvert.SerializeObject(originJSON, Formatting.None);//压缩Json
        }

        public static string SubstringUp(this string value, int beginIndex, int endIndex)
        {
            int length = value.Length;
            CheckBoundsBeginEnd(beginIndex, endIndex, length);
            //int subLen = endIndex - beginIndex;
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
                throw new ArgumentException($"begin {begin}, end {end}, length {length}");
            }
        }

        public static string GetUUID() => Guid.NewGuid().ToString("N") + Environment.CurrentManagedThreadId;

        public static string GetUUID(int endAt) => GetUUID().Substring(0, endAt);

        /// <summary>
        /// 是否 http 或 https连接
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsAvailableUrl(string url) => !string.IsNullOrWhiteSpace(url) && (url.StartsWith("http://") || url.StartsWith("https://"));

        /// <summary>
        /// 值为Null使用默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T DefaultIfEmpty<T>(T value, T defaultValue) => value == null ? defaultValue : value;
        /// <summary>
        /// 值为Null或空白时使用默认值空字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DefaultString(string str) => DefaultString(str, "");
        /// <summary>
        /// 值为Null或空白时使用默认值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultStr"></param>
        /// <returns></returns>
        public static string DefaultString(string str, string defaultStr) => string.IsNullOrWhiteSpace(str) ? defaultStr : str;
        /// <summary>
        /// 第一个非Null和非空字符串
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static string FirstNonNullAndNonEmptyString(params string[] strs) => strs.FirstOrDefault(s => !string.IsNullOrEmpty(s));
        /// <summary>
        /// 第一个非Null和非空白字符串
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static string FirstNonNullAndNonWhiteSpaceString(params string[] strs) => strs.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));
        /// <summary>
        /// 连接非Null和非空字符串
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static string JoinNonNullAndNonEmptyString(string separator, params string[] strs) => string.Join(separator, strs.Where(s => !string.IsNullOrEmpty(s)));
        /// <summary>
        /// 连接非Null和非空白字符串
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static string JoinNonNullAndNonWhiteSpaceString(string separator, params string[] strs) => string.Join(separator, strs.Where(s => !string.IsNullOrWhiteSpace(s)));
        /// <summary>
        /// 任意为Null或空
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static bool IsAnyNullOrEmpty(params string[] strs) => strs.Any(s => string.IsNullOrEmpty(s));
        /// <summary>
        /// 任意为Null或空白
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static bool IsAnyNullOrWhiteSpace(params string[] strs) => strs.Any(s => string.IsNullOrWhiteSpace(s));
        /// <summary>
        /// 全部为Null或空
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static bool IsAllNullOrEmpty(params string[] strs) => !strs.Any(s => !string.IsNullOrEmpty(s));
        /// <summary>
        /// 全部为Null或空白
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static bool IsAllNullOrWhiteSpace(params string[] strs) => !strs.Any(s => !string.IsNullOrWhiteSpace(s));
        /// <summary>
        /// 全部不为Null或空
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static bool IsAllNotNullOrEmpty(params string[] strs) => !strs.Any(s => string.IsNullOrEmpty(s));
        /// <summary>
        /// 全部不为Null或空白
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static bool IsAllNotNullOrWhiteSpace(params string[] strs) => !strs.Any(s => string.IsNullOrWhiteSpace(s));
    }
}
