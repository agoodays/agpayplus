using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Components.Third.Models;

namespace AGooday.AgPay.Components.Third.Channel
{
    /// <summary>
    /// 商户进件策略接口
    /// </summary>
    public interface IApplymentService
    {
        /// <summary>接口代码</summary>
        string GetIfCode();

        /// <summary>
        /// 提交商户进件到上游渠道
        /// </summary>
        Task<RQRS.Applyment.ApplymentSubmitRS> SubmitAsync(RQRS.Applyment.ApplymentSubmitRQ rq, MchApplyDto mchApply, IsvConfigContext isvCtx);

        /// <summary>
        /// 查询进件结果
        /// </summary>
        Task<RQRS.Applyment.ApplymentQueryRS> QueryAsync(string applyId, string channelApplyNo, string channelMchId, MchApplyDto mchApply, IsvConfigContext isvCtx);

        /// <summary>
        /// 获取签约链接
        /// </summary>
        Task<RQRS.Applyment.ApplymentSubmitRS> GetSignUrlAsync(string applyId, string channelMchId, MchApplyDto mchApply, IsvConfigContext isvCtx);

        /// <summary>
        /// 小额打款验证
        /// </summary>
        Task<RQRS.Applyment.ApplymentQueryRS> VerifyAsync(string applyId, long verifyAmount, string verifyCode, MchApplyDto mchApply, IsvConfigContext isvCtx);
    }
}
