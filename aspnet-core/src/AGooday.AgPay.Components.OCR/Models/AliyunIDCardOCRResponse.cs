namespace AGooday.AgPay.Components.OCR.Models
{
    public class AliyunIDCardOCRResponse
    {
        public OCRData Face { get; set; }
        public OCRData Back { get; set; }

        public class OCRData
        {
            public OCRResult Data { get; set; }
        }

        public class OCRResult
        {
            public string Name { get; set; }
            public string Sex { get; set; }
            public string Ethnicity { get; set; }
            public string BirthDate { get; set; }
            public string Address { get; set; }
            public string IdNumber { get; set; }
            public string IssueAuthority { get; set; }
            public string ValidPeriod { get; set; }
        }
    }
}
