﻿using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.DataTransfer
{
    public class SysUserAuthInfoDto : BaseModel
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
        /// 简称
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// 商户类型: 1-普通商户, 2-特约商户(服务商模式)
        /// </summary>
        public byte? MchType { get; set; }

        /// <summary>
        /// 商户级别: M0商户-简单模式（页面简洁，仅基础收款功能）, M1商户-高级模式（支持api调用，支持配置应用及分账、转账功能）
        /// </summary>
        public string MchLevel { get; set; }

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
        /// 预留信息
        /// </summary>
        public string SafeWord { get; set; }

        /// <summary>
        /// 初始用户
        /// </summary>
        public bool InitUser { get; set; }

        /// <summary>
        /// 是否超管（超管拥有全部权限） 0-否 1-是
        /// </summary>
        public byte IsAdmin { get; set; }

        /// <summary>
        /// 用户类型: 1-超级管理员, 2-普通操作员, 3-商户拓展员, 11-店长, 12-店员
        /// </summary>
        public byte UserType { get; set; }

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
        public byte State { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, AGENT-代理商中心, MCH-商户中心
        /// </summary>
        public string SysType { get; set; }

        /// <summary>
        /// 登录类型  1-昵称 2-手机号 3-邮箱  10-微信  11-QQ 12-支付宝 13-微博
        /// </summary>
        public byte IdentityType { get; set; }

        /// <summary>
        /// 认证标识 ( 用户名 | open_id )
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// 密码凭证
        /// </summary>
        public string Credential { get; set; }

        /// <summary>
        /// 所属商户ID / 代理商ID / 0(平台)
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
