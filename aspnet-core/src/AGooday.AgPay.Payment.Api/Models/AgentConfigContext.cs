using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Payment.Api.Models
{
    /// <summary>
    /// 代理商配置信息
    /// 放置到内存， 避免多次查询操作
    /// </summary>
    public class AgentConfigContext
    {
        #region agent信息缓存
        /// <summary>
        /// 代理商号
        /// </summary>
        public string AgentNo { get; set; }
        /// <summary>
        /// 代理商信息
        /// </summary>
        public AgentInfoDto AgentInfo { get; set; }
        #endregion
    }
}
