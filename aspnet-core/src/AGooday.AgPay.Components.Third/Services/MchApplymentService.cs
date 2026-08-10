using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Applyment;
using AGooday.AgPay.Domain.Models;
using AutoMapper;

namespace AGooday.AgPay.Components.Third.Services
{
    /// <summary>
    /// 商户进件业务编排服务
    ///
    /// 本服务负责：
    ///   1. 业务状态流转（预审通过 → 调通道提交 → 保存结果）
    ///   2. 第三方渠道调用（通过 IApplymentService 通道实现）
    ///   3. 异步场景发 MQ（定时任务轮询 / 通道结果查询等非实时场景）
    ///
    /// 本服务不直接操作 DB，所有数据读写通过 IMchApplyService（Application 层）完成。
    /// 这样 Application 层专注数据持久化，Components.Third 层专注外部系统交互，职责清晰。
    ///
    /// ┌───────────────────┐    ┌───────────────────────┐    ┌─────────────┐
    /// │  Controller / Job  │───▶│  MchApplymentService   │───▶│ 通道 IApply │
    /// └───────────────────┘    │  (业务编排 + 通道调用) │    └─────────────┘
    ///                          └───────────┬───────────┘
    ///                                      │
    ///                      ┌───────────────┼───────────────┐
    ///                      ▼                               ▼
    ///             IMchApplyService                 IMQSender
    ///            (纯 DB CRUD)                   (异步回写)
    /// </summary>
    public class MchApplymentService
    {
        private readonly IMapper _mapper;
        private readonly IMchApplyService _mchApplyService;
        private readonly IMQSender _mqSender;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConfigContextService _configContextService;
        private readonly ILogger<MchApplymentService> _logger;

        public MchApplymentService(
            IMapper mapper,
            IMchApplyService mchApplyService,
            IMQSender mqSender,
            IServiceProvider serviceProvider,
            ConfigContextService configContextService,
            ILogger<MchApplymentService> logger)
        {
            _mapper = mapper;
            _mchApplyService = mchApplyService;
            _mqSender = mqSender;
            _serviceProvider = serviceProvider;
            _configContextService = configContextService;
            _logger = logger;
        }

        // ==================== 业务编排 + 通道调用 ====================

        /// <summary>
        /// 预审通过后 → 调渠道提交进件 → 同步写 DB
        /// 同步场景：管理员点"审核通过"后希望立即看到渠道返回的状态
        /// </summary>
        public async Task<MchApplyDto> AuditAsync(MchApplyAuditDto dto, long? userId, string userName)
        {
            var entity = await _mchApplyService.GetByIdAsNoTrackingAsync(dto.ApplyId);
            if (entity == null) throw new Exception($"进件单不存在: {dto.ApplyId}");

            var updated = await _mchApplyService.UpdateAuditInfoAsync(dto.ApplyId,
                isPass: "PASS".Equals(dto.Action, StringComparison.OrdinalIgnoreCase),
                remark: dto.Remark,
                auditUid: userId, auditBy: userName);

            if ("REJECT".Equals(dto.Action, StringComparison.OrdinalIgnoreCase))
            {
                return updated;
            }

            // PASS → 必须同时具备 IsvNo 和通道实现，否则无法提交渠道
            if (string.IsNullOrEmpty(updated.IsvNo))
            {
                throw new Exception($"进件单[{dto.ApplyId}]未关联服务商号(IsvNo)，当前仅支持服务商模式进件。请检查数据或改为服务商通道后再提交。");
            }

            var applymentService = ResolveApplymentService(updated.IfCode);
            if (applymentService == null)
            {
                throw new Exception($"未找到通道[{updated.IfCode}]的进件服务实现。目前仅支持微信支付(WXPAY)和支付宝(ALIPAY)。");
            }

            var isvCtx = await _configContextService.GetIsvConfigContextAsync(updated.IsvNo);
            var submitRQ = new ApplymentSubmitRQ
            {
                MchApply = updated,
                IsvNo = updated.IsvNo,
                IfCode = updated.IfCode,
            };
            var submitRS = await applymentService.SubmitAsync(submitRQ, updated, isvCtx);

            var patch = new MchApplyChannelSubmittedPatch
            {
                ChannelApplyNo = submitRS.ChannelApplyNo,
                SuccResParameter = submitRS.ChannelResp,
            };
            ApplyChannelSubmitResult(submitRS, patch);

            return await _mchApplyService.MarkChannelSubmittedAsync(dto.ApplyId, patch);
        }

        /// <summary>
        /// 查询渠道进件结果 → 同步调通道 + 发 MQ 异步写 DB
        /// 异步场景：定时任务轮询（ApplymentReissueJob），不阻塞 Job 循环
        /// Controller 手动查询时，MQ 消费者很快写回，刷新即可看到
        /// </summary>
        public async Task<MchApplyDto> QueryChannelResultAsync(string applyId)
        {
            var entity = await _mchApplyService.GetByIdAsNoTrackingAsync(applyId);
            if (entity == null) throw new Exception($"进件单不存在: {applyId}");

            if (string.IsNullOrEmpty(entity.ChannelApplyNo))
            {
                throw new Exception("未提交到渠道，无法查询");
            }

            IsvConfigContext isvCtx = null;
            if (!string.IsNullOrEmpty(entity.IsvNo))
            {
                isvCtx = await _configContextService.GetIsvConfigContextAsync(entity.IsvNo);
            }

            var applymentService = ResolveApplymentService(entity.IfCode);
            if (applymentService == null)
            {
                throw new Exception($"未找到进件服务: {entity.IfCode}");
            }

            var queryRS = await applymentService.QueryAsync(
                entity.ApplyId, entity.ChannelApplyNo, entity.ChannelMchId,
                entity, isvCtx);

            // 构造 MQ 消息 → 发 MQ，由消费者异步写 DB
            var patch = BuildResultPatch(entity, queryRS);
            var msgPayload = new ApplymentChannelResultMQ.MsgPayload(
                applyId: entity.ApplyId,
                state: patch.State,
                progress: patch.Progress,
                channelApplyNo: patch.ChannelApplyNo,
                channelMchId: patch.ChannelMchId,
                applyErrorInfo: patch.ApplyErrorInfo,
                succResParameter: patch.SuccResParameter,
                signUrl: patch.SignUrl,
                signExpireAt: patch.SignExpireAt,
                verifyAmount: patch.VerifyAmount);

            try
            {
                await _mqSender.SendAsync(ApplymentChannelResultMQ.Build(msgPayload));
                _logger.LogInformation("进件[{ApplyId}] 查询渠道完成，结果已发 MQ", applyId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "进件[{ApplyId}] MQ 发送失败，降级同步写 DB", applyId);
                await _mchApplyService.PatchChannelResultAsync(applyId, patch);
            }

            // 返回当前快照（MQ 消费者稍后会更新 DB）
            var result = _mapper.Map<MchApplyDto>(entity);
            ApplyQueryResultToDto(queryRS, result);
            return result;
        }

        /// <summary>
        /// 获取签约链接 → 调通道 + 发 MQ 写 DB
        /// 异步场景：签约链接通常用于页面跳转，后台更新签约状态由 MQ 消费者完成
        /// </summary>
        public async Task<MchApplyDto> GetSignUrlAsync(string applyId)
        {
            var entity = await _mchApplyService.GetByIdAsNoTrackingAsync(applyId);
            if (entity == null) throw new Exception($"进件单不存在: {applyId}");

            if (entity.State != (byte)ApplymentState.PENDING_SIGN && entity.State != (byte)ApplymentState.SIGNING)
            {
                throw new Exception($"当前状态不允许获取签约链接: {(ApplymentState)entity.State}");
            }

            if (string.IsNullOrEmpty(entity.ChannelMchId))
            {
                throw new Exception("缺少渠道商户标识，无法获取签约链接");
            }

            IsvConfigContext isvCtx = null;
            if (!string.IsNullOrEmpty(entity.IsvNo))
            {
                isvCtx = await _configContextService.GetIsvConfigContextAsync(entity.IsvNo);
            }

            var applymentService = ResolveApplymentService(entity.IfCode);
            if (applymentService == null)
            {
                throw new Exception($"未找到进件服务: {entity.IfCode}");
            }

            var result = await applymentService.GetSignUrlAsync(
                entity.ApplyId, entity.ChannelMchId,
                entity, isvCtx);

            if (!string.IsNullOrEmpty(result.SignUrl))
            {
                // 发 MQ 写 DB（更新签约链接 + 状态为 SIGNING）
                var patch = new MchApplyChannelResultPatch
                {
                    State = (byte)ApplymentState.SIGNING,
                    Progress = 60,
                    SignUrl = result.SignUrl,
                    SignExpireAt = result.SignExpireAt,
                };

                var msgPayload = new ApplymentChannelResultMQ.MsgPayload(
                    applyId: applyId,
                    state: patch.State,
                    progress: patch.Progress,
                    signUrl: patch.SignUrl,
                    signExpireAt: patch.SignExpireAt);

                try
                {
                    await _mqSender.SendAsync(ApplymentChannelResultMQ.Build(msgPayload));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "进件[{ApplyId}] 签约链接 MQ 发送失败，降级同步写 DB", applyId);
                    await _mchApplyService.PatchChannelResultAsync(applyId, patch);
                }

                entity.SignUrl = result.SignUrl;
                entity.SignExpireAt = result.SignExpireAt;
                entity.State = (byte)ApplymentState.SIGNING;
            }

            return _mapper.Map<MchApplyDto>(entity);
        }

        /// <summary>
        /// 小额打款验证 → 同步调通道 + 同步写 DB
        /// 同步场景：用户输入验证码后立即反馈成功/失败
        /// </summary>
        public async Task<MchApplyDto> VerifyAsync(MchApplyVerifyDto dto, long? userId, string userName)
        {
            var entity = await _mchApplyService.GetByIdAsNoTrackingAsync(dto.ApplyId);
            if (entity == null) throw new Exception($"进件单不存在: {dto.ApplyId}");

            if (entity.State != (byte)ApplymentState.PENDING_VERIFY)
            {
                throw new Exception($"当前状态不允许验证: {(ApplymentState)entity.State}");
            }

            IsvConfigContext isvCtx = null;
            if (!string.IsNullOrEmpty(entity.IsvNo))
            {
                isvCtx = await _configContextService.GetIsvConfigContextAsync(entity.IsvNo);
            }

            var applymentService = ResolveApplymentService(entity.IfCode);
            if (applymentService == null)
            {
                throw new Exception($"未找到进件服务: {entity.IfCode}");
            }

            long verifyAmount = entity.VerifyAmount ?? 0;
            var verifyRS = await applymentService.VerifyAsync(
                entity.ApplyId, verifyAmount, dto.VerifyCode,
                entity, isvCtx);

            var patch = new MchApplyChannelResultPatch
            {
                SuccResParameter = verifyRS.ChannelResp,
                VerifyAmount = verifyAmount,
            };

            if ("SUCCESS".Equals(verifyRS.ChannelState))
            {
                patch.State = (byte)ApplymentState.SUCCESS;
                patch.Progress = 100;
                if (!string.IsNullOrEmpty(verifyRS.ChannelMchId))
                {
                    patch.ChannelMchId = verifyRS.ChannelMchId;
                }
            }
            else
            {
                throw new Exception($"验证失败: {verifyRS.ChannelStateDesc}");
            }

            var saved = await _mchApplyService.PatchChannelResultAsync(dto.ApplyId, patch);
            if (!saved) throw new Exception("保存验证结果失败");

            return await _mchApplyService.GetByIdAsync(dto.ApplyId);
        }

        // ==================== 私有辅助方法 ====================

        private IApplymentService ResolveApplymentService(string ifCode)
        {
            var services = _serviceProvider.GetServices<IApplymentService>();
            return services.FirstOrDefault(s => s.GetIfCode().Equals(ifCode, StringComparison.OrdinalIgnoreCase));
        }

        private void ApplyChannelSubmitResult(ApplymentSubmitRS rs, MchApplyChannelSubmittedPatch patch)
        {
            if (!string.IsNullOrEmpty(rs.ErrMsg))
            {
                patch.State = (byte)ApplymentState.CHANNEL_REJECTED;
                patch.ApplyErrorInfo = rs.ErrMsg;
                patch.Progress = 0;
                return;
            }

            switch (rs.ChannelState)
            {
                case 1:
                    patch.State = (byte)ApplymentState.CHANNEL_AUDITING;
                    patch.Progress = 40;
                    break;
                case 2:
                    patch.State = (byte)ApplymentState.CHANNEL_AUDITING;
                    patch.Progress = 50;
                    break;
                case 3:
                    patch.State = (byte)ApplymentState.PENDING_SIGN;
                    patch.SignUrl = rs.SignUrl;
                    patch.SignExpireAt = rs.SignExpireAt;
                    patch.Progress = 60;
                    break;
                case 4:
                    patch.State = (byte)ApplymentState.PENDING_VERIFY;
                    patch.VerifyAmount = rs.VerifyAmount;
                    patch.Progress = 60;
                    break;
                default:
                    patch.State = (byte)ApplymentState.CHANNEL_AUDITING;
                    patch.Progress = 40;
                    break;
            }
        }

        private MchApplyChannelResultPatch BuildResultPatch(MchApplyDto entity, ApplymentQueryRS rs)
        {
            var patch = new MchApplyChannelResultPatch
            {
                ChannelApplyNo = entity.ChannelApplyNo,
                SuccResParameter = rs.ChannelResp,
            };

            if (!string.IsNullOrEmpty(rs.ChannelMchId))
            {
                patch.ChannelMchId = rs.ChannelMchId;
            }

            switch (rs.ChannelState)
            {
                case "SUCCESS":
                    patch.State = (byte)ApplymentState.SUCCESS;
                    patch.Progress = 100;
                    break;
                case "FAIL":
                    patch.State = (byte)ApplymentState.CHANNEL_REJECTED;
                    patch.ApplyErrorInfo = rs.ChannelStateDesc;
                    patch.Progress = 0;
                    break;
                case "SIGNING":
                    patch.State = (byte)ApplymentState.PENDING_SIGN;
                    if (!string.IsNullOrEmpty(rs.SignUrl)) patch.SignUrl = rs.SignUrl;
                    patch.Progress = 60;
                    break;
                case "VERIFYING":
                    patch.State = (byte)ApplymentState.PENDING_VERIFY;
                    if (rs.VerifyAmount.HasValue) patch.VerifyAmount = rs.VerifyAmount.Value;
                    patch.Progress = 65;
                    break;
                default:
                    patch.State = (byte)ApplymentState.CHANNEL_AUDITING;
                    patch.Progress = 50;
                    break;
            }

            return patch;
        }

        private void ApplyQueryResultToDto(ApplymentQueryRS rs, MchApplyDto dto)
        {
            if (!string.IsNullOrEmpty(rs.ChannelMchId)) dto.ChannelMchId = rs.ChannelMchId;

            switch (rs.ChannelState)
            {
                case "SUCCESS":
                    dto.State = (byte)ApplymentState.SUCCESS;
                    dto.Progress = 100;
                    break;
                case "FAIL":
                    dto.State = (byte)ApplymentState.CHANNEL_REJECTED;
                    dto.ApplyErrorInfo = rs.ChannelStateDesc;
                    dto.Progress = 0;
                    break;
                case "SIGNING":
                    dto.State = (byte)ApplymentState.PENDING_SIGN;
                    if (!string.IsNullOrEmpty(rs.SignUrl)) dto.SignUrl = rs.SignUrl;
                    dto.Progress = 60;
                    break;
                case "VERIFYING":
                    dto.State = (byte)ApplymentState.PENDING_VERIFY;
                    if (rs.VerifyAmount.HasValue) dto.VerifyAmount = rs.VerifyAmount.Value;
                    dto.Progress = 65;
                    break;
                default:
                    dto.State = (byte)ApplymentState.CHANNEL_AUDITING;
                    dto.Progress = 50;
                    break;
            }
            dto.StateName = ((ApplymentState)dto.State).GetDescription();
        }
    }
}
