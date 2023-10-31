using AGooday.AgPay.Notice.Core;

namespace AGooday.AgPay.Notice.Email
{
    public class EmailSendRequest : NoticeSendRequest
    {
        public List<string> ToAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public byte[] Attachments { get; set; }
        public string FileName { get; set; } = "未命名文件.txt";
    }
}
