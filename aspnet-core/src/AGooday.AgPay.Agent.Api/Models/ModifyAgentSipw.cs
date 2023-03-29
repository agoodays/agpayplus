namespace AGooday.AgPay.Agent.Api.Models
{
    public class ModifyAgentSipw
    {
        public long RecordId { get; set; }
        public string OriginalPwd { get; set; }
        public string ConfirmPwd { get; set; }
    }
}
