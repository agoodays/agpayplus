namespace AGooday.AgPay.Agent.Api.Models
{
    public class RefundOrderModel
    {
        public long RefundAmount { get; set; }
        public string RefundPassword { get; set; }
        public string RefundReason { get; set; }
    }
}
