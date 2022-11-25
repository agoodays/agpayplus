using Newtonsoft.Json;
using System.Text;

namespace AGooday.AgPay.AopSdk.Nets
{
    public class HttpContent
    {
        public byte[] ByteArrayContent { get; private set; }

        public string ContentType { get; private set; }

        private HttpContent(byte[] byteArrayContent, string contentType)
        {
            this.ByteArrayContent = byteArrayContent;
            this.ContentType = contentType;
        }

        public static HttpContent BuildJsonContent(Dictionary<string, object> @params)
        {
            if (@params == null)
            {
                throw new ArgumentNullException();
            }
            return new HttpContent(
                Encoding.GetEncoding(APIResource.CHARSET).GetBytes(CreateJsonString(@params)),
                $"application/json; charset={APIResource.CHARSET}");
        }

        private static string CreateJsonString(Dictionary<string, object> @params)
        {
            return JsonConvert.SerializeObject(@params);
        }
    }
}
