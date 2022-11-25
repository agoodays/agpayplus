using AGooday.AgPay.Domain.Commands.SysUsers;

namespace AGooday.AgPay.Domain.Validations.SysUsers
{
    /// <summary>
    /// 添加 SysUser 命令模型验证
    /// 继承 SysUserValidation 基类
    /// </summary>
    public class ModifySysUserCommandValidation : SysUserValidation<ModifySysUserCommand>
    {
        public ModifySysUserCommandValidation()
        {
            ValidateLoginUsername();
            ValidateRealname();
            ValidatePhone();
            ValidateSex();
        }
    }
}
