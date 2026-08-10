namespace AGooday.AgPay.Components.Third.RQRS.Applyment
{
    public class ApplymentQueryRS : AbstractRS
    {
        /// <summary>渠道状态: SUCCESS/FAIL/ING/SIGNING/VERIFYING</summary>
        public string ChannelState { get; set; }

        /// <summary>渠道状态描述</summary>
        public string ChannelStateDesc { get; set; }

        /// <summary>渠道商户ID(审核通过后)</summary>
        public string ChannelMchId { get; set; }

        /// <summary>签约链接(签约待完成时)</summary>
        public string SignUrl { get; set; }

        /// <summary>小额打款金额(分)(验证阶段)</summary>
        public long? VerifyAmount { get; set; }

        /// <summary>渠道原始响应</summary>
        public string ChannelResp { get; set; }
    }
}
