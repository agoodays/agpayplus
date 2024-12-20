using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace AGooday.AgPay.Merchant.Api.OpLog
{
    /// <summary>
    /// 操作日志处理
    /// </summary>
    public class OpLogHandler : IOpLogHandler
    {
        private readonly ILogger _logger;
        private readonly ISysLogService _sysLogService;
        private static IHttpContextAccessor _context;

        public OpLogHandler(ILogger<OpLogHandler> logger, IHttpContextAccessor httpContextAccessor, ISysLogService sysLogService)
        {
            _logger = logger;
            _context = httpContextAccessor;
            _sysLogService = sysLogService;
        }

        public async Task LogAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var sw = new Stopwatch();
            sw.Start();
            //操作参数
            var args = JsonConvert.SerializeObject(context.ActionArguments);
            var actionExecutedContext = await next();
            sw.Stop();
            //操作结果
            //var result = JsonConvert.SerializeObject(actionResult?.Value);
            var model = new SysLogDto();
            try
            {
                var sysUserId = _context.HttpContext.User.FindFirstValue(ClaimAttributes.SysUserId);
                var realname = _context.HttpContext.User.FindFirstValue(ClaimAttributes.Realname);
                var sysType = _context.HttpContext.User.FindFirstValue(ClaimAttributes.SysType);
                model.UserId = string.IsNullOrWhiteSpace(sysUserId) ? null : Convert.ToInt64(sysUserId);
                model.UserName = string.IsNullOrWhiteSpace(realname) ? null : realname;
                string ua = context.HttpContext.Request.Headers.UserAgent;
                var clientInfo = UAParser.Parser.GetDefault().Parse(ua);
                var device = clientInfo.Device.Family;
                device = device.Equals("other", StringComparison.CurrentCultureIgnoreCase) ? "" : device;
                model.Browser = clientInfo.UA.Family;
                model.Os = clientInfo.OS.Family;
                model.Device = device;
                model.BrowserInfo = ua;
                model.UserIp = IpUtil.GetIP(context?.HttpContext?.Request);
                model.SysType = sysType;
                model.MethodName = context.ActionDescriptor.DisplayName.Split(" (").First();
                model.ReqUrl = GetAbsoluteUri(context?.HttpContext?.Request).ToLower();//context.ActionDescriptor.AttributeRouteInfo.Template.ToLower();
                model.ReqMethod = context.HttpContext.Request.Method.ToLower();
                model.OptReqParam = args;
                if (context.ActionDescriptor.EndpointMetadata.Any(m => m.GetType() == typeof(MethodLogAttribute)))
                {
                    var methodLogAttribute = (MethodLogAttribute)context.ActionDescriptor.EndpointMetadata.First(m => m.GetType() == typeof(MethodLogAttribute));
                    model.LogType = (byte)methodLogAttribute.Type;
                    model.MethodRemark = methodLogAttribute.Remark;
                }
                if (actionExecutedContext.Result is ObjectResult result)
                {
                    var controllername = ((ControllerActionDescriptor)context.ActionDescriptor).ControllerName;
                    var actionname = ((ControllerActionDescriptor)context.ActionDescriptor).ActionName;
                    if (controllername.EndsWith("Auth") && (actionname.EndsWith("Validate") || actionname.EndsWith("PhoneCode")))
                    {
                        var jwtStr = (result.Value as ApiRes).Data.ToKeyValue().Values.First();
                        var tokenModelJwt = JwtBearerAuthenticationExtension.SerializeJwt(jwtStr);
                        sysUserId = tokenModelJwt.SysUserId;
                        realname = tokenModelJwt.Realname;
                        model.UserId = string.IsNullOrWhiteSpace(sysUserId) ? null : Convert.ToInt64(sysUserId);
                        model.UserName = string.IsNullOrWhiteSpace(realname) ? null : realname;
                    }
                    model.OptResInfo = JsonConvert.SerializeObject(result.Value);
                }
                else
                {
                    if (actionExecutedContext.Exception != null)
                    {
                        model.OptResInfo = actionExecutedContext.Exception.Message;
                    }
                }
                model.ElapsedMs = sw.ElapsedMilliseconds;
                model.CreatedAt = DateTime.Now;
                await _sysLogService.AddAsync(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"操作日志：{JsonConvert.SerializeObject(model)}");
            }
        }

        /// <summary>
        /// 获取绝对路径 https://localhost:9418/api/anon/auth/validate
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static string GetAbsoluteUri(HttpRequest request)
        {
            return new StringBuilder()
             .Append(request.Scheme)
             .Append("://")
             .Append(request.Host)
             .Append(request.PathBase)
             .Append(request.Path)
             .Append(request.QueryString)
             .ToString();
        }

        /// <summary>
        /// 获取接口相对路径 /api/anon/auth/validate
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static string GetrelativeUri(HttpRequest request)
        {
            return request.Path;
        }
    }
}
