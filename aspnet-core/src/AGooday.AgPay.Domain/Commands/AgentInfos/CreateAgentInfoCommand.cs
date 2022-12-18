using AGooday.AgPay.Domain.Validations.AgentInfos;

namespace AGooday.AgPay.Domain.Commands.AgentInfos
{
    public class CreateAgentInfoCommand : AgentInfoCommand
    {
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginUsername { get; set; }

        /// <summary>
        /// 密码类型
        /// </summary>
        public string PasswordType { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string LoginPassword { get; set; }

        /// <summary>
        /// 是否发送开通提醒
        /// </summary>
        public byte IsNotify { get; set; }

        // 重写基类中的 是否有效 方法
        // 主要是为了引入命令验证 CreateAgentInfoCommandValidation。
        public override bool IsValid()
        {
            ValidationResult = new CreateAgentInfoCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
