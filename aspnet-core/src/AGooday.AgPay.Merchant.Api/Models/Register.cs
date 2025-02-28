namespace AGooday.AgPay.Merchant.Api.Models
{
    public class Register
    {
        public string MchName { get; set; }
        public string Phone { get; set; }
        public string Code { get; set; }
        public string ConfirmPwd { get; set; }
        public byte MchType { get; set; }
        public string InviteCode { get; set; }
    }
}
