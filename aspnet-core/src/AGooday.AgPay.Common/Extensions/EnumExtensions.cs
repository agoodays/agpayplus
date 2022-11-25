using System.ComponentModel;

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
