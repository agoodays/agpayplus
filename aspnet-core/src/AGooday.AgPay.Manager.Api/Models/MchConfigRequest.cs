namespace AGooday.AgPay.Manager.Api.Models
{
    public class MchConfigRequest
    {
        public string MchNo { get; set; }

        public Dictionary<string, string> Configs { get; set; }
    }
}
