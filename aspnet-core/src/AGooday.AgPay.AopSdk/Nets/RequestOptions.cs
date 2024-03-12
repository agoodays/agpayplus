namespace AGooday.AgPay.AopSdk.Nets
{
    /// <summary>
    /// 请求参数选项内容
    /// </summary>
    public class RequestOptions
    {
        private readonly string uri;
        private readonly string version;
        private readonly string signType;

        private readonly string apiKey;

        private readonly int connectTimeout;
        private readonly int readTimeout;
        private readonly int maxNetworkRetries;
        private readonly string acceptLanguage;

        public static RequestOptions GetDefault(string uri, string version)
        {
            return new RequestOptions(
                    uri,
                    version,
                    AgPay.DEFAULT_SIGN_TYPE,
                    AgPay.ApiKey,
                    AgPay.GetConnectTimeout(),
                    AgPay.GetReadTimeout(),
                    AgPay.GetMaxNetworkRetries(),
                    AgPay.AcceptLanguage);
        }

        private RequestOptions(
                string uri,
                string version,
                string signType,
                string apiKey,
                int connectTimeout,
                int readTimeout,
                int maxNetworkRetries,
                string acceptLanguage)
        {
            this.uri = uri;
            this.version = version;
            this.signType = signType;
            this.apiKey = apiKey;
            this.connectTimeout = connectTimeout;
            this.readTimeout = readTimeout;
            this.maxNetworkRetries = maxNetworkRetries;
            this.acceptLanguage = acceptLanguage;
        }

        public string GetUri()
        {
            return uri;
        }

        public string GetVersion()
        {
            return version;
        }

        public string GetSignType()
        {
            return signType;
        }

        public string GetApiKey()
        {
            return apiKey;
        }

        public int GetConnectTimeout()
        {
            return connectTimeout;
        }

        public int GetReadTimeout()
        {
            return readTimeout;
        }

        public int GetMaxNetworkRetries()
        {
            return maxNetworkRetries;
        }

        public string GetAcceptLanguage()
        {
            return acceptLanguage;
        }

        public static RequestOptionsBuilder Builder()
        {
            return new RequestOptionsBuilder();
        }

        public class RequestOptionsBuilder
        {
            private string uri;
            private string version;
            private string signType;
            private string apiKey;
            private int connectTimeout;
            private int readTimeout;
            private int maxNetworkRetries;
            private string acceptLanguage;

            public RequestOptionsBuilder()
            {
                this.signType = AgPay.DEFAULT_SIGN_TYPE;
                this.apiKey = AgPay.ApiKey;
                this.connectTimeout = AgPay.GetConnectTimeout();
                this.readTimeout = AgPay.GetReadTimeout();
                this.maxNetworkRetries = AgPay.GetMaxNetworkRetries();
                this.acceptLanguage = AgPay.AcceptLanguage;
            }

            public string GetUri()
            {
                return uri;
            }

            public RequestOptionsBuilder SetUri(string uri)
            {
                this.uri = NormalizeApiUri(uri);
                return this;
            }

            public string GetVersion()
            {
                return version;
            }

            public RequestOptionsBuilder SetVersion(string version)
            {
                this.version = version;
                return this;
            }

            public string GetSignType()
            {
                return signType;
            }

            public RequestOptionsBuilder SetSignType(string signType)
            {
                this.signType = signType;
                return this;
            }

            public string GetApiKey()
            {
                return apiKey;
            }

            public RequestOptionsBuilder SetApiKey(string apiKey)
            {
                this.apiKey = NormalizeApiKey(apiKey);
                return this;
            }

            public RequestOptionsBuilder ClearApiKey()
            {
                this.apiKey = null;
                return this;
            }

            public int GetConnectTimeout()
            {
                return connectTimeout;
            }

            public RequestOptionsBuilder SetConnectTimeout(int connectTimeout)
            {
                this.connectTimeout = connectTimeout;
                return this;
            }

            public int GetReadTimeout()
            {
                return readTimeout;
            }

            public RequestOptionsBuilder SetReadTimeout(int readTimeout)
            {
                this.readTimeout = readTimeout;
                return this;
            }

            public int GetMaxNetworkRetries()
            {
                return maxNetworkRetries;
            }

            public RequestOptionsBuilder SetMaxNetworkRetries(int maxNetworkRetries)
            {
                this.maxNetworkRetries = maxNetworkRetries;
                return this;
            }

            public string GetAcceptLanguage()
            {
                return acceptLanguage;
            }

            public RequestOptionsBuilder SetAcceptLanguage(string acceptLanguage)
            {
                this.acceptLanguage = NormalizeAcceptLanguage(acceptLanguage);
                return this;
            }

            public RequestOptions Build()
            {
                return new RequestOptions(
                        NormalizeApiUri(this.uri),
                        version,
                        signType,
                        NormalizeApiKey(this.apiKey),
                        connectTimeout,
                        readTimeout,
                        maxNetworkRetries,
                        acceptLanguage);
            }
        }

        private static string NormalizeApiUri(string apiUri)
        {
            if (apiUri == null)
            {
                throw new InvalidRequestOptionsException("接口URI不能为空!");
            }
            if (apiUri.StartsWith("/"))
            {
                throw new InvalidRequestOptionsException($"接口URI({apiUri})不能以'/'开头");
            }
            return apiUri;
        }

        private static string NormalizeApiKey(string apiKey)
        {
            if (apiKey == null)
            {
                return null;
            }
            string normalized = apiKey.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                throw new InvalidRequestOptionsException("API key不能为空!");
            }
            return normalized;
        }

        private static string NormalizeAcceptLanguage(string acceptLanguage)
        {
            if (acceptLanguage == null)
            {
                return null;
            }
            string normalized = acceptLanguage.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                throw new InvalidRequestOptionsException("Accept-Language不能空!");
            }
            return normalized;
        }

        public class InvalidRequestOptionsException : Exception
        {
            public InvalidRequestOptionsException(string message) : base(message)
            {
            }
        }
    }
}
