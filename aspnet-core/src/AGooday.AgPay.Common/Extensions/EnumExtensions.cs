using System.ComponentModel;
using System.Text.RegularExpressions;

namespace AGooday.AgPay.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum val)
        {
            var field = val.GetType().GetField(val.ToString());
            var customAttribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            if (customAttribute == null) { return val.ToString(); }
            else { return ((DescriptionAttribute)customAttribute).Description; }
        }

        public static string Mask(this string s, char mask = '*')
        {
            if (string.IsNullOrWhiteSpace(s?.Trim()))
            {
                return s;
            }
            s = s.Trim();

            int headLength = 0, middleLength = 0, tailLength = 0, maskLength = 0;

            if (s.Length >= 12)
            {
                int inputLength = s.Length;
                int quotient = inputLength / 5;
                int remainder = inputLength % 5;

                headLength = quotient;
                middleLength = quotient;
                tailLength = quotient;
                maskLength = quotient;

                // X1+Y+X2+Y+X3
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

            string masks = mask.ToString().PadLeft(s.Length < 12 ? 4 : maskLength, mask);

            return s.Length switch
            {
                _ when s.Length >= 12 => Regex.Replace(s, $@"(\w{3})\w*(\w{3})\w*(\w{4})", $"$1{masks}$2"),
                _ when s.Length == 11 => Regex.Replace(s, @"(\w{3})\w*(\w{4})", $"$1{masks}$2"),
                _ when s.Length == 10 => Regex.Replace(s, @"(\w{3})\w*(\w{3})", $"$1{masks}$2"),
                _ when s.Length == 9 => Regex.Replace(s, @"(\w{2})\w*(\w{3})", $"$1{masks}$2"),
                _ when s.Length == 8 => Regex.Replace(s, @"(\w{2})\w*(\w{2})", $"$1{masks}$2"),
                _ when s.Length == 7 => Regex.Replace(s, @"(\w{1})\w*(\w{2})", $"$1{masks}$2"),
                _ when s.Length >= 2 && s.Length < 7 => Regex.Replace(s, @"(\w{1})\w*(\w{1})", $"$1{masks}$2"),
                _ => s + masks
            };
        }
    }
}