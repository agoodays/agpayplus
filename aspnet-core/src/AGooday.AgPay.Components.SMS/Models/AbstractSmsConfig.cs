using AGooday.AgPay.Common.Constants;
using Newtonsoft.Json;

namespace AGooday.AgPay.Components.SMS.Models
{
    public abstract class AbstractSmsConfig
    {
        public AbstractSmsConfig GetSmsConfig(string smsProviderKey, string configVal)
        {
            if (CS.SMS_PROVIDER.ALIYUNDY.Equals(smsProviderKey))
            {
                return JsonConvert.DeserializeObject<AliyundySmsConfig>(configVal);
            }
            return null;
        }
    }
}
