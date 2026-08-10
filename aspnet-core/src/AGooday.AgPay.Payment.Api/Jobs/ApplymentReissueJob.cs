using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Components.Third.Services;
using Quartz;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    /// <summary>
    /// 商户进件状态同步定时任务
    ///
    /// 扫描"尚未终态"的进件单，主动查询渠道侧审核结果并更新本地状态。
    /// 支持的轮询状态：
    ///   CHANNEL_AUDITING（渠道审核中）— 微信 V3 异步审核、支付宝 ZFT 代扣
    ///   PENDING_SIGN / SIGNING（待签约/签约中）— 轮询微信签约状态
    ///   PENDING_VERIFY（待验证）— 轮询渠道验证完成情况
    ///
    /// 依赖分离（参考 PayOrderProcessService 模式）：
    ///   IMchApplyService        → Application 层纯 DB（分页扫描待轮询进件）
    ///   MchApplymentService     → Components.Third 层业务编排（QueryChannelResultAsync 发 MQ 异步写回）
    ///
    /// 防打爆策略：
    ///   1. 分布式锁（Redis）— 多实例部署只一个执行
    ///   2. 频率控制：同一条进件 10 分钟内只查一次
    ///
    /// Cron: 0 0/10 * * * ?  → 每 10 分钟执行一次
    /// </summary>
    [DisallowConcurrentExecution]
    public class ApplymentReissueJob : AbstractJob
    {
        private const int PAGE_SIZE = 100;

        /// <summary>轮询间隔（秒）— 避免高频打爆上游</summary>
        private const int POLL_INTERVAL_SECONDS = 600;

        /// <summary>允许查询的状态集合（非终态）</summary>
        private static readonly HashSet<byte> PollableStates = new()
        {
            (byte)ApplymentState.CHANNEL_AUDITING,
            (byte)ApplymentState.PENDING_SIGN,
            (byte)ApplymentState.SIGNING,
            (byte)ApplymentState.PENDING_VERIFY,
        };

        private readonly IMchApplyService _mchApplyService;
        private readonly MchApplymentService _mchApplymentService;
        private readonly ILogger<ApplymentReissueJob> _typedLogger;

        public ApplymentReissueJob(
            ILogger<ApplymentReissueJob> logger,
            ICacheService cacheService,
            IMchApplyService mchApplyService,
            MchApplymentService mchApplymentService)
            : base(logger, cacheService)
        {
            _mchApplyService = mchApplyService;
            _mchApplymentService = mchApplymentService;
            _typedLogger = logger;
        }

        protected override TimeSpan GetLockExpiry() => TimeSpan.FromMinutes(15);
        protected override TimeSpan GetMaxExecutionTime() => TimeSpan.FromMinutes(25);

        public override async Task Execute(IJobExecutionContext context)
        {
            await ExecuteLockTakeAsync(context.JobDetail.Key.ToString(), async () =>
            {
                int totalProcessed = 0;
                int totalSuccess = 0;
                int totalRejected = 0;
                int totalRateLimited = 0;
                int totalError = 0;

                // 对每个状态分别分页扫描
                foreach (var state in PollableStates)
                {
                    int currentPage = 1;

                    while (true)
                    {
                        var dto = new MchApplyQueryDto
                        {
                            PageNumber = currentPage,
                            PageSize = PAGE_SIZE,
                            State = state,
                        };

                        var list = await _mchApplyService.GetPaginatedDataAsync(dto);
                        if (list == null || list.Items.Count == 0) break;

                        _typedLogger.LogInformation(
                            "扫描状态 [{State}] 第 {Page} 页，共 {Count} 条",
                            (ApplymentState)state, currentPage, list.Items.Count);

                        foreach (var item in list.Items)
                        {
                            totalProcessed++;

                            // 频率控制：Redis 缓存 key = task:applyment:poll:{applyId}，存在则跳过
                            string rateKey = $"task:applyment:poll:{item.ApplyId}";
                            bool canPoll = false;
                            try
                            {
                                canPoll = await _cacheService.AcquireLockAsync(rateKey, Guid.NewGuid().ToString(), TimeSpan.FromSeconds(POLL_INTERVAL_SECONDS));
                            }
                            catch
                            {
                                canPoll = true; // Redis 异常时降级查询（保守放行）
                            }

                            if (!canPoll)
                            {
                                totalRateLimited++;
                                continue;
                            }

                            try
                            {
                                var updated = await _mchApplymentService.QueryChannelResultAsync(item.ApplyId);

                                switch ((ApplymentState)updated.State)
                                {
                                    case ApplymentState.SUCCESS:
                                        totalSuccess++;
                                        _typedLogger.LogInformation(
                                            "进件[{ApplyId}] ✅ 成功，渠道商户号: {ChannelMchId}",
                                            item.ApplyId, updated.ChannelMchId);
                                        break;

                                    case ApplymentState.CHANNEL_REJECTED:
                                        totalRejected++;
                                        _typedLogger.LogWarning(
                                            "进件[{ApplyId}] ❌ 渠道拒绝，原因: {Error}",
                                            item.ApplyId, updated.ApplyErrorInfo);
                                        break;

                                    default:
                                        // 仍处于轮询状态，继续等下一轮
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                totalError++;
                                _typedLogger.LogError(ex,
                                    "进件[{ApplyId}] 查询异常（第 {Idx} 条）",
                                    item.ApplyId, totalProcessed);
                            }
                        }

                        if (list.TotalPages <= currentPage) break;
                        currentPage++;
                    }
                }

                _typedLogger.LogInformation(
                    "任务 [{JobKey}] 结束 | 扫描 {TotalProcessed} | 成功 {Success} | 拒绝 {Rejected} | 限流跳过 {RateLimited} | 异常 {Error}",
                    context.JobDetail.Key,
                    totalProcessed, totalSuccess, totalRejected, totalRateLimited, totalError);
            });
        }
    }
}
