using AGooday.AgPay.Notice.Core;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;

namespace AGooday.AgPay.Notice.Email
{
    internal class EmailProvider : IEmailProvider
    {
        private List<string> ToAddress;

        private readonly EmailOptions _emailOptions;
        private readonly NoticeOptions _noticeOptions;

        public EmailProvider(IOptions<EmailOptions> emailOptions, IOptions<NoticeOptions> noticeOptions)
        {
            _emailOptions = emailOptions.Value;
            _noticeOptions = noticeOptions.Value;
        }

        public void SetToAddress(List<string> toAddress)
        {
            ToAddress = toAddress;
        }

        public Task<NoticeSendResponse> SendAsync(NoticeSendRequest request)
        {
            var rq = (EmailSendRequest)request;
            rq.ToAddress ??= ToAddress;
            return SendBaseAsync((EmailSendRequest)request);
        }

        /// <summary>
        /// 发送异常消息
        /// </summary>
        public Task<NoticeSendResponse> SendAsync(string title, Exception exception)
        {
            var request = new EmailSendRequest
            {
                ToAddress = ToAddress,
                Subject = title,
                Body = $"{exception.Message}{Environment.NewLine}{exception}"
            };

            return SendBaseAsync(request);
        }

        /// <summary>
        /// 发送普通消息
        /// </summary>
        public Task<NoticeSendResponse> SendAsync(string title, string message)
        {
            var request = new EmailSendRequest
            {
                ToAddress = ToAddress,
                Subject = title,
                Body = message
            };

            return SendBaseAsync(request);
        }

        /// <summary>
        /// 发送消息公共方法
        /// </summary>
        private async Task<NoticeSendResponse> SendBaseAsync(EmailSendRequest input)
        {
            var response = new NoticeSendResponse();
            try
            {
                await IntervalHelper.IntervalExcuteAsync(async () =>
                {
                    var message = EmailHelper.CreateMimeMessage(input, _emailOptions);
                    using (SmtpClient client = new SmtpClient())
                    {
                        client.CheckCertificateRevocation = false;
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.Timeout = 10 * 1000;
                        await client.ConnectAsync(_emailOptions.Host, _emailOptions.Port, SecureSocketOptions.Auto);
                        await client.AuthenticateAsync(_emailOptions.FromAddress, _emailOptions.Password);
                        await client.SendAsync(message);
                    }
                }, input.Subject, _emailOptions.IntervalSeconds ?? _noticeOptions.IntervalSeconds ?? 10);

            }
            catch (Exception e)
            {
                response.ErrMsg = $"邮件发送异常:{e.Message}";
            }
            return response;
        }
    }
}
