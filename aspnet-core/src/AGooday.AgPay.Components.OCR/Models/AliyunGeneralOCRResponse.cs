namespace AGooday.AgPay.Components.OCR.Models
{
    public class AliyunGeneralOCRResponse
    {
        public OCRResult Data { get; set; }

        public class OCRResult
        {
            public string Content { get; set; }
        }
    }
}
