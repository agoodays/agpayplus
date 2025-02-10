namespace AGooday.AgPay.Payment.Api.Jobs
{
    public class QuartzJobSettings
    {
        public List<JobSetting> Jobs { get; set; }
    }

    public class JobSetting
    {
        public string JobType { get; set; }
        public string CronExpression { get; set; }
    }
}
