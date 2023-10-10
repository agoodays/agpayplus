using AGooday.AgPay.Common.Utils;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Common.Models
{
    public class ApiRes
    {
        /// <summary>
        /// 业务响应码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 业务响应信息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 数据对象
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 签名值
        /// </summary>
        public string Sign { get; set; }

        public ApiRes() { }

        public ApiRes(int code, string msg, object data, string sign)
        {
            this.Code = code;
            this.Msg = msg;
            this.Data = data;
            this.Sign = sign;
        }

        /// <summary>
        /// 业务处理成功
        /// </summary>
        /// <returns></returns>
        public static ApiRes Ok()
        {
            return Ok(null);
        }

        /// <summary>
        /// 业务处理成功
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ApiRes Ok(object data)
        {
            return new ApiRes(ApiCode.SUCCESS.GetCode(), ApiCode.SUCCESS.GetMsg(), data, sign: null);
        }

        /// <summary>
        /// 业务处理成功, 自动签名
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mchKey"></param>
        /// <returns></returns>
        public static ApiRes OkWithSign(object data, string signType, string mchKey)
        {
            if (data == null)
            {
                return new ApiRes(ApiCode.SUCCESS.GetCode(), ApiCode.SUCCESS.GetMsg(), data: null, sign: null);
            }
            var jsonObject = JObject.FromObject(data);
            string sign = AgPayUtil.Sign(jsonObject, signType, mchKey);
            return new ApiRes(ApiCode.SUCCESS.GetCode(), ApiCode.SUCCESS.GetMsg(), data, sign);
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

        /// <summary>
        /// 业务处理失败
        /// </summary>
        /// <param name="apiCode"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ApiRes Fail(ApiCode apiCode, params string[] args)
        {
            return Fail(apiCode, null, args);
        }

        /// <summary>
        /// 业务处理失败
        /// </summary>
        /// <param name="apiCode"></param>
        /// <param name="data"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ApiRes Fail(ApiCode apiCode, object data, params string[] args)
        {
            if (args == null || args.Length <= 0)
            {
                return new ApiRes(apiCode.GetCode(), apiCode.GetMsg(), null, null);
            }
            return new ApiRes(apiCode.GetCode(), string.Format(apiCode.GetMsg(), args), data, null);
        }

        /// <summary>
        /// 自定义错误信息, 原封不用的返回输入的错误信息
        /// </summary>
        /// <param name="customMsg"></param>
        /// <returns></returns>
        public static ApiRes CustomFail(string customMsg)
        {
            return new ApiRes(ApiCode.CUSTOM_FAIL.GetCode(), customMsg, null, null);
        }
        public static ApiRes CustomFail(string[] customMsgs)
        {
            return new ApiRes(ApiCode.CUSTOM_FAIL.GetCode(), string.Join(";", customMsgs), null, null);
        }
    }
}
