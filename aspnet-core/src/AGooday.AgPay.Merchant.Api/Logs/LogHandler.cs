using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;

namespace AGooday.AgPay.Merchant.Api.Logs
{
    /// <summary>
    /// 操作日志处理
    /// </summary>
    public class LogHandler : ILogHandler
    {
        private readonly ILogger _logger;
        private readonly ISysLogService _sysLogService;
        private static IHttpContextAccessor _context;

        public LogHandler(ILogger<LogHandler> logger, IHttpContextAccessor httpContextAccessor, ISysLogService sysLogService)
        {
            _logger = logger;
            _context = httpContextAccessor;
            _sysLogService = sysLogService;
        }

        public async Task LogAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var sw = new Stopwatch();
            sw.Start();
            var actionExecutedContext = await next();
            sw.Stop();

            //操作参数
            var args = JsonConvert.SerializeObject(context.ActionArguments);
            //操作结果
            //var result = JsonConvert.SerializeObject(actionResult?.Value);
            var model = new SysLogDto();
            try
            {
                var sysUserId = _context.HttpContext.User.FindFirstValue(ClaimAttributes.SysUserId);
                var realname = _context.HttpContext.User.FindFirstValue(ClaimAttributes.Realname);
                model.UserId = string.IsNullOrWhiteSpace(sysUserId) ? null : Convert.ToInt64(sysUserId);
                model.UserName = string.IsNullOrWhiteSpace(realname) ? null : realname;
                string ua = context.HttpContext.Request.Headers["User-Agent"];
                var clientInfo = UAParser.Parser.GetDefault().Parse(ua);
                var device = clientInfo.Device.Family;
                device = device.ToLower() == "other" ? "" : device;
                model.Browser = clientInfo.UA.Family;
                model.Os = clientInfo.OS.Family;
                model.Device = device;
                model.BrowserInfo = ua;
                model.UserIp = IpUtil.GetIP(context?.HttpContext?.Request);
                model.SysType = CS.SYS_TYPE.MCH;
                model.MethodName = context.ActionDescriptor.DisplayName.Split(" (").First();
                model.ReqUrl = GetAbsoluteUri(context?.HttpContext?.Request).ToLower();//context.ActionDescriptor.AttributeRouteInfo.Template.ToLower();
                model.ReqMethod = context.HttpContext.Request.Method.ToLower();
                model.OptReqParam = args;
                if (context.ActionDescriptor.EndpointMetadata.Any(m => m.GetType() == typeof(MethodLogAttribute)))
                {
                    model.MethodRemark = ((MethodLogAttribute)context.ActionDescriptor.EndpointMetadata.First(m => m.GetType() == typeof(MethodLogAttribute))).Remark;
                }
                ObjectResult result = actionExecutedContext.Result as ObjectResult;
                if (result != null)
                {
                    var controllername = ((ControllerActionDescriptor)context.ActionDescriptor).ControllerName;
                    var actionname = ((ControllerActionDescriptor)context.ActionDescriptor).ActionName;
                    if (controllername.EndsWith("Auth") && actionname.EndsWith("Validate"))
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
                _sysLogService.Add(model);
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
        private string GetAbsoluteUri(HttpRequest request)
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
        private string GetrelativeUri(HttpRequest request)
        {
            return request.Path;
        }
    }
}
