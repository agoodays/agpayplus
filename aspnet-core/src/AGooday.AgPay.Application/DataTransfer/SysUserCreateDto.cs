﻿using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 系统用户表
    /// </summary>
    public class SysUserCreateDto
    {
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
        public byte? Sex { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        [BindNever]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 员工编号
        /// </summary>
        public string UserNo { get; set; }

        /// <summary>
        /// 预留信息
        /// </summary>
        public string SafeWord { get; set; }

        /// <summary>
        /// 用户类型: 1-超级管理员, 2-普通操作员, 3-商户拓展员, 11-店长, 12-店员
        /// </summary>
        public byte? UserType { get; set; }

        /// <summary>
        /// 权限配置规则 ["USER_TYPE_11_INIT", "STORE"]
        /// </summary>
        public List<string> EntRules { get; set; }

        /// <summary>
        /// 绑定门店ID [1001, 1002]
        /// </summary>
        public List<long> BindStoreIds { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>
        [BindNever]
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
        /// 状态 0-停用 1-启用
        /// </summary>
        public byte? State { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, AGENT-代理商中心, MCH-商户中心
        /// </summary>
        public string SysType { get; set; }

        /// <summary>
        /// 所属商户ID / 代理商ID / 0(平台)
        /// </summary>
        [BindNever]
        public string BelongInfoId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [BindNever]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [BindNever]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 密码类型
        /// </summary>
        public string PasswordType { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string LoginPassword { get; set; }

        /// <summary>
        /// 是否发送开通提醒
        /// </summary>
        public byte? IsNotify { get; set; }
    }
}
