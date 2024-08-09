﻿using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

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
        /// 获取请求参数并转换为对象，通用验证
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cls"></param>
        /// <returns></returns>
        protected T GetRQ<T>() where T : AbstractRQ
        {
            T bizRQ = GetObject<T>();

            // [1]. 验证通用字段规则
            // 先清除验证状态
            ModelState.ClearValidationState(nameof(T));
            // 重新进行验证
            if (!TryValidateModel(bizRQ, nameof(T)))
            {
                var errors = string.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
                ModelState.Where(s => s.Value != null && s.Value.ValidationState == ModelValidationState.Invalid)
                .ToList().ForEach((item) =>
                {
                    throw new BizException(string.Join(Environment.NewLine, item.Value!.Errors.Select(e => e.ErrorMessage)));
                });
            }
            return bizRQ;
        }

        /// <summary>
        /// 获取请求参数并转换为对象，商户通用验证
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cls"></param>
        /// <returns></returns>
        protected T GetRQByWithMchSign<T>() where T : AbstractRQ
        {
            //获取请求RQ, and 通用验证
            T bizRQ = GetRQ<T>();

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

            MchAppConfigContext mchAppConfigContext = _configContextQueryService.QueryMchInfoAndAppInfo(mchNo, appId);

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
        /// 获取对象类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetObject<T>()
        {
            return JsonConvert.DeserializeObject<T>(this.GetReqParamJson().ToString());
        }

        /// <summary>
        /// 获取json格式的请求参数
        /// </summary>
        /// <returns></returns>
        protected JObject GetReqParamJson()
        {
            Request.EnableBuffering();

            string body = "";
            var stream = Request.Body;
            if (stream != null)
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true))
                {
                    body = reader.ReadToEnd();
                }
                stream.Seek(0, SeekOrigin.Begin);
            }

            return JObject.Parse(body);
        }

        protected async Task<JObject> GetReqParamToJsonAsync()
        {
            Request.EnableBuffering();
            var request = Request;

            // 获取 URL 参数
            var urlParameters = GetUrlParameters(request.Query);
            //var urlParameters = request.Query.ToDictionary(x => x.Key, x => GetQueryParameterValue(x.Value));

            // 获取请求体参数（JSON 格式）
            var requestBody = await ReadRequestBodyAsync(request.Body);
            var requestBodyJson = !string.IsNullOrEmpty(requestBody) && !request.HasFormContentType ? JObject.Parse(requestBody) : new JObject();

            // 获取表单参数
            var formParameters = request.HasFormContentType ? GetFormParameters(request.Form) : new Dictionary<string, JToken>();

            // 合并所有参数
            var allParameters = new JObject();
            allParameters.Merge(JObject.FromObject(urlParameters));
            allParameters.Merge(requestBodyJson);
            allParameters.Merge(JObject.FromObject(formParameters));

            return allParameters;
        }

        private static async Task<string> ReadRequestBodyAsync(Stream body)
        {
            using var reader = new StreamReader(body, Encoding.UTF8, true, 1024, true);
            return await reader.ReadToEndAsync();
        }

        private static Dictionary<string, JToken> GetUrlParameters(IQueryCollection formCollection)
        {
            var parameters = new Dictionary<string, JToken>();
            foreach (var keyValuePair in formCollection)
            {
                if (keyValuePair.Value.Count > 1)
                {
                    parameters[keyValuePair.Key] = JToken.Parse(JsonConvert.SerializeObject(keyValuePair.Value));
                }
                else
                {
                    parameters[keyValuePair.Key] = JToken.FromObject(keyValuePair.Value.FirstOrDefault());
                }
            }
            return parameters;
        }

        private static JToken GetQueryParameterValue(string[] values)
        {
            if (values.Length > 1)
            {
                return JToken.FromObject(values);
            }
            else
            {
                return JToken.FromObject(values[0]);
            }
        }

        private static Dictionary<string, JToken> GetFormParameters(IFormCollection formCollection)
        {
            var parameters = new Dictionary<string, JToken>();
            foreach (var keyValuePair in formCollection)
            {
                if (keyValuePair.Value.Count > 1)
                {
                    parameters[keyValuePair.Key] = JToken.Parse(JsonConvert.SerializeObject(keyValuePair.Value));
                }
                else
                {
                    parameters[keyValuePair.Key] = JToken.FromObject(keyValuePair.Value.FirstOrDefault());
                }
            }
            return parameters;
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
