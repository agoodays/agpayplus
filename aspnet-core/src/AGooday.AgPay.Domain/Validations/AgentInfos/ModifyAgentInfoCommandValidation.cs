using AGooday.AgPay.Domain.Commands.AgentInfos;

namespace AGooday.AgPay.Domain.Validations.AgentInfos
{
    public class ModifyAgentInfoCommandValidation : AgentInfoValidation<ModifyAgentInfoCommand>
    {
        public ModifyAgentInfoCommandValidation()
        {
            ValidateAgentName();
            ValidateAgentShortName();
            ValidateContactName();
            ValidateContactTel();
            ValidateIsvNo();
        }
    }
}
