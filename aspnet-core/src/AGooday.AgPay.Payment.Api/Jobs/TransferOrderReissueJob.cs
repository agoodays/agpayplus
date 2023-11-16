using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Payment.Api.Services;
using Quartz;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    /// <summary>
    /// 转账补单定时任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class TransferOrderReissueJob : IJob
    {
        private static readonly int QUERY_PAGE_SIZE = 100; //每次查询数量
        private readonly ILogger<TransferOrderReissueJob> logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TransferOrderReissueJob(ILogger<TransferOrderReissueJob> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            this.logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
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
                            var transferOrders = transferOrderService.GetPaginatedData(dto);

                            if (transferOrders == null || !transferOrders.Any())
                            {
                                //本次查询无结果, 不再继续查询;
                                break;
                            }

                            foreach (var transferOrder in transferOrders)
                            {
                                transferOrderReissueService.ProcessOrder(transferOrder);
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
                            logger.LogError("error", e);
                            break;
                        }
                    }
                }
            });
        }
    }
}
