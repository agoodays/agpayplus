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
    }
}