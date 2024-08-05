namespace AGooday.AgPay.Components.OCR.Models
{
    public class BaiduOcrConfig : AbstractOcrConfig
    {
        public string AppID { get; set; }
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string AesKey { get; set; }
    }
}
