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
        [AllowedValues("1.0", ErrorMessage = "接口版本号，固定：1.0")]
        public string Version { get; set; }

        /// <summary>
        /// 签名类型
        /// </summary>
        [Required(ErrorMessage = "签名类型不能为空")]
        [AllowedValues("MD5", "RSA2", ErrorMessage = "签名类型，目前仅支持MD5和RSA2")]
        public string SignType { get; set; }

        /// <summary>
        /// 签名值
        /// </summary>
        [Required(ErrorMessage = "签名值不能为空")]
        public string Sign { get; set; }

        /// <summary>
        /// 接口请求时间
        /// </summary>
        [Required(ErrorMessage = "时间戳不能为空")]
        public string ReqTime { get; set; }
    }
}
