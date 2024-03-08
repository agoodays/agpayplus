namespace AGooday.AgPay.Application.DataTransfer
{
    public class SysEntMatchRuleSetDto
    {
        /// <summary>
        /// 所属系统： MGR-运营平台, AGENT-代理商中心, MCH-商户中心
        /// </summary>
        public string SysType { get; set; }

        /// <summary>
        /// 操作类型: add-新增, delete-删除, modify-修改
        /// </summary>
        public string OpType { get; set; }

        /// <summary>
        /// 权限信息集合
        /// </summary>
        public List<string> EntIds { get; set; }

        /// <summary>
        /// 权限匹配规则
        /// </summary>
        public SysEntitlementDto.EntMatchRule MatchRule { get; set; }
    }
}
