using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.ViewModels
{
    /// <summary>
    /// 支付接口配置参数表
    /// </summary>
    public class PayInterfaceConfigVM
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 账号类型:1-服务商 2-商户
        /// </summary>
        public byte InfoType { get; set; }

        /// <summary>
        /// 服务商或商户No
        /// </summary>
        public string InfoId { get; set; }

        /// <summary>
        /// 支付接口
        /// </summary>
        public string IfCode { get; set; }

        /// <summary>
        /// 接口配置参数,json字符串
        /// </summary>
        public string IfParams { get; set; }

        /// <summary>
        /// 支付接口费率
        /// </summary>
        public decimal IfRate { get; set; }

        /// <summary>
        /// 状态: 0-停用, 1-启用
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 创建者用户ID
        /// </summary>
        public long? CreatedUid { get; set; }

        /// <summary>
        /// 创建者姓名
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新者用户ID
        /// </summary>
        public long? UpdatedUid { get; set; }

        /// <summary>
        /// 更新者姓名
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
