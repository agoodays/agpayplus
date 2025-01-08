using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
            var allAmount = await payOrders.SumAsync(s => s.Amount);
            var allCount = await payOrders.CountAsync();
            // 成交金额: 支付成功的订单金额，包含部分退款及全额退款的订单
            var pay = payOrders.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));
            var payAmount = await pay.SumAsync(s => s.Amount);
            var payCount = await pay.CountAsync();
            var fee = await pay.SumAsync(s => s.MchOrderFeeAmount);
            var refund = refundOrders.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS));
            var refundAmount = await refund.SumAsync(s => s.RefundAmount);
            var refundCount = await refund.CountAsync();
            var refundFeeAmount = await refund.SumAsync(s => s.RefundFeeAmount);
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

        public async Task<PaginatedList<StatisticResultDto>> StatisticsAsync(string agentNo, StatisticQueryDto dto)
        {
            return dto.Method switch
            {
                StatisticCS.Method.TRANSACTION => await TransactionStatisticsAsync(agentNo, dto),
                StatisticCS.Method.MCH => await MchStatisticsAsync(agentNo, dto),
                StatisticCS.Method.STORE => await StoreStatisticsAsync(dto),
                StatisticCS.Method.WAY_CODE => await WayCodeStatisticsAsync(dto),
                StatisticCS.Method.WAY_TYPE => WayTypeStatistics(dto),
                StatisticCS.Method.AGENT => await AgentStatisticsAsync(agentNo, dto),
                StatisticCS.Method.ISV => await IsvStatisticsAsync(dto),
                StatisticCS.Method.CHANNEL => await ChannelStatisticsAsync(dto),
                _ => throw new NotImplementedException()
            };
        }

        private async Task<PaginatedList<StatisticResultDto>> TransactionStatisticsAsync(string agentNo, StatisticQueryDto dto)
        {
            IEnumerable<AgentInfo> agents = await _agentInfoRepository.GetSubAgentsAsync(agentNo);
            var agentNos = agents?.Select(s => s.AgentNo);
            var (payOrders, refundOrders) = SelectOrderCount(dto, agentNos: agentNos);

            var payRecords = payOrders.AsEnumerable().GroupBy(g => g.CreatedAt.Value.ToString(dto.Format))
                .Select(s =>
                {
                    var pay = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));

                    return new StatisticResultDto()
                    {
                        GroupDate = s.Key,
                        AllAmount = s.Sum(s => s.Amount),
                        AllCount = s.Count(),
                        PayAmount = pay.Sum(s => s.Amount),
                        PayCount = pay.Count(),
                        Fee = pay.Sum(s => s.MchOrderFeeAmount),
                    };
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

            var result = PaginatedList<StatisticResultDto>.Create(records, dto.PageNumber, dto.PageSize);
            return result;
        }

        private async Task<PaginatedList<StatisticResultDto>> MchStatisticsAsync(string agentNo, StatisticQueryDto dto)
        {
            var agents = await _agentInfoRepository.GetAllOrSubAgentsAsync(agentNo);
            var agentNos = agentNo == null ? null : agents.Select(s => s.AgentNo);

            var mchs = _mchInfoRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.MchName) || w.MchName.Contains(dto.MchName) || w.MchShortName.Contains(dto.MchName))
                && (agentNos == null || agentNos.Contains(w.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                ).OrderByDescending(o => o.CreatedAt);
            var mchInfos = await PaginatedList<MchInfo>.CreateAsync<MchInfoDto>(mchs, _mapper, dto.PageNumber, dto.PageSize);
            var mchNos = mchInfos.Select(s => s.MchNo);
            var (payOrders, refundOrders) = SelectOrderCount(dto, agentNos: agentNos, mchNos: mchNos);

            var payRecords = payOrders.GroupBy(g => g.MchNo).AsEnumerable()
                .Select(s =>
                {
                    var pay = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));

                    return new StatisticResultDto()
                    {
                        MchNo = s.Key,
                        AllAmount = s.Sum(s => s.Amount),
                        AllCount = s.Count(),
                        PayAmount = pay.Sum(s => s.Amount),
                        PayCount = pay.Count(),
                        Fee = pay.Sum(s => s.MchOrderFeeAmount),
                    };
                });

            var refundRecords = refundOrders.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS)).GroupBy(g => g.MchNo)
                .Select(s => new StatisticResultDto()
                {
                    MchNo = s.Key,
                    RefundAmount = s.Sum(s => s.RefundAmount),
                    RefundCount = s.Count(),
                    RefundFee = s.Sum(s => s.RefundFeeAmount),
                });

            var records = mchInfos.Select(s =>
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
            var result = new PaginatedList<StatisticResultDto>(records?.ToList(), mchInfos.TotalCount, dto.PageNumber, dto.PageSize);
            return result;
        }

        private async Task<PaginatedList<StatisticResultDto>> StoreStatisticsAsync(StatisticQueryDto dto)
        {
            var stores = _mchStoreRepository.GetAllAsNoTracking()
                .Where(w => (dto.StoreId.Equals(null) || w.StoreId.Equals(dto.StoreId))
                && (string.IsNullOrWhiteSpace(dto.StoreName) || w.StoreName.Contains(dto.StoreName))
                && (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Contains(dto.MchNo))
                ).OrderByDescending(o => o.CreatedAt);
            var mchStores = await PaginatedList<MchStore>.CreateAsync<MchStoreDto>(stores, _mapper, dto.PageNumber, dto.PageSize);
            var storeIds = mchStores.Select(s => s.StoreId);
            var (payOrders, refundOrders) = SelectOrderCount(dto, storeIds: storeIds);

            var payRecords = payOrders.GroupBy(g => g.StoreId).AsEnumerable()
                .Select(s =>
                {
                    var pay = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));

                    return new StatisticResultDto()
                    {
                        StoreId = s.Key,
                        AllAmount = s.Sum(s => s.Amount),
                        AllCount = s.Count(),
                        PayAmount = pay.Sum(s => s.Amount),
                        PayCount = pay.Count(),
                        Fee = pay.Sum(s => s.MchOrderFeeAmount),
                    };
                });

            var refundRecords = refundOrders.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS)).GroupBy(g => g.StoreId)
                .Select(s => new StatisticResultDto()
                {
                    StoreId = s.Key,
                    RefundAmount = s.Sum(s => s.RefundAmount),
                    RefundCount = s.Count(),
                    RefundFee = s.Sum(s => s.RefundFeeAmount),
                });

            var records = mchStores
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
            var result = new PaginatedList<StatisticResultDto>(records?.ToList(), mchStores.TotalCount, dto.PageNumber, dto.PageSize);
            return result;
        }

        private async Task<PaginatedList<StatisticResultDto>> WayCodeStatisticsAsync(StatisticQueryDto dto)
        {
            var ways = _payWayRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.WayCode) || w.WayCode.Equals(dto.WayCode))
                //&& (string.IsNullOrWhiteSpace(dto.WayName) || w.WayName.Contains(dto.WayName))
                ).OrderByDescending(o => o.WayCode).ThenByDescending(o => o.CreatedAt);
            var payWays = await PaginatedList<PayWay>.CreateAsync<PayWayDto>(ways, _mapper, dto.PageNumber, dto.PageSize);
            var wayCodes = payWays.Select(s => s.WayCode);
            var (payOrders, refundOrders) = SelectOrderCount(dto, wayCodes: wayCodes);

            var payRecords = payOrders.AsEnumerable().GroupBy(g => g.WayCode)
                .Select(s =>
                {
                    var pay = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));

                    return new StatisticResultDto()
                    {
                        WayCode = s.Key,
                        AllAmount = s.Sum(s => s.Amount),
                        AllCount = s.Count(),
                        PayAmount = pay.Sum(s => s.Amount),
                        PayCount = pay.Count(),
                        Fee = pay.Sum(s => s.MchOrderFeeAmount),
                    };
                });

            var refundRecords = refundOrders.AsEnumerable().GroupBy(g => g.WayCode)
                .Select(s =>
                {
                    var refund = s.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS));

                    return new StatisticResultDto()
                    {
                        WayCode = s.Key,
                        RefundAmount = refund.Sum(s => s.RefundAmount),
                        RefundCount = refund.Count(),
                        RefundFee = refund.Sum(s => s.RefundFeeAmount),
                    };
                });

            var records = payWays
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
            var result = new PaginatedList<StatisticResultDto>(records?.ToList(), payWays.TotalCount, dto.PageNumber, dto.PageSize);
            return result;
        }

        private PaginatedList<StatisticResultDto> WayTypeStatistics(StatisticQueryDto dto)
        {
            var (payOrders, refundOrders) = SelectOrderCount(dto);

            var payRecords = payOrders.AsEnumerable().GroupBy(g => g.WayType)
                .Select(s =>
                {
                    var pay = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));

                    return new StatisticResultDto()
                    {
                        WayType = s.Key,
                        AllAmount = s.Sum(s => s.Amount),
                        AllCount = s.Count(),
                        PayAmount = pay.Sum(s => s.Amount),
                        PayCount = pay.Count(),
                        Fee = pay.Sum(s => s.MchOrderFeeAmount),
                    };
                });

            var refundRecords = refundOrders.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS)).GroupBy(g => g.WayType)
                .Select(s => new StatisticResultDto()
                {
                    WayType = s.Key,
                    RefundAmount = s.Sum(s => s.RefundAmount),
                    RefundCount = s.Count(),
                    RefundFee = s.Sum(s => s.RefundFeeAmount),
                });

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
            var result = PaginatedList<StatisticResultDto>.Create(records, dto.PageNumber, dto.PageSize);
            return result;
        }

        private async Task<PaginatedList<StatisticResultDto>> AgentStatisticsAsync(string agentNo, StatisticQueryDto dto)
        {
            var agents = (await _agentInfoRepository.GetAllOrSubAgentsAsync(agentNo))
                .Where(w => (string.IsNullOrWhiteSpace(dto.AgentNo) || w.IsvNo.Equals(dto.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.AgentName) || w.AgentName.Contains(dto.AgentName) || w.AgentShortName.Contains(dto.IsvName))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                ).OrderByDescending(o => o.CreatedAt);
            var agentInfos = PaginatedList<AgentInfo>.Create(agents, dto.PageNumber, dto.PageSize);
            var agentNos = agentNo == null ? null : agentInfos.Select(s => s.AgentNo);
            var (payOrders, refundOrders) = SelectOrderCount(dto, agentNos: agentNos);

            var payRecords = payOrders.AsEnumerable().GroupBy(g => g.AgentNo)
                .Select(s =>
                {
                    var pay = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));

                    return new StatisticResultDto()
                    {
                        AgentNo = s.Key,
                        AllAmount = s.Sum(s => s.Amount),
                        AllCount = s.Count(),
                        PayAmount = pay.Sum(s => s.Amount),
                        PayCount = pay.Count(),
                        Fee = pay.Sum(s => s.MchOrderFeeAmount),
                    };
                });

            var refundRecords = refundOrders.AsEnumerable().GroupBy(g => g.AgentNo)
                .Select(s =>
                {
                    var refund = s.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS));

                    return new StatisticResultDto()
                    {
                        AgentNo = s.Key,
                        RefundAmount = refund.Sum(s => s.RefundAmount),
                        RefundCount = refund.Count(),
                        RefundFee = refund.Sum(s => s.RefundFeeAmount),
                    };
                });

            var records = agentInfos
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
            var result = new PaginatedList<StatisticResultDto>(records?.ToList(), agentInfos.TotalCount, dto.PageNumber, dto.PageSize);
            // var result = PaginatedList<StatisticResultDto>.Create(records, dto.PageNumber, dto.PageSize);
            return result;
        }

        private async Task<PaginatedList<StatisticResultDto>> IsvStatisticsAsync(StatisticQueryDto dto)
        {
            var isvs = _isvInfoRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (string.IsNullOrWhiteSpace(dto.IsvName) || w.IsvName.Contains(dto.IsvName) || w.IsvShortName.Contains(dto.IsvName))
                ).OrderByDescending(o => o.CreatedAt);
            var isvInfos = await PaginatedList<IsvInfo>.CreateAsync<IsvInfoDto>(isvs, _mapper, dto.PageNumber, dto.PageSize);
            var isvNos = isvInfos.Select(s => s.IsvNo);
            var (payOrders, refundOrders) = SelectOrderCount(dto, isvNos: isvNos);

            var payRecords = payOrders.AsEnumerable().GroupBy(g => g.IsvNo)
                .Select(s =>
                {
                    var pay = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));

                    return new StatisticResultDto()
                    {
                        IsvNo = s.Key,
                        AllAmount = s.Sum(s => s.Amount),
                        AllCount = s.Count(),
                        PayAmount = pay.Sum(s => s.Amount),
                        PayCount = pay.Count(),
                        Fee = pay.Sum(s => s.MchOrderFeeAmount),
                    };
                });

            var refundRecords = refundOrders.AsEnumerable().GroupBy(g => g.IsvNo)
                .Select(s =>
                {
                    var refund = s.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS));

                    return new StatisticResultDto()
                    {
                        IsvNo = s.Key,
                        RefundAmount = refund.Sum(s => s.RefundAmount),
                        RefundCount = refund.Count(),
                        RefundFee = refund.Sum(s => s.RefundFeeAmount),
                    };
                });

            var records = isvInfos
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
            var result = new PaginatedList<StatisticResultDto>(records?.ToList(), isvInfos.TotalCount, dto.PageNumber, dto.PageSize);
            return result;
        }

        private async Task<PaginatedList<StatisticResultDto>> ChannelStatisticsAsync(StatisticQueryDto dto)
        {
            var payIfDefines = _payInterfaceDefineRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.IfCode) || w.IfCode.Equals(dto.IfCode))
                && (string.IsNullOrWhiteSpace(dto.IsvName) || w.IfName.Equals(dto.IfName))
                ).OrderByDescending(o => o.CreatedAt);
            var payInterfaceDefines = await PaginatedList<PayInterfaceDefine>.CreateAsync<PayInterfaceDefineDto>(payIfDefines, _mapper, dto.PageNumber, dto.PageSize);

            var (payOrders, refundOrders) = SelectOrderCount(dto);

            var payRecords = payOrders.AsEnumerable().GroupBy(g => g.IfCode)
                .Select(s =>
                {
                    var pay = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));

                    return new StatisticResultDto()
                    {
                        IfCode = s.Key,
                        AllAmount = s.Sum(s => s.Amount),
                        AllCount = s.Count(),
                        PayAmount = pay.Sum(s => s.Amount),
                        PayCount = pay.Count(),
                        Fee = pay.Sum(s => s.MchOrderFeeAmount),
                    };
                });

            var refundRecords = refundOrders.AsEnumerable().GroupBy(g => g.IfCode)
                .Select(s =>
                {
                    var refund = s.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS));

                    return new StatisticResultDto()
                    {
                        IfCode = s.Key,
                        RefundAmount = refund.Sum(s => s.RefundAmount),
                        RefundCount = refund.Count(),
                        RefundFee = refund.Sum(s => s.RefundFeeAmount),
                    };
                });

            var records = payInterfaceDefines
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
            var result = new PaginatedList<StatisticResultDto>(records?.ToList(), payInterfaceDefines.TotalCount, dto.PageNumber, dto.PageSize);
            return result;
        }

        private (IQueryable<PayOrder> payOrders, IQueryable<RefundOrder> refundOrders) SelectOrderCount(StatisticQueryDto dto, IEnumerable<string> isvNos = null, IEnumerable<string> agentNos = null, IEnumerable<string> mchNos = null, IEnumerable<long?> storeIds = null, IEnumerable<string> wayCodes = null)
        {
            var payOrders = _payOrderRepository.GetAllAsNoTracking()
                 .Where(w => ((mchNos == null || mchNos.Contains(w.MchNo))
                 && string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                 && (string.IsNullOrWhiteSpace(dto.MchName) || w.MchName.Equals(dto.MchName))
                 && (agentNos == null || agentNos.Contains(w.AgentNo))
                 && (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                 && (string.IsNullOrWhiteSpace(dto.AgentName) || w.AgentName.Equals(dto.AgentName))
                 && (isvNos == null || isvNos.Contains(w.IsvNo))
                 && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                 && (storeIds == null || storeIds.Contains(w.StoreId))
                 && (string.IsNullOrWhiteSpace(dto.IsvName) || w.IsvName.Equals(dto.IsvName))
                 && (dto.StoreId.Equals(null) || w.StoreId.Equals(dto.StoreId))
                 && (string.IsNullOrWhiteSpace(dto.StoreName) || w.StoreName.Equals(dto.StoreName))
                 && (wayCodes == null || wayCodes.Contains(w.WayCode))
                 && (string.IsNullOrWhiteSpace(dto.WayCode) || w.WayCode.Equals(dto.WayCode))
                 && (string.IsNullOrWhiteSpace(dto.WayType) || w.WayType.Equals(dto.WayType))
                 && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
                 && (dto.CreatedEnd.Equals(null) || w.CreatedAt <= dto.CreatedEnd));

            var refundOrders = _refundOrderRepository.GetAllAsNoTracking()
                 .Where(w => ((mchNos == null || mchNos.Contains(w.MchNo))
                 && string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                 && (string.IsNullOrWhiteSpace(dto.MchName) || w.MchName.Equals(dto.MchName))
                 && (agentNos == null || agentNos.Contains(w.AgentNo))
                 && (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                 && (string.IsNullOrWhiteSpace(dto.AgentName) || w.AgentName.Equals(dto.AgentName))
                 && (isvNos == null || isvNos.Contains(w.IsvNo))
                 && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                 && (string.IsNullOrWhiteSpace(dto.IsvName) || w.IsvName.Equals(dto.IsvName))
                 && (storeIds == null || storeIds.Contains(w.StoreId))
                 && (dto.StoreId.Equals(null) || w.StoreId.Equals(dto.StoreId))
                 && (string.IsNullOrWhiteSpace(dto.StoreName) || w.StoreName.Equals(dto.StoreName))
                 && (wayCodes == null || wayCodes.Contains(w.WayCode))
                 && (string.IsNullOrWhiteSpace(dto.WayCode) || w.WayCode.Equals(dto.WayCode))
                 && (string.IsNullOrWhiteSpace(dto.WayType) || w.WayType.Equals(dto.WayType))
                 && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
                 && (dto.CreatedEnd.Equals(null) || w.CreatedAt <= dto.CreatedEnd));

            return (payOrders, refundOrders);
        }
    }
}
