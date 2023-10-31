using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Events.MchInfos;
using AGooday.AgPay.Notice.Email;
using MediatR;

namespace AGooday.AgPay.Domain.EventHandlers
{
    public class MchInfoEventHandler :
        INotificationHandler<MchInfoCreatedEvent>
    {
        private readonly IEmailProvider _mailProvider;

        public MchInfoEventHandler(IEmailProvider mailProvider)
        {
            _mailProvider = mailProvider;
        }

        public Task Handle(MchInfoCreatedEvent notification, CancellationToken cancellationToken)
        {
            if (notification.IsNotify.Equals(CS.YES))
            {
                if (RegUtil.IsEmail(notification.ContactEmail))
                {
                    var body = $"尊敬的商户，\r\n\r\n恭喜您成功注册为我们的商户！我们非常高兴地欢迎您加入我们的平台。\r\n\r\n以下是您的注册信息：\r\n商户号：{notification.MchNo}\r\n商户名称：{notification.MchName}\r\n账户名称：{notification.LoginUsername}\r\n联系人电话：{notification.ContactTel}\r\n账户密码：{notification.LoginPassword}\r\n注册时间：{notification.CreatedAt}\r\n\r\n您现在可以使用您的商户账号登录到我们的平台，并开始享受以下服务：\r\n- 管理您的商户资料和信息\r\n- 查看交易统计数据\r\n- 收银、转账查看交易记录\r\n- 管理员工信息\r\n\r\n如果您有任何问题、疑问或需要帮助，请随时与我们的客户服务团队联系。我们将竭诚为您提供支持和协助。\r\n\r\n再次感谢您选择成为我们的商户！期待与您建立长期的合作关系。\r\n\r\n祝生意兴隆！\r\n\r\n最诚挚的问候，\r\n[吉日科技]";
                    // 发送开通提醒
                    var request = new EmailSendRequest()
                    {
                        ToAddress = new List<string>() {
                        notification.ContactEmail
                    },
                        Subject = "商户注册成功通知",
                        Body = body,
                    };
                    _mailProvider.SendAsync(request);

                    _mailProvider.SetToAddress(new List<string>() { notification.ContactEmail });
                    _mailProvider.SendAsync("商户注册成功通知", body);
                }
            }
            return Task.CompletedTask;
        }
    }
}
