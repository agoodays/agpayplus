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
        private byte[] ByteArrayContent;

        private string ContentType;

        private HttpContent(byte[] byteArrayContent, String contentType)
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
                Encoding.GetEncoding(APIResource.CHARSET).GetBytes(CreateJSONString(@params)),
                $"application/json; charset={APIResource.CHARSET}");
        }

        private static string CreateJSONString(Dictionary<string, object> @params)
        {
            return JsonConvert.SerializeObject(@params);
        }
    }
}
