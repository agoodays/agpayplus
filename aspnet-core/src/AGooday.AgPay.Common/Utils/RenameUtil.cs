using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

            return new String(name);
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
            return Regex.Replace("CamelClassName", "([A-Z])", "_$1").ToLower().TrimStart('_');
        }
    }
}
