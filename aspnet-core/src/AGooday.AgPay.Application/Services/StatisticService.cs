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
            IAgentInfoRepository agentInfoRepository,
            IIsvInfoRepository isvInfoRepository,
            IPayWayRepository payWayRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _payOrderRepository = payOrderRepository;
            _refundOrderRepository = refundOrderRepository;
            _mchInfoRepository = mchInfoRepository;
            _agentInfoRepository = agentInfoRepository;
            _isvInfoRepository = isvInfoRepository;
            _payWayRepository = payWayRepository;
        }

        public JObject Total(StatisticQueryDto dto)
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
            SelectOrderCount(dto, out IQueryable<PayOrder> payOrders, out IQueryable<RefundOrder> refundOrders);
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

        public PaginatedList<StatisticResultDto> Statistics(StatisticQueryDto dto)
        {
            return dto.Method switch
            {
                "transaction" => TransactionStatistics(dto),
                "mch" => MchStatistics(dto),
                _ => throw new NotImplementedException()
            };
        }

        private PaginatedList<StatisticResultDto> TransactionStatistics(StatisticQueryDto dto)
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

            SelectOrderCount(dto, out IQueryable<PayOrder> payOrders, out IQueryable<RefundOrder> refundOrders);

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
                    var pay = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));

                    return new StatisticResultDto()
                    {
                        GroupDate = s.Key,
                        RefundAmount = pay.Sum(s => s.RefundAmount),
                        RefundCount = pay.Count(),
                        RefundFee = pay.Sum(s => s.RefundFeeAmount),
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

        private PaginatedList<StatisticResultDto> MchStatistics(StatisticQueryDto dto)
        {
            SelectOrderCount(dto, out IQueryable<PayOrder> payOrders, out IQueryable<RefundOrder> refundOrders);

            var payRecords = payOrders.AsEnumerable().GroupBy(g => new { g.MchNo, g.MchName })
                .Select(s =>
                {
                    var pay = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));

                    return new StatisticResultDto()
                    {
                        MchNo = s.Key.MchNo,
                        MchName = s.Key.MchName,
                        AllAmount = s.Sum(s => s.Amount),
                        AllCount = s.Count(),
                        PayAmount = pay.Sum(s => s.Amount),
                        PayCount = pay.Count(),
                        Fee = pay.Sum(s => s.MchOrderFeeAmount),
                    };
                });

            var refundRecords = refundOrders.AsEnumerable().GroupBy(g => new { g.MchNo, g.MchName })
                .Select(s =>
                {
                    var pay = s.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));

                    return new StatisticResultDto()
                    {
                        MchNo = s.Key.MchNo,
                        MchName = s.Key.MchName,
                        RefundAmount = pay.Sum(s => s.RefundAmount),
                        RefundCount = pay.Count(),
                        RefundFee = pay.Sum(s => s.RefundFeeAmount),
                    };
                });

            var records = payRecords
                .GroupJoin(refundRecords, p => p.MchNo, r => r.MchNo, (p, r) => new { p, r })
                .SelectMany(x => x.r.DefaultIfEmpty(), (s, r) => new StatisticResultDto()
                {
                    MchNo = s.p.MchNo,
                    MchName = s.p.MchName,
                    AllAmount = s.p.AllAmount,
                    AllCount = s.p.AllCount,
                    PayAmount = s.p.PayAmount,
                    PayCount = s.p.PayCount,
                    Round = Math.Round(s.p.AllCount > 0 ? (decimal)s.p.PayCount / s.p.AllCount : 0M, 2, MidpointRounding.AwayFromZero),
                    Fee = s.p.Fee,
                    RefundAmount = r?.RefundAmount ?? 0,
                    RefundCount = r?.RefundCount ?? 0,
                    RefundFee = r?.RefundFee ?? 0
                });

            var result = PaginatedList<StatisticResultDto>.Create(records, dto.PageNumber, dto.PageSize);
            return result;
        }

        private void SelectOrderCount(StatisticQueryDto dto, out IQueryable<PayOrder> payOrders, out IQueryable<RefundOrder> refundOrders)
        {
            payOrders = _payOrderRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.MchName) || w.MchName.Equals(dto.MchName))
                && (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.AgentName) || w.AgentName.Equals(dto.AgentName))
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
                && (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.AgentName) || w.AgentName.Equals(dto.AgentName))
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
