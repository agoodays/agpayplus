using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Components.Third.Services;
using Quartz;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    /// <summary>
    /// 补单定时任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class PayOrderReissueJob : AbstractJob
    {
        private static readonly int QUERY_PAGE_SIZE = 100; //每次查询数量
        private readonly IPayOrderService _payOrderService;
        private readonly ChannelOrderReissueService _channelOrderReissueService;

        public PayOrderReissueJob(
            ILogger<PayOrderReissueJob> logger,
            ICacheService cacheService,
            IPayOrderService payOrderService,
            ChannelOrderReissueService channelOrderReissueService)
            : base(logger, cacheService)
        {
            _payOrderService = payOrderService;
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
                        //查询条件： 支付中的订单 & （ 订单创建时间 + 10分钟 >= 当前时间 ）
                        var dto = new PayOrderQueryDto()
                        {
                            PageNumber = currentPageIndex,
                            PageSize = QUERY_PAGE_SIZE,
                            State = (byte)PayOrderState.STATE_ING,
                            CreatedStart = DateTime.Now.AddMinutes(-10),// 当前时间 减去10分钟。
                        };
                        var payOrders = await _payOrderService.GetPaginatedDataAsync(dto);

                        if (payOrders == null || payOrders.Items.Count == 0)
                        {
                            //本次查询无结果, 不再继续查询;
                            break;
                        }

                        foreach (var payOrder in payOrders.Items)
                        {
                            await _channelOrderReissueService.ProcessPayOrderAsync(payOrder);
                        }

                        //已经到达页码最大量，无需再次查询
                        if (payOrders.TotalPages <= currentPageIndex)
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
