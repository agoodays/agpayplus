namespace AGooday.AgPay.AopSdk.Nets
{
    public class APIAgPayResponse
    {
        public int ResponseCode { get; private set; }
        public string ResponseBody { get; private set; }
        public HttpHeaders ResponseHeaders { get; private set; }
        public APIAgPayResponse(int responseCode, string responseBody, HttpHeaders responseHeaders)
        {
            this.ResponseCode = responseCode;
            this.ResponseBody = responseBody;
            this.ResponseHeaders = responseHeaders;
        }
}
}
