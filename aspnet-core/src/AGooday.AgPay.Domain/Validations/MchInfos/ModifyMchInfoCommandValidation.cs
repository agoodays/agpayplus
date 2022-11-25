using AGooday.AgPay.Domain.Commands.MchInfos;

namespace AGooday.AgPay.Domain.Validations.MchInfos
{
    public class ModifyMchInfoCommandValidation : MchInfoValidation<ModifyMchInfoCommand>
    {
        public ModifyMchInfoCommandValidation()
        {
            ValidateMchName();
            ValidateMchShortName();
            ValidateContactName();
            ValidateContactTel();
        }
    }
}
