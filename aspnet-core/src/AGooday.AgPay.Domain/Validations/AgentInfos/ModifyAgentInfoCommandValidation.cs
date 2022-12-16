using AGooday.AgPay.Domain.Commands.AgentInfos;

namespace AGooday.AgPay.Domain.Validations.AgentInfos
{
    public class ModifyAgentInfoCommandValidation : AgentInfoValidation<ModifyAgentInfoCommand>
    {
        public ModifyAgentInfoCommandValidation()
        {
            ValidateMchName();
            ValidateMchShortName();
            ValidateContactName();
            ValidateContactTel();
        }
    }
}
