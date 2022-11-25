using AGooday.AgPay.Domain.Commands.SysUsers;
using FluentValidation;

namespace AGooday.AgPay.Domain.Validations.SysUsers
{
    /// <summary>
    /// 添加 SysUser 命令模型验证
    /// 继承 SysUserValidation 基类
    /// </summary>
    public class RemoveSysUserCommandValidation : AbstractValidator<RemoveSysUserCommand>
    {
        public RemoveSysUserCommandValidation()
        {
            RuleFor(c => c.SysUserId)
                .NotEmpty().WithMessage("参数错误！")
                ;

            RuleFor(c => c.CurrentSysUserId)
                .NotEmpty().WithMessage("参数错误！")
                ;

            RuleFor(c => c.SysType)
                .NotEmpty().WithMessage("参数错误！")
                ;
        }
    }
}
