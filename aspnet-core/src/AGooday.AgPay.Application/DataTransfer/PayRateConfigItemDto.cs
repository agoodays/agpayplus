namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 支付费率信息表
    /// </summary>
    public class PayRateConfigSaveDto
    {
        public string InfoId { get; set; }
        public string IfCode { get; set; }
        public string ConfigMode { get; set; }
        public byte NoCheckRuleFlag { get; set; }
        public List<PayRateConfigItem> ISVCOST { get; set; }
        public List<PayRateConfigItem> AGENTDEF { get; set; }
        public List<PayRateConfigItem> MCHAPPLYDEF { get; set; }
        public List<PayRateConfigItem> AGENTRATE { get; set; }
        public List<PayRateConfigItem> MCHRATE { get; set; }

        public class PayRateConfigItem
        {
            public string WayCode { get; set; }
            public byte State { get; set; }
            public string FeeType { get; set; }
            public string LevelMode { get; set; }
            public byte ApplymentSupport { get; set; }
            public decimal? FeeRate { get; set; }
            public List<Levels> UNIONPAY { get; set; }
            public List<Levels> NORMAL { get; set; }
        }

        public class Levels
        {
            public int MinFee { get; set; }
            public int MaxFee { get; set; }
            public string BankCardType { get; set; }
            public List<LevelList> LevelList { get; set; }
        }

        public class LevelList
        {
            public long Id { get; set; }
            public int MinAmount { get; set; }
            public int MaxAmount { get; set; }
            public decimal? FeeRate { get; set; }
        }
    }
}
