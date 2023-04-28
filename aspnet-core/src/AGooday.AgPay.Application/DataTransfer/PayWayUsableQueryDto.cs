using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 支付方式表
    /// </summary>
    public class PayWayUsableQueryDto : PayWayQueryDto
    {
        /// <summary>
        /// 配置模式: mgrIsv-服务商, mgrAgent-代理商, agentSubagent-子代理商, mgrMch-商户, agentMch-代理商商户
        /// </summary>
        public string ConfigMode { get; set; }

        /// <summary>
        /// 服务商号/商户号/应用ID
        /// </summary>
        public string InfoId { get; set; }

        /// <summary>
        /// 支付接口
        /// </summary>
        public string IfCode { get; set; }
    }
}
