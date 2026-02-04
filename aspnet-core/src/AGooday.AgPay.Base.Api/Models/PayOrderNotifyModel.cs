namespace AGooday.AgPay.Base.Api.Models
{
    public class PayOrderNotifyModel
    {
        public string PayOrderId { get; set; }
        public string MchNo { get; set; }
        public string AppId { get; set; }
        public byte State { get; set; }
        public string ErrCode { get; set; }
        public string ErrMsg { get; set; }
        public string Sign { get; set; }
    }
}
