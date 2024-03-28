using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Payment.Api.Services;
using Quartz;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    /// <summary>
    /// 补单定时任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class PayOrderReissueJob : IJob
    {
        private static readonly int QUERY_PAGE_SIZE = 100; //每次查询数量
        private readonly ILogger<PayOrderReissueJob> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PayOrderReissueJob(ILogger<PayOrderReissueJob> logger,
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
                    var payOrderService = scope.ServiceProvider.GetService<IPayOrderService>();
                    var channelOrderReissueService = scope.ServiceProvider.GetService<ChannelOrderReissueService>();

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
                            var payOrders = payOrderService.GetPaginatedData(dto);

                            if (payOrders == null || payOrders.Count == 0)
                            {
                                //本次查询无结果, 不再继续查询;
                                break;
                            }

                            foreach (var payOrder in payOrders)
                            {
                                channelOrderReissueService.ProcessPayOrder(payOrder);
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
                            _logger.LogError(e, "error");
                            break;
                        }
                    }
                }
            });
        }
    }
}
