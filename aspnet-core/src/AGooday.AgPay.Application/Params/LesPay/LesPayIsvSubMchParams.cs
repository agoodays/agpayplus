namespace AGooday.AgPay.Application.Params.LesPay
{
    public class LesPayIsvSubMchParams : IsvSubMchParams
    {
        /// <summary>
        /// 商户编号
        /// </summary>
        public string MerchantId { get; set; }

        public string SubMchAppId { get; set; }

        public string SubMchLiteAppId { get; set; }

        /// <summary>
        /// 是否禁止信用卡（默认为不禁用）
        /// </summary>
        public byte? LimitPay { get; set; }

        /// <summary>
        /// T0交易标志
        /// 默认为0，0：d1交易 1：d0交易
        /// </summary>
        public byte? T0 { get; set; }
    }
}
