namespace AGooday.AgPay.Merchant.Api.Models
{
    public class PayOrderModel
    {
        public string AppId { get; set; }
        public decimal Amount { get; set; }
        public string MchOrderNo { get; set; }
        public string WayCode { get; set; }
        public byte DivisionMode { get; set; }
        public string OrderTitle { get; set; }
        public string PayDataType { get; set; }
        public string AuthCode { get; set; }
    }
}
