using AGooday.AgPay.AopSdk.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.AopSdk.Nets
{
    /// <summary>
    /// Http请求客户端
    /// </summary>
    public class HttpClient
    {
        /// <summary>
        /// 发送http请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public APIAgPayResponse Request(APIAgPayRequest request)
        {
            int responseCode = 0;
            string responseBody = string.Empty;

            HttpWebRequest req = null;
            HttpWebResponse res = null;
            Stream resStream = null;

            try
            {
                req = WebRequest.Create(request.Url) as HttpWebRequest;
                byte[] reqBytes = request.Content.ByteArrayContent;
                req.Method = request.Method.ToString();
                req.ContentType = request.Content.ContentType;
                req.ContentLength = reqBytes.Length;
                Stream reqStream = req.GetRequestStream();
                reqStream.Write(reqBytes, 0, reqBytes.Length);
                reqStream.Close();

                res = (HttpWebResponse)req.GetResponse();
                responseCode = (int)res.StatusCode;
                resStream = res.GetResponseStream();
                StreamReader reader = new StreamReader(resStream, Encoding.GetEncoding(APIResource.CHARSET));
                responseBody = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
                resStream.Close();
            }
            catch (WebException we)
            {
                ///这个说明服务器返回了信息了，不过是非200,301,302这样正常的状态码
                if (we.Response != null)
                {
                    res = (HttpWebResponse)we.Response;
                    resStream = res?.GetResponseStream();
                    StreamReader reader = new StreamReader(resStream, Encoding.GetEncoding(APIResource.CHARSET));
                    responseBody = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();
                    resStream.Close();
                }
            }
            catch (Exception e)
            {
                throw new APIConnectionException($"请求AgPay({request.Url})异常,请检查网络或重试.异常信息:{e.Message}", e);
            }
            finally
            {
                if (req != null) req.Abort();
                if (res != null) res.Close();
                if (resStream != null) resStream.Close();
            }

            return new APIAgPayResponse(responseCode, responseBody, null);
        }
    }
}
