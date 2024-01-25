namespace AGooday.AgPay.Components.OCR.Models
{
    public class AliyunOcrConfig : AbstractOcrConfig
    {
        public string Endpoint { get; set; } = "ocr-api.cn-hangzhou.aliyuncs.com";
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
    }
}
