namespace AGooday.AgPay.AopSdk.Exceptions
{
    /// <summary>
    /// 无效请求异常
    /// </summary>
    public class InvalidRequestException : AgPayException
    {
        public InvalidRequestException(string errorMessage)
            : base(errorMessage)
        {
        }
        public InvalidRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InvalidRequestException(int statusCode, string message, Exception innerException)
            : base(statusCode, message, innerException)
        {
        }
    }
}
