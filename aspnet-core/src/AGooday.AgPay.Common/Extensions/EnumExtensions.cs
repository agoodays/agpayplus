using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
