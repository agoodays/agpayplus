using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.AopSdk.Exceptions
{
    /// <summary>
    /// API异常
    /// </summary>
    public class APIException : AgPayException
    {
        public APIException(string errorMessage) 
            : base(errorMessage)
        {
        }
    }
}
