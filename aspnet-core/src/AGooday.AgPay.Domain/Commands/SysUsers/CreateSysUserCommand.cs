using AGooday.AgPay.Domain.Validations.SysUsers;

namespace AGooday.AgPay.Domain.Commands.SysUsers
{
    public class CreateSysUserCommand : SysUserCommand
    {
        // 重写基类中的 是否有效 方法
        // 主要是为了引入命令验证 RegisterUsersCommandValidation。
        public override bool IsValid()
        {
            ValidationResult = new CreateSysUserCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
