using System.Text.RegularExpressions;

namespace AGooday.AgPay.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 字符串脱敏
        /// </summary>
        /// <param name="input"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static string Mask(this string input, char mask = '*')
        {
            if (string.IsNullOrWhiteSpace(input?.Trim()))
            {
                return input;
            }
            input = input.Trim();

            int headLength = 0, middleLength = 0, tailLength = 0, maskLength = 0;
            if (input.Length >= 12)
            {
                int inputLength = input.Length;
                int quotient = inputLength / 5;
                int remainder = inputLength % 5;

                headLength = quotient;
                middleLength = quotient;
                tailLength = quotient;
                maskLength = quotient;

                switch (remainder)
                {
                    case 1:
                        middleLength++;
                        break;
                    case 2:
                        maskLength++;
                        break;
                    case 3:
                        middleLength++;
                        maskLength++;
                        break;
                    case 4:
                        headLength++;
                        tailLength++;
                        maskLength++;
                        break;
                }
            }
            string masks = mask.ToString().PadLeft(input.Length < 12 ? 4 : maskLength, mask);

            bool isEmail = Regex.IsMatch(input, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            if (isEmail)
            {
                return Regex.Replace(input, @"(.{3})(.+)@", $"$1{masks}@");
            }

            return input.Length switch
            {
                _ when input.Length >= 12 => Regex.Replace(input, $@"(.{{{headLength}}}).{{{maskLength}}}(.{{{middleLength}}}).{{{maskLength}}}(.{{{tailLength}}})", $"$1{masks}$2{masks}$3"),
                _ when input.Length == 11 => Regex.Replace(input, @"(\w{3})\w*(\w{4})", $"$1{masks}$2"),
                _ when input.Length == 10 => Regex.Replace(input, @"(\w{3})\w*(\w{3})", $"$1{masks}$2"),
                _ when input.Length == 9 => Regex.Replace(input, @"(\w{2})\w*(\w{3})", $"$1{masks}$2"),
                _ when input.Length == 8 => Regex.Replace(input, @"(\w{2})\w*(\w{2})", $"$1{masks}$2"),
                _ when input.Length == 7 => Regex.Replace(input, @"(\w{1})\w*(\w{2})", $"$1{masks}$2"),
                _ when input.Length >= 2 && input.Length < 7 => Regex.Replace(input, @"(\w{1})\w*(\w{1})", $"$1{masks}$2"),
                _ => input + masks
            };
        }

        /// <summary>
        /// 智能字符串脱敏
        /// </summary>
        public static string IntellectMask(this string input, char mask = '*')
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            input = input.Trim();

            // 优先处理已知格式
            if (IsEmail(input))
                return MaskEmail(input, mask);

            if (IsChineseIdCard(input))
                return MaskIdCard(input, mask);

            if (IsChineseMobile(input))
                return MaskMobile(input, mask);

            // 通用脱敏规则
            return MaskGeneric(input, mask);
        }

        #region 格式判断
        private static bool IsEmail(string input)
        {
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(input);
                return mailAddress.Address == input;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsChineseMobile(string input) =>
            Regex.IsMatch(input, @"^1[3-9]\d{9}$");

        private static bool IsChineseIdCard(string input) =>
            Regex.IsMatch(input, @"^[1-9]\d{5}(18|19|20)\d{2}(0[1-9]|1[0-2])(0[1-9]|[12]\d|3[01])\d{3}[\dXx]$");
        #endregion

        #region 脱敏方法
        private static string MaskEmail(string input, char mask)
        {
            var atIndex = input.IndexOf('@');
            if (atIndex <= 3) // 处理短用户名
                return new string(mask, input.Length);

            return input.Substring(0, 3) +
                   new string(mask, atIndex - 3) +
                   input.Substring(atIndex);
        }

        private static string MaskIdCard(string input, char mask) =>
            Regex.Replace(input, @"(\d{4})\d{10}(\w{4})",
                m => $"{m.Groups[1]}{new string(mask, 10)}{m.Groups[2]}");

        private static string MaskMobile(string input, char mask) =>
            Regex.Replace(input, @"(\d{3})\d{4}(\d{4})",
                $"$1{new string(mask, 4)}$2");

        private static string MaskGeneric(string input, char mask)
        {
            // 保留首尾各20%字符（至少1个）
            int headLength = Math.Max(input.Length / 5, 1);
            int tailLength = Math.Max(input.Length / 5, 1);

            // 中间掩码部分
            int maskLength = input.Length - headLength - tailLength;
            if (maskLength <= 0)
                return new string(mask, input.Length);

            return input.Substring(0, headLength) +
                   new string(mask, maskLength) +
                   input.Substring(input.Length - tailLength);
        }
        #endregion
    }
}