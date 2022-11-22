using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Manager.Api.Attributes;
using System.Security.Claims;

namespace AGooday.AgPay.Manager.Api.Logs
{
    /// <summary>
    /// 操作日志处理
    /// </summary>
    public class LogHandler : ILogHandler
    {
        private readonly ILogger _logger;
        private readonly ISysLogService _sysLogService;
        private static IHttpContextAccessor _context;

        public LogHandler(ILogger<LogHandler> logger, ISysLogService sysLogService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _sysLogService = sysLogService;
            _context = httpContextAccessor;
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
                var sysUserId = _context.HttpContext.User.FindFirstValue("sysUserId");
                var realname = _context.HttpContext.User.FindFirstValue("realname");
                model.UserId = string.IsNullOrWhiteSpace(sysUserId) ? null : Convert.ToInt64(sysUserId);
                model.UserName = string.IsNullOrWhiteSpace(realname) ? null : realname;
                model.UserIp = IpUtil.GetIP(context?.HttpContext?.Request);
                model.SysType = CS.SYS_TYPE.MGR;
                model.MethodName = context.HttpContext.Request.Method.ToLower();
                model.ReqUrl = context.ActionDescriptor.AttributeRouteInfo.Template.ToLower();
                model.OptReqParam = args;
                if (context.ActionDescriptor.EndpointMetadata.Any(m => m.GetType() == typeof(MethodRemarkAttribute)))
                {
                    model.MethodRemark = ((MethodRemarkAttribute)context.ActionDescriptor.EndpointMetadata.First(m => m.GetType() == typeof(MethodRemarkAttribute))).Remark;
                }
                ObjectResult result = actionExecutedContext.Result as ObjectResult;
                if (result != null)
                {
                    model.OptResInfo = JsonConvert.SerializeObject(result.Value);
                }
                else
                {
                    if (actionExecutedContext.Exception != null)
                    {
                        model.OptResInfo = actionExecutedContext.Exception.Message;
                    }
                }
                model.CreatedAt = DateTime.Now;
                _sysLogService.Add(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"操作日志：{JsonConvert.SerializeObject(model)}");
            }
        }
    }
}
