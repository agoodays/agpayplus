using AGooday.AgPay.Notice.Core;

namespace AGooday.AgPay.Notice.Email
{
    public interface IEmailProvider : INotice
    {
        public void SetToAddress(List<string> toAddress);
    }
}
