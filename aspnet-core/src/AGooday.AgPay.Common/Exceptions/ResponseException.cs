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
        public static ResponseException BuildText(string text)
        {
            var entity = new ContentResult
            {
                StatusCode = 200,
                Content = text,
                ContentType = "text/html"
            }; ;
            return new ResponseException(entity);
        }
    }
}
