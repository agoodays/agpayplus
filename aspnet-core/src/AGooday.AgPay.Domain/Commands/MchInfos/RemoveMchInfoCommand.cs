using AGooday.AgPay.Domain.Core.Commands;
using AGooday.AgPay.Domain.Validations.MchInfos;

namespace AGooday.AgPay.Domain.Commands.MchInfos
{
    public class RemoveMchInfoCommand : Command
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }

        public override bool IsValid()
        {
            ValidationResult = new RemoveMchInfoCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
