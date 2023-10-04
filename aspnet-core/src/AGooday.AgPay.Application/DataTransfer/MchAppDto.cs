using AGooday.AgPay.Common.Models;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 商户应用表
    /// </summary>
    public class MchAppDto : BaseModel
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }

        /// <summary>
        /// 应用状态: 0-停用, 1-正常
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// 是否默认: 0-否, 1-是
        /// </summary>
        public byte DefaultFlag { get; set; }

        /// <summary>
        /// 支持的签名方式 ["MD5", "RSA2"]
        /// </summary>
        public JArray AppSignType { get; set; }

        /// <summary>
        /// 应用私钥
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// RSA2应用公钥
        /// </summary>
        public string AppRsa2PublicKey { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 创建者用户ID
        /// </summary>
        public long CreatedUid { get; set; }

        /// <summary>
        /// 创建者姓名
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
