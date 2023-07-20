using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 支付方式表
    /// </summary>
    public class PayWayUsableQueryDto : PayWayQueryDto
    {
        /// <summary>
        /// 配置模式: 
        /// mgrIsv-服务商
        /// mgrAgent-代理商
        /// mgrMch-商户
        /// mgrApplyment-进件
        /// agentSelf-代理商自身
        /// agentSubagent-子代理商
        /// agentMch-代理商商户
        /// agentApplyment-代理商进件
        /// mchSelfApp1-商户自身应用(普通商户)
        /// mchSelfApp2--商户自身应用(特约商户)
        /// mchApplyment--商户进件
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
