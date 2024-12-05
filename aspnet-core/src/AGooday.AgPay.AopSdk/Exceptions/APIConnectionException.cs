namespace AGooday.AgPay.AopSdk.Exceptions
{
    /// <summary>
    /// API连接异常
    /// </summary>
    public class APIConnectionException : AgPayException
    {
        public APIConnectionException(string errorMessage)
            : base(errorMessage)
        {
        }

        public APIConnectionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
