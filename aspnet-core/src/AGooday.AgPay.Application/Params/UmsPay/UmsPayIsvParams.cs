using AGooday.AgPay.Common.Extensions;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params.UmsPay
{
    /// <summary>
    /// 随行付 配置信息
    /// </summary>
    public class UmsPayIsvParams : IsvParams
    {
        /// <summary>
        /// 是否沙箱环境
        /// </summary>
        public byte? Sandbox { get; set; }

        /// <summary>
        /// 产品ID，由银联商务方提供
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 产品密钥，由银联商务方提供
        /// </summary>
        public string AppKey { get; set; }

        public override string DeSenData()
        {
            if (!string.IsNullOrWhiteSpace(AppId))
            {
                AppId = AppId.Mask();
            }
            if (!string.IsNullOrWhiteSpace(AppKey))
            {
                AppKey = AppKey.Mask();
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}
