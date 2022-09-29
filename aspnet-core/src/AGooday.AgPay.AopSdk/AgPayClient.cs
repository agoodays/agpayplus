using AGooday.AgPay.AopSdk.Nets;
using AGooday.AgPay.AopSdk.Request;
using AGooday.AgPay.AopSdk.Response;
using Newtonsoft.Json;

namespace AGooday.AgPay.AopSdk
{
    public class AgPayClient : APIResource
    {
        public string SignType { get; set; } = AgPay.DEFAULT_SIGN_TYPE;
        public string ApiKey { get; set; } = AgPay.ApiKey;
        public string ApiBase { get; set; } = AgPay.ApiBase;

        public AgPayClient(string apiBase, string signType, string apiKey)
        {
            this.ApiBase = apiBase;
            this.SignType = signType;
            this.ApiKey = apiKey;
        }

        public AgPayClient(string apiBase, string apiKey)
        {
            this.ApiBase = apiBase;
            this.ApiKey = apiKey;
        }

        public AgPayClient(string apiKey)
        {
            this.ApiKey = apiKey;
        }

        public AgPayClient()
        {
        }

        public T Execute<T>(IAgPayRequest<T> request) where T : AgPayResponse
        {
            // 支持用户自己设置RequestOptions
            if (request.GetRequestOptions() == null)
            {
                RequestOptions options = RequestOptions.Builder()
                        .SetVersion(request.GetApiVersion())
                        .SetUri(request.GetApiUri())
                        .SetApiKey(this.ApiKey)
                        .Build();
                request.SetRequestOptions(options);
            }

            return base.Execute(request, RequestMethod.POST, this.ApiBase);
        }
    }
}