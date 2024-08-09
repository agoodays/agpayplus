using System.ComponentModel.DataAnnotations;

namespace AGooday.AgPay.Components.Third.RQRS
{
    public class ChannelUserIdRQ : AbstractMchAppRQ
    {
        /// <summary>
        /// 接口代码,  AUTO表示：自动获取
        /// </summary>
        [Required(ErrorMessage = "接口代码不能为空")]
        public string IfCode { get; set; }

        /// <summary>
        /// 商户扩展参数，将原样返回
        /// </summary>
        public string ExtParam { get; set; }

        /// <summary>
        /// 回调地址
        /// </summary>
        [Required(ErrorMessage = "回调地址不能为空")]
        public string RedirectUrl { get; set; }
    }
}
