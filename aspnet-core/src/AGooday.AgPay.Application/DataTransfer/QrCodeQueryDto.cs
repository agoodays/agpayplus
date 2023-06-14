using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 码牌信息表
    /// </summary>
    public class QrCodeQueryDto : DatePageQuery
    {
        /// <summary>
        /// 码牌ID
        /// </summary>
        public string QrcId { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchId { get; set; }

        /// <summary>
        /// 代理商号
        /// </summary>
        public string AgentNo { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        public long? StoreId { get; set; }

        /// <summary>
        /// 状态: 0-停用, 1-启用
        /// </summary>
        public byte? State { get; set; }
    }
}
