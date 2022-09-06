using Newtonsoft.Json;
using System.Text.Json.Nodes;

namespace AGooday.AgPay.Payment.Api.Models
{
    public class ResultBase
    {
        /// <summary>
        /// 业务响应码
        /// </summary>
        private int code { get; set; }

        /// <summary>
        /// 业务响应信息
        /// </summary>
        private string msg { get; set; }

        /// <summary>
        /// 数据对象
        /// </summary>
        private object data { get; set; }

        /// <summary>
        /// 签名值
        /// </summary>
        private string sign { get; set; }

        /// <summary>
        /// 输出json格式字符串
        /// </summary>
        /// <returns></returns>
        public string ToJSONString() => JsonConvert.SerializeObject(this);

        public ResultBase(int code, string msg, object data, object sign)
        {
            this.code = code;
            this.msg = msg;
            this.data = data;
        }

        /// <summary>
        /// 业务处理成功
        /// </summary>
        /// <returns></returns>
        public static ResultBase Ok()
        {
            return Ok(null);
        }

        /** 业务处理成功 **/
        public static ResultBase Ok(object data)
        {
            return new ResultBase(code: 0, msg: "SUCCESS", data, sign: null);
        }

        /** 业务处理成功, 自动签名 **/
        public static ResultBase OkWithSign(object data, String mchKey)
        {

            if (data == null)
            {
                return new ResultBase(code: 0, msg: "SUCCESS", data: null, sign: null);
            }

            string sign = "";
            return new ResultBase(code: 0, msg: "SUCCESS", data, sign);
        }

        /// <summary>
        /// 业务处理成功, 返回简单json格式
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ResultBase Ok4newJson(string key, object val)
        {
            return Ok(new Dictionary<string, object> { { key, val } });
        }
    }
}
