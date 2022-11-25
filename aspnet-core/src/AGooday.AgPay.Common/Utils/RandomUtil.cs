using System.Text;

namespace AGooday.AgPay.Common.Utils
{
    public class RandomUtil
    {
        /// <summary>
        /// 用于随机选的数字
        /// </summary>
        public const string BASE_NUMBER = "0123456789";

        /// <summary>
        /// 用于随机选的字符
        /// </summary>
        public const string BASE_CHAR = "abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// 用于随机选的字符和数字
        /// </summary>
        public const string BASE_CHAR_NUMBER = "abcdefghijklmnopqrstuvwxyz0123456789";

        /// <summary>
        /// 获得一个随机的字符串（只包含数字和字符）
        /// </summary>
        /// <param name="length">字符串的长度</param>
        /// <returns>随机字符串</returns>
        public static string RandomString(int length)
        {
            return RandomString(BASE_CHAR_NUMBER, length);
        }

        /// <summary>
        /// 获得一个随机的字符串（只包含数字和大写字符）
        /// </summary>
        /// <param name="length">字符串的长度</param>
        /// <returns>随机字符串</returns>
        public static string RandomStringUpper(int length)
        {
            return RandomString(BASE_CHAR_NUMBER, length).ToUpper();
        }

        /// <summary>
        /// 获得一个只包含数字的字符串
        /// </summary>
        /// <param name="length">字符串的长度</param>
        /// <returns>随机字符串</returns>
        public static string RandomNumbers(int length)
        {
            return RandomString(BASE_NUMBER, length);
        }

        /// <summary>
        /// 获得一个随机的字符串
        /// </summary>
        /// <param name="baseString">随机字符选取的样本</param>
        /// <param name="length">字符串的长度</param>
        /// <returns>随机字符串</returns>
        public static string RandomString(string baseString, int length)
        {
            if (string.IsNullOrWhiteSpace(baseString))
            {
                return "";
            }
            else
            {
                StringBuilder sb = new StringBuilder(length);
                if (length < 1)
                {
                    length = 1;
                }

                int baseLength = baseString.Length;

                for (int i = 0; i < length; ++i)
                {
                    int number = Random.Shared.Next(baseLength);
                    sb.Append(baseString[number]);
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// 通过Linq方法获得一个随机的字符串
        /// </summary>
        /// <param name="baseString">随机字符选取的样本</param>
        /// <param name="length">字符串的长度</param>
        /// <returns>随机字符串</returns>
        public static string RandomStringByLinq(string baseString, int length)
        {
            if (string.IsNullOrWhiteSpace(baseString))
                return "";
            if (length < 1)
                length = 1;
            return new string(Enumerable.Repeat(baseString, length).Select(chars => chars[Random.Shared.Next(baseString.Length)]).ToArray());
        }
    }
}
