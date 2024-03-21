using AGooday.AgPay.Notice.Core;
using Microsoft.Extensions.Options;

namespace AGooday.AgPay.Notice.Sms
{
    internal class AliyunSmsProvider : ISmsProvider
    {
        private readonly SmsOptions _smsOptions;
        private readonly NoticeOptions _noticeOptions;

        public AliyunSmsProvider(IOptions<SmsOptions> smsOptions, IOptions<NoticeOptions> noticeOptions)
        {
            _smsOptions = smsOptions.Value;
            _noticeOptions = noticeOptions.Value;
        }

        public Task<NoticeSendResponse> SendAsync(NoticeSendRequest request)
        {
            return SendBaseAsync((SmsSendRequest)request);
        }

        /// <summary>
        /// 发送消息公共方法
        /// </summary>
        private async Task<NoticeSendResponse> SendBaseAsync(SmsSendRequest input)
        {
            var response = new NoticeSendResponse();
            try
            {
                await IntervalHelper.IntervalExcuteAsync(async () =>
                {
                    await Task.Delay(1000);
                }, input.PhoneNumbers, _smsOptions.IntervalSeconds ?? _noticeOptions.IntervalSeconds ?? 10);
            }
            catch (Exception ex)
            {
                response.ErrMsg = $"短信发送异常:{ex.Message}";
            }
            return response;
        }
    }
}
