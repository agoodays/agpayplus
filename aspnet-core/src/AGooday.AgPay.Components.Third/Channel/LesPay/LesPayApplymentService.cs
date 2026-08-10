using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.LesPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Applyment;
using AGooday.AgPay.Components.Third.Services;

namespace AGooday.AgPay.Components.Third.Channel.LesPay
{
    public class LesPayApplymentService : AbstractApplymentService
    {
        public LesPayApplymentService(
            ILogger<LesPayApplymentService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override string GetIfCode() => CS.IF_CODE.LESPAY;

        public override Task<ApplymentSubmitRS> SubmitAsync(ApplymentSubmitRQ rq, MchApplyDto mchApply, IsvConfigContext isvCtx)
        {
            var result = new ApplymentSubmitRS();
            var isvParams = isvCtx?.GetIsvParamsByIfCode<LesPayIsvParams>(CS.IF_CODE.LESPAY);
            if (isvParams == null)
            {
                result.ErrMsg = "乐刷通道配置不存在";
                return Task.FromResult(result);
            }

            result.ErrMsg = "乐刷通道进件服务待接入，请联系技术支持";
            return Task.FromResult(result);
        }

        public override Task<ApplymentQueryRS> QueryAsync(string applyId, string channelApplyNo, string channelMchId, MchApplyDto mchApply, IsvConfigContext isvCtx)
        {
            var result = new ApplymentQueryRS { ChannelState = "FAIL" };
            var isvParams = isvCtx?.GetIsvParamsByIfCode<LesPayIsvParams>(CS.IF_CODE.LESPAY);
            if (isvParams == null)
            {
                result.ChannelStateDesc = "乐刷通道配置不存在";
                return Task.FromResult(result);
            }

            result.ChannelStateDesc = "乐刷通道进件服务待接入";
            return Task.FromResult(result);
        }
    }
}
