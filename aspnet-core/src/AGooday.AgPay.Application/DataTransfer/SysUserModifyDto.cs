namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 系统用户表
    /// </summary>
    public class SysUserModifyDto : SysUserCreateDto
    {
        /// <summary>
        /// 系统用户ID
        /// </summary>
        public long SysUserId { get; set; }

        /// <summary>
        /// 默认密码
        /// </summary>
        public bool DefaultPass { get; set; }

        /// <summary>
        /// 重置密码
        /// </summary>
        public bool ResetPass { get; set; }

        /// <summary>
        /// 确认密码
        /// </summary>
        public string ConfirmPwd { get; set; }

        /// <summary>
        /// 当前系统用户ID
        /// </summary>
        public long CurrentSysUserId { get; set; }
    }
}
