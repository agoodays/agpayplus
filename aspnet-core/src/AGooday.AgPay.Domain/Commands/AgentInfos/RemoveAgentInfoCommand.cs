using AGooday.AgPay.Domain.Core.Commands;
using AGooday.AgPay.Domain.Validations.AgentInfos;

namespace AGooday.AgPay.Domain.Commands.AgentInfos
{
    public class RemoveAgentInfoCommand : Command
    {
        /// <summary>
        /// 代理商号
        /// </summary>
        public string AgentNo { get; set; }

        public override bool IsValid()
        {
            ValidationResult = new RemoveAgentInfoCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
