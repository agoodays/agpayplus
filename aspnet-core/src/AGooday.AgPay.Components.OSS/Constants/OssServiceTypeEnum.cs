using System.ComponentModel;

namespace AGooday.AgPay.Components.OSS.Constants
{
    /// <summary>
    /// oss 服务枚举值
    /// </summary>
    public enum OssServiceTypeEnum
    {
        /// <summary>
        /// 本地存储
        /// </summary>
        [Description("local")]
        LOCAL,
        /// <summary>
        /// 阿里云oss
        /// </summary>
        [Description("aliyun-oss")]
        ALIYUN_OSS
    }
}
