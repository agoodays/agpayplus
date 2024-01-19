using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Payment.Api.Channel;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using Quartz;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    public class PayOrderDivisionRecordReissueJob : IJob
    {
        private static readonly int QUERY_PAGE_SIZE = 100; //每次查询数量

        private readonly ILogger<TransferOrderReissueJob> logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly Func<string, IDivisionService> divisionServiceFactory;

        public PayOrderDivisionRecordReissueJob(ILogger<TransferOrderReissueJob> logger,
            IServiceScopeFactory serviceScopeFactory,
            Func<string, IDivisionService> divisionServiceFactory)
        {
            this.logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            this.divisionServiceFactory = divisionServiceFactory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var payOrderService = scope.ServiceProvider.GetService<IPayOrderService>();
                    var payOrderDivisionRecordService = scope.ServiceProvider.GetService<IPayOrderDivisionRecordService>();
                    var configContextQueryService = scope.ServiceProvider.GetService<ConfigContextQueryService>();
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
                            var pageRecordList = payOrderDivisionRecordService.DistinctBatchOrderIdList(dto);

                            logger.LogInformation($"处理分账补单任务, 共计{pageRecordList.TotalCount}条");

                            if (pageRecordList == null || pageRecordList.Count == 0)
                            {
                                //本次查询无结果, 不再继续查询;
                                break;
                            }

                            foreach (var batchRecord in pageRecordList)
                            {
                                try
                                {
                                    string batchOrderId = batchRecord.BatchOrderId;

                                    // 通过 batchId 查询出列表（ 注意：  需要按照ID 排序！！！！ ）
                                    List<PayOrderDivisionRecordDto> recordList = payOrderDivisionRecordService.GetByBatchOrderId(new PayOrderDivisionRecordQueryDto()
                                    {
                                        BatchOrderId = batchOrderId,
                                        State = (byte)PayOrderDivisionRecordState.STATE_ACCEPT,
                                    });

                                    if (recordList == null || recordList.Count == 0)
                                    {
                                        continue;
                                    }

                                    // 查询支付订单信息
                                    PayOrderDto payOrder = payOrderService.GetById(batchRecord.PayOrderId);
                                    if (payOrder == null)
                                    {
                                        logger.LogError($"支付订单记录不存在：{batchRecord.PayOrderId}");
                                        continue;
                                    }
                                    // 查询分账接口是否存在
                                    IDivisionService divisionService = divisionServiceFactory(payOrder.IfCode);

                                    if (divisionService == null)
                                    {
                                        logger.LogError($"查询分账接口不存在：{payOrder.IfCode}");
                                        continue;
                                    }
                                    MchAppConfigContext mchAppConfigContext = configContextQueryService.QueryMchInfoAndAppInfo(payOrder.MchNo, payOrder.AppId);
                                    // 调用渠道侧的查单接口：   注意：  渠道内需保证：
                                    // 1. 返回的条目 必须全部来自recordList， 可以少于recordList但是不得高于 recordList 数量；
                                    // 2. recordList 的记录可能与接口返回的数量不一致，  接口实现不要求对条目数量做验证；
                                    // 3. 接口查询的记录若recordList 不存在， 忽略即可。  （  例如两条相同的accNo, 则可能仅匹配一条。 那么另外一条将在下一次循环中处理。  ）
                                    // 4. 仅明确状态的再返回，若不明确则不需返回；
                                    Dictionary<long, ChannelRetMsg> queryDivision = divisionService.QueryDivision(payOrder, recordList, mchAppConfigContext);

                                    //// 处理查询结果
                                    foreach (var record in recordList)
                                    {
                                        ChannelRetMsg channelRetMsg = queryDivision.GetValueOrDefault(record.RecordId.Value);

                                        // 响应状态为分账成功或失败时，更新该记录状态
                                        if (channelRetMsg.ChannelState == ChannelState.CONFIRM_SUCCESS || channelRetMsg.ChannelState == ChannelState.CONFIRM_FAIL)
                                        {
                                            byte state = (byte)(ChannelState.CONFIRM_SUCCESS == channelRetMsg.ChannelState ? PayOrderDivisionRecordState.STATE_SUCCESS : PayOrderDivisionRecordState.STATE_FAIL);
                                            // 更新记录状态
                                            payOrderDivisionRecordService.UpdateRecordSuccessOrFailBySingleItem(record.RecordId.Value, state, channelRetMsg.ChannelErrMsg);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    logger.LogError(e, $"处理补单任务单条[{batchRecord.BatchOrderId}]异常");
                                }
                            }

                            //已经到达页码最大量，无需再次查询
                            if (pageRecordList.TotalPages <= currentPageIndex)
                            {
                                break;
                            }
                            currentPageIndex++;
                        }
                        catch (Exception e)
                        {
                            //出现异常，直接退出，避免死循环。
                            logger.LogError(e, "error");
                            break;
                        }
                    }
                }
            });
        }
    }
}
