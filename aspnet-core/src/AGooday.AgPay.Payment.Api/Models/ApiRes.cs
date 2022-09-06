namespace AGooday.AgPay.Payment.Api.Models
{
    public class ApiRes
    {
        /// <summary>
        /// 业务响应码
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 业务响应信息
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 数据对象
        /// </summary>
        public object data { get; set; }

        /// <summary>
        /// 签名值
        /// </summary>
        public string sign { get; set; }

        public ApiRes(int code, string msg, object data, string sign)
        {
            this.code = code;
            this.msg = msg;
            this.data = data;
            this.sign = sign;
        }

        /// <summary>
        /// 业务处理成功
        /// </summary>
        /// <returns></returns>
        public static ApiRes Ok()
        {
            return Ok(null);
        }

        /** 业务处理成功 **/
        public static ApiRes Ok(object data)
        {
            return new ApiRes(code: 0, msg: "SUCCESS", data, sign: null);
        }

        /** 业务处理成功, 自动签名 **/
        public static ApiRes OkWithSign(object data, String mchKey)
        {
            if (data == null)
            {
                return new ApiRes(code: 0, msg: "SUCCESS", data: null, sign: null);
            }

            string sign = "";
            return new ApiRes(code: 0, msg: "SUCCESS", data, sign);
        }

        /// <summary>
        /// 业务处理成功, 返回简单json格式
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ApiRes Ok4newJson(string key, object val)
        {
            return Ok(new Dictionary<string, object> { { key, val } });
        }
    }
}
