using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AspectCore.DynamicProxy;
using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace AGooday.AgPay.Manager.Api.AOP
{
    public class MethodLog : AbstractInterceptorAttribute
    {
        private readonly string Remark;

        public MethodLog(string remark)
        {
            Remark = remark;
        }

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var accessor = context.ServiceProvider.GetService<IHttpContextAccessor>();
            var sysLogService = context.ServiceProvider.GetService<ISysLogService>();
            //var loggerFactory = context.ServiceProvider.GetService<ILoggerFactory>();
            //var logger = loggerFactory.CreateLogger<MethodLog>();
            var logger = context.ServiceProvider.GetService<ILogger<MethodLog>>();
            var sysLog = new SysLogDto();
            try
            {
                sysLog.SysType = CS.SYS_TYPE.MGR;                
                sysLog.MethodName = context.ServiceMethod.Name;
                sysLog.MethodRemark = Remark;
                sysLog.ReqUrl = "";
                await next(context);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                //try
                //{
                //    sysLogService.Add(sysLog);
                //}
                //catch (Exception ex)
                //{
                //    logger.LogError(ex, ex.Message);
                //}
            }
        }
    }
}
