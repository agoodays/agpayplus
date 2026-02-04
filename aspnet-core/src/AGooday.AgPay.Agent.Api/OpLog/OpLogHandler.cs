using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Base.Api.OpLog;
using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Agent.Api.OpLog
{
    /// <summary>
    /// 操作日志处理
    /// </summary>
    public class OpLogHandler : OpLogHandlerBase
    {
        public OpLogHandler(ILogger<OpLogHandler> logger, IHttpContextAccessor httpContextAccessor, ISysLogService sysLogService)
            : base(logger, httpContextAccessor, sysLogService)
        {
        }

        protected override string GetSysType()
        {
            return CS.SYS_TYPE.AGENT;
        }
    }
}
