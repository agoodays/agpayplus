using System.Text.RegularExpressions;

namespace AGooday.AgPay.Common.Utils
{
    public class RenameUtil
    {
        /// <summary>
        /// 将大驼峰命名转为小驼峰命名
        /// </summary>
        public static string UpperCamelCaseToLowerCamelCase(string str)
        {
            var firstChar = str[0];

            if (firstChar == char.ToLowerInvariant(firstChar))
            {
                return str;
            }

            var name = str.ToCharArray();
            name[0] = char.ToLowerInvariant(firstChar);

            return new string(name);
        }

        /// <summary>
        /// 将蛇形命名转为大驼峰命名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SnakeCaseToUpperCamelCase(string str)
        {
            var splits = str.Split('_');
            var result = splits.Select(s =>
            {
                if (s.Length > 0)
                    return s.Substring(0, 1).ToUpper() + s.Substring(1);
                else
                    return "";
            }).ToArray();
            return string.Join("", result);
        }

        /// <summary>
        /// 将大驼峰命名转为蛇形命名
        /// </summary>
        public static string UpperCamelCaseToSnakeCase(string str)
        {
            //var builder = new StringBuilder();
            //var name = str;
            //var previousUpper = false;

            //for (var i = 0; i < name.Length; i++)
            //{
            //    var c = name[i];
            //    if (char.IsUpper(c))
            //    {
            //        if (i > 0 && !previousUpper)
            //        {
            //            builder.Append("_");
            //        }
            //        builder.Append(char.ToLowerInvariant(c));
            //        previousUpper = true;
            //    }
            //    else
            //    {
            //        builder.Append(c);
            //        previousUpper = false;
            //    }
            //}
            //return builder.ToString();
            return Regex.Replace(str, "([A-Z])", "_$1").ToLower().TrimStart('_');
        }
    }
}
