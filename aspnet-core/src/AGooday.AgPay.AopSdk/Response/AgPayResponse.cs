using AGooday.AgPay.AopSdk.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.AopSdk.Response
{
    /// <summary>
    /// AgPay响应抽象类
    /// </summary>
    public abstract class AgPayResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("sign")]
        public string Sign { get; set; }

        [JsonProperty("data")]
        public JObject Data { get; set; }

        public bool CheckSign(string signType, string apiKey)
        {
            if (Data == null && string.IsNullOrWhiteSpace(Sign)) return true;
            return AgPayUtil.Verify(Data.ToObject<Dictionary<string, object>>(), signType, Sign, apiKey);
        }

        public virtual bool IsSuccess(string signType, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(signType) || string.IsNullOrWhiteSpace(apiKey)) return Code == 0;
            return Code == 0 && CheckSign(signType, apiKey);
        }
    }
}
