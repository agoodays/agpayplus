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
                // 发送开通提醒
                if (RegUtil.IsMobile(notification.ContactTel))
                {

                }
                if (RegUtil.IsEmail(notification.ContactEmail))
                {
                    var subject = "商户注册成功通知";
                    var body = $@"尊敬的商户，<br/><br/>
                    恭喜您成功注册为我们的商户！我们非常高兴地欢迎您加入我们的平台。<br/><br/>
                    以下是您的注册信息：<br/>
                    商户号：{notification.MchNo}<br/>
                    商户名称：{notification.MchName}<br/>
                    联系人电话：{notification.ContactTel}<br/>
                    商户账号：{notification.LoginUsername}<br/>
                    账户密码：{notification.LoginPassword}<br/>
                    注册时间：{notification.CreatedAt:yyyy-MM-dd HH:mm:ss}<br/><br/>
                    您现在可以使用您的商户账号或手机号登录到我们的平台，并开始享受以下服务：<br/>
                    - 管理您的商户资料和信息<br/>
                    - 生成报告和统计数据<br/>
                    - 在线收银、支付测试、转账和配置分账<br/>
                    - 管理支付订单和交易记录<br/>
                    - 设置支付方式和费率<br/>
                    - 管理设备、员工信息和系统配置<br/><br/>
                    如果您有任何问题、疑问或需要帮助，请随时与我们的客户服务团队联系。我们将竭诚为您提供支持和协助。<br/><br/>
                    再次感谢您选择成为我们的商户！期待与您建立长期的合作关系。<br/><br/>
                    祝生意兴隆！<br/><br/>
                    最诚挚的问候，<br/>
                    [吉日科技]";

                    //var request = new EmailSendRequest()
                    //{
                    //    ToAddress = new List<string>() {
                    //        notification.ContactEmail
                    //    },
                    //    Subject = subject,
                    //    Body = body,
                    //};
                    //_mailProvider.SendAsync(request);

                    _mailProvider.SetToAddress(new List<string>() { notification.ContactEmail });
                    _mailProvider.SendAsync(subject, body);
                }
            }
            return Task.CompletedTask;
        }
    }
}
