namespace AGooday.AgPay.Merchant.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MethodLogAttribute : Attribute
    {
        public string Remark { get; private set; }
        public LogType Type { get; set; } = LogType.Operate;

        public MethodLogAttribute(string remark)
        {
            Remark = remark;
        }
    }

    public enum LogType
    {
        Login,
        Operate
    }
}
