﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 代理商信息表
    /// </summary>
    [Comment("代理商信息表")]
    [Table("t_agent_info")]
    public class AgentInfo
    {
        /// <summary>
        /// 代理商号
        /// </summary>
        [Comment("代理商号")]
        [Key, Required, Column("agent_no", TypeName = "varchar(64)")]
        public string AgentNo { get; set; }

        /// <summary>
        /// 代理商名称
        /// </summary>
        [Comment("代理商名称")]
        [Required, Column("agent_name", TypeName = "varchar(64)")]
        public string AgentName { get; set; }

        /// <summary>
        /// 代理商简称
        /// </summary>
        [Comment("代理商简称")]
        [Required, Column("agent_short_name", TypeName = "varchar(32)")]
        public string AgentShortName { get; set; }

        /// <summary>
        /// 代理商类型: 1-个人, 2-企业
        /// </summary>
        [Comment("代理商类型: 1-个人, 2-企业")]
        [Required, Column("agent_type", TypeName = "tinyint(6)")]
        public byte AgentType { get; set; }

        /// <summary>
        /// 等级: 1-一级, 2-二级, 3-三级 ...
        /// </summary>
        [Comment("等级: 1-一级, 2-二级, 3-三级 ...")]
        [Required, Column("level", TypeName = "tinyint(6)")]
        public byte Level { get; set; }

        /// <summary>
        /// 上级代理商号（外键属性）
        /// </summary>
        [Comment("上级代理商号")]
        [Column("pid", TypeName = "varchar(64)")]
        public string Pid { get; set; }

        /// <summary>
        /// 服务商号
        /// </summary>
        [Comment("服务商号")]
        [Column("isv_no", TypeName = "varchar(64)")]
        public string IsvNo { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        [Comment("联系人姓名")]
        [Required, Column("contact_name", TypeName = "varchar(32)")]
        public string ContactName { get; set; }

        /// <summary>
        /// 联系人手机号
        /// </summary>
        [Comment("联系人手机号")]
        [Required, Column("contact_tel", TypeName = "varchar(32)")]
        public string ContactTel { get; set; }

        /// <summary>
        /// 联系人邮箱
        /// </summary>
        [Comment("联系人邮箱")]
        [Column("contact_email", TypeName = "varchar(32)")]
        public string ContactEmail { get; set; }

        /// <summary>
        /// 是否允许发展下级: 0-否, 1-是
        /// </summary>
        [Comment("是否允许发展下级: 0-否, 1-是")]
        [Required, Column("add_agent_flag", TypeName = "tinyint(6)")]
        public byte AddAgentFlag { get; set; }

        /// <summary>
        /// 支付密码
        /// </summary>
        [Comment("支付密码")]
        [Column("sipw", TypeName = "varchar(128)")]
        public string Sipw { get; set; }

        /// <summary>
        /// 状态: 0-停用, 1-正常
        /// </summary>
        [Comment("状态: 0-停用, 1-正常")]
        [Required, Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Comment("备注")]
        [Column("remark", TypeName = "varchar(128)")]
        public string Remark { get; set; }

        /// <summary>
        /// 初始用户ID（创建代理商时，允许代理商登录的用户）
        /// </summary>
        [Comment("初始用户ID（创建代理商时，允许代理商登录的用户）")]
        [Column("init_user_id", TypeName = "bigint")]
        public long? InitUserId { get; set; }

        /// <summary>
        /// 账户类型: ALIPAY_CASH-支付宝转账, WX_CASH-微信零钱, BANK_CARD-银行卡
        /// </summary>
        [Comment("账户类型: ALIPAY_CASH-支付宝转账, WX_CASH-微信零钱, BANK_CARD-银行卡")]
        [Column("sett_account_type", TypeName = "varchar(32)")]
        public string SettAccountType { get; set; }

        /// <summary>
        /// 结算账户名
        /// </summary>
        [Comment("结算账户名")]
        [Column("sett_account_name", TypeName = "varchar(32)")]
        public string SettAccountName { get; set; }

        /// <summary>
        /// 结算人手机号
        /// </summary>
        [Comment("结算人手机号")]
        [Column("sett_account_telphone", TypeName = "varchar(32)")]
        public string SettAccountTelphone { get; set; }

        /// <summary>
        /// 结算账号
        /// </summary>
        [Comment("结算账号")]
        [Column("sett_account_no", TypeName = "varchar(32)")]
        public string SettAccountNo { get; set; }

        /// <summary>
        /// 开户行名称
        /// </summary>
        [Comment("开户行名称")]
        [Column("sett_account_bank", TypeName = "varchar(32)")]
        public string SettAccountBank { get; set; }

        /// <summary>
        /// 提现配置类型: 1-使用系统默认, 2-自定义
        /// </summary>
        [Comment("提现配置类型: 1-使用系统默认, 2-自定义")]
        [Required, Column("cashout_fee_rule_type", TypeName = "tinyint(6)")]
        public byte CashoutFeeRuleType { get; set; }

        /// <summary>
        /// 提现手续费规则
        /// </summary>
        [Comment("提现手续费规则")]
        [Column("cashout_fee_rule", TypeName = "varchar(255)")]
        public string CashoutFeeRule { get; set; }

        /// <summary>
        /// 营业执照照片
        /// </summary>
        [Comment("营业执照照片")]
        [Column("license_img", TypeName = "varchar(128)")]
        public string LicenseImg { get; set; }

        /// <summary>
        /// 开户许可证照片
        /// </summary>
        [Comment("开户许可证照片")]
        [Column("permit_img", TypeName = "varchar(128)")]
        public string PermitImg { get; set; }

        /// <summary>
        /// 营业执照照片
        /// </summary>
        [Comment("身份证人像面照片")]
        [Column("idcard1_img", TypeName = "varchar(128)")]
        public string Idcard1Img { get; set; }

        /// <summary>
        /// 营业执照照片
        /// </summary>
        [Comment("身份证国徽面照片")]
        [Column("idcard2_img", TypeName = "varchar(128)")]
        public string Idcard2Img { get; set; }

        /// <summary>
        /// 营业执照照片
        /// </summary>
        [Comment("手持身份证照片")]
        [Column("idcard_in_hand_img", TypeName = "varchar(128)")]
        public string IdcardInHandImg { get; set; }

        /// <summary>
        /// 营业执照照片
        /// </summary>
        [Comment("银行卡照片")]
        [Column("bank_card_img", TypeName = "varchar(128)")]
        public string BankCardImg { get; set; }

        /// <summary>
        /// 不可用金额
        /// </summary>
        [Comment("不可用金额")]
        [Required, Column("un_amount", TypeName = "bigint")]
        public long UnAmount { get; set; }

        /// <summary>
        /// 钱包余额
        /// </summary>
        [Comment("钱包余额")]
        [Required, Column("balance_amount", TypeName = "bigint")]
        public long BalanceAmount { get; set; }

        /// <summary>
        /// 在途佣金
        /// </summary>
        [Comment("在途佣金")]
        [Required, Column("audit_profit_amount", TypeName = "bigint")]
        public long AuditProfitAmount { get; set; }

        /// <summary>
        /// 冻结金额
        /// </summary>
        [Comment("冻结金额")]
        [Required, Column("freeze_amount", TypeName = "bigint")]
        public long FreezeAmount { get; set; }

        /// <summary>
        /// 创建者用户ID
        /// </summary>
        [Comment("创建者用户ID")]
        [Column("created_uid", TypeName = "bigint")]
        public long? CreatedUid { get; set; }

        /// <summary>
        /// 创建者姓名
        /// </summary>
        [Comment("创建者姓名")]
        [Column("created_by", TypeName = "varchar(64)")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Comment("创建时间")]
        [Required, Column("created_at", TypeName = "timestamp(6)")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Comment("更新时间")]
        [Required, Column("updated_at", TypeName = "timestamp(6)")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 导航属性（上级代理商）
        /// </summary>
        public virtual AgentInfo ParentAgent { get; set; }

        /// <summary>
        /// 子代理商集合
        /// </summary>
        public virtual ICollection<AgentInfo> SubAgents { get; set; }
    }
}
