namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 商户进件创建/修改
    /// </summary>
    public class MchApplySaveDto
    {
        /// <summary>进件单号(修改时必填)</summary>
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

        /// <summary>申请详细信息(JSON, 非标准基础字段的汇总)</summary>
        public string ApplyDetailInfo { get; set; }

        /// <summary>进件参数详情(JSON, 各通道差异化参数)</summary>
        public string ApplyParams { get; set; }

        /// <summary>商户名称全称</summary>
        public string MchFullName { get; set; }

        /// <summary>进件商户简称</summary>
        public string MchShortName { get; set; }

        /// <summary>商户类型: 1-个人, 2-个体工商户, 3-企业</summary>
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
    }
}
