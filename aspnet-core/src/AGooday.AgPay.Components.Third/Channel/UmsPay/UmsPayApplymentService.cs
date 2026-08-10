using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params.UmsPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Applyment;
using AGooday.AgPay.Components.Third.Services;

namespace AGooday.AgPay.Components.Third.Channel.UmsPay
{
    public class UmsPayApplymentService : AbstractApplymentService
    {
        public UmsPayApplymentService(
            ILogger<UmsPayApplymentService> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override string GetIfCode() => CS.IF_CODE.UMSPAY;

        public override Task<ApplymentSubmitRS> SubmitAsync(ApplymentSubmitRQ rq, MchApplyDto mchApply, IsvConfigContext isvCtx)
        {
            var result = new ApplymentSubmitRS();
            var isvParams = isvCtx?.GetIsvParamsByIfCode<UmsPayIsvParams>(CS.IF_CODE.UMSPAY);
            if (isvParams == null)
            {
                result.ErrMsg = "银联商务通道配置不存在";
                return Task.FromResult(result);
            }

            result.ErrMsg = "银联商务通道进件服务待接入，请联系技术支持";
            return Task.FromResult(result);
        }

        public override Task<ApplymentQueryRS> QueryAsync(string applyId, string channelApplyNo, string channelMchId, MchApplyDto mchApply, IsvConfigContext isvCtx)
        {
            var result = new ApplymentQueryRS { ChannelState = "FAIL" };
            var isvParams = isvCtx?.GetIsvParamsByIfCode<UmsPayIsvParams>(CS.IF_CODE.UMSPAY);
            if (isvParams == null)
            {
                result.ChannelStateDesc = "银联商务通道配置不存在";
                return Task.FromResult(result);
            }

            result.ChannelStateDesc = "银联商务通道进件服务待接入";
            return Task.FromResult(result);
        }
    }
}
