using AGooday.AgPay.Domain.Validations.SysUsers;

namespace AGooday.AgPay.Domain.Commands.SysUsers
{
    public class ModifySysUserCommand : SysUserCommand
    {
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
        // 主要是为了引入命令验证 RegisterUsersCommandValidation。
        public override bool IsValid()
        {
            ValidationResult = new ModifySysUserCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
