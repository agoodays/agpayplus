using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Components.Third.Services;
using Quartz;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    /// <summary>
    /// 补单定时任务(退款单)
    /// </summary>
    [DisallowConcurrentExecution]
    public class RefundOrderReissueJob : AbstractJob
    {
        private static readonly int QUERY_PAGE_SIZE = 100; //每次查询数量
        private readonly IRefundOrderService _refundOrderService;
        private readonly ChannelOrderReissueService _channelOrderReissueService;

        public RefundOrderReissueJob(
            ILogger<RefundOrderReissueJob> logger,
            ICacheService cacheService,
            IRefundOrderService refundOrderService,
            ChannelOrderReissueService channelOrderReissueService)
            : base(logger, cacheService)
        {
            _refundOrderService = refundOrderService;
            _channelOrderReissueService = channelOrderReissueService;
        }

        protected override TimeSpan GetLockExpiry() => TimeSpan.FromMinutes(10);
        protected override TimeSpan GetMaxExecutionTime() => TimeSpan.FromMinutes(15);

        public override async Task Execute(IJobExecutionContext context)
        {
            await ExecuteLockTakeAsync(context.JobDetail.Key.ToString(), async () =>
            {
                int currentPageIndex = 1; //当前页码
                while (true)
                {
                    try
                    {
                        var dto = new RefundOrderQueryDto()
                        {
                            PageNumber = currentPageIndex,
                            PageSize = QUERY_PAGE_SIZE,
                            State = (byte)RefundOrderState.STATE_ING
                        };
                        var refundOrders = await _refundOrderService.GetPaginatedDataAsync(dto);

                        if (refundOrders == null || refundOrders.Items.Count == 0)
                        {
                            //本次查询无结果, 不再继续查询;
                            break;
                        }

                        foreach (var refundOrder in refundOrders.Items)
                        {
                            await _channelOrderReissueService.ProcessRefundOrderAsync(refundOrder);
                        }

                        //已经到达页码最大量，无需再次查询
                        if (refundOrders.TotalPages <= currentPageIndex)
                        {
                            break;
                        }
                        currentPageIndex++;
                    }
                    catch (Exception e)
                    {
                        //出现异常，直接退出，避免死循环。
                        _logger.LogError(e, "任务 [{JobKey}] 执行时发生异常", context.JobDetail.Key);
                        break;
                    }
                }
            });
        }
    }
}
