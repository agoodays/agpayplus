using AGooday.AgPay.Domain.Commands.MchInfos;
using FluentValidation;

namespace AGooday.AgPay.Domain.Validations.MchInfos
{
    public class CreateMchInfoCommandValidation : MchInfoValidation<CreateMchInfoCommand>
    {
        public CreateMchInfoCommandValidation()
        {
            ValidateMchName();
            ValidateMchShortName();
            ValidateLoginUsername();
            ValidateContactName();
            ValidateContactTel();
        }

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
    }
}
