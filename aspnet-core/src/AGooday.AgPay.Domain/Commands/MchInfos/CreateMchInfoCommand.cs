using AGooday.AgPay.Domain.Validations.MchInfos;

namespace AGooday.AgPay.Domain.Commands.MchInfos
{
    public class CreateMchInfoCommand : MchInfoCommand
    {
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginUsername { get; set; }

        // 重写基类中的 是否有效 方法
        // 主要是为了引入命令验证 CreateMchInfoCommandValidation。
        public override bool IsValid()
        {
            ValidationResult = new CreateMchInfoCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
