using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Components.Third.Services;
using Quartz;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    /// <summary>
    /// 补单定时任务(退款单)
    /// </summary>
    [DisallowConcurrentExecution]
    public class RefundOrderReissueJob : IJob
    {
        private static readonly int QUERY_PAGE_SIZE = 100; //每次查询数量
        private readonly ILogger<RefundOrderReissueJob> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RefundOrderReissueJob(ILogger<RefundOrderReissueJob> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var refundOrderService = scope.ServiceProvider.GetService<IRefundOrderService>();
                    var channelOrderReissueService = scope.ServiceProvider.GetService<ChannelOrderReissueService>();
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
                            var refundOrders = refundOrderService.GetPaginatedData(dto);

                            if (refundOrders == null || refundOrders.Count == 0)
                            {
                                //本次查询无结果, 不再继续查询;
                                break;
                            }

                            foreach (var refundOrder in refundOrders)
                            {
                                channelOrderReissueService.ProcessRefundOrder(refundOrder);
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
                            _logger.LogError(e, "error");
                            break;
                        }
                    }
                }
            });
        }
    }
}
