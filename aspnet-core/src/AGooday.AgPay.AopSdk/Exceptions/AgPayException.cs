namespace AGooday.AgPay.AopSdk.Exceptions
{
    /// <summary>
    /// AgPay异常抽象类
    /// </summary>
    public abstract class AgPayException : Exception
    {
        public int StatusCode { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        public AgPayException(string errorMessage)
            : base(errorMessage)
        {
        }

        public AgPayException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public AgPayException(int statusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            this.StatusCode = statusCode;
        }
    }
}
