using AGooday.AgPay.Domain.Validations.MchInfos;
using AGooday.AgPay.Domain.Validations.SysUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Commands.MchInfos
{
    public class ModifyMchInfoCommand : MchInfoCommand
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }

        /// <summary>
        /// 默认密码
        /// </summary>
        public bool DefaultPass { get; set; }

        /// <summary>
        /// 重置密码
        /// </summary>
        public bool ResetPass { get; set; }

        /// <summary>
        /// 确认密码
        /// </summary>
        public string ConfirmPwd { get; set; }

        /// <summary>
        /// 当前系统用户ID
        /// </summary>
        public long CurrentSysUserId { get; set; }

        // 重写基类中的 是否有效 方法
        // 主要是为了引入命令验证 CreateMchInfoCommandValidation。
        public override bool IsValid()
        {
            ValidationResult = new ModifyMchInfoCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
