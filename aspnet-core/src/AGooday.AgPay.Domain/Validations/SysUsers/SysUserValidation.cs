using AGooday.AgPay.Domain.Commands.SysUsers;
using FluentValidation;

namespace AGooday.AgPay.Domain.Validations.SysUsers
{
    /// <summary>
    /// 定义基于 SysUserCommand 的抽象基类 SysUserValidation
    /// 继承 抽象类 AbstractValidator
    /// 注意需要引用 FluentValidation
    /// 注意这里的 T 是命令模型
    /// </summary>
    /// <typeparam name="T">泛型类</typeparam>
    public abstract class SysUserValidation<T> : AbstractValidator<T> where T : SysUserCommand
    {
        /// <summary>
        /// 验证登录用户名
        /// </summary>
        protected void ValidateLoginUsername()
        {
            //定义规则，c 就是当前 SysUserValidation 类
            RuleFor(c => c.LoginUsername)
                .NotEmpty().WithMessage("登录用户名不能为空！")//判断不能为空，如果为空则显示Message
                .Length(6, 32).WithMessage("登录用户名在6~32个字符之间")//定义字段的长度
                ;
        }

        /// <summary>
        /// 验证真实姓名
        /// </summary>
        protected void ValidateRealname()
        {
            RuleFor(c => c.Realname)
                .NotEmpty().WithMessage("真实姓名不能为空！")
                ;
        }

        /// <summary>
        /// 验证手机号
        /// </summary>
        protected void ValidatePhone()
        {
            RuleFor(c => c.Telphone)
                .NotEmpty()
                .Must(HavePhone)
                .WithMessage("手机号应该为11位！")
                ;
        }

        /// <summary>
        /// 验证性别
        /// </summary>
        protected void ValidateSex()
        {
            RuleFor(c => c.Sex)
                .NotEmpty().WithMessage("性别不能为空！")
                ;
        }

        // 表达式
        private static bool HavePhone(string phone)
        {
            return phone?.Length == 11;
        }
    }
}
