using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
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

        public static string AppendUrlQuery(string url, JObject param)
        {
            if (string.IsNullOrWhiteSpace(url) || param == null)
            {
                return url;
            }

            StringBuilder sb = new StringBuilder(url);
            if (url.IndexOf("?") < 0)
            {
                sb.Append("?");
            }

            //是否包含query条件
            bool isHasCondition = url.IndexOf("=") >= 0;

            foreach (var item in param)
            {
                if (item.Value != null)
                {
                    if (isHasCondition)
                    {
                        sb.Append("&"); //包含了查询条件， 那么应当拼接&符号
                    }
                    else
                    {
                        isHasCondition = true; //变更为： 已存在query条件
                    }
                    sb.Append(item.Key).Append("=").Append(HttpUtility.UrlEncode(item.Value.ToString(), Encoding.UTF8));
                }
            }

            return sb.ToString();
        }
    }
}
