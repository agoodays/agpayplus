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
        private static int QUERY_PAGE_SIZE = 100; //每次查询数量
        private readonly ILogger<PayOrderExpiredJob> logger;
        private readonly IPayOrderService payOrderService;
        private readonly ChannelOrderReissueService channelOrderReissueService;

        public PayOrderReissueJob(ILogger<PayOrderExpiredJob> logger, 
            IPayOrderService payOrderService, 
            ChannelOrderReissueService channelOrderReissueService)
        {
            this.logger = logger;
            this.payOrderService = payOrderService;
            this.channelOrderReissueService = channelOrderReissueService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                int currentPageIndex = 1; //当前页码
                while (true)
                {
                    try
                    {
                        var dto = new PayOrderQueryDto()
                        {
                            PageNumber = currentPageIndex,
                            PageSize = QUERY_PAGE_SIZE,
                            State = (byte)PayOrderState.STATE_ING,
                            CreatedStart = DateTime.Now.AddMinutes(-10),
                        };
                        var payOrders = payOrderService.GetPaginatedData(dto);

                        if (payOrders == null || !payOrders.Any())
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
                        logger.LogError("error", e);
                        break;
                    }
                }
            });
        }
    }
}
