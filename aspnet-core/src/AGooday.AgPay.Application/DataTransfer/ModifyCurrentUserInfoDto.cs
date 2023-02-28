namespace AGooday.AgPay.Application.DataTransfer
{
    public class ModifyCurrentUserInfoDto
    {
        public long SysUserId { get; set; }
        public string AvatarUrl { get; set; }
        public string Realname { get; set; }
        public string SafeWord { get; set; }
        public byte Sex { get; set; }
    }
}
