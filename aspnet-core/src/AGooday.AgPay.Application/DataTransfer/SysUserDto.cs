namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 系统用户表
    /// </summary>
    public class SysUserDto
    {
        /// <summary>
        /// 系统用户ID
        /// </summary>
        public long SysUserId { get; set; }

        /// <summary>
        /// 登录用户名
        /// </summary>
        public string LoginUsername { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string Realname { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Telphone { get; set; }

        /// <summary>
        /// 性别 0-未知, 1-男, 2-女
        /// </summary>
        public byte Sex { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 员工编号
        /// </summary>
        public string UserNo { get; set; }

        /// <summary>
        /// 是否超管（超管拥有全部权限） 0-否 1-是
        /// </summary>
        public byte IsAdmin { get; set; }

        /// <summary>
        /// 状态 0-停用 1-启用
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, AGENT-代理商中心, MCH-商户中心
        /// </summary>
        public string SysType { get; set; }

        /// <summary>
        /// 用户类型: 1-超级管理员, 2-普通操作员, 3-商户拓展员, 11-店长, 12-店员
        /// </summary>
        public byte UserType { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>
        public string InviteCode { get; set; }

        /// <summary>
        /// 团队ID
        /// </summary>
        public long? TeamId { get; set; }

        /// <summary>
        /// 是否队长:  0-否 1-是
        /// </summary>
        public byte? IsTeamLeader { get; set; }

        /// <summary>
        /// 所属商户ID / 0(平台)
        /// </summary>
        public string BelongInfoId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
