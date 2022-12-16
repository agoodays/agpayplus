using AGooday.AgPay.Domain.Commands.AgentInfos;
using FluentValidation;

namespace AGooday.AgPay.Domain.Validations.AgentInfos
{
    public abstract class AgentInfoValidation<T> : AbstractValidator<T> where T : AgentInfoCommand
    {
        /// <summary>
        /// 验证代理商名称
        /// </summary>
        protected void ValidateMchName()
        {
            //定义规则，c 就是当前 AgentInfoValidation 类
            RuleFor(c => c.AgentName)
                .NotEmpty().WithMessage("代理商名称不能为空！")//判断不能为空，如果为空则显示Message
                ;
        }
        /// <summary>
        /// 验证代理商简称
        /// </summary>
        protected void ValidateMchShortName()
        {
            //定义规则，c 就是当前 AgentInfoValidation 类
            RuleFor(c => c.AgentShortName)
                .NotEmpty().WithMessage("代理商名称不能为空！")//判断不能为空，如果为空则显示Message
                ;
        }

        /// <summary>
        /// 验证联系人姓名
        /// </summary>
        protected void ValidateContactName()
        {
            RuleFor(c => c.ContactName)
                .NotEmpty().WithMessage("联系人姓名不能为空！")
                ;
        }

        /// <summary>
        /// 验证联系人手机号
        /// </summary>
        protected void ValidateContactTel()
        {
            RuleFor(c => c.ContactTel)
                .NotEmpty()
                .Must(HavePhone)
                .WithMessage("联系人手机号应该为11位！")
                ;
        }

        // 表达式
        private static bool HavePhone(string phone)
        {
            return phone?.Length == 11;
        }
    }
}
