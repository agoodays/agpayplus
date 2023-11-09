using AGooday.AgPay.Notice.Core;

namespace AGooday.AgPay.Notice.Sms
{
    public class SmsSendRequest : NoticeSendRequest
    {
        public string PhoneNumbers { get; set; }
    }
}
