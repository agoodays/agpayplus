using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Events.SysUsers;
using AGooday.AgPay.Notice.Email;
using MediatR;

namespace AGooday.AgPay.Domain.EventHandlers
{
    public class SysUserEventHandler :
        INotificationHandler<SysUserCreatedEvent>
    {
        private readonly IEmailProvider _mailProvider;

        public SysUserEventHandler(IEmailProvider mailProvider)
        {
            _mailProvider = mailProvider;
        }

        public Task Handle(SysUserCreatedEvent notification, CancellationToken cancellationToken)
        {
            if (notification.IsNotify.Equals(CS.YES))
            {
                // 发送开通提醒
                if (RegUtil.IsMobile(notification.Telphone))
                {

                }
                if (RegUtil.IsEmail(notification.Email))
                {
                    var subject = "注册成功通知";
                    var body = $@"尊敬的用户，<br/><br/>
                    恭喜您成功注册为我们的用户！欢迎您加入我们的大家庭。<br/><br/>
                    以下是您的注册信息：<br/>
                    用户姓名：{notification.Realname}<br/>
                    联系人电话：{notification.Telphone}<br/>
                    用户账号：{notification.LoginUsername}<br/>
                    账户密码：{notification.LoginPassword}<br/>
                    注册时间：{notification.CreatedAt:yyyy-MM-dd HH:mm:ss}<br/><br/>
                    您现在可以使用您的代理商账号或手机号登录系统。";
                    _mailProvider.SetToAddress(new List<string>() { notification.Email });
                    _mailProvider.SendAsync(subject, body);
                }
            }
            return Task.CompletedTask;
        }
    }
}
