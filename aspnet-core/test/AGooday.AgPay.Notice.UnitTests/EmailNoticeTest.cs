using AGooday.AgPay.Notice.Email;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AGooday.AgPay.Notice.UnitTests
{
    public class EmailNoticeTest
    {
        private readonly IEmailProvider _emailProvider;

        public EmailNoticeTest()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddNotice(config =>
                    {
                        config.IntervalSeconds = 10;
                        config.UseEmail(option =>
                        {
                            option.Host = "smtp.qq.com";
                            option.Port = 465;
                            option.FromName = "jones.xiang@foxmail.com";
                            option.FromAddress = "jones.xiang@foxmail.com";
                            option.Password = "ygslrbotmxsdecdh";
                            option.ToAddress = new List<string>()
                            {
                                "1697504283@qq.com"
                            };
                        });
                    });
                })
                .Build();

            _emailProvider = host.Services.GetRequiredService<IEmailProvider>();
        }

        [Fact]
        public async Task EmailSendShouldBeSucceed()
        {
            var response = await _emailProvider.SendAsync("邮件标题", new Exception("custom exception"));
            Assert.True(response.IsSuccess);
            var body = $"尊敬的商户，<br/><br/>恭喜您成功注册为我们的商户！我们非常高兴地欢迎您加入我们的平台。<br/><br/>以下是您的注册信息：<br/>商户号：M0000000000<br/>商户名称：测试注册通知<br/>账户名称：ceshi<br/>联系人电话：18888888888<br/>账户密码：888888<br/>注册时间：{DateTime.Now}<br/><br/>您现在可以使用您的商户账号登录到我们的平台，并开始享受以下服务：<br/>- 管理您的商户资料和信息<br/>- 查看交易统计数据<br/>- 收银、转账查看交易记录<br/>- 管理员工信息<br/><br/>如果您有任何问题、疑问或需要帮助，请随时与我们的客户服务团队联系。我们将竭诚为您提供支持和协助。<br/><br/>再次感谢您选择成为我们的商户！期待与您建立长期的合作关系。<br/><br/>祝生意兴隆！<br/><br/>最诚挚的问候，<br/>[吉日科技]";
            // 发送开通提醒
            var request = new EmailSendRequest()
            {
                ToAddress = new List<string>() {
                    "1697504283@qq.com"
                },
                Subject = "商户注册成功通知",
                Body = body,
            };
            response = await _emailProvider.SendAsync(request);
            Assert.True(response.IsSuccess);

            _emailProvider.SetToAddress(new List<string>() { "1697504283@qq.com" });
            response = await _emailProvider.SendAsync("商户注册成功通知", body);

            Assert.True(response.IsSuccess);
        }
    }
}