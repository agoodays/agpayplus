using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Common.Exceptions
{
    public class BizException : Exception
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        public BizException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}
