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
                            option.FromName = "System";
                            option.FromAddress = "12345@qq.com";
                            option.Password = "passaword";
                            option.ToAddress = new List<string>()
                            {
                                "12345@qq.com"
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
            var response = await _emailProvider.SendAsync("ÓÊ¼þ±êÌâ", new Exception("custom exception"));
            Assert.True(response.IsSuccess);
        }
    }
}