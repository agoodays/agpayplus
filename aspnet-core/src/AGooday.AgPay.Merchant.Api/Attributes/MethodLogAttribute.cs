namespace AGooday.AgPay.Merchant.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MethodLogAttribute : Attribute
    {
        public string Remark { get; private set; }

        public MethodLogAttribute(string remark)
        {
            Remark = remark;
        }
    }
}
