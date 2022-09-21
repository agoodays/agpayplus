namespace AGooday.AgPay.Manager.Api.Extensions.AuthContext
{
    /// <summary>
    /// 登录用户上下文
    /// </summary>
    public class AuthContextUser
    {
        public string Name { get; set; }
        public string UserId { get; set; }
        /// <summary>
        /// 头像地址
        /// </summary>
        public string Avator { get; set; }
        public string Avatar { get; set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }
        public string Telphone { get; set; }
        public string UserNo { get; set; }
        public string IdentityType { get; set; }
        public string SysType { get; set; }
        public string CacheKey { get; set; }
    }
}
