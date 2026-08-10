using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 商户进件申请表
    /// </summary>
    public class MchApplyDto : BaseModel
    {
        /// <summary>进件单号</summary>
        public string ApplyId { get; set; }

        /// <summary>商户号</summary>
        public string MchNo { get; set; }

        /// <summary>代理商号</summary>
        public string AgentNo { get; set; }

        /// <summary>顶级代理商号</summary>
        public string TopAgentNo { get; set; }

        /// <summary>服务商号</summary>
        public string IsvNo { get; set; }

        /// <summary>接口代码</summary>
        public string IfCode { get; set; }

        /// <summary>自动配置到应用ID</summary>
        public string AutoConfigMchAppId { get; set; }

        /// <summary>来源</summary>
        public string ApplyPageType { get; set; }

        /// <summary>申请详细信息(JSON)</summary>
        public string ApplyDetailInfo { get; set; }

        /// <summary>进件参数详情(JSON)</summary>
        public string ApplyParams { get; set; }

        /// <summary>申请错误信息</summary>
        public string ApplyErrorInfo { get; set; }

        /// <summary>渠道申请单号</summary>
        public string ChannelApplyNo { get; set; }

        /// <summary>渠道返回的商户标识</summary>
        public string ChannelMchId { get; set; }

        /// <summary>成功响应参数</summary>
        public string SuccResParameter { get; set; }

        /// <summary>渠道扩展参数(JSON)</summary>
        public string ChannelExtParams { get; set; }

        /// <summary>状态</summary>
        public byte State { get; set; }

        /// <summary>状态名称</summary>
        public string StateName { get; set; }

        /// <summary>审核人用户ID</summary>
        public long? AuditUid { get; set; }

        /// <summary>审核人姓名</summary>
        public string AuditBy { get; set; }

        /// <summary>审核意见</summary>
        public string AuditRemark { get; set; }

        /// <summary>审核时间</summary>
        public DateTime? AuditedAt { get; set; }

        /// <summary>签约链接</summary>
        public string SignUrl { get; set; }

        /// <summary>签约链接过期时间</summary>
        public DateTime? SignExpireAt { get; set; }

        /// <summary>小额打款金额(分)</summary>
        public long? VerifyAmount { get; set; }

        /// <summary>验证金额/验证码</summary>
        public string VerifyCode { get; set; }

        /// <summary>验证时间</summary>
        public DateTime? VerifiedAt { get; set; }

        /// <summary>进件进度百分比</summary>
        public byte Progress { get; set; }

        /// <summary>是否临时数据</summary>
        public bool IsTempData { get; set; }

        /// <summary>商户名称全称</summary>
        public string MchFullName { get; set; }

        /// <summary>进件商户简称</summary>
        public string MchShortName { get; set; }

        /// <summary>商户类型</summary>
        public byte MerchantType { get; set; }

        /// <summary>联系人姓名</summary>
        public string ContactName { get; set; }

        /// <summary>联系人电话</summary>
        public string ContactPhone { get; set; }

        /// <summary>联系人邮箱</summary>
        public string ContactEmail { get; set; }

        /// <summary>省代码</summary>
        public string ProvinceCode { get; set; }

        /// <summary>市代码</summary>
        public string CityCode { get; set; }

        /// <summary>区代码</summary>
        public string DistrictCode { get; set; }

        /// <summary>详细地址</summary>
        public string Address { get; set; }

        /// <summary>拓展员ID</summary>
        public long? EpUserId { get; set; }

        /// <summary>上次进件时间</summary>
        public DateTime LastApplyAt { get; set; }

        /// <summary>创建者用户ID</summary>
        public long? CreatedUid { get; set; }

        /// <summary>创建者姓名</summary>
        public string CreatedBy { get; set; }

        /// <summary>创建时间</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>更新时间</summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>接口名称(扩展)</summary>
        public string IfName { get; set; }

        /// <summary>代理商名称(扩展)</summary>
        public string AgentName { get; set; }

        /// <summary>服务商名称(扩展)</summary>
        public string IsvName { get; set; }
    }
}
