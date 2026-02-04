namespace AGooday.AgPay.Base.Api.Models
{
    public class TransferOrderModel
    {
        public string AppId { get; set; }
        public string MchOrderNo { get; set; }
        public string IfCode { get; set; }
        public string EntryType { get; set; }
        public decimal Amount { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public string TransferDesc { get; set; }
    }
}
