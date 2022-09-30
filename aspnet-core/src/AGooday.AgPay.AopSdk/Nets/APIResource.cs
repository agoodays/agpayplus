using AGooday.AgPay.AopSdk.Exceptions;
using AGooday.AgPay.AopSdk.Request;
using AGooday.AgPay.AopSdk.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.AopSdk.Nets
{
    /// <summary>
    /// API资源抽象类
    /// </summary>
    public abstract class APIResource
    {
        public const string CHARSET = "utf-8";

        private static HttpClient httpClient = new HttpClient();

        public enum RequestMethod
        {
            GET,
            POST,
            DELETE,
            PUT
        }

        public T Execute<T>(IAgPayRequest<T> request, RequestMethod method, string url) where T : AgPayResponse
        {
            var jsonParam = JsonConvert.SerializeObject(request.GetBizModel());
            var @params = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonParam);
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

        private static void HandleAPIError(APIAgPayResponse response)
        {
            string rBody = response.ResponseBody;
            int rCode = response.ResponseCode;
            var jsonObject = new JObject();
            try
            {
                jsonObject = JObject.Parse(rBody);
            }
            catch (JsonException e)
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
