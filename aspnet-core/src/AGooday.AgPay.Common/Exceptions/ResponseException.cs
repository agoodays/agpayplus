using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Common.Exceptions
{
    public class ResponseException : Exception
    {
        public ActionResult ResponseEntity { get; private set; }
        /// <summary>
        /// 业务自定义异常
        /// </summary>
        public ResponseException(ActionResult resp)
            : base()
        {
            ResponseEntity = resp;
        }

        /// <summary>
        /// 生成文本类型的响应
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static ResponseException BuildText(string text, int statusCode = (int)HttpStatusCode.OK)
        {
            var entity = new ContentResult
            {
                StatusCode = statusCode,
                Content = text,
                ContentType = MediaTypeNames.Text.Html
            }; ;
            return new ResponseException(entity);
        }
    }
}
