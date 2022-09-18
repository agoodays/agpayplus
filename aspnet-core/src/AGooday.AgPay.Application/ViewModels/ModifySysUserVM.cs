using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.ViewModels
{
    public class ModifySysUserVM : SysUserVM
    {
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
