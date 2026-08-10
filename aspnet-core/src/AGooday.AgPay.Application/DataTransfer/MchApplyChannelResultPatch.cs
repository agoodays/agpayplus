namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 渠道结果补丁 — 用于异步场景（MQ 消费者 / 定时任务）写回 DB
    /// 只携带需要更新的字段，避免暴露完整实体
    /// </summary>
    public class MchApplyChannelResultPatch
    {
        public byte? State { get; set; }
        public byte Progress { get; set; }
        public string ChannelApplyNo { get; set; }
        public string ChannelMchId { get; set; }
        public string ApplyErrorInfo { get; set; }
        public string SuccResParameter { get; set; }
        public string SignUrl { get; set; }
        public DateTime? SignExpireAt { get; set; }
        public long? VerifyAmount { get; set; }
    }

    /// <summary>
    /// 标记进件已提交渠道的补丁
    /// </summary>
    public class MchApplyChannelSubmittedPatch
    {
        public string ChannelApplyNo { get; set; }
        public string SuccResParameter { get; set; }
        public byte State { get; set; }
        public byte Progress { get; set; }
        public string ApplyErrorInfo { get; set; }
        public string SignUrl { get; set; }
        public DateTime? SignExpireAt { get; set; }
        public long? VerifyAmount { get; set; }
    }
}
