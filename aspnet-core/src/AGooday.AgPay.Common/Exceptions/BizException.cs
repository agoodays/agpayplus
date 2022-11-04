using AGooday.AgPay.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Common.Exceptions
{
    public class BizException : Exception
    {
        private ApiRes ApiRes;

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
