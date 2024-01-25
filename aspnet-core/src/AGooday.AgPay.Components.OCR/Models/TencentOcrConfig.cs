namespace AGooday.AgPay.Components.OCR.Models
{
    public class TencentOcrConfig : AbstractOcrConfig
    {
        public string Endpoint { get; set; } = "ocr.tencentcloudapi.com";
        public string SecretId { get; set; }
        public string SecretKey { get; set; }
    }
}
