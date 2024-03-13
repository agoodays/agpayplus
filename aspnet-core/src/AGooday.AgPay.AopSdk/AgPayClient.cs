using AGooday.AgPay.AopSdk.Nets;
using AGooday.AgPay.AopSdk.Request;
using AGooday.AgPay.AopSdk.Response;
using Newtonsoft.Json;

namespace AGooday.AgPay.AopSdk
{
    public class AgPayClient : APIResource
    {
        private static readonly Dictionary<string, AgPayClient> clientMap = new Dictionary<string, AgPayClient>();

        public string SignType { get; set; } = AgPay.DEFAULT_SIGN_TYPE;
        public string AppId { get; set; } = AgPay.AppId;
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

        public static AgPayClient GetInstance(string appId, string apiKey, string apiBase)
        {
            lock (clientMap)
            {
                if (clientMap.TryGetValue(appId, out AgPayClient client))
                {
                    return client;
                }
                client = new AgPayClient();
                clientMap.TryAdd(appId, client);
                client.AppId = appId;
                client.ApiKey = apiKey;
                client.ApiBase = apiBase;
                return client;
            }
        }

        public static AgPayClient GetInstance(string appId, string apiKey)
        {
            lock (clientMap)
            {
                if (clientMap.TryGetValue(appId, out AgPayClient client))
                {
                    return client;
                }
                client = new AgPayClient();
                clientMap.TryAdd(appId, client);
                client.AppId = appId;
                client.ApiKey = apiKey;
                return client;
            }
        }

        public static AgPayClient GetInstance(string appId)
        {
            lock (clientMap)
            {
                if (clientMap.TryGetValue(appId, out AgPayClient client))
                {
                    return client;
                }
                client = new AgPayClient();
                clientMap.TryAdd(appId, client);
                client.AppId = appId;
                return client;
            }
        }

        public T Execute<T>(IAgPayRequest<T> request) where T : AgPayResponse
        {
            // 支持用户自己设置RequestOptions
            if (request.GetRequestOptions() == null)
            {
                RequestOptions options = RequestOptions.Builder()
                        .SetVersion(request.GetApiVersion())
                        .SetUri(request.GetApiUri())
                        .SetAppId(this.AppId)
                        .SetApiKey(this.ApiKey)
                        .Build();
                request.SetRequestOptions(options);
            }

            return base.Execute(request, RequestMethod.POST, this.ApiBase);
        }

        public async Task<T> ExecuteAsync<T>(IAgPayRequest<T> request) where T : AgPayResponse
        {
            // 支持用户自己设置RequestOptions
            if (request.GetRequestOptions() == null)
            {
                RequestOptions options = RequestOptions.Builder()
                        .SetVersion(request.GetApiVersion())
                        .SetUri(request.GetApiUri())
                        .SetAppId(this.AppId)
                        .SetApiKey(this.ApiKey)
                        .Build();
                request.SetRequestOptions(options);
            }

            return await base.ExecuteAsync(request, RequestMethod.POST, this.ApiBase);
        }

        public string GetRequestUrl<T>(IAgPayRequest<T> request) where T : AgPayResponse
        {
            // 支持用户自己设置RequestOptions
            if (request.GetRequestOptions() == null)
            {
                RequestOptions options = RequestOptions.Builder()
                    .SetVersion(request.GetApiVersion())
                    .SetUri(request.GetApiUri())
                    .SetAppId(this.AppId)
                    .SetApiKey(this.ApiKey)
                    .Build();
                request.SetRequestOptions(options);
            }
            string jsonParam = JsonConvert.SerializeObject(request.GetBizModel());

            var parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonParam);
            request.GetRequestOptions();

            return APIAgPayRequest.BuildURLWithSign(this.ApiBase, parameters, request.GetRequestOptions()).ToString();
        }
    }
}