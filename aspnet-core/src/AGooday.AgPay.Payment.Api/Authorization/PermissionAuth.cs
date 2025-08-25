using System.Text;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PermissionAuth : Attribute, IAuthorizationFilter
    {
        public PermissionAuth(params string[] name)
        {
            Name = name;
        }

        public string[] Name { get; set; }

        /// <summary>
        /// 权限校验逻辑，如果商户没有对应接口的权限则抛出异常。
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="BizException"></exception>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string mchNo = GetMchNoFromRequest(context.HttpContext.Request);
            if (string.IsNullOrWhiteSpace(mchNo))
            {
                throw new BizException($"商户号不能为空");
            }

            var sysConfigService = context.HttpContext.RequestServices.GetRequiredService<ISysConfigService>();
            var config = sysConfigService.GetByGroupKey("mchApiEnt", CS.SYS_TYPE.MCH, mchNo)
                .Where(w => w.ConfigKey.Equals("mchApiEntList")).FirstOrDefault();
            if (config == null || string.IsNullOrWhiteSpace(config.ConfigVal))
                throw new BizException("商户未配置接口权限");

            var merchantPermissions = JsonConvert.DeserializeObject<HashSet<string>>(config.ConfigVal);
            if (!Name.Any(merchantPermissions.Contains))
            {
                throw new BizException($"商户无此接口[{string.Join(",", Name)}]权限!");
                //throw new UnauthorizeException();
                //context.Result = new ForbidResult();
            }
        }

        /// <summary>
        /// 从请求中获取商户号 商户号优先从 QueryString 获取，如果没有再尝试从 Body 中获取。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static string GetMchNoFromRequest(HttpRequest request)
        {
            // 1. 优先从 QueryString 获取
            if (request.Query.TryGetValue("mchNo", out var mchNoValues) && !string.IsNullOrWhiteSpace(mchNoValues))
            {
                return mchNoValues.ToString();
            }

            // 2. 再尝试从 Body 获取
            // 从请求体中获取 mchNo，请求体是 JSON 格式，mchNo 是 JSON 对象中的一个属性
            request.EnableBuffering();
            string requestBody = "";
            if (request.Body.CanSeek)
            {
                request.Body.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
                {
                    requestBody = reader.ReadToEnd();
                }
                request.Body.Seek(0, SeekOrigin.Begin);
            }

            if (!string.IsNullOrWhiteSpace(requestBody))
            {
                try
                {
                    JObject json = JObject.Parse(requestBody);
                    if (json.TryGetValue("mchNo", out var mchNoToken))
                    {
                        return mchNoToken?.ToString();
                    }
                }
                catch
                {
                    // Body 不是 JSON 格式时忽略
                }
            }

            return null;
        }
    }
}
