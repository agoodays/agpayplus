using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

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

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string mchNo = GetMchNoFromRequest(context.HttpContext.Request);
            if (string.IsNullOrWhiteSpace(mchNo))
            {
                throw new BizException($"商户号不能为空");
            }
            var sysConfigService = context.HttpContext.RequestServices.GetRequiredService<ISysConfigService>();
            var configVal = sysConfigService.GetByGroupKey("mchApiEnt", CS.SYS_TYPE.MCH, mchNo)
                .Where(w => w.ConfigKey.Equals("mchApiEntList")).FirstOrDefault().ConfigVal;
            var merchantPermissions = JsonConvert.DeserializeObject<List<string>>(configVal);
            if (!merchantPermissions.Intersect(Name).Any())
            {
                throw new BizException($"商户无此接口[{string.Join(",", Name)}]权限!");
                //throw new UnauthorizeException();
                //context.Result = new ForbidResult();
            }
        }

        private static string GetMchNoFromRequest(HttpRequest request)
        {
            // 从请求体中获取 mchNo，请求体是 JSON 格式，mchNo 是 JSON 对象中的一个属性
            request.EnableBuffering();
            string requestBody = new StreamReader(request.Body, Encoding.UTF8).ReadToEnd();
            request.Body.Position = 0;

            // 使用适当的 JSON 解析库解析请求体，并获取 mchNo
            JObject json = JObject.Parse(requestBody);
            json.TryGetString("mchNo", out string mchNo);

            return mchNo;
        }
    }
}
