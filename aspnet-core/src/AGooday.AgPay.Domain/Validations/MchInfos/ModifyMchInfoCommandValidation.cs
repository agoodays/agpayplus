using AGooday.AgPay.Domain.Commands.MchInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Validations.MchInfos
{
    public class ModifyMchInfoCommandValidation : MchInfoValidation<ModifyMchInfoCommand>
    {
        public ModifyMchInfoCommandValidation()
        {
            ValidateMchName();
            ValidateMchShortName();
            ValidateLoginUsername();
            ValidateContactName();
            ValidateContactTel();
        }
    }
}
