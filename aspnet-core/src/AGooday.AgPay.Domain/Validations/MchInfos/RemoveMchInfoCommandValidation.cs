using AGooday.AgPay.Domain.Commands.MchInfos;
using FluentValidation;

namespace AGooday.AgPay.Domain.Validations.MchInfos
{
    /// <summary>
    /// 添加 MchInfo 命令模型验证
    /// 继承 MchInfoValidation 基类
    /// </summary>
    public class RemoveMchInfoCommandValidation : AbstractValidator<RemoveMchInfoCommand>
    {
        public RemoveMchInfoCommandValidation()
        {
            RuleFor(c => c.MchNo)
                .NotEmpty().WithMessage("参数错误！")
                ;
        }
    }
}
