using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 商户进件查询
    /// </summary>
    public class MchApplyQueryDto : PageQuery
    {
        /// <summary>进件单号</summary>
        public string ApplyId { get; set; }

        /// <summary>商户号</summary>
        public string MchNo { get; set; }

        /// <summary>代理商号</summary>
        public string AgentNo { get; set; }

        /// <summary>服务商号</summary>
        public string IsvNo { get; set; }

        /// <summary>接口代码</summary>
        public string IfCode { get; set; }

        /// <summary>状态</summary>
        public byte? State { get; set; }

        /// <summary>商户名称(模糊)</summary>
        public string MchFullName { get; set; }

        /// <summary>商户类型</summary>
        public byte? MerchantType { get; set; }

        /// <summary>来源</summary>
        public string ApplyPageType { get; set; }

        /// <summary>创建者用户ID</summary>
        public long? CreatedUid { get; set; }
    }
}
