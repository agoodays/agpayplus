using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Components.Third.Services;
using Quartz;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    /// <summary>
    /// 转账补单定时任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class TransferOrderReissueJob : AbstractJob
    {
        private static readonly int QUERY_PAGE_SIZE = 100; //每次查询数量

        public TransferOrderReissueJob(ILogger<TransferOrderReissueJob> logger,
            IServiceScopeFactory serviceScopeFactory,
            ICacheService cacheService)
            : base(logger, serviceScopeFactory, cacheService)
        {
        }

        public override async Task Execute(IJobExecutionContext context)
        {
            await ExecuteLockTakeAsync(context.JobDetail.Key.ToString(), async () =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var transferOrderService = scope.ServiceProvider.GetService<ITransferOrderService>();
                    var transferOrderReissueService = scope.ServiceProvider.GetService<TransferOrderReissueService>();
                    int currentPageIndex = 1; //当前页码
                    while (true)
                    {
                        try
                        {
                            var dto = new TransferOrderQueryDto()
                            {
                                PageNumber = currentPageIndex,
                                PageSize = QUERY_PAGE_SIZE,
                                State = (byte)TransferOrderState.STATE_ING, // 转账中
                                CreatedStart = DateTime.Now.AddDays(-1), // 只查询一天内的转账单;
                            };
                            var transferOrders = await transferOrderService.GetPaginatedDataAsync(dto);

                            if (transferOrders == null || transferOrders.Count == 0)
                            {
                                //本次查询无结果, 不再继续查询;
                                break;
                            }

                            foreach (var transferOrder in transferOrders)
                            {
                                await transferOrderReissueService.ProcessOrderAsync(transferOrder);
                            }

                            //已经到达页码最大量，无需再次查询
                            if (transferOrders.TotalPages <= currentPageIndex)
                            {
                                break;
                            }
                            currentPageIndex++;
                        }
                        catch (Exception e)
                        {
                            //出现异常，直接退出，避免死循环。
                            _logger.LogError(e, "error");
                            break;
                        }
                    }
                }
            });
        }
    }
}
