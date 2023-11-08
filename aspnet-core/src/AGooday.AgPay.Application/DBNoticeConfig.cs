namespace AGooday.AgPay.Application
{
    public class DBNoticeConfig
    {
        public NoticeConfig Notice { get; set; }

        public class NoticeConfig
        {
            public int IntervalSeconds { get; set; } = 10;
            public MailConfig Mail { get; set; }

            public class MailConfig
            {
                public string Host { get; set; }
                public int Port { get; set; }
                public string FromName { get; set; }
                public string FromAddress { get; set; }
                public string Password { get; set; }
                public List<string> ToAddress { get; set; }
            }
        }
    }
}
