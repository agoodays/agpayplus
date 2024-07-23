using AGooday.AgPay.AopSdk.Exceptions;
using AGooday.AgPay.AopSdk.Request;
using AGooday.AgPay.AopSdk.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.AopSdk.Nets
{
    /// <summary>
    /// API资源抽象类
    /// </summary>
    public abstract class APIResource
    {
        public const string CHARSET = "utf-8";

        private static readonly APIHttpClient httpClient = new APIHttpClient();

        public enum RequestMethod
        {
            GET,
            POST,
            DELETE,
            PUT
        }

        public T Execute<T>(IAgPayRequest<T> request, RequestMethod method, string url) where T : AgPayResponse
        {
            Dictionary<string, object> @params = GetParams(request);
            var apiAgPayRequest = new APIAgPayRequest(method, url, @params, request.GetRequestOptions());
            var response = httpClient.Request(apiAgPayRequest);
            int responseCode = response.ResponseCode;
            string responseBody = response.ResponseBody;
            if (responseCode != 200)
            {
                HandleAPIError(response);
            }
            return JsonConvert.DeserializeObject<T>(responseBody);
        }

        public async Task<T> ExecuteAsync<T>(IAgPayRequest<T> request, RequestMethod method, string url) where T : AgPayResponse
        {
            Dictionary<string, object> @params = GetParams(request);
            var apiAgPayRequest = new APIAgPayRequest(method, url, @params, request.GetRequestOptions());
            var response = await httpClient.RequestAsync(apiAgPayRequest);
            int responseCode = response.ResponseCode;
            string responseBody = response.ResponseBody;
            if (responseCode != 200)
            {
                HandleAPIError(response);
            }
            return JsonConvert.DeserializeObject<T>(responseBody);
        }

        private static Dictionary<string, object> GetParams<T>(IAgPayRequest<T> request) where T : AgPayResponse
        {
            //1.把requset转为map
            var bizModel = request.GetBizModel();
            var jsonParam = JsonConvert.SerializeObject(bizModel);
            var @params = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonParam);
            //2.把bizModel.extendInfos 里的键值，覆盖已有的
            var extendInfos = bizModel.GetExtendInfos();
            if (extendInfos != null && extendInfos.Count > 0)
            {
                foreach (var item in extendInfos)
                {
                    if (@params.ContainsKey(item.Key))
                    {
                        @params.Remove(item.Key);
                    }
                    @params.Add(item.Key, item.Value);
                }
            }

            return @params;
        }

        private static void HandleAPIError(APIAgPayResponse response)
        {
            string rBody = response.ResponseBody;
            int rCode = response.ResponseCode;
            var jsonObject = new JObject();
            try
            {
                jsonObject = JObject.Parse(rBody);
            }
            catch (Exception e)
            {
                RaiseMalformedJsonError(rBody, rCode, e);
            }

            if (rCode == 404)
            {
                throw new InvalidRequestException(rCode,
                    $"{jsonObject.GetValue("status")}, {jsonObject.GetValue("error")}, {jsonObject.GetValue("path")}",
                    null);
            }
        }

        private static void RaiseMalformedJsonError(string responseBody, int responseCode, Exception innerException)
        {
            throw new APIException(responseCode,
                $"Invalid response object from API: {responseBody}. (HTTP response code was {responseCode})",
                innerException);
        }
    }
}
