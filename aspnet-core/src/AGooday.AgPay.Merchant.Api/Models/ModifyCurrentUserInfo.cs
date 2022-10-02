namespace AGooday.AgPay.Merchant.Api.Models
{
    public class ModifyCurrentUserInfo
    {
        public long SysUserId { get; set; }
        public string AvatarUrl { get; set; }
        public string Realname { get; set; }
        public byte Sex { get; set; }
    }
}
