using AGooday.AgPay.AopSdk.Exceptions;
using System.Text;

namespace AGooday.AgPay.AopSdk.Nets
{
    /// <summary>
    /// Http请求客户端
    /// </summary>
    public class APIHttpClient
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public async Task<APIAgPayResponse> RequestAsync(APIAgPayRequest request)
        {
            int responseCode = 0;
            string responseBody = string.Empty;

            try
            {
                var httpRequest = new HttpRequestMessage(new HttpMethod(request.Method.ToString()), request.Url);

                using (var content = new StringContent(request.Content.Content, Encoding.GetEncoding(request.Content.CharSet), request.Content.MediaType))
                {
                    httpRequest.Content = content;

                    var response = await httpClient.SendAsync(httpRequest);

                    responseCode = (int)response.StatusCode;
                    responseBody = await response.Content.ReadAsStringAsync();
                }
            }
            catch (HttpRequestException ex)
            {
                responseCode = (int)ex.StatusCode;
                throw;
            }
            catch (Exception ex)
            {
                throw new APIConnectionException($"请求AgPay({request.Url})异常,请检查网络或重试.异常信息:{ex.Message}", ex);
            }

            return new APIAgPayResponse(responseCode, responseBody, null);
        }

        /// <summary>
        /// 发送http请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public APIAgPayResponse Request(APIAgPayRequest request)
        {
            int responseCode = 0;
            string responseBody = string.Empty;

            try
            {
                var httpRequest = new HttpRequestMessage(new HttpMethod(request.Method.ToString()), request.Url);

                using (var content = new StringContent(request.Content.Content, Encoding.GetEncoding(request.Content.CharSet), request.Content.MediaType))
                {
                    httpRequest.Content = content;

                    var response = httpClient.SendAsync(httpRequest).Result;

                    responseCode = (int)response.StatusCode;
                    responseBody = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (HttpRequestException ex)
            {
                responseCode = (int)ex.StatusCode;
                throw;
            }
            catch (Exception ex)
            {
                throw new APIConnectionException($"请求AgPay({request.Url})异常,请检查网络或重试.异常信息:{ex.Message}", ex);
            }

            return new APIAgPayResponse(responseCode, responseBody, null);
        }
    }
}
