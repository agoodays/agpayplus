using System.ComponentModel.DataAnnotations;

namespace AGooday.AgPay.Payment.Api.RQRS
{
    /// <summary>
    /// 基础请求参数
    /// </summary>
    public abstract class AbstractRQ
    {
        /// <summary>
        /// 版本号
        /// </summary>
        [Required(ErrorMessage = "版本号不能为空")]
        protected string version { get; set; }

        /// <summary>
        /// 签名类型
        /// </summary>
        [Required(ErrorMessage = "签名类型不能为空")]
        protected string signType { get; set; }

        /// <summary>
        /// 签名值
        /// </summary>
        [Required(ErrorMessage = "签名值不能为空")]
        protected string sign { get; set; }

        /// <summary>
        /// 接口请求时间
        /// </summary>
        [Required(ErrorMessage = "时间戳不能为空")]
        protected string reqTime { get; set; }
    }
}
