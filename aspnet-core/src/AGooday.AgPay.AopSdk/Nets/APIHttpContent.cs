﻿using System.Net.Mime;
using Newtonsoft.Json;

namespace AGooday.AgPay.AopSdk.Nets
{
    public class APIHttpContent
    {
        public string Content { get; private set; }

        public string CharSet { get; private set; }

        public string MediaType { get; private set; }

        private APIHttpContent(string content, string mediaType)
        {
            Content = content;
            MediaType = mediaType;
            CharSet = APIResource.CHARSET;
        }

        public static APIHttpContent BuildJsonContent(Dictionary<string, object> @params)
        {
            ArgumentNullException.ThrowIfNull(@params);
            return new APIHttpContent(CreateJsonString(@params), MediaTypeNames.Application.Json);
        }

        private static string CreateJsonString(Dictionary<string, object> @params)
        {
            return JsonConvert.SerializeObject(@params);
        }
    }
}
