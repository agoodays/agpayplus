using AGooday.AgPay.Domain.Core.Commands;
using AGooday.AgPay.Domain.Validations.MchInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
