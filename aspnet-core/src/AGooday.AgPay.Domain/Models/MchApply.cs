using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AGooday.AgPay.Domain.Core.Models;
using AGooday.AgPay.Domain.Core.Tracker;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 商户进件申请表
    /// </summary>
    [Table("t_mch_apply")]
    [Comment("商户进件申请表")]
    public class MchApply : AbstractTrackableTimestamps, ITrackableUser
    {
        /// <summary>进件单号</summary>
        [Comment("进件单号")]
        [Key, Required, Column("apply_id", TypeName = "varchar(64)")]
        public string ApplyId { get; set; }

        /// <summary>商户号</summary>
        [Comment("商户号")]
        [Required, Column("mch_no", TypeName = "varchar(64)")]
        public string MchNo { get; set; }

        /// <summary>代理商号</summary>
        [Comment("代理商号")]
        [Column("agent_no", TypeName = "varchar(64)")]
        public string AgentNo { get; set; }

        /// <summary>顶级代理商号</summary>
        [Comment("顶级代理商号")]
        [Column("top_agent_no", TypeName = "varchar(64)")]
        public string TopAgentNo { get; set; }

        /// <summary>服务商号</summary>
        [Comment("服务商号")]
        [Column("isv_no", TypeName = "varchar(64)")]
        public string IsvNo { get; set; }

        /// <summary>接口代码 全小写 wxpay alipay</summary>
        [Comment("接口代码 全小写 wxpay alipay")]
        [Required, Column("if_code", TypeName = "varchar(20)")]
        public string IfCode { get; set; }

        /// <summary>自动配置到应用ID</summary>
        [Comment("自动配置到应用ID")]
        [Column("auto_config_mch_app_id", TypeName = "varchar(64)")]
        public string AutoConfigMchAppId { get; set; }

        /// <summary>来源: PLATFORM_WEB/AGENT_WEB/MCH_WEB/AGENT_LITE/MCH_LITE</summary>
        [Comment("来源: PLATFORM_WEB/AGENT_WEB/MCH_WEB/AGENT_LITE/MCH_LITE")]
        [Required, Column("apply_page_type", TypeName = "varchar(20)")]
        public string ApplyPageType { get; set; }

        /// <summary>申请详细信息(JSON)</summary>
        [Comment("申请详细信息(JSON)")]
        [Column("apply_detail_info", TypeName = "text")]
        public string ApplyDetailInfo { get; set; }

        /// <summary>进件参数详情(各通道差异化参数, JSON)</summary>
        [Comment("进件参数详情(各通道差异化参数, JSON)")]
        [Column("apply_params", TypeName = "json")]
        public string ApplyParams { get; set; }

        /// <summary>申请错误信息</summary>
        [Comment("申请错误信息")]
        [Column("apply_error_info", TypeName = "text")]
        public string ApplyErrorInfo { get; set; }

        /// <summary>渠道申请单号</summary>
        [Comment("渠道申请单号")]
        [Column("channel_apply_no", TypeName = "varchar(64)")]
        public string ChannelApplyNo { get; set; }

        /// <summary>渠道返回的商户标识(如微信sub_mch_id、支付宝smid)</summary>
        [Comment("渠道返回的商户标识(如微信sub_mch_id、支付宝smid)")]
        [Column("channel_mch_id", TypeName = "varchar(64)")]
        public string ChannelMchId { get; set; }

        /// <summary>成功响应参数(渠道响应参数)</summary>
        [Comment("成功响应参数(渠道响应参数)")]
        [Column("succ_res_parameter", TypeName = "text")]
        public string SuccResParameter { get; set; }

        /// <summary>渠道扩展参数(JSON, 替代原 channel_var1/2)</summary>
        [Comment("渠道扩展参数(JSON, 替代原 channel_var1/2)")]
        [Column("channel_ext_params", TypeName = "json")]
        public string ChannelExtParams { get; set; }

        /// <summary>状态: 0-草稿, 7-等待预审, 8-预审拒绝, 1-审核中, 3-驳回待修改, 5-待签约, 4-待验证, 2-进件成功</summary>
        [Comment("状态: 0-草稿, 7-等待预审, 8-预审拒绝, 1-审核中, 3-驳回待修改, 5-待签约, 4-待验证, 2-进件成功")]
        [Required, Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }

        /// <summary>审核人用户ID</summary>
        [Comment("审核人用户ID")]
        [Column("audit_uid", TypeName = "bigint(20)")]
        public long? AuditUid { get; set; }

        /// <summary>审核人姓名</summary>
        [Comment("审核人姓名")]
        [Column("audit_by", TypeName = "varchar(64)")]
        public string AuditBy { get; set; }

        /// <summary>审核意见</summary>
        [Comment("审核意见")]
        [Column("audit_remark", TypeName = "varchar(512)")]
        public string AuditRemark { get; set; }

        /// <summary>审核时间</summary>
        [Comment("审核时间")]
        [Column("audited_at", TypeName = "timestamp(6)")]
        public DateTime? AuditedAt { get; set; }

        /// <summary>签约链接</summary>
        [Comment("签约链接")]
        [Column("sign_url", TypeName = "varchar(512)")]
        public string SignUrl { get; set; }

        /// <summary>签约链接过期时间</summary>
        [Comment("签约链接过期时间")]
        [Column("sign_expire_at", TypeName = "timestamp(6)")]
        public DateTime? SignExpireAt { get; set; }

        /// <summary>小额打款金额(分)</summary>
        [Comment("小额打款金额(分)")]
        [Column("verify_amount", TypeName = "bigint(20)")]
        public long? VerifyAmount { get; set; }

        /// <summary>验证金额/验证码</summary>
        [Comment("验证金额/验证码")]
        [Column("verify_code", TypeName = "varchar(32)")]
        public string VerifyCode { get; set; }

        /// <summary>验证时间</summary>
        [Comment("验证时间")]
        [Column("verified_at", TypeName = "timestamp(6)")]
        public DateTime? VerifiedAt { get; set; }

        /// <summary>进件进度百分比: 0-100</summary>
        [Comment("进件进度百分比: 0-100")]
        [Required, Column("progress", TypeName = "tinyint(6)")]
        public byte Progress { get; set; }

        /// <summary>是否临时数据</summary>
        [Comment("是否临时数据")]
        [Required, Column("is_temp_data")]
        public bool IsTempData { get; set; }

        /// <summary>商户名称全称</summary>
        [Comment("商户名称全称")]
        [Required, Column("mch_full_name", TypeName = "varchar(64)")]
        public string MchFullName { get; set; }

        /// <summary>进件商户简称</summary>
        [Comment("进件商户简称")]
        [Required, Column("mch_short_name", TypeName = "varchar(32)")]
        public string MchShortName { get; set; }

        /// <summary>商户类型: 1-个人, 2-个体工商户, 3-企业</summary>
        [Comment("商户类型: 1-个人, 2-个体工商户, 3-企业")]
        [Required, Column("merchant_type", TypeName = "tinyint(6)")]
        public byte MerchantType { get; set; }

        /// <summary>商户联系人姓名</summary>
        [Comment("商户联系人姓名")]
        [Required, Column("contact_name", TypeName = "varchar(32)")]
        public string ContactName { get; set; }

        /// <summary>商户联系人电话</summary>
        [Comment("商户联系人电话")]
        [Required, Column("contact_phone", TypeName = "varchar(32)")]
        public string ContactPhone { get; set; }

        /// <summary>商户联系人邮箱</summary>
        [Comment("商户联系人邮箱")]
        [Column("contact_email", TypeName = "varchar(32)")]
        public string ContactEmail { get; set; }

        /// <summary>省代码</summary>
        [Comment("省代码")]
        [Required, Column("province_code", TypeName = "varchar(32)")]
        public string ProvinceCode { get; set; }

        /// <summary>市代码</summary>
        [Comment("市代码")]
        [Required, Column("city_code", TypeName = "varchar(32)")]
        public string CityCode { get; set; }

        /// <summary>区代码</summary>
        [Comment("区代码")]
        [Required, Column("district_code", TypeName = "varchar(32)")]
        public string DistrictCode { get; set; }

        /// <summary>商户详细地址</summary>
        [Comment("商户详细地址")]
        [Required, Column("address", TypeName = "varchar(128)")]
        public string Address { get; set; }

        /// <summary>商户拓展员ID</summary>
        [Comment("商户拓展员ID")]
        [Column("ep_user_id", TypeName = "bigint(20)")]
        public long? EpUserId { get; set; }

        /// <summary>上次进件时间</summary>
        [Comment("上次进件时间")]
        [Required, Column("last_apply_at", TypeName = "timestamp(6)")]
        public DateTime LastApplyAt { get; set; }

        /// <summary>创建者用户ID</summary>
        [Comment("创建者用户ID")]
        [Column("created_uid", TypeName = "bigint(20)")]
        public long? CreatedUid { get; set; }

        /// <summary>创建者姓名</summary>
        [Comment("创建者姓名")]
        [Column("created_by", TypeName = "varchar(64)")]
        public string CreatedBy { get; set; }
    }
}
