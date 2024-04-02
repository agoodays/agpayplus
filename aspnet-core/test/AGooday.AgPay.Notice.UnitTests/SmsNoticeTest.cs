using AGooday.AgPay.Notice.Core;
using AGooday.AgPay.Notice.Sms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AGooday.AgPay.Notice.UnitTests
{
    public class SmsNoticeTest
    {
        private readonly ISmsProvider _SmsProvider;

        public SmsNoticeTest()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddNotice(config =>
                    {
                        config.IntervalSeconds = 10;
                        config.UseSms(option =>
                        {
                            option.IntervalSeconds = 0;
                            option.SmsUseType = "aliyunSms";
                            option.AliyunSms = new AliyunSmsOptions()
                            {
                                Endpoint = "dysmsapi.aliyuncs.com",
                                AccessKeyId = "T5w****gI0****qch",
                                AccessKeySecret = "u17oH**********aoApV**********",
                                AccountOpenTemplateId = "SMS_135330112"
                            };
                        });
                    });
                })
                .Build();

            _SmsProvider = host.Services.GetRequiredService<ISmsProvider>();
        }

        [Fact]
        public async Task SmsSendShouldBeSucceed()
        {
            var response = await _SmsProvider.SendAsync(new NoticeSendRequest());
            Assert.True(response.IsSuccess);
        }
    }
}