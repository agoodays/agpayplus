using System.ComponentModel.DataAnnotations;

namespace AGooday.AgPay.Payment.Api.RQRS
{
    /// <summary>
    /// 通用RQ, 包含mchNo和appId 必填项
    /// </summary>
    public class AbstractMchAppRQ : AbstractRQ
    {
        /// <summary>
        /// 商户号
        /// </summary>
        [Required(ErrorMessage = "商户号不能为空")]
        public string mchNo { get; set; }

        /// <summary>
        /// 商户应用ID
        /// </summary>
        [Required(ErrorMessage = "商户应用ID不能为空")]
        public string appId { get; set; }
    }
}
