using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
            if (@params != null)
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
