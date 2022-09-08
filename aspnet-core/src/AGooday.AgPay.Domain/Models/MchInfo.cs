using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 商户信息表
    /// </summary>
    public class MchInfo
    {
        /**
         * 商户号
         */
        public string MchNo { get; set; }

        /**
         * 商户名称
         */
        public string MchName { get; set; }

        /**
         * 商户简称
         */
        public string MchShortName { get; set; }

        /**
         * 类型: 1-普通商户, 2-特约商户(服务商模式)
         */
        public byte Type { get; set; }

        /**
         * 服务商号
         */
        public string IsvNo { get; set; }

        /**
         * 联系人姓名
         */
        public string ContactName { get; set; }

        /**
         * 联系人手机号
         */
        public string ContactTel { get; set; }

        /**
         * 联系人邮箱
         */
        public string ContactEmail { get; set; }

        /**
         * 商户状态: 0-停用, 1-正常
         */
        public byte State { get; set; }

        /**
         * 商户备注
         */
        public string Remark { get; set; }

        /**
         * 初始用户ID（创建商户时，允许商户登录的用户）
         */
        public long InitUserId { get; set; }

        /**
         * 创建者用户ID
         */
        public long CreatedUid { get; set; }

        /**
         * 创建者姓名
         */
        public string CreatedBy { get; set; }

        /**
         * 创建时间
         */
        public DateTime CreatedAt { get; set; }

        /**
         * 更新时间
         */
        public DateTime UpdatedAt { get; set; }
    }
}
