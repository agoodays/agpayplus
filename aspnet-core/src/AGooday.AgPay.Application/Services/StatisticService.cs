using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
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
            IPayWayRepository payWayRepository)
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
        }

        public JObject Total(string agentNo, StatisticQueryDto dto)
        {
            switch (dto.QueryDateType)
            {
                case "day":
                    dto.CreatedStart ??= DateTime.Today.AddMonths(-1);
                    dto.CreatedEnd ??= DateTime.Today.AddSeconds(-1);
                    break;
                case "month":
                    dto.CreatedStart ??= DateTime.Today.AddYears(-1);
                    dto.CreatedEnd ??= DateTime.Today.AddSeconds(-1);
                    break;
                case "year":
                    dto.CreatedStart ??= DateTime.Today.AddYears(-1);
                    dto.CreatedEnd ??= DateTime.Today.AddSeconds(-1);
                    break;
                default:
                    break;
            }

            IEnumerable<AgentInfo> agents = _agentInfoRepository.GetAllOrSubAgents(agentNo);
            var agentNos = agentNo == null ? null : agents.Select(s => s.AgentNo);
            SelectOrderCount(dto, agentNos, null, out IQueryable<PayOrder> payOrders, out IQueryable<RefundOrder> refundOrders);
            var allAmount = payOrders.Sum(s => s.Amount);
            var allCount = payOrders.Count();
            // 成交金额: 支付成功的订单金额，包含部分退款及全额退款的订单
            var pay = payOrders.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));
            var payAmount = pay.Sum(s => s.Amount);
            var payCount = pay.Count();
            var fee = pay.Sum(s => s.MchOrderFeeAmount);
            var refund = refundOrders.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS));
            var refundAmount = refund.Sum(s => s.RefundAmount);
            var refundCount = refund.Count();
            var refundFeeAmount = refund.Sum(s => s.RefundFeeAmount);
            JObject json = new JObject();
            json.Add("allAmount", Decimal.Round(allAmount / 100M, 2, MidpointRounding.AwayFromZero));
            json.Add("allCount", allCount);
            json.Add("payAmount", Decimal.Round(payAmount / 100M, 2, MidpointRounding.AwayFromZero));
            json.Add("payCount", payCount);
            json.Add("fee", Decimal.Round(fee / 100M, 2, MidpointRounding.AwayFromZero));
            json.Add("refundAmount", Decimal.Round(refundAmount / 100M, 2, MidpointRounding.AwayFromZero));
            json.Add("refundCount", refundCount);
            json.Add("refundFeeAmount", Decimal.Round(refundFeeAmount / 100M, 2, MidpointRounding.AwayFromZero));
            json.Add("round", Math.Round(allCount > 0 ? payCount / Convert.ToDecimal(allCount) : 0M, 2, MidpointRounding.AwayFromZero));
            return json;
        }

        public PaginatedList<StatisticResultDto> Statistics(string agentNo, StatisticQueryDto dto)
        {
            return dto.Method switch
            {
                "transaction" => TransactionStatistics(agentNo, dto),
                "mch" => MchStatistics(agentNo, dto),
                "store" => StoreStatistics(dto),
                "agent" => AgentStatistics(agentNo, dto),
                "isv" => IsvStatistics(dto),
                _ => throw new NotImplementedException()
            };
        }

        private PaginatedList<StatisticResultDto> TransactionStatistics(string agentNo, StatisticQueryDto dto)
        {
            var format = "yyyy-MM-dd";
            switch (dto.QueryDateType)
            {
                case "day":
                    format = "yyyy-MM-dd";
                    dto.CreatedStart ??= DateTime.Today.AddMonths(-1);
                    dto.CreatedEnd ??= DateTime.Today.AddSeconds(-1);
                    break;
                case "month":
                    format = "yyyy-MM";
                    dto.CreatedStart ??= DateTime.Today.AddYears(-1);
                    dto.CreatedEnd ??= DateTime.Today.AddSeconds(-1);
                    break;
                case "year":
                    format = "yyyy";
                    dto.CreatedStart ??= DateTime.Today.AddYears(-1);
                    dto.CreatedEnd ??= DateTime.Today.AddSeconds(-1);
                    break;
                default:
                    break;
            }

            IEnumerable<AgentInfo> agents = _agentInfoRepository.GetSubAgents(agentNo);
            var agentNos = agents?.Select(s => s.AgentNo);
            SelectOrderCount(dto, agentNos, null, out IQueryable<PayOrder> payOrders, out IQueryable<RefundOrder> refundOrders);

            var payRecords = payOrders.AsEnumerable().GroupBy(g => g.CreatedAt.ToString(format))
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

            var refundRecords = refundOrders.AsEnumerable().GroupBy(g => g.CreatedAt.ToString(format))
                .Select(s =>
                {
                    IEnumerable<RefundOrder> refund = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));

                    return new StatisticResultDto()
                    {
                        GroupDate = s.Key,
                        RefundAmount = refund.Sum(s => s.RefundAmount),
                        RefundCount = refund.Count(),
                        RefundFee = refund.Sum(s => s.RefundFeeAmount),
                    };
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

        private PaginatedList<StatisticResultDto> MchStatistics(string agentNo, StatisticQueryDto dto)
        {
            IEnumerable<AgentInfo> agents = _agentInfoRepository.GetAllOrSubAgents(agentNo);
            var agentNos = agentNo == null ? null : agents.Select(s => s.AgentNo);

            var mchs = _mchInfoRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.MchName) || w.MchName.Contains(dto.MchName) || w.MchShortName.Contains(dto.MchName))
                && (agentNos == null || agentNos.Contains(w.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                ).OrderByDescending(o => o.CreatedAt);
            var mchInfos = PaginatedList<MchInfo>.Create<MchInfoDto>(mchs, _mapper, dto.PageNumber, dto.PageSize);

            SelectOrderCount(dto, agentNos, null, out IQueryable<PayOrder> payOrders, out IQueryable<RefundOrder> refundOrders);

            var payRecords = payOrders.AsEnumerable().GroupBy(g => g.MchNo)
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

            var refundRecords = refundOrders.AsEnumerable().GroupBy(g => g.MchNo)
                .Select(s =>
                {
                    var refund = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));

                    return new StatisticResultDto()
                    {
                        MchNo = s.Key,
                        RefundAmount = refund.Sum(s => s.RefundAmount),
                        RefundCount = refund.Count(),
                        RefundFee = refund.Sum(s => s.RefundFeeAmount),
                    };
                });

            var records = mchInfos
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
            var result = new PaginatedList<StatisticResultDto>(records?.ToList(), mchInfos.TotalCount, dto.PageNumber, dto.PageSize);
            return result;
        }

        private PaginatedList<StatisticResultDto> StoreStatistics(StatisticQueryDto dto)
        {
            var stores = _mchStoreRepository.GetAllAsNoTracking()
                .Where(w => (dto.StoreId.Equals(null) || w.StoreId.Equals(dto.StoreId))
                && (string.IsNullOrWhiteSpace(dto.StoreName) || w.StoreName.Contains(dto.StoreName))
                ).OrderByDescending(o => o.CreatedAt);
            var mchStores = PaginatedList<MchStore>.Create<MchStoreDto>(stores, _mapper, dto.PageNumber, dto.PageSize);

            SelectOrderCount(dto, null, null, out IQueryable<PayOrder> payOrders, out IQueryable<RefundOrder> refundOrders);

            var payRecords = payOrders.AsEnumerable().GroupBy(g => g.StoreId)
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

            var refundRecords = refundOrders.AsEnumerable().GroupBy(g => g.StoreId)
                .Select(s =>
                {
                    var refund = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));

                    return new StatisticResultDto()
                    {
                        StoreId = s.Key,
                        RefundAmount = refund.Sum(s => s.RefundAmount),
                        RefundCount = refund.Count(),
                        RefundFee = refund.Sum(s => s.RefundFeeAmount),
                    };
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

        private PaginatedList<StatisticResultDto> AgentStatistics(string agentNo, StatisticQueryDto dto)
        {
            IEnumerable<AgentInfo> agents = _agentInfoRepository.GetAllOrSubAgents(agentNo)
                .Where(w => (string.IsNullOrWhiteSpace(dto.AgentNo) || w.IsvNo.Equals(dto.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.AgentName) || w.AgentName.Contains(dto.AgentName) || w.AgentShortName.Contains(dto.IsvName))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                ).OrderByDescending(o => o.CreatedAt);
            var agentInfos = PaginatedList<AgentInfo>.Create(agents, dto.PageNumber, dto.PageSize);
            var agentNos = agentNo == null ? null : agentInfos.Select(s => s.AgentNo);
            SelectOrderCount(dto, agentNos, null, out IQueryable<PayOrder> payOrders, out IQueryable<RefundOrder> refundOrders);

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
                    var refund = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));

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

        private PaginatedList<StatisticResultDto> IsvStatistics(StatisticQueryDto dto)
        {
            var isvs = _isvInfoRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (string.IsNullOrWhiteSpace(dto.IsvName) || w.IsvName.Contains(dto.IsvName) || w.IsvShortName.Contains(dto.IsvName))
                ).OrderByDescending(o => o.CreatedAt);
            var isvInfos = PaginatedList<IsvInfo>.Create<IsvInfoDto>(isvs, _mapper, dto.PageNumber, dto.PageSize);
            var isvNos = isvInfos.Select(s => s.IsvNo);
            SelectOrderCount(dto, null, isvNos, out IQueryable<PayOrder> payOrders, out IQueryable<RefundOrder> refundOrders);

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
                    var refund = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));

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

        private void SelectOrderCount(StatisticQueryDto dto, IEnumerable<string> agentNos, IEnumerable<string> isvNos, out IQueryable<PayOrder> payOrders, out IQueryable<RefundOrder> refundOrders)
        {
            payOrders = _payOrderRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.MchName) || w.MchName.Equals(dto.MchName))
                && (agentNos == null || agentNos.Contains(w.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.AgentName) || w.AgentName.Equals(dto.AgentName))
                && (isvNos == null || isvNos.Contains(w.IsvNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (string.IsNullOrWhiteSpace(dto.IsvName) || w.IsvName.Equals(dto.IsvName))
                && (dto.StoreId.Equals(null) || w.StoreId.Equals(dto.StoreId))
                && (string.IsNullOrWhiteSpace(dto.StoreName) || w.StoreName.Equals(dto.StoreName))
                && (string.IsNullOrWhiteSpace(dto.WayCode) || w.WayCode.Equals(dto.WayCode))
                && (string.IsNullOrWhiteSpace(dto.WayType) || w.WayType.Equals(dto.WayType))
                && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
                && (dto.CreatedEnd.Equals(null) || w.CreatedAt <= dto.CreatedEnd));

            refundOrders = _refundOrderRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.MchName) || w.MchName.Equals(dto.MchName))
                && (agentNos == null || agentNos.Contains(w.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.AgentName) || w.AgentName.Equals(dto.AgentName))
                && (isvNos == null || isvNos.Contains(w.IsvNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (string.IsNullOrWhiteSpace(dto.IsvName) || w.IsvName.Equals(dto.IsvName))
                && (dto.StoreId.Equals(null) || w.StoreId.Equals(dto.StoreId))
                && (string.IsNullOrWhiteSpace(dto.StoreName) || w.StoreName.Equals(dto.StoreName))
                && (string.IsNullOrWhiteSpace(dto.WayCode) || w.WayCode.Equals(dto.WayCode))
                && (string.IsNullOrWhiteSpace(dto.WayType) || w.WayType.Equals(dto.WayType))
                && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
                && (dto.CreatedEnd.Equals(null) || w.CreatedAt <= dto.CreatedEnd));
        }
    }
}
