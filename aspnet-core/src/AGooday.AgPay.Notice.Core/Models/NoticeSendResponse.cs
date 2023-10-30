namespace AGooday.AgPay.Notice.Core
{
    public class NoticeSendResponse
    {
        public string ErrMsg { get; set; }

        public int ErrCode { get; set; }

        public bool IsSuccess => string.IsNullOrEmpty(ErrMsg);
    }
}
