using AGooday.AgPay.Domain.Commands.AgentInfos;
using FluentValidation;

namespace AGooday.AgPay.Domain.Validations.AgentInfos
{
    /// <summary>
    /// 添加 AgentInfo 命令模型验证
    /// 继承 AgentInfoValidation 基类
    /// </summary>
    public class RemoveAgentInfoCommandValidation : AbstractValidator<RemoveAgentInfoCommand>
    {
        public RemoveAgentInfoCommandValidation()
        {
            RuleFor(c => c.AgentNo)
                .NotEmpty().WithMessage("参数错误！")
                ;
        }
    }
}
