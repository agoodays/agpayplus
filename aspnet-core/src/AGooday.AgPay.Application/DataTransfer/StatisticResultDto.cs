using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 统计结果
    /// </summary>
    public class StatisticResultDto : BaseModel
    {
        public long RefundFee { get; set; }
        public long RefundCount { get; set; }
        public long PayAmount { get; set; }
        public long AllAmount { get; set; }
        public long Round { get; set; }
        public long Fee { get; set; }
        public string GroupDate { get; set; }
        public long PayCount { get; set; }
        public long AllCount { get; set; }
        public long RefundAmount { get; set; }
    }
}
