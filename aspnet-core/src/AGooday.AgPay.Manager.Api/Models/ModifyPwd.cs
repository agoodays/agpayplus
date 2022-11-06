namespace AGooday.AgPay.Manager.Api.Models
{
    public class ModifyPwd
    {
        public long RecordId { get; set; }
        public string OriginalPwd { get; set; }
        public string ConfirmPwd { get; set; }
    }
}
