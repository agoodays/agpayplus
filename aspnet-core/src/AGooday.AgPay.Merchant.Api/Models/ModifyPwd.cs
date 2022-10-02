namespace AGooday.AgPay.Merchant.Api.Models
{
    public class ModifyPwd
    {
        public long SysUserId { get; set; }
        public string OriginalPwd { get; set; }
        public string ConfirmPwd { get; set; }
    }
}
