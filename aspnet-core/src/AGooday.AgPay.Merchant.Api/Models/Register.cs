namespace AGooday.AgPay.Merchant.Api.Models
{
    public class Register
    {
        public string mchName { get; set; }
        public string phone { get; set; }
        public string code { get; set; }
        public string confirmPwd { get; set; }
        public byte mchType { get; set; }
        public string inviteCode { get; set; }
    }
}
