using System.Net;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.Services;
using Microsoft.AspNetCore.Mvc;
using static AGooday.AgPay.Components.Third.Channel.IApplymentChannelNoticeService;

namespace AGooday.AgPay.Payment.Api.Controllers.Applyment
{
    /// <summary>
    /// 渠道进件回调通知入口（参考 ChannelNoticeController 模式）
    ///
    /// 路由: api/applyment/notify/{ifCode}
    ///
    /// 处理流程:
    ///   1. 通过工厂 IChannelServiceFactory 获取对应通道的回调实现
    ///   2. ParseParamsAsync 解析请求 → 返回 applyId + 原始参数
    ///   3. IMchApplyService.GetByIdAsync 查询进件单
    ///   4. ConfigContextService.GetIsvConfigContextAsync 获取服务商配置（进件用 ISV 模式）
    ///   5. DoNoticeAsync 通道验签 + 业务解析 → 返回 ApplymentNoticeResult
    ///   6. IMchApplyService.PatchChannelResultAsync 同步更新 DB
    ///   7. 返回通道需要的响应体
    /// </summary>
    [ApiController]
    public class ApplymentChannelNoticeController : Controller
    {
        private readonly ILogger<ApplymentChannelNoticeController> _logger;
        private readonly IMchApplyService _mchApplyService;
        private readonly IChannelServiceFactory<IApplymentChannelNoticeService> _applymentNoticeFactory;
        private readonly ConfigContextService _configContextService;

        public ApplymentChannelNoticeController(
            ILogger<ApplymentChannelNoticeController> logger,
            IMchApplyService mchApplyService,
            IChannelServiceFactory<IApplymentChannelNoticeService> applymentNoticeFactory,
            ConfigContextService configContextService)
        {
            _logger = logger;
            _mchApplyService = mchApplyService;
            _applymentNoticeFactory = applymentNoticeFactory;
            _configContextService = configContextService;
        }

        /// <summary>
        /// 异步回调入口
        /// </summary>
        [HttpPost]
        [Route("api/applyment/notify/{ifCode}")]
        [Route("api/applyment/notify/{ifCode}/{applyId}")]
        public async Task<ActionResult> DoNotifyAsync(string ifCode, string applyId)
        {
            string urlApplyId = applyId;
            string logPrefix = $"进入[{ifCode}]进件回调：urlApplyId=[{applyId}] ";
            _logger.LogInformation("===== {LogPrefix} =====", logPrefix);

            try
            {
                if (string.IsNullOrWhiteSpace(ifCode))
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, "ifCode is empty");
                }

                // 获取通道回调实现
                IApplymentChannelNoticeService noticeService = _applymentNoticeFactory.GetService(ifCode);
                if (noticeService == null)
                {
                    _logger.LogError("{LogPrefix}, 通道回调实现不存在", logPrefix);
                    return BadRequest($"[{ifCode}] interface not exists");
                }

                // 解析参数
                Dictionary<string, object> parsed = await noticeService.ParseParamsAsync(
                    Request, urlApplyId, NoticeTypeEnum.DO_NOTIFY);
                if (parsed == null)
                {
                    throw new BizException("解析数据异常");
                }

                // 从解析结果获取 applyId / channelApplyNo / subMchId
                applyId = parsed.First().Key;
                object rawParams = parsed.First().Value;

                _logger.LogInformation("{LogPrefix}, 解析完成: applyId={ApplyId}", logPrefix, applyId);

                // 查找进件单
                MchApplyDto mchApply = await _mchApplyService.GetByIdAsNoTrackingAsync(applyId)
                    ?? await _mchApplyService.GetByChannelApplyNoAsync(applyId);

                if (mchApply == null)
                {
                    _logger.LogError("{LogPrefix}, 进件单不存在: applyId={ApplyId}", logPrefix, applyId);
                    return noticeService.DoNotifyApplyNotExists(Request);
                }

                // 获取服务商配置（进件走 ISV 模式）
                IsvConfigContext isvCtx = null;
                if (!string.IsNullOrEmpty(mchApply.IsvNo))
                {
                    isvCtx = await _configContextService.GetIsvConfigContextAsync(mchApply.IsvNo);
                }

                // 通道验签 + 业务解析
                ApplymentNoticeResult noticeResult = await noticeService.DoNoticeAsync(
                    Request, rawParams, mchApply, isvCtx, NoticeTypeEnum.DO_NOTIFY);

                if (noticeResult == null || noticeResult.State == ApplymentNoticeState.SYS_ERROR)
                {
                    _logger.LogError("{LogPrefix}, 回调处理异常: {NoticeResult}", logPrefix, noticeResult?.State);
                    throw new BizException("处理回调事件异常");
                }

                // 构建 Patch
                var patch = new MchApplyChannelResultPatch
                {
                    ChannelApplyNo = noticeResult.ChannelApplyNo,
                    ChannelMchId = noticeResult.ChannelMchId,
                    SuccResParameter = noticeResult.ChannelOriginResponse,
                    SignUrl = noticeResult.SignUrl,
                    VerifyAmount = noticeResult.VerifyAmount,
                };

                bool updateOk = true;
                bool stateChanged = false;

                switch (noticeResult.State)
                {
                    case ApplymentNoticeState.CONFIRM_SUCCESS:
                        patch.State = (byte)ApplymentState.SUCCESS;
                        patch.Progress = 100;
                        updateOk = await _mchApplyService.PatchChannelResultAsync(mchApply.ApplyId, patch);
                        stateChanged = true;
                        _logger.LogInformation("✅ 进件[{ApplyId}] 审核通过 | ChannelMchId={ChMchId}",
                            mchApply.ApplyId, noticeResult.ChannelMchId);
                        break;

                    case ApplymentNoticeState.CONFIRM_FAIL:
                        patch.State = (byte)ApplymentState.CHANNEL_REJECTED;
                        patch.Progress = 0;
                        patch.ApplyErrorInfo = $"{noticeResult.ErrCode}: {noticeResult.ErrMsg}";
                        updateOk = await _mchApplyService.PatchChannelResultAsync(mchApply.ApplyId, patch);
                        stateChanged = true;
                        _logger.LogWarning("❌ 进件[{ApplyId}] 审核拒绝 | Code={Code}, Msg={Msg}",
                            mchApply.ApplyId, noticeResult.ErrCode, noticeResult.ErrMsg);
                        break;

                    case ApplymentNoticeState.WAITING:
                        // 仍在处理中，更新 progress 即可
                        patch.Progress = (byte)Math.Max((int)mchApply.Progress, 50);
                        updateOk = await _mchApplyService.PatchChannelResultAsync(mchApply.ApplyId, patch);
                        break;

                    default:
                        _logger.LogWarning("⚠️ 进件[{ApplyId}] 状态未知: {State}", mchApply.ApplyId, noticeResult.State);
                        break;
                }

                if (!updateOk)
                {
                    _logger.LogError("{LogPrefix}, DB 更新失败", logPrefix);
                    return noticeService.DoNotifyApplyStateUpdateFail(Request);
                }

                _logger.LogInformation("===== {LogPrefix}, 进件回调完成。 applyId={ApplyId}, state={State}, changed={Changed} =====",
                    logPrefix, mchApply.ApplyId, noticeResult.State, stateChanged);

                return noticeResult.ResponseEntity ?? Ok();
            }
            catch (BizException e)
            {
                _logger.LogError(e, "{LogPrefix}, BizException: {Msg}", logPrefix, e.Message);
                return BadRequest(e.Message);
            }
            catch (ResponseException e)
            {
                _logger.LogError(e, "{LogPrefix}, ResponseException", logPrefix);
                return e.ResponseEntity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{LogPrefix}, 系统异常", logPrefix);
                return BadRequest(e.Message);
            }
        }
    }
}
