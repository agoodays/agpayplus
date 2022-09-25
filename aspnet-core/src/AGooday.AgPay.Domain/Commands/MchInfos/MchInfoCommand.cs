using AGooday.AgPay.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Commands.MchInfos
{
    public abstract class MchInfoCommand : Command
    {
        /// <summary>
        /// 商户名称
        /// </summary>
        public string MchName { get; set; }

        /// <summary>
        /// 商户简称
        /// </summary>
        public string MchShortName { get; set; }

        /// <summary>
        /// 类型: 1-普通商户, 2-特约商户(服务商模式)
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 服务商号
        /// </summary>
        public string IsvNo { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        /// 联系人手机号
        /// </summary>
        public string ContactTel { get; set; }

        /// <summary>
        /// 联系人邮箱
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        /// 商户状态: 0-停用, 1-正常
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// 商户备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginUsername { get; set; }
    }
}
