using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using Quartz;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    /// <summary>
    /// 分账补单定时任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class PayOrderDivisionRecordReissueJob : AbstractJob
    {
        private static readonly int QUERY_PAGE_SIZE = 100; //每次查询数量
        private readonly IChannelServiceFactory<IDivisionService> _divisionServiceFactory;
        private readonly IPayOrderService _payOrderService;
        private readonly IPayOrderDivisionRecordService _payOrderDivisionRecordService;
        private readonly ConfigContextQueryService _configContextQueryService;

        public PayOrderDivisionRecordReissueJob(
            ILogger<PayOrderDivisionRecordReissueJob> logger,
            ICacheService cacheService,
            IChannelServiceFactory<IDivisionService> divisionServiceFactory,
            IPayOrderService payOrderService,
            IPayOrderDivisionRecordService payOrderDivisionRecordService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, cacheService)
        {
            _divisionServiceFactory = divisionServiceFactory;
            _payOrderService = payOrderService;
            _payOrderDivisionRecordService = payOrderDivisionRecordService;
            _configContextQueryService = configContextQueryService;
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
                        //查询条件： 受理中的订单 & （ 订单创建时间 + 5分钟 >= 当前时间 ）
                        var dto = new PayOrderDivisionRecordQueryDto()
                        {
                            PageNumber = currentPageIndex,
                            PageSize = QUERY_PAGE_SIZE,
                            State = (byte)PayOrderDivisionRecordState.STATE_ACCEPT,
                            CreatedStart = DateTime.Now.AddMinutes(-5), // 当前时间 减去5分钟。
                        };
                        var paginatedResult = await _payOrderDivisionRecordService.DistinctBatchOrderIdListAsync(dto);

                        _logger.LogInformation("任务 [{JobKey}] 处理分账补单任务, 共计 {TotalCount} 条",
                            context.JobDetail.Key, paginatedResult.TotalCount);

                        if (paginatedResult == null || paginatedResult.Items.Count == 0)
                        {
                            //本次查询无结果, 不再继续查询;
                            break;
                        }

                        foreach (var batchRecord in paginatedResult.Items)
                        {
                            try
                            {
                                string batchOrderId = batchRecord.BatchOrderId;

                                // 通过 batchId 查询出列表（ 注意：  需要按照ID 排序！！！！ ）
                                List<PayOrderDivisionRecordDto> recordList = await _payOrderDivisionRecordService.GetByBatchOrderIdAsync(
                                    new PayOrderDivisionRecordQueryDto()
                                    {
                                        BatchOrderId = batchOrderId,
                                        State = (byte)PayOrderDivisionRecordState.STATE_ACCEPT,
                                    });

                                if (recordList == null || recordList.Count == 0)
                                {
                                    continue;
                                }

                                // 查询支付订单信息
                                PayOrderDto payOrder = await _payOrderService.GetByIdAsync(batchRecord.PayOrderId);
                                if (payOrder == null)
                                {
                                    _logger.LogError("任务 [{JobKey}] 支付订单记录不存在：{PayOrderId}",
                                        context.JobDetail.Key, batchRecord.PayOrderId);
                                    continue;
                                }

                                // 查询分账接口是否存在
                                IDivisionService divisionService = _divisionServiceFactory.GetService(payOrder.IfCode);
                                if (divisionService == null)
                                {
                                    _logger.LogError("任务 [{JobKey}] 查询分账接口不存在：{IfCode}",
                                        context.JobDetail.Key, payOrder.IfCode);
                                    continue;
                                }

                                MchAppConfigContext mchAppConfigContext = await _configContextQueryService.QueryMchInfoAndAppInfoAsync(
                                    payOrder.MchNo, payOrder.AppId);

                                // 调用渠道侧的查单接口
                                Dictionary<long, ChannelRetMsg> queryDivision = await divisionService.QueryDivisionAsync(
                                    payOrder, recordList, mchAppConfigContext);

                                // 处理查询结果
                                foreach (var record in recordList)
                                {
                                    ChannelRetMsg channelRetMsg = queryDivision.GetValueOrDefault(record.RecordId.Value);

                                    // 响应状态为分账成功或失败时，更新该记录状态
                                    if (channelRetMsg?.ChannelState == ChannelState.CONFIRM_SUCCESS ||
                                        channelRetMsg?.ChannelState == ChannelState.CONFIRM_FAIL)
                                    {
                                        byte state = (byte)(ChannelState.CONFIRM_SUCCESS == channelRetMsg.ChannelState
                                            ? PayOrderDivisionRecordState.STATE_SUCCESS
                                            : PayOrderDivisionRecordState.STATE_FAIL);

                                        // 更新记录状态
                                        await _payOrderDivisionRecordService.UpdateRecordSuccessOrFailBySingleItemAsync(
                                            record.RecordId.Value, state, channelRetMsg.ChannelErrMsg);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, "任务 [{JobKey}] 处理补单任务单条 [{BatchOrderId}] 异常",
                                    context.JobDetail.Key, batchRecord.BatchOrderId);
                            }
                        }

                        //已经到达页码最大量，无需再次查询
                        if (paginatedResult.TotalPages <= currentPageIndex)
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
