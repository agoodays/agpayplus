using AGooday.AgPay.Components.SMS.Constants;
using Newtonsoft.Json;

namespace AGooday.AgPay.Components.SMS.Models
{
    public abstract class AbstractSmsConfig
    {
        public static AbstractSmsConfig GetSmsConfig(string smsProviderKey, string configVal)
        {
            if (SmsProviderCS.ALIYUNDY.Equals(smsProviderKey))
            {
                return JsonConvert.DeserializeObject<AliyundySmsConfig>(configVal);
            }
            return null;
        }
    }
}
