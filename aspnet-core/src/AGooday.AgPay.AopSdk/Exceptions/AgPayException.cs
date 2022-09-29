using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.AopSdk.Exceptions
{
    /// <summary>
    /// AgPay异常抽象类
    /// </summary>
    public abstract class AgPayException : Exception
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        public AgPayException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}
