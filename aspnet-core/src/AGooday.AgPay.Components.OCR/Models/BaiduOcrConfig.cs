namespace AGooday.AgPay.Components.OCR.Models
{
    public class BaiduOcrConfig : AbstractOcrConfig
    {
        public string AppID { get; set; }
        public string APIKey { get; set; }
        public string SecretKey { get; set; }
        public string AESKey { get; set; }
    }
}
