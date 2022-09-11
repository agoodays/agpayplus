using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.ViewModels
{
    /// <summary>
    /// 商户支付通道表
    /// </summary>
    public class MchPayPassageVM
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }

        /// <summary>
        /// 应用Id
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 支付接口
        /// </summary>
        public string IfCode { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string WayCode { get; set; }

        /// <summary>
        /// 支付方式费率
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 风控数据
        /// </summary>
        public string RiskConfig { get; set; }

        /// <summary>
        /// 状态: 0-停用, 1-启用
        /// </summary>
        public byte State { get; set; }

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
