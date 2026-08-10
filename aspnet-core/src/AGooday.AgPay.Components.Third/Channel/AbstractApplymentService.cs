using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Applyment;
using AGooday.AgPay.Components.Third.Services;

namespace AGooday.AgPay.Components.Third.Channel
{
    /// <summary>
    /// 商户进件抽象基类
    /// </summary>
    public abstract class AbstractApplymentService : IApplymentService
    {
        protected readonly ILogger<AbstractApplymentService> _logger;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ISysConfigService _sysConfigService;
        protected readonly ConfigContextQueryService _configContextQueryService;

        protected AbstractApplymentService(
            ILogger<AbstractApplymentService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _sysConfigService = sysConfigService;
            _configContextQueryService = configContextQueryService;
        }

        protected AbstractApplymentService()
        {
        }

        public abstract string GetIfCode();

        public abstract Task<ApplymentSubmitRS> SubmitAsync(ApplymentSubmitRQ rq, MchApplyDto mchApply, IsvConfigContext isvCtx);

        public abstract Task<ApplymentQueryRS> QueryAsync(string applyId, string channelApplyNo, string channelMchId, MchApplyDto mchApply, IsvConfigContext isvCtx);

        public virtual Task<ApplymentSubmitRS> GetSignUrlAsync(string applyId, string channelMchId, MchApplyDto mchApply, IsvConfigContext isvCtx)
        {
            return Task.FromResult(new ApplymentSubmitRS { ErrMsg = "当前通道不支持获取签约链接" });
        }

        public virtual Task<ApplymentQueryRS> VerifyAsync(string applyId, long verifyAmount, string verifyCode, MchApplyDto mchApply, IsvConfigContext isvCtx)
        {
            return Task.FromResult(new ApplymentQueryRS { ChannelState = "FAIL", ChannelStateDesc = "当前通道不支持小额打款验证" });
        }

        protected string GetNotifyUrl()
        {
            return $"{_sysConfigService.GetDBApplicationConfig().PaySiteUrl}/api/apply/notify/{GetIfCode()}";
        }
    }
}
