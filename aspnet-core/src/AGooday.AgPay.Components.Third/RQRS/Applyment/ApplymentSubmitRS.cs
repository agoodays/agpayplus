using AGooday.AgPay.Components.Third.RQRS.Msg;

namespace AGooday.AgPay.Components.Third.RQRS.Applyment
{
    public class ApplymentSubmitRS : AbstractRS
    {
        /// <summary>渠道返回的申请单号</summary>
        public string ChannelApplyNo { get; set; }

        /// <summary>渠道商户ID (如微信 sub_mch_id, 支付宝 smid)</summary>
        public string ChannelMchId { get; set; }

        /// <summary>签约链接(如有)</summary>
        public string SignUrl { get; set; }

        /// <summary>签约链接过期时间(如有)</summary>
        public DateTime? SignExpireAt { get; set; }

        /// <summary>小额打款金额(分)(如有)</summary>
        public long? VerifyAmount { get; set; }

        /// <summary>渠道原始响应(用于问题排查)</summary>
        public string ChannelResp { get; set; }

        /// <summary>状态: 1-受理成功, 2-审核中, 3-签约待完成, 4-打款验证待完成</summary>
        public byte ChannelState { get; set; }

        /// <summary>错误描述(失败时)</summary>
        public string ErrMsg { get; set; }

        /// <summary>渠道请求报文</summary>
        public string ChannelReqMsg { get; set; }
    }
}
