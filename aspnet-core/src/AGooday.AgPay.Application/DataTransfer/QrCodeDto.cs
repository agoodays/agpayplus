namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 码牌信息表
    /// </summary>
    public class QrCodeDto
    {
        /// <summary>
        /// 码牌ID
        /// </summary>
        public string QrcId { get; set; }

        /// <summary>
        /// 码牌模板ID
        /// </summary>
        public long? QrcShellId { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchId { get; set; }

        /// <summary>
        /// 是否固定金额: 0-任意金额, 1-固定金额
        /// </summary>
        public byte FixedFlag { get; set; }

        /// <summary>
        /// 固定金额
        /// </summary>
        public int FixedPayAmount { get; set; }

        /// <summary>
        /// 选择页面类型: default-默认(未指定，取决于二维码是否绑定到微信侧), h5-固定H5页面, lite-固定小程序页面
        /// </summary>
        public string EntryPage { get; set; }

        /// <summary>
        /// 支付宝支付方式(仅H5呈现时生效)
        /// </summary>
        public string AlipaWayCode { get; set; }

        /// <summary>
        /// 码牌别名
        /// </summary>
        public string QrcAlias { get; set; }

        /// <summary>
        /// 码牌绑定状态: 0-未绑定, 1-已绑定
        /// </summary>
        public byte BindState { get; set; }

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
        /// 二维码Url
        /// </summary>
        public string QrUrl { get; set; }

        /// <summary>
        /// 状态: 0-停用, 1-启用
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// 所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心
        /// </summary>
        public string SysType { get; set; }

        /// <summary>
        /// 所属商户ID / 所属代理商ID / 0(平台)
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
