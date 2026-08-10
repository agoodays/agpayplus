namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 商户进件审核
    /// </summary>
    public class MchApplyAuditDto
    {
        /// <summary>进件单号</summary>
        public string ApplyId { get; set; }

        /// <summary>审核动作: PASS-通过, REJECT-驳回</summary>
        public string Action { get; set; }

        /// <summary>审核意见(驳回时必填)</summary>
        public string Remark { get; set; }
    }
}
