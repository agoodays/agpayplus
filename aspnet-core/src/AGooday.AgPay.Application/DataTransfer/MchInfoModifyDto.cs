using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 商户信息表
    /// </summary>
    public class MchInfoModifyDto : MchInfoCreateDto
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }

        /// <summary>
        /// 系统用户ID
        /// </summary>
        public long SysUserId { get; set; }

        /// <summary>
        /// 默认密码
        /// </summary>
        public string DefaultPass { get; set; }

        /// <summary>
        /// 重置密码
        /// </summary>
        public bool ResetPass { get; set; }

        /// <summary>
        /// 确认密码
        /// </summary>
        public bool ConfirmPwd { get; set; }

        /// <summary>
        /// 当前系统用户ID
        /// </summary>
        public long CurrentSysUserId { get; set; }
    }
}
