using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Components.SMS.Models;
using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.SDK.Dysmsapi20170525;
using AlibabaCloud.SDK.Dysmsapi20170525.Models;
using AlibabaCloud.TeaUtil.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AGooday.AgPay.Components.SMS.Services
{
    public class AliyundySmsService : ISmsService
    {
        private readonly ILogger<AliyundySmsService> logger;
        private readonly AliyundySmsConfig smsConfig;
        private readonly Client client;

        public AliyundySmsService(ILogger<AliyundySmsService> logger, ISysConfigService sysConfigService)
        {
            this.logger = logger;
            var dbSmsConfig = sysConfigService.GetDBSmsConfig();
            smsConfig = (AliyundySmsConfig)smsConfig.GetSmsConfig(dbSmsConfig.SmsProviderKey, dbSmsConfig.AliyundySmsConfig);
            Config config = new()
            {
                AccessKeyId = smsConfig.AccessKeyId,
                AccessKeySecret = smsConfig.AccessKeySecret,
                Endpoint = "dysmsapi.aliyuncs.com",// Endpoint 请参考 https://api.aliyun.com/product/Dysmsapi
            };
            client = new Client(config);
        }

        public void SendVercode(SmsBizVercodeModel smsBizVercodeModel)
        {
            string templateCode = string.Empty;
            switch (smsBizVercodeModel.SmsType)
            {
                case CS.SMS_TYPE.AUTH:
                    templateCode = smsConfig.LoginMchTemplateId; break;
                case CS.SMS_TYPE.REGISTER:
                    templateCode = smsConfig.RegisterMchTemplateId; break;
                case CS.SMS_TYPE.RETRIEVE:
                    templateCode = smsConfig.ForgetPwdTemplateId; break;
                default:
                    break;
            }
            SendSmsRequest request = new()
            {
                PhoneNumbers = smsBizVercodeModel.Mobile,
                SignName = smsConfig.SignName,
                TemplateCode = templateCode,
                TemplateParam = JsonConvert.SerializeObject(new
                {
                    code = smsBizVercodeModel.Vercode
                }),
            };
            // 发送短信
            try
            {
                var runtime = new RuntimeOptions();
                SendSmsResponse response = client.SendSmsWithOptions(request, runtime);
                // 处理发送短信的结果
                if (response.Body.Code == "OK")
                {
                    // 短信发送成功
                }
                else
                {
                    // 短信发送失败
                    logger.LogInformation($"短信发送失败，请求报文：{JsonConvert.SerializeObject(request)}，响应报文：{JsonConvert.SerializeObject(response)}");
                }
            }
            catch (Exception ex)
            {
                // 处理异常
                logger.LogError(ex, $"短信发送异常，请求报文：{JsonConvert.SerializeObject(request)}");
                throw;
            }
        }

        public void SendDiyContent(SmsBizDiyContentModel smsBizDiyContentModel)
        {
            SendSmsRequest request = new()
            {
                PhoneNumbers = smsBizDiyContentModel.Mobile,
                SignName = smsConfig.SignName,
                TemplateCode = "SMS_136700041",
                TemplateParam = JsonConvert.SerializeObject(new
                {
                    content = smsBizDiyContentModel.Content
                }),
            };
            // 发送短信
            try
            {
                var runtime = new RuntimeOptions();
                SendSmsResponse response = client.SendSmsWithOptions(request, runtime);
                // 处理发送短信的结果
                if (response.Body.Code == "OK")
                {
                    // 短信发送成功
                }
                else
                {
                    // 短信发送失败
                    logger.LogInformation($"短信发送失败，请求报文：{JsonConvert.SerializeObject(request)}，响应报文：{JsonConvert.SerializeObject(response)}");
                }
            }
            catch (Exception ex)
            {
                // 处理异常
                logger.LogError(ex, $"短信发送异常，请求报文：{JsonConvert.SerializeObject(request)}");
                throw;
            }
        }

        public string QuerySmsInfo(string bizQueryType)
        {
            throw new NotImplementedException();
        }
    }
}
