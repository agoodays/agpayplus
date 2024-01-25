using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Common.Exceptions
{
    public class BizException : Exception
    {
        public ApiRes ApiRes { get; private set; }

        /// <summary>
        /// 业务自定义异常
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        public BizException(string errorMessage)
            : base(errorMessage)
        {
            this.ApiRes = ApiRes.CustomFail(errorMessage);
        }

        public BizException(ApiCode apiCodeEnum, params string[] args)
            : base()
        {
            this.ApiRes = ApiRes.Fail(apiCodeEnum, args);
        }

        public BizException(ApiRes apiRes)
            : base(apiRes.Msg)
        {
            this.ApiRes = apiRes;
        }
    }
}
