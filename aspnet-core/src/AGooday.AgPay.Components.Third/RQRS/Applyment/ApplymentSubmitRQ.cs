using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Components.Third.RQRS.Applyment
{
    public class ApplymentSubmitRQ
    {
        /// <summary>进件单(含完整参数)</summary>
        public MchApplyDto MchApply { get; set; }

        /// <summary>服务商号(服务商模式)</summary>
        public string IsvNo { get; set; }

        /// <summary>接口代码</summary>
        public string IfCode { get; set; }
    }
}
