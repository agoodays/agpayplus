using System.Text;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Controllers
{
    /// <summary>
    /// api 抽象接口， 公共函数
    /// </summary>
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected readonly RequestKit _requestKit;
        protected readonly ConfigContextQueryService _configContextQueryService;

        protected ApiControllerBase(RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
        {
            _requestKit = requestKit;
            _configContextQueryService = configContextQueryService;
        }

        /// <summary>
        /// 获取请求参数并转换为对象，商户通用验证
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cls"></param>
        /// <returns></returns>
        protected async Task<T> GetRQByWithMchSignAsync<T>() where T : AbstractRQ
        {
            //获取请求RQ, and 通用验证
            T bizRQ = await this.GetRQAsync<T>();

            AbstractMchAppRQ abstractMchAppRQ = bizRQ as AbstractMchAppRQ;

            //业务校验， 包括： 验签， 商户状态是否可用， 是否支持该支付方式下单等。
            string mchNo = abstractMchAppRQ.MchNo;
            string appId = abstractMchAppRQ.AppId;
            string signType = bizRQ.SignType;
            string sign = bizRQ.Sign;

            if (StringUtil.IsAnyNullOrEmpty(mchNo, appId, sign))
            {
                throw new BizException("参数有误！");
            }

            MchAppConfigContext mchAppConfigContext = await _configContextQueryService.QueryMchInfoAndAppInfoAsync(mchNo, appId);

            if (mchAppConfigContext == null)
            {
                throw new BizException("商户或商户应用不存在");
            }

            if (mchAppConfigContext.MchInfo == null || mchAppConfigContext.MchInfo.State != CS.YES)
            {
                throw new BizException("商户信息不存在或商户状态不可用");
            }

            var mchApp = mchAppConfigContext.MchApp;
            if (mchApp == null || mchApp.State != CS.YES)
            {
                throw new BizException("商户应用不存在或应用状态不可用");
            }

            if (!(mchApp.AppSignType?.Any(item => signType.Equals(item.ToString())) ?? false))
            {
                throw new BizException($"商户应用不支持[{signType}]签名方式");
            }

            if (!mchApp.MchNo.Equals(mchNo))
            {
                throw new BizException("参数appId与商户号不匹配");
            }

            // 验签
            string appSecret = mchApp.AppSecret;
            string appPublicKey = mchApp.AppRsa2PublicKey;

            // 转换为 JSON
            JObject bizReqJSON = JObject.FromObject(bizRQ);
            bizReqJSON.Remove("sign");

            if (!AgPayUtil.Verify(bizReqJSON, signType, sign, appSecret, appPublicKey))
            {
                throw new BizException("验签失败");
            }

            return bizRQ;
        }

        /// <summary>
        /// 获取请求参数并转换为对象，通用验证
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cls"></param>
        /// <returns></returns>
        protected async Task<T> GetRQAsync<T>() where T : AbstractRQ
        {
            T bizRQ = await this.GetObjectAsync<T>();

            // [1]. 验证通用字段规则
            // 先清除验证状态
            ModelState.ClearValidationState(nameof(T));
            // 重新进行验证
            if (!TryValidateModel(bizRQ, nameof(T)))
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                if (errors.Count > 0)
                    throw new BizException(string.Join(Environment.NewLine, errors));
            }
            return bizRQ;
        }

        /// <summary>
        /// 获取对象类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected async Task<T> GetObjectAsync<T>()
        {
            var param = await this.GetReqParamToJsonAsync();
            return JsonConvert.DeserializeObject<T>(param.ToString());
        }

        /// <summary>
        /// 获取请求参数并转换为JSON对象，包含URL参数、表单参数和请求体参数
        /// </summary>
        /// <returns></returns>
        protected async Task<JObject> GetReqParamToJsonAsync()
        {
            Request.EnableBuffering();
            var request = Request;

            // 只读取一次请求体
            string requestBody = "";
            if (request.Body.CanSeek)
            {
                request.Body.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
                {
                    requestBody = await reader.ReadToEndAsync();
                }
                request.Body.Seek(0, SeekOrigin.Begin);
            }

            var requestBodyJson = !string.IsNullOrEmpty(requestBody) && !request.HasFormContentType
                ? JObject.Parse(requestBody)
                : new JObject();

            // 合并URL参数和表单参数
            // 获取 URL 参数
            var urlParameters = request.Query.ToDictionary(x => x.Key, x => x.Value.Count > 1 ? JToken.FromObject(x.Value.ToArray()) : JToken.FromObject(x.Value.FirstOrDefault()));
            // 获取表单参数
            var formParameters = request.HasFormContentType
                ? request.Form.ToDictionary(x => x.Key, x => x.Value.Count > 1 ? JToken.FromObject(x.Value.ToArray()) : JToken.FromObject(x.Value.FirstOrDefault()))
                : new Dictionary<string, JToken>();

            // 合并所有参数
            var allParameters = new JObject();
            allParameters.Merge(JObject.FromObject(urlParameters));
            allParameters.Merge(requestBodyJson);
            allParameters.Merge(JObject.FromObject(formParameters));

            return allParameters;
        }

        /// <summary>
        /// 获取json格式的请求参数
        /// </summary>
        /// <returns></returns>
        protected async Task<JObject> GetReqParamJsonAsync()
        {
            Request.EnableBuffering();
            string requestBody = "";
            if (Request.Body.CanSeek)
            {
                Request.Body.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    requestBody = await reader.ReadToEndAsync();
                }
                Request.Body.Seek(0, SeekOrigin.Begin);
            }
            return string.IsNullOrWhiteSpace(requestBody) ? new JObject() : JObject.Parse(requestBody);
        }

        /// <summary>
        /// 获取客户端ip地址
        /// </summary>
        /// <returns></returns>
        protected string GetClientIp()
        {
            return _requestKit.GetClientIp();
        }
    }
}
