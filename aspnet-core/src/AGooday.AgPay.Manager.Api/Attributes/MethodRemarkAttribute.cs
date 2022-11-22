namespace AGooday.AgPay.Manager.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class MethodRemarkAttribute : Attribute
    {
        public string Remark { get; private set; }

        public MethodRemarkAttribute(string remark)
        {
            Remark = remark;
        }
    }
}
