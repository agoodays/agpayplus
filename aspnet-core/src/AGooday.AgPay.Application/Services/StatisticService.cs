using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Extensions;
using AutoMapper;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 数据统计 服务实现类
    /// </summary>
    public class StatisticService : IStatisticService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayOrderRepository _payOrderRepository;
        private readonly IRefundOrderRepository _refundOrderRepository;
        private readonly IMchInfoRepository _mchInfoRepository;
        private readonly IMchStoreRepository _mchStoreRepository;
        private readonly IAgentInfoRepository _agentInfoRepository;
        private readonly IIsvInfoRepository _isvInfoRepository;
        private readonly IPayWayRepository _payWayRepository;
        private readonly IPayInterfaceDefineRepository _payInterfaceDefineRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public StatisticService(IMapper mapper, IMediatorHandler bus,
            IPayOrderRepository payOrderRepository,
            IRefundOrderRepository refundOrderRepository,
            IMchInfoRepository mchInfoRepository,
            IMchStoreRepository mchStoreRepository,
            IAgentInfoRepository agentInfoRepository,
            IIsvInfoRepository isvInfoRepository,
            IPayWayRepository payWayRepository,
            IPayInterfaceDefineRepository payInterfaceDefineRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _payOrderRepository = payOrderRepository;
            _refundOrderRepository = refundOrderRepository;
            _mchInfoRepository = mchInfoRepository;
            _mchStoreRepository = mchStoreRepository;
            _agentInfoRepository = agentInfoRepository;
            _isvInfoRepository = isvInfoRepository;
            _payWayRepository = payWayRepository;
            _payInterfaceDefineRepository = payInterfaceDefineRepository;
        }

        public async Task<JObject> TotalAsync(string agentNo, StatisticQueryDto dto)
        {
            var agents = await _agentInfoRepository.GetAllOrSubAgentsAsync(agentNo);
            var agentNos = agentNo == null ? null : agents.Select(s => s.AgentNo);
            var (payOrders, refundOrders) = SelectOrderCount(dto, agentNos: agentNos);
            var allAmount = await payOrders.SafeSumAsync(s => s.Amount);
            var allCount = await payOrders.SafeCountAsync();
            // 成交金额: 支付成功的订单金额，包含部分退款及全额退款的订单
            var pay = payOrders.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));
            var payAmount = await pay.SafeSumAsync(s => s.Amount);
            var payCount = await pay.SafeCountAsync();
            var fee = await pay.SafeSumAsync(s => s.MchOrderFeeAmount);
            var refund = refundOrders.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS));
            var refundAmount = await refund.SafeSumAsync(s => s.RefundAmount);
            var refundCount = await refund.SafeCountAsync();
            var refundFeeAmount = await refund.SafeSumAsync(s => s.RefundFeeAmount);
            JObject result = new JObject();
            result.Add("allAmount", Decimal.Round(allAmount / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("allCount", allCount);
            result.Add("payAmount", Decimal.Round(payAmount / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("payCount", payCount);
            result.Add("fee", Decimal.Round(fee / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("refundAmount", Decimal.Round(refundAmount / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("refundCount", refundCount);
            result.Add("refundFeeAmount", Decimal.Round(refundFeeAmount / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("round", Math.Round(allCount > 0 ? payCount / Convert.ToDecimal(allCount) : 0M, 2, MidpointRounding.AwayFromZero));
            return result;
        }

        public async Task<PaginatedResult<StatisticResultDto>> StatisticsAsync(string agentNo, StatisticQueryDto dto)
        {
            return dto.Method switch
            {
                StatisticCS.Method.TRANSACTION => await TransactionStatisticsAsync(agentNo, dto),
                StatisticCS.Method.MCH => await MchStatisticsAsync(agentNo, dto),
                StatisticCS.Method.STORE => await StoreStatisticsAsync(dto),
                StatisticCS.Method.WAY_CODE => await WayCodeStatisticsAsync(dto),
                StatisticCS.Method.WAY_TYPE => await WayTypeStatisticsAsync(dto),
                StatisticCS.Method.AGENT => await AgentStatisticsAsync(agentNo, dto),
                StatisticCS.Method.ISV => await IsvStatisticsAsync(dto),
                StatisticCS.Method.CHANNEL => await ChannelStatisticsAsync(dto),
                _ => throw new NotImplementedException()
            };
        }

        private async Task<PaginatedResult<StatisticResultDto>> TransactionStatisticsAsync(string agentNo, StatisticQueryDto dto)
        {
            IEnumerable<AgentInfo> agents = await _agentInfoRepository.GetSubAgentsAsync(agentNo);
            var agentNos = agents?.Select(s => s.AgentNo);
            var (payOrders, refundOrders) = SelectOrderCount(dto, agentNos: agentNos);

            var payRecords = payOrders
                .GroupBy(g => g.CreatedAt.Value.ToString(dto.Format))
                .Select(s => new StatisticResultDto()
                {
                    GroupDate = s.Key,
                    AllAmount = s.Sum(s => s.Amount),
                    AllCount = s.Count(),
                    PayAmount = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)).Sum(s => s.Amount),
                    PayCount = s.Count(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)),
                    Fee = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)).Sum(s => s.MchOrderFeeAmount),
                });

            var refundRecords = refundOrders.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS)).AsEnumerable()
                .GroupBy(g => g.CreatedAt.Value.ToString(dto.Format))
                .Select(s => new StatisticResultDto()
                {
                    GroupDate = s.Key,
                    RefundAmount = s.Sum(s => s.RefundAmount),
                    RefundCount = s.Count(),
                    RefundFee = s.Sum(s => s.RefundFeeAmount),
                });

            var records = payRecords
                .Join(refundRecords, p => p.GroupDate, r => r.GroupDate, (p, r) => new { p, r })
                .Select(s => new StatisticResultDto()
                {
                    GroupDate = s.p.GroupDate,
                    AllAmount = s.p.AllAmount,
                    AllCount = s.p.AllCount,
                    PayAmount = s.p.PayAmount,
                    PayCount = s.p.PayCount,
                    Round = Math.Round(s.p.AllCount > 0 ? s.p.PayCount / Convert.ToDecimal(s.p.AllCount) : 0M, 2, MidpointRounding.AwayFromZero),
                    Fee = s.p.Fee,
                    RefundAmount = s.r.RefundAmount,
                    RefundCount = s.r.RefundCount,
                    RefundFee = s.r.RefundFee,
                })
                .OrderByDescending(o => o.GroupDate);

            return await records.ToPaginatedResultAsync(dto.PageNumber, dto.PageSize);
        }

        private async Task<PaginatedResult<StatisticResultDto>> MchStatisticsAsync(string agentNo, StatisticQueryDto dto)
        {
            var agents = await _agentInfoRepository.GetAllOrSubAgentsAsync(agentNo);
            var agentNos = agentNo == null ? null : agents.Select(s => s.AgentNo);

            var mchs = _mchInfoRepository.GetAllAsNoTracking()
                .WhereIfNotEmpty(dto.MchNo, w => w.MchNo.Equals(dto.MchNo))
                .WhereIfNotEmpty(dto.AgentNo, w => w.AgentNo.Equals(dto.AgentNo))
                .WhereIfNotEmpty(dto.IsvNo, w => w.IsvNo.Equals(dto.IsvNo))
                .WhereIfNotEmpty(dto.MchName, w => w.MchName.Contains(dto.MchName) || w.MchShortName.Contains(dto.MchName))
                .WhereIfNotNull(agentNos, w => agentNos.Contains(w.AgentNo))
                .OrderByDescending(o => o.CreatedAt);
            var mchInfos = await mchs.ToPaginatedResultAsync<MchInfo, MchInfoDto>(_mapper, dto.PageNumber, dto.PageSize);
            var mchNos = mchInfos.Items.Select(s => s.MchNo);
            var (payOrders, refundOrders) = SelectOrderCount(dto, agentNos: agentNos, mchNos: mchNos);

            var payRecords = await payOrders
                .GroupBy(g => g.MchNo)
                .Select(s => new StatisticResultDto()
                {
                    MchNo = s.Key,
                    AllAmount = s.Sum(s => s.Amount),
                    AllCount = s.Count(),
                    PayAmount = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)).Sum(s => s.Amount),
                    PayCount = s.Count(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)),
                    Fee = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)).Sum(s => s.MchOrderFeeAmount),
                }).ToListAsync();

            var refundRecords = await refundOrders.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS))
                .GroupBy(g => g.MchNo)
                .Select(s => new StatisticResultDto()
                {
                    MchNo = s.Key,
                    RefundAmount = s.Sum(s => s.RefundAmount),
                    RefundCount = s.Count(),
                    RefundFee = s.Sum(s => s.RefundFeeAmount),
                }).ToListAsync();

            var records = mchInfos.Items
                .Select(s =>
                {
                    var pay = payRecords?.Where(w => s.MchNo.Equals(w.MchNo))?.FirstOrDefault();
                    var refund = refundRecords?.Where(w => s.MchNo.Equals(w.MchNo))?.FirstOrDefault();

                    return new StatisticResultDto()
                    {
                        MchNo = s.MchNo,
                        MchName = s.MchName,
                        AllAmount = pay?.AllAmount ?? 0,
                        AllCount = pay?.AllCount ?? 0,
                        PayAmount = pay?.PayAmount ?? 0,
                        PayCount = pay?.PayCount ?? 0,
                        Round = Math.Round((pay?.AllCount ?? 0) > 0 ? (decimal)pay?.PayCount / (pay?.AllCount ?? 0) : 0M, 2, MidpointRounding.AwayFromZero),
                        Fee = pay?.Fee ?? 0,
                        RefundAmount = refund?.RefundAmount ?? 0,
                        RefundCount = refund?.RefundCount ?? 0,
                        RefundFee = refund?.RefundFee ?? 0
                    };
                });
            var result = new PaginatedResult<StatisticResultDto>(records?.ToList(), mchInfos.TotalCount, dto.PageNumber, dto.PageSize);
            return result;
        }

        private async Task<PaginatedResult<StatisticResultDto>> StoreStatisticsAsync(StatisticQueryDto dto)
        {
            var stores = _mchStoreRepository.GetAllAsNoTracking()
                .WhereIfNotNull(dto.StoreId, w => w.StoreId.Equals(dto.StoreId))
                .WhereIfNotEmpty(dto.StoreName, w => w.StoreName.Contains(dto.StoreName))
                .WhereIfNotEmpty(dto.MchNo, w => w.MchNo.Contains(dto.MchNo))
                .OrderByDescending(o => o.CreatedAt);
            var mchStores = await stores.ToPaginatedResultAsync<MchStore, MchStoreDto>(_mapper, dto.PageNumber, dto.PageSize);
            var storeIds = mchStores.Items.Select(s => s.StoreId);
            var (payOrders, refundOrders) = SelectOrderCount(dto, storeIds: storeIds);

            var payRecords = await payOrders
                .GroupBy(g => g.StoreId)
                .Select(s => new StatisticResultDto()
                {
                    StoreId = s.Key,
                    AllAmount = s.Sum(s => s.Amount),
                    AllCount = s.Count(),
                    PayAmount = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)).Sum(s => s.Amount),
                    PayCount = s.Count(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)),
                    Fee = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)).Sum(s => s.MchOrderFeeAmount),
                }).ToListAsync();

            var refundRecords = await refundOrders.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS))
                .GroupBy(g => g.StoreId)
                .Select(s => new StatisticResultDto()
                {
                    StoreId = s.Key,
                    RefundAmount = s.Sum(s => s.RefundAmount),
                    RefundCount = s.Count(),
                    RefundFee = s.Sum(s => s.RefundFeeAmount),
                }).ToListAsync();

            var records = mchStores.Items
                .Select(s =>
                {
                    var pay = payRecords?.Where(w => s.StoreId.Equals(w.StoreId))?.FirstOrDefault();
                    var refund = refundRecords?.Where(w => s.StoreId.Equals(w.StoreId))?.FirstOrDefault();

                    return new StatisticResultDto()
                    {
                        StoreId = s.StoreId,
                        StoreName = s.StoreName,
                        AllAmount = pay?.AllAmount ?? 0,
                        AllCount = pay?.AllCount ?? 0,
                        PayAmount = pay?.PayAmount ?? 0,
                        PayCount = pay?.PayCount ?? 0,
                        Round = Math.Round((pay?.AllCount ?? 0) > 0 ? (decimal)pay?.PayCount / (pay?.AllCount ?? 0) : 0M, 2, MidpointRounding.AwayFromZero),
                        Fee = pay?.Fee ?? 0,
                        RefundAmount = refund?.RefundAmount ?? 0,
                        RefundCount = refund?.RefundCount ?? 0,
                        RefundFee = refund?.RefundFee ?? 0
                    };
                });
            var result = new PaginatedResult<StatisticResultDto>(records?.ToList(), mchStores.TotalCount, dto.PageNumber, dto.PageSize);
            return result;
        }

        private async Task<PaginatedResult<StatisticResultDto>> WayCodeStatisticsAsync(StatisticQueryDto dto)
        {
            var ways = _payWayRepository.GetAllAsNoTracking()
                .WhereIfNotEmpty(dto.WayCode, w => w.WayCode.Equals(dto.WayCode))
                .WhereIfNotEmpty(dto.WayName, w => w.WayName.Contains(dto.WayName))
                .OrderByDescending(o => o.WayCode).ThenByDescending(o => o.CreatedAt);
            var payWays = await ways.ToPaginatedResultAsync<PayWay, PayWayDto>(_mapper, dto.PageNumber, dto.PageSize);
            var wayCodes = payWays.Items.Select(s => s.WayCode);
            var (payOrders, refundOrders) = SelectOrderCount(dto, wayCodes: wayCodes);

            var payRecords = await payOrders
                .GroupBy(g => g.WayCode)
                .Select(s => new StatisticResultDto()
                {
                    WayCode = s.Key,
                    AllAmount = s.Sum(s => s.Amount),
                    AllCount = s.Count(),
                    PayAmount = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)).Sum(s => s.Amount),
                    PayCount = s.Count(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)),
                    Fee = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)).Sum(s => s.MchOrderFeeAmount),
                }).ToListAsync();

            var refundRecords = await refundOrders.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS))
                .GroupBy(g => g.WayCode)
                .Select(s => new StatisticResultDto()
                {
                    WayCode = s.Key,
                    RefundAmount = s.Sum(s => s.RefundAmount),
                    RefundCount = s.Count(),
                    RefundFee = s.Sum(s => s.RefundFeeAmount),
                }).ToListAsync();

            var records = payWays.Items
                .Select(s =>
                {
                    var pay = payRecords?.Where(w => s.WayCode.Equals(w.WayCode))?.FirstOrDefault();
                    var refund = refundRecords?.Where(w => s.WayCode.Equals(w.WayCode))?.FirstOrDefault();

                    return new StatisticResultDto()
                    {
                        WayCode = s.WayCode,
                        WayName = s.WayName,
                        AllAmount = pay?.AllAmount ?? 0,
                        AllCount = pay?.AllCount ?? 0,
                        PayAmount = pay?.PayAmount ?? 0,
                        PayCount = pay?.PayCount ?? 0,
                        Round = Math.Round((pay?.AllCount ?? 0) > 0 ? (decimal)pay?.PayCount / (pay?.AllCount ?? 0) : 0M, 2, MidpointRounding.AwayFromZero),
                        Fee = pay?.Fee ?? 0,
                        RefundAmount = refund?.RefundAmount ?? 0,
                        RefundCount = refund?.RefundCount ?? 0,
                        RefundFee = refund?.RefundFee ?? 0
                    };
                });
            var result = new PaginatedResult<StatisticResultDto>(records?.ToList(), payWays.TotalCount, dto.PageNumber, dto.PageSize);
            return result;
        }

        private async Task<PaginatedResult<StatisticResultDto>> WayTypeStatisticsAsync(StatisticQueryDto dto)
        {
            var (payOrders, refundOrders) = SelectOrderCount(dto);

            var payRecords = await payOrders
                .GroupBy(g => g.WayType)
                .Select(s => new StatisticResultDto()
                {
                    WayType = s.Key,
                    AllAmount = s.Sum(s => s.Amount),
                    AllCount = s.Count(),
                    PayAmount = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)).Sum(s => s.Amount),
                    PayCount = s.Count(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)),
                    Fee = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)).Sum(s => s.MchOrderFeeAmount),
                }).ToListAsync();

            var refundRecords = await refundOrders.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS))
                .GroupBy(g => g.WayType)
                .Select(s => new StatisticResultDto()
                {
                    WayType = s.Key,
                    RefundAmount = s.Sum(s => s.RefundAmount),
                    RefundCount = s.Count(),
                    RefundFee = s.Sum(s => s.RefundFeeAmount),
                }).ToListAsync();

            var records = Enum.GetValues(typeof(PayWayType)).Cast<PayWayType>()
                .Where(w => (string.IsNullOrWhiteSpace(dto.WayType) || w.ToString().Equals(dto.WayType)))
                .Select(s =>
                {
                    var pay = payRecords?.Where(w => s.ToString().Equals(w.WayType))?.FirstOrDefault();
                    var refund = refundRecords?.Where(w => s.ToString().Equals(w.WayType))?.FirstOrDefault();

                    return new StatisticResultDto()
                    {
                        WayType = s.ToString(),
                        WayTypeName = s.GetDescriptionOrDefault("未知"),
                        AllAmount = pay?.AllAmount ?? 0,
                        AllCount = pay?.AllCount ?? 0,
                        PayAmount = pay?.PayAmount ?? 0,
                        PayCount = pay?.PayCount ?? 0,
                        Round = Math.Round((pay?.AllCount ?? 0) > 0 ? (decimal)pay?.PayCount / (pay?.AllCount ?? 0) : 0M, 2, MidpointRounding.AwayFromZero),
                        Fee = pay?.Fee ?? 0,
                        RefundAmount = refund?.RefundAmount ?? 0,
                        RefundCount = refund?.RefundCount ?? 0,
                        RefundFee = refund?.RefundFee ?? 0
                    };
                });
            var result = PaginatedResult<StatisticResultDto>.Create(records, dto.PageNumber, dto.PageSize);
            return result;
        }

        private async Task<PaginatedResult<StatisticResultDto>> AgentStatisticsAsync(string agentNo, StatisticQueryDto dto)
        {
            var agents = (await _agentInfoRepository.GetAllOrSubAgentsAsync(agentNo))
                .Where(w => (string.IsNullOrWhiteSpace(dto.AgentNo) || w.IsvNo.Equals(dto.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.AgentName) || w.AgentName.Contains(dto.AgentName) || w.AgentShortName.Contains(dto.IsvName))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                ).OrderByDescending(o => o.CreatedAt);
            var agentInfos = PaginatedResult<AgentInfo>.Create(agents, dto.PageNumber, dto.PageSize);
            var agentNos = agentNo == null ? null : agentInfos.Items.Select(s => s.AgentNo);
            var (payOrders, refundOrders) = SelectOrderCount(dto, agentNos: agentNos);

            var payRecords = await payOrders
                .GroupBy(g => g.AgentNo)
                .Select(s => new StatisticResultDto()
                {
                    AgentNo = s.Key,
                    AllAmount = s.Sum(s => s.Amount),
                    AllCount = s.Count(),
                    PayAmount = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)).Sum(s => s.Amount),
                    PayCount = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)).Count(),
                    Fee = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)).Sum(s => s.MchOrderFeeAmount),
                }).ToListAsync();

            var refundRecords = await refundOrders.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS))
                .GroupBy(g => g.AgentNo)
                .Select(s => new StatisticResultDto()
                {
                    AgentNo = s.Key,
                    RefundAmount = s.Sum(s => s.RefundAmount),
                    RefundCount = s.Count(),
                    RefundFee = s.Sum(s => s.RefundFeeAmount),
                }).ToListAsync();

            var records = agentInfos.Items
                .Select(s =>
                {
                    var pay = payRecords?.Where(w => s.AgentNo.Equals(w.AgentNo))?.FirstOrDefault();
                    var refund = refundRecords?.Where(w => s.AgentNo.Equals(w.AgentNo))?.FirstOrDefault();

                    return new StatisticResultDto()
                    {
                        AgentNo = s.AgentNo,
                        AgentName = s.AgentName,
                        AllAmount = pay?.AllAmount ?? 0,
                        AllCount = pay?.AllCount ?? 0,
                        PayAmount = pay?.PayAmount ?? 0,
                        PayCount = pay?.PayCount ?? 0,
                        Round = Math.Round((pay?.AllCount ?? 0) > 0 ? (decimal)pay?.PayCount / (pay?.AllCount ?? 0) : 0M, 2, MidpointRounding.AwayFromZero),
                        Fee = pay?.Fee ?? 0,
                        RefundAmount = refund?.RefundAmount ?? 0,
                        RefundCount = refund?.RefundCount ?? 0,
                        RefundFee = refund?.RefundFee ?? 0
                    };
                });
            var result = new PaginatedResult<StatisticResultDto>(records?.ToList(), agentInfos.TotalCount, dto.PageNumber, dto.PageSize);
            // var result = query.ToPaginatedResultAsync<StatisticResultDto>.Create(records, dto.PageNumber, dto.PageSize);
            return result;
        }

        private async Task<PaginatedResult<StatisticResultDto>> IsvStatisticsAsync(StatisticQueryDto dto)
        {
            var isvs = _isvInfoRepository.GetAllAsNoTracking()
                .WhereIfNotEmpty(dto.IsvNo, w => w.IsvNo.Equals(dto.IsvNo))
                .WhereIfNotEmpty(dto.IsvName, w => w.IsvName.Contains(dto.IsvName) || w.IsvShortName.Contains(dto.IsvName))
                .OrderByDescending(o => o.CreatedAt);
            var isvInfos = await isvs.ToPaginatedResultAsync<IsvInfo, IsvInfoDto>(_mapper, dto.PageNumber, dto.PageSize);
            var isvNos = isvInfos.Items.Select(s => s.IsvNo);
            var (payOrders, refundOrders) = SelectOrderCount(dto, isvNos: isvNos);

            var payRecords = await payOrders
                .GroupBy(g => g.IsvNo)
                .Select(s => new StatisticResultDto()
                {
                    IsvNo = s.Key,
                    AllAmount = s.Sum(s => s.Amount),
                    AllCount = s.Count(),
                    PayAmount = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)).Sum(s => s.Amount),
                    PayCount = s.Count(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)),
                    Fee = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)).Sum(s => s.MchOrderFeeAmount),
                }).ToListAsync();

            var refundRecords = await refundOrders.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS))
                .GroupBy(g => g.IsvNo)
                .Select(s => new StatisticResultDto()
                {
                    IsvNo = s.Key,
                    RefundAmount = s.Sum(s => s.RefundAmount),
                    RefundCount = s.Count(),
                    RefundFee = s.Sum(s => s.RefundFeeAmount),
                }).ToListAsync();

            var records = isvInfos.Items
                .Select(s =>
                {
                    var pay = payRecords?.Where(w => s.IsvNo.Equals(w.IsvNo))?.FirstOrDefault();
                    var refund = refundRecords?.Where(w => s.IsvNo.Equals(w.IsvNo))?.FirstOrDefault();

                    return new StatisticResultDto()
                    {
                        IsvNo = s.IsvNo,
                        IsvName = s.IsvName,
                        AllAmount = pay?.AllAmount ?? 0,
                        AllCount = pay?.AllCount ?? 0,
                        PayAmount = pay?.PayAmount ?? 0,
                        PayCount = pay?.PayCount ?? 0,
                        Round = Math.Round((pay?.AllCount ?? 0) > 0 ? (decimal)pay?.PayCount / (pay?.AllCount ?? 0) : 0M, 2, MidpointRounding.AwayFromZero),
                        Fee = pay?.Fee ?? 0,
                        RefundAmount = refund?.RefundAmount ?? 0,
                        RefundCount = refund?.RefundCount ?? 0,
                        RefundFee = refund?.RefundFee ?? 0
                    };
                });
            var result = new PaginatedResult<StatisticResultDto>(records?.ToList(), isvInfos.TotalCount, dto.PageNumber, dto.PageSize);
            return result;
        }

        private async Task<PaginatedResult<StatisticResultDto>> ChannelStatisticsAsync(StatisticQueryDto dto)
        {
            var payIfDefines = _payInterfaceDefineRepository.GetAllAsNoTracking()
                .WhereIfNotEmpty(dto.IfCode, w => w.IfCode.Equals(dto.IfCode))
                .WhereIfNotEmpty(dto.IsvName, w => w.IfName.Equals(dto.IfName))
                .OrderByDescending(o => o.CreatedAt);
            var payInterfaceDefines = await payIfDefines.ToPaginatedResultAsync<PayInterfaceDefine, PayInterfaceDefineDto>(_mapper, dto.PageNumber, dto.PageSize);

            var (payOrders, refundOrders) = SelectOrderCount(dto);

            var payRecords = await payOrders
                .GroupBy(g => g.IfCode)
                .Select(s => new StatisticResultDto()
                {
                    IfCode = s.Key,
                    AllAmount = s.Sum(s => s.Amount),
                    AllCount = s.Count(),
                    PayAmount = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)).Sum(s => s.Amount),
                    PayCount = s.Count(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)),
                    Fee = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)).Sum(s => s.MchOrderFeeAmount),
                }).ToListAsync();

            var refundRecords = await refundOrders.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS))
                .GroupBy(g => g.IfCode)
                .Select(s => new StatisticResultDto()
                {
                    IfCode = s.Key,
                    RefundAmount = s.Sum(s => s.RefundAmount),
                    RefundCount = s.Count(),
                    RefundFee = s.Sum(s => s.RefundFeeAmount),
                }).ToListAsync();

            var records = payInterfaceDefines.Items
                .Select(s =>
                {
                    var pay = payRecords?.Where(w => s.IfCode.Equals(w.IfCode))?.FirstOrDefault();
                    var refund = refundRecords?.Where(w => s.IfCode.Equals(w.IfCode))?.FirstOrDefault();

                    return new StatisticResultDto()
                    {
                        IfCode = s.IfCode,
                        IfName = s.IfName,
                        AllAmount = pay?.AllAmount ?? 0,
                        AllCount = pay?.AllCount ?? 0,
                        PayAmount = pay?.PayAmount ?? 0,
                        PayCount = pay?.PayCount ?? 0,
                        Round = Math.Round((pay?.AllCount ?? 0) > 0 ? (decimal)pay?.PayCount / (pay?.AllCount ?? 0) : 0M, 2, MidpointRounding.AwayFromZero),
                        Fee = pay?.Fee ?? 0,
                        RefundAmount = refund?.RefundAmount ?? 0,
                        RefundCount = refund?.RefundCount ?? 0,
                        RefundFee = refund?.RefundFee ?? 0
                    };
                });
            var result = new PaginatedResult<StatisticResultDto>(records?.ToList(), payInterfaceDefines.TotalCount, dto.PageNumber, dto.PageSize);
            return result;
        }

        private (IQueryable<PayOrder> payOrders, IQueryable<RefundOrder> refundOrders) SelectOrderCount(StatisticQueryDto dto, IEnumerable<string> isvNos = null, IEnumerable<string> agentNos = null, IEnumerable<string> mchNos = null, IEnumerable<long?> storeIds = null, IEnumerable<string> wayCodes = null)
        {
            var payOrders = _payOrderRepository.GetAllAsNoTracking()
                .WhereIfNotNull(mchNos, w => mchNos.Contains(w.MchNo))
                .WhereIfNotEmpty(dto.MchNo, w => w.MchNo.Equals(dto.MchNo))
                .WhereIfNotEmpty(dto.MchName, w => w.MchName.Equals(dto.MchName))
                .WhereIfNotNull(agentNos, w => agentNos.Contains(w.AgentNo))
                .WhereIfNotEmpty(dto.AgentNo, w => w.AgentNo.Equals(dto.AgentNo))
                .WhereIfNotEmpty(dto.AgentName, w => w.AgentName.Equals(dto.AgentName))
                .WhereIfNotNull(isvNos, w => isvNos.Contains(w.IsvNo))
                .WhereIfNotEmpty(dto.IsvNo, w => w.IsvNo.Equals(dto.IsvNo))
                .WhereIfNotNull(storeIds, w => storeIds.Contains(w.StoreId))
                .WhereIfNotEmpty(dto.IsvName, w => w.IsvName.Equals(dto.IsvName))
                .WhereIfNotNull(dto.StoreId, w => w.StoreId.Equals(dto.StoreId))
                .WhereIfNotEmpty(dto.StoreName, w => w.StoreName.Equals(dto.StoreName))
                .WhereIfNotNull(wayCodes, w => wayCodes.Contains(w.WayCode))
                .WhereIfNotEmpty(dto.WayCode, w => w.WayCode.Equals(dto.WayCode))
                .WhereIfNotEmpty(dto.WayType, w => w.WayType.Equals(dto.WayType))
                .WhereIfNotNull(dto.CreatedStart, w => w.CreatedAt >= dto.CreatedStart)
                .WhereIfNotNull(dto.CreatedEnd, w => w.CreatedAt <= dto.CreatedEnd);

            var refundOrders = _refundOrderRepository.GetAllAsNoTracking()
                .WhereIfNotNull(mchNos, w => mchNos.Contains(w.MchNo))
                .WhereIfNotEmpty(dto.MchNo, w => w.MchNo.Equals(dto.MchNo))
                .WhereIfNotEmpty(dto.MchName, w => w.MchName.Equals(dto.MchName))
                .WhereIfNotNull(agentNos, w => agentNos.Contains(w.AgentNo))
                .WhereIfNotEmpty(dto.AgentNo, w => w.AgentNo.Equals(dto.AgentNo))
                .WhereIfNotEmpty(dto.AgentName, w => w.AgentName.Equals(dto.AgentName))
                .WhereIfNotNull(isvNos, w => isvNos.Contains(w.IsvNo))
                .WhereIfNotEmpty(dto.IsvNo, w => w.IsvNo.Equals(dto.IsvNo))
                .WhereIfNotEmpty(dto.IsvName, w => w.IsvName.Equals(dto.IsvName))
                .WhereIfNotNull(storeIds, w => storeIds.Contains(w.StoreId))
                .WhereIfNotNull(dto.StoreId, w => w.StoreId.Equals(dto.StoreId))
                .WhereIfNotEmpty(dto.StoreName, w => w.StoreName.Equals(dto.StoreName))
                .WhereIfNotNull(wayCodes, w => wayCodes.Contains(w.WayCode))
                .WhereIfNotEmpty(dto.WayCode, w => w.WayCode.Equals(dto.WayCode))
                .WhereIfNotEmpty(dto.WayType, w => w.WayType.Equals(dto.WayType))
                .WhereIfNotNull(dto.CreatedStart, w => w.CreatedAt >= dto.CreatedStart)
                .WhereIfNotNull(dto.CreatedEnd, w => w.CreatedAt <= dto.CreatedEnd);

            return (payOrders, refundOrders);
        }
    }
}
