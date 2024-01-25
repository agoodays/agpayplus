namespace AGooday.AgPay.Components.OCR.Models
{
    public class AliyunBankCardOCRResponse
    {
        public OCRResult Data { get; set; }

        public class OCRResult
        {
            public string BankName { get; set; }
            public string CardNumber { get; set; }
            public string ValidToDate { get; set; }
            public string CardType { get; set; }
        }
    }
}
