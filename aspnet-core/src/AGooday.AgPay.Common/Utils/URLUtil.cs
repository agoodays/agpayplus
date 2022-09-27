using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AGooday.AgPay.Common.Utils
{
    public class URLUtil
    {
        public static string EncodeAll(string url)
        {
            return HttpUtility.UrlEncode(url, Encoding.UTF8);
        }
    }
}
