namespace AGooday.AgPay.Components.OCR.Models
{
    public class AliyunBizLicenseOCRResponse
    {
        public OCRResult Data { get; set; }

        public class OCRResult
        {
            public string CreditCode { get; set; }
            public string CompanyName { get; set; }
            public string CompanyType { get; set; }
            public string BusinessAddress { get; set; }
            public string LegalPerson { get; set; }
            public string BusinessScope { get; set; }
            public string RegisteredCapital { get; set; }
            public string RegistrationDate { get; set; }
            public string ValidPeriod { get; set; }
            public string ValidFromDate { get; set; }
            public string ValidToDate { get; set; }
            public string CompanyForm { get; set; }
        }
    }
}
