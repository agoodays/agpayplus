using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Extensions;
using AutoMapper;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 支付订单表 服务实现类
    /// </summary>
    public class PayOrderService : AgPayService<PayOrderDto, PayOrder>, IPayOrderService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayOrderRepository _payOrderRepository;
        private readonly IRefundOrderRepository _refundOrderRepository;
        private readonly IMchInfoRepository _mchInfoRepository;
        private readonly IAgentInfoRepository _agentInfoRepository;
        private readonly IIsvInfoRepository _isvInfoRepository;
        private readonly IPayWayRepository _payWayRepository;
        private readonly IPayOrderDivisionRecordRepository _payOrderDivisionRecordRepository;

        public PayOrderService(IMapper mapper, IMediatorHandler bus,
            IPayOrderRepository payOrderRepository,
            IRefundOrderRepository refundOrderRepository,
            IMchInfoRepository mchInfoRepository,
            IAgentInfoRepository agentInfoRepository,
            IIsvInfoRepository isvInfoRepository,
            IPayWayRepository payWayRepository,
            IPayOrderDivisionRecordRepository payOrderDivisionRecordRepository)
            : base(mapper, bus, payOrderRepository)
        {
            _payOrderRepository = payOrderRepository;
            _refundOrderRepository = refundOrderRepository;
            _mchInfoRepository = mchInfoRepository;
            _agentInfoRepository = agentInfoRepository;
            _isvInfoRepository = isvInfoRepository;
            _payWayRepository = payWayRepository;
            _payOrderDivisionRecordRepository = payOrderDivisionRecordRepository;
        }

        public Task<bool> IsExistOrderUseIfCodeAsync(string ifCode)
        {
            return _payOrderRepository.IsExistOrderUseIfCodeAsync(ifCode);
        }
        public Task<bool> IsExistOrderUseWayCodeAsync(string wayCode)
        {
            return _payOrderRepository.IsExistOrderUseWayCodeAsync(wayCode);
        }
        public Task<bool> IsExistOrderByMchOrderNoAsync(string mchNo, string mchOrderNo)
        {
            return _payOrderRepository.IsExistOrderByMchOrderNoAsync(mchNo, mchOrderNo);
        }

        public async Task<PayOrderDto> QueryMchOrderAsync(string mchNo, string payOrderId, string mchOrderNo)
        {
            var entity = await _payOrderRepository.GetAllAsNoTracking().FirstOrDefaultAsync(w => w.MchNo.Equals(mchNo)
            && (w.PayOrderId.Equals(payOrderId) || w.MchOrderNo.Equals(mchOrderNo)));
            return _mapper.Map<PayOrderDto>(entity);
        }

        public Task<PaginatedResult<PayOrderDto>> GetPaginatedDataAsync(PayOrderQueryDto dto)
        {
            var query = GetPayOrders(dto).OrderByDescending(o => o.CreatedAt);
            return query.ToPaginatedResultAsync<PayOrder, PayOrderDto>(_mapper, dto.PageNumber, dto.PageSize);
        }

        private IQueryable<PayOrder> GetPayOrders(PayOrderQueryDto dto)
        {
            var result = _payOrderRepository.GetAllAsNoTracking()
                 .WhereIfNotEmpty(dto.MchNo, w => w.MchNo.Equals(dto.MchNo))
                 .WhereIfNotEmpty(dto.AgentNo, w => w.AgentNo.Equals(dto.AgentNo))
                 .WhereIfNotEmpty(dto.IsvNo, w => w.IsvNo.Equals(dto.IsvNo))
                 .WhereIfNotNull(dto.MchType, w => w.MchType.Equals(dto.MchType))
                 .WhereIfNotEmpty(dto.IfCode, w => w.IfCode.Equals(dto.IfCode))
                 .WhereIfNotEmpty(dto.WayCode, w => w.WayCode.Equals(dto.WayCode))
                 .WhereIfNotEmpty(dto.MchOrderNo, w => w.MchOrderNo.Equals(dto.MchOrderNo))
                 .WhereIfNotNull(dto.State, w => w.State.Equals(dto.State))
                 .WhereIfNotEmpty(dto.AppId, w => w.AppId.Equals(dto.AppId))
                 .WhereIfNotNull(dto.StoreId, w => w.StoreId.Equals(dto.StoreId))
                 .WhereIfNotEmpty(dto.StoreName, w => w.StoreName.Equals(dto.StoreName))
                 .WhereIfNotNull(dto.DivisionState, w => w.DivisionState.Equals(dto.DivisionState))
                 .WhereIfNotEmpty(dto.PayOrderId, w => w.PayOrderId.Equals(dto.PayOrderId))
                 .WhereIfNotEmpty(dto.MchOrderNo, w => w.MchOrderNo.Equals(dto.MchOrderNo))
                 .WhereIfNotEmpty(dto.ChannelOrderNo, w => w.ChannelOrderNo.Equals(dto.ChannelOrderNo))
                 .WhereIfNotEmpty(dto.PlatformOrderNo, w => w.PlatformOrderNo.Equals(dto.PlatformOrderNo))
                 .WhereIfNotEmpty(dto.PlatformMchOrderNo, w => w.PlatformMchOrderNo.Equals(dto.PlatformMchOrderNo))
                 .WhereIfNotEmpty(dto.UnionOrderId, w => w.PayOrderId.Equals(dto.UnionOrderId) || w.MchOrderNo.Equals(dto.UnionOrderId) || w.ChannelOrderNo.Equals(dto.UnionOrderId))
                 .WhereIfNotNull(dto.CreatedStart, w => w.CreatedAt >= dto.CreatedStart)
                 .WhereIfNotNull(dto.CreatedEnd, w => w.CreatedAt <= dto.CreatedEnd);
            return result;
        }

        public async Task<JObject> StatisticsAsync(PayOrderQueryDto dto)
        {
            // 获取所有统计数据
            var statistics = await GetPayOrders(dto)
                .GroupBy(x => 1) // 常量分组，将所有数据分为一组
                .Select(g => new
                {
                    // 总统计
                    AllPayAmount = g.Sum(o => o.Amount),
                    AllPayCount = g.Count(),

                    // 失败支付统计
                    FailPayAmount = g.Where(p => !(p.State == (byte)PayOrderState.STATE_SUCCESS || p.State == (byte)PayOrderState.STATE_REFUND))
                                   .Sum(o => o.Amount),
                    FailPayCount = g.Count(p => !(p.State == (byte)PayOrderState.STATE_SUCCESS || p.State == (byte)PayOrderState.STATE_REFUND)),

                    // 成功支付统计
                    MchFeeAmount = g.Where(p => p.State == (byte)PayOrderState.STATE_SUCCESS || p.State == (byte)PayOrderState.STATE_REFUND)
                                  .Sum(o => o.MchFeeAmount),
                    PayAmount = g.Where(p => p.State == (byte)PayOrderState.STATE_SUCCESS || p.State == (byte)PayOrderState.STATE_REFUND)
                               .Sum(o => o.Amount),
                    PayCount = g.Count(p => p.State == (byte)PayOrderState.STATE_SUCCESS || p.State == (byte)PayOrderState.STATE_REFUND),

                    // 退款统计
                    RefundAmount = g.Where(p => p.State == (byte)PayOrderRefund.REFUND_STATE_SUB || p.State == (byte)PayOrderRefund.REFUND_STATE_ALL)
                                  .Sum(o => o.Amount),
                    RefundCount = g.Count(p => p.State == (byte)PayOrderRefund.REFUND_STATE_SUB || p.State == (byte)PayOrderRefund.REFUND_STATE_ALL)
                })
                .FirstOrDefaultAsync();

            // 处理空结果
            statistics ??= new
            {
                AllPayAmount = 0L,
                AllPayCount = 0,
                FailPayAmount = 0L,
                FailPayCount = 0,
                MchFeeAmount = 0L,
                PayAmount = 0L,
                PayCount = 0,
                RefundAmount = 0L,
                RefundCount = 0
            };

            JObject result = new JObject();
            result.Add("allPayAmount", Decimal.Round(statistics.AllPayAmount / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("allPayCount", statistics.AllPayCount);
            result.Add("failPayAmount", Decimal.Round(statistics.FailPayAmount / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("failPayCount", statistics.FailPayCount);
            result.Add("mchFeeAmount", Decimal.Round(statistics.MchFeeAmount / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("payAmount", Decimal.Round(statistics.PayAmount / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("payCount", statistics.PayCount);
            result.Add("refundAmount", Decimal.Round(statistics.RefundAmount / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("refundCount", statistics.RefundCount);
            return result;
        }

        /// <summary>
        /// 更新订单状态 【订单生成】 --》 【支付中】
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <param name="payOrder"></param>
        /// <returns></returns>
        public async Task<bool> UpdateInit2IngAsync(string payOrderId, PayOrderDto payOrder)
        {
            var updateRecord = await _payOrderRepository.GetByIdAsync(payOrderId);
            if (updateRecord.State != (byte)PayOrderState.STATE_INIT)
            {
                return false;
            }

            updateRecord.State = (byte)PayOrderState.STATE_ING;

            //同时更新， 未确定 --》 已确定的其他信息。  如支付接口的确认、 费率的计算。
            updateRecord.IfCode = payOrder.IfCode;
            updateRecord.WayCode = payOrder.WayCode;
            updateRecord.WayType = payOrder.WayType;
            updateRecord.MchFeeRate = payOrder.MchFeeRate;
            updateRecord.MchFeeAmount = payOrder.MchFeeAmount;
            updateRecord.MchOrderFeeAmount = payOrder.MchOrderFeeAmount;
            updateRecord.ChannelUser = payOrder.ChannelUser;
            _payOrderRepository.Update(updateRecord);
            var (result, _) = await _payOrderRepository.SaveChangesWithResultAsync();
            return result;
        }
        /// <summary>
        /// 更新订单状态 【支付中】 --》 【支付成功】
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <param name="channelMchNo"></param>
        /// <param name="channelIsvNo"></param>
        /// <param name="channelOrderNo"></param>
        /// <param name="channelUserId"></param>
        /// <param name="platformOrderNo"></param>
        /// <param name="platformMchOrderNo"></param>
        /// <returns></returns>
        public async Task<bool> UpdateIng2SuccessAsync(string payOrderId, string channelMchNo, string channelIsvNo, string channelOrderNo, string channelUserId, string platformOrderNo, string platformMchOrderNo)
        {
            var updateRecord = await _payOrderRepository.GetByIdAsync(payOrderId);
            if (updateRecord.State != (byte)PayOrderState.STATE_ING)
            {
                return false;
            }
            updateRecord.State = (byte)PayOrderState.STATE_SUCCESS;
            updateRecord.ChannelMchNo = channelMchNo;
            updateRecord.ChannelIsvNo = channelIsvNo;
            updateRecord.ChannelOrderNo = channelOrderNo;
            updateRecord.ChannelUser = channelUserId;
            updateRecord.PlatformOrderNo = platformOrderNo;
            updateRecord.PlatformMchOrderNo = platformMchOrderNo;
            updateRecord.SuccessTime = DateTime.Now;
            _payOrderRepository.Update(updateRecord);
            var (result, _) = await _payOrderRepository.SaveChangesWithResultAsync();
            return result;
        }
        /// <summary>
        /// 更新订单状态  【支付中】 --》 【订单关闭】
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateIng2CloseAsync(string payOrderId)
        {
            var updateRecord = await _payOrderRepository.GetByIdAsync(payOrderId);
            if (updateRecord.State != (byte)PayOrderState.STATE_ING)
            {
                return false;
            }
            updateRecord.State = (byte)PayOrderState.STATE_CLOSED;
            updateRecord.SuccessTime = DateTime.Now;
            _payOrderRepository.Update(updateRecord);
            var (result, _) = await _payOrderRepository.SaveChangesWithResultAsync();
            return result;
        }
        /// <summary>
        /// 更新订单状态 【支付中】 --》 【支付失败】
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <param name="channelMchNo"></param>
        /// <param name="channelIsvNo"></param>
        /// <param name="channelOrderNo"></param>
        /// <param name="channelUserId"></param>
        /// <param name="platformOrderNo"></param>
        /// <param name="platformMchOrderNo"></param>
        /// <param name="channelErrCode"></param>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        public async Task<bool> UpdateIng2FailAsync(string payOrderId, string channelMchNo, string channelIsvNo, string channelOrderNo, string channelUserId, string platformOrderNo, string platformMchOrderNo, string channelErrCode, string channelErrMsg)
        {
            var updateRecord = await _payOrderRepository.GetByIdAsync(payOrderId);
            if (updateRecord.State != (byte)PayOrderState.STATE_ING)
            {
                return false;
            }
            updateRecord.State = (byte)PayOrderState.STATE_FAIL;
            updateRecord.ErrCode = channelErrCode;
            updateRecord.ErrMsg = channelErrMsg;
            updateRecord.ChannelMchNo = channelMchNo;
            updateRecord.ChannelIsvNo = channelIsvNo;
            updateRecord.ChannelOrderNo = channelOrderNo;
            updateRecord.ChannelUser = channelUserId;
            updateRecord.PlatformOrderNo = platformOrderNo;
            updateRecord.PlatformMchOrderNo = platformMchOrderNo;
            _payOrderRepository.Update(updateRecord);
            var (result, _) = await _payOrderRepository.SaveChangesWithResultAsync();
            return result;
        }
        /// <summary>
        /// 更新订单状态 【支付中】 --》 【支付成功/支付失败】
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <param name="updateState"></param>
        /// <param name="channelMchNo"></param>
        /// <param name="channelIsvNo"></param>
        /// <param name="channelOrderNo"></param>
        /// <param name="channelUserId"></param>
        /// <param name="platformOrderNo"></param>
        /// <param name="platformMchOrderNo"></param>
        /// <param name="channelErrCode"></param>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        public Task<bool> UpdateIng2SuccessOrFail(string payOrderId, byte updateState, string channelMchNo, string channelIsvNo, string channelOrderNo, string channelUserId, string platformOrderNo, string platformMchOrderNo, string channelErrCode, string channelErrMsg)
        {
            if (updateState == (byte)PayOrderState.STATE_ING)
            {
                return Task.FromResult(true);
            }
            else if (updateState == (byte)PayOrderState.STATE_SUCCESS)
            {
                return UpdateIng2SuccessAsync(payOrderId, channelMchNo, channelIsvNo, channelOrderNo, channelUserId, platformOrderNo, platformMchOrderNo);
            }
            else if (updateState == (byte)PayOrderState.STATE_FAIL)
            {
                return UpdateIng2FailAsync(payOrderId, channelMchNo, channelIsvNo, channelOrderNo, channelUserId, platformOrderNo, platformMchOrderNo, channelErrCode, channelErrMsg);
            }
            return Task.FromResult(false);
        }
        /// <summary>
        /// 更新订单为 超时状态
        /// </summary>
        /// <returns></returns>
        public Task<int> UpdateOrderExpiredAsync()
        {
            // 使用 ExecuteUpdate 直接在数据库中批量更新
            var now = DateTime.Now;
            var updatedCount = _payOrderRepository.GetAll()
                .Where(w => new byte[] { (byte)PayOrderState.STATE_INIT, (byte)PayOrderState.STATE_ING }.Contains(w.State)
                    && w.ExpiredTime < now)
                .UpdateAsync(s => s.SetProperty(p => p.State, p => (byte)PayOrderState.STATE_CLOSED)
                    .SetProperty(p => p.UpdatedAt, now));
            return updatedCount;
        }
        /// <summary>
        /// 更新订单 通知状态 --> 已发送
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateNotifySentAsync(string orderId)
        {
            var updateRecord = await _payOrderRepository.GetByIdAsync(orderId);
            updateRecord.NotifyState = CS.YES;
            _payOrderRepository.Update(updateRecord);
            var (result, _) = await _payOrderRepository.SaveChangesWithResultAsync();
            return result;
        }
        /// <summary>
        /// 更新订单表分账状态为： 等待分账任务处理
        /// </summary>
        /// <param name="payOrder"></param>
        /// <returns></returns>
        public async Task<bool> UpdateDivisionStateAsync(PayOrderDto payOrder)
        {
            var updateRecord = await _payOrderRepository.GetByIdAsync(payOrder.PayOrderId);
            if (updateRecord.DivisionState != (byte)PayOrderDivisionState.DIVISION_STATE_UNHAPPEN)
            {
                return false;
            }
            updateRecord.DivisionState = (byte)PayOrderDivisionState.DIVISION_STATE_WAIT_TASK;
            _payOrderRepository.Update(updateRecord);
            var (result, _) = await _payOrderRepository.SaveChangesWithResultAsync();
            return result;
        }

        /// <summary>
        /// 计算支付订单商家入账金额
        /// 商家订单入账金额 （支付金额 - 手续费 - 退款金额 - 总分账金额）</summary>
        /// <param name="dbPayOrder"></param>
        /// <returns></returns>
        public async Task<long> CalMchIncomeAmountAsync(PayOrderDto dbPayOrder)
        {
            //商家订单入账金额 （支付金额 - 手续费 - 退款金额 - 总分账金额）
            long mchIncomeAmount = dbPayOrder.Amount - dbPayOrder.MchFeeAmount - dbPayOrder.RefundAmount;

            //减去已分账金额
            mchIncomeAmount -= await _payOrderDivisionRecordRepository.SumSuccessDivisionAmountAsync(dbPayOrder.PayOrderId);

            return mchIncomeAmount <= 0 ? 0 : mchIncomeAmount;
        }

        /// <summary>
        /// 支付方式统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <param name="dayStart"></param>
        /// <param name="dayEnd"></param>
        /// <returns></returns>
        private async Task<List<PayTypeCountDto>> PayTypeCountAsync(string mchNo, string agentNo, DateTime? dayStart, DateTime? dayEnd)
        {
            var result = await _payOrderRepository.GetAllAsNoTracking()
                .Where(w => (new List<byte> { (byte)PayOrderState.STATE_SUCCESS, (byte)PayOrderState.STATE_REFUND }).Contains(w.State))
                .WhereIfNotEmpty(mchNo, w => w.MchNo.Equals(mchNo))
                .WhereIfNotEmpty(agentNo, w => w.AgentNo.Equals(agentNo))
                .WhereIfNotNull(dayStart, w => w.CreatedAt >= dayStart)
                .WhereIfNotNull(dayEnd, w => w.CreatedAt <= dayEnd)
                .GroupBy(g => g.WayType)
                .Select(s => new PayTypeCountDto
                {
                    WayType = s.Key,
                    TypeCount = s.Count(),
                    TypeAmount = Decimal.Round(s.Sum(s => s.Amount) / 100M, 2, MidpointRounding.AwayFromZero)
                })
                .ToListAsync();
            return result;
        }

        /// <summary>
        /// 成功、退款订单统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <param name="dayStart"></param>
        /// <param name="dayEnd"></param>
        /// <returns></returns>
        private async Task<IEnumerable<(string GroupDate, decimal PayAmount, int PayCount, decimal RefundAmount)>> SelectOrderCountAsync(string mchNo, string agentNo, DateTime? dayStart, DateTime? dayEnd)
        {
            var ordercounts = await _payOrderRepository.GetAllAsNoTracking()
                .Where(w => (new List<byte> { (byte)PayOrderState.STATE_SUCCESS, (byte)PayOrderState.STATE_REFUND }).Contains(w.State))
                .WhereIfNotEmpty(mchNo, w => w.MchNo.Equals(mchNo))
                .WhereIfNotEmpty(agentNo, w => w.AgentNo.Equals(agentNo))
                .WhereIfNotNull(dayStart, w => w.CreatedAt >= dayStart)
                .WhereIfNotNull(dayEnd, w => w.CreatedAt <= dayEnd)
                .GroupBy(g => g.CreatedAt.Value.Date)
                .Select(s => new
                {
                    GroupDate = s.Key.ToString("yyyy-MM-dd"),
                    PayAmount = (s.Sum(s => s.Amount) - s.Sum(s => s.RefundAmount)),
                    PayCount = s.Count(),
                    RefundAmount = s.Sum(s => s.RefundAmount)
                })
                .ToListAsync();
            var result = ordercounts.Select(s => (s.GroupDate, Decimal.Round(s.PayAmount / 100M, 2, MidpointRounding.AwayFromZero), s.PayCount, Decimal.Round(s.RefundAmount / 100M, 2, MidpointRounding.AwayFromZero)));
            return result;
        }

        /// <summary>
        /// 支付订单统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <param name="dayStart"></param>
        /// <param name="dayEnd"></param>
        /// <returns></returns>
        private async Task<List<(string GroupDate, long PayAmount, int PayCount)>> SelectPayOrderCountAsync(string mchNo, string agentNo, DateTime? dayStart, DateTime? dayEnd)
        {
            var payOrders = await _payOrderRepository.GetAllAsNoTracking()
                .Where(w => (new List<byte> { (byte)PayOrderState.STATE_SUCCESS, (byte)PayOrderState.STATE_REFUND }).Contains(w.State))
                .WhereIfNotEmpty(mchNo, w => w.MchNo.Equals(mchNo))
                .WhereIfNotEmpty(agentNo, w => w.AgentNo.Equals(agentNo))
                .WhereIfNotNull(dayStart, w => w.CreatedAt >= dayStart)
                .WhereIfNotNull(dayEnd, w => w.CreatedAt <= dayEnd)
                .GroupBy(g => g.CreatedAt.Value.Date)
                .Select(s => new
                {
                    GroupDate = s.Key.ToString("yyyy-MM-dd"),
                    PayAmount = s.Sum(s => s.Amount),
                    PayCount = s.Count()
                })
                .ToListAsync();
            var result = payOrders.Select(s => (s.GroupDate, s.PayAmount, s.PayCount)).ToList();
            return result;
        }
        /// <summary>
        /// 退款订单统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <param name="dayStart"></param>
        /// <param name="dayEnd"></param>
        /// <returns></returns>
        private async Task<List<(string GroupDate, long RefundAmount, int RefundCount)>> SelectRefundOrderCountAsync(string mchNo, string agentNo, DateTime? dayStart, DateTime? dayEnd)
        {
            var refundOrders = await _refundOrderRepository.GetAllAsNoTracking()
                .Where(w => (new List<byte> { (byte)RefundOrderState.STATE_SUCCESS }).Contains(w.State))
                .WhereIfNotEmpty(mchNo, w => w.MchNo.Equals(mchNo))
                .WhereIfNotEmpty(agentNo, w => w.AgentNo.Equals(agentNo))
                .WhereIfNotNull(dayStart, w => w.CreatedAt >= dayStart)
                .WhereIfNotNull(dayEnd, w => w.CreatedAt <= dayEnd)
                .GroupBy(g => g.CreatedAt.Value.Date)
                .Select(s => new
                {
                    GroupDate = s.Key.ToString("yyyy-MM-dd"),
                    RefundCount = s.Count(),
                    RefundAmount = s.Sum(s => s.RefundAmount)
                })
                .ToListAsync();
            var result = refundOrders.Select(s => (s.GroupDate, s.RefundAmount, s.RefundCount)).ToList();
            return result;
        }

        /// <summary>
        /// 服务商/代理商/商户统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <returns></returns>
        public async Task<JObject> MainPageIsvAndMchCountAsync(string mchNo, string agentNo)
        {
            JObject result = new JObject();
            // 商户总数
            var mchCounts = await _mchInfoRepository.GetAllAsNoTracking()
                .WhereIfNotEmpty(mchNo, w => w.MchNo.Equals(mchNo))
                .WhereIfNotEmpty(agentNo, w => w.AgentNo.Equals(agentNo))
                .Select(w => new
                {
                    IsIsvSub = w.Type == CS.MCH_TYPE_ISVSUB ? 1 : 0,
                    IsNormal = w.Type == CS.MCH_TYPE_NORMAL ? 1 : 0
                })
                .GroupBy(g => 1) // 常量分组，将所有数据分为一组
                .Select(g => new
                {
                    TotalCount = g.Count(),
                    IsvSubMchCount = g.Sum(x => x.IsIsvSub),
                    NormalMchCount = g.Sum(x => x.IsNormal)
                })
                .FirstOrDefaultAsync() ?? new { TotalCount = 0, IsvSubMchCount = 0, NormalMchCount = 0 };

            int mchCount = mchCounts.TotalCount;
            int isvSubMchCount = mchCounts.IsvSubMchCount;
            int normalMchCount = mchCounts.NormalMchCount;

            int agentCount = 0;

            if (string.IsNullOrWhiteSpace(agentNo))
            {
                // 代理商总数
                var agentInfos = _agentInfoRepository.GetAllAsNoTracking()
                    .WhereIfNotEmpty(agentNo, w => w.AgentNo.Equals(agentNo));
                agentCount = await agentInfos.SafeCountAsync();
            }
            else
            {
                var subAgentInfos = GetSons(_agentInfoRepository.GetAllAsNoTracking(), agentNo);
                agentCount = subAgentInfos.Count();
            }

            // 服务商总数
            var isvInfos = _isvInfoRepository.GetAllAsNoTracking();
            int isvCount = await isvInfos.SafeCountAsync();
            if (string.IsNullOrWhiteSpace(mchNo))
            {
#if DEBUG
                // 生成虚拟数据
                isvSubMchCount = isvSubMchCount < 10 ? Random.Shared.Next(0, 500) : isvSubMchCount;
                normalMchCount = normalMchCount < 10 ? Random.Shared.Next(0, 500) : normalMchCount;
                agentCount = agentCount < 10 ? Random.Shared.Next(0, 500) : agentCount;
                agentCount = isvCount < 10 ? Random.Shared.Next(0, 500) : isvCount;
#endif
                mchCount = isvSubMchCount + normalMchCount;
                result.Add("isvSubMchCount", isvSubMchCount);
                result.Add("normalMchCount", normalMchCount);
                result.Add("totalMch", mchCount);
                result.Add("totalAgent", agentCount);
                if (string.IsNullOrWhiteSpace(agentNo))
                {
                    result.Add("totalIsv", isvCount);
                }
            }
            return result;
        }

        /// <summary>
        /// 今日/昨日交易统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <returns></returns>
        public async Task<JObject> MainPagePayDayCountAsync(string mchNo, string agentNo, DateTime? day)
        {
            DateTime? dayStart = day;
            DateTime? dayEnd = day?.AddDays(1).AddSeconds(-1);
            JObject json = new JObject();
            int allCount = 0;
            var payStats = await _payOrderRepository.GetAllAsNoTracking()
                //.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS))
                .WhereIfNotEmpty(mchNo, w => w.MchNo.Equals(mchNo))
                .WhereIfNotEmpty(agentNo, w => w.AgentNo.Equals(agentNo))
                .WhereIfNotNull(dayStart, w => w.CreatedAt >= dayStart)
                .WhereIfNotNull(dayEnd, w => w.CreatedAt <= dayEnd)
                .GroupBy(x => 1) // 常量分组
                .Select(g => new
                {
                    AllCount = g.Count(),
                    PayAmount = g.Where(w => w.State == (byte)PayOrderState.STATE_SUCCESS || w.State == (byte)PayOrderState.STATE_REFUND).Sum(w => w.Amount),
                    PayCount = g.Count(w => w.State == (byte)PayOrderState.STATE_SUCCESS || w.State == (byte)PayOrderState.STATE_REFUND)
                })
                .FirstOrDefaultAsync() ?? new { AllCount = 0, PayAmount = 0L, PayCount = 0 };

            var refundStats = await _refundOrderRepository.GetAllAsNoTracking()
                //.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS))
                .WhereIfNotEmpty(mchNo, w => w.MchNo.Equals(mchNo))
                .WhereIfNotEmpty(agentNo, w => w.AgentNo.Equals(agentNo))
                .WhereIfNotNull(dayStart, w => w.CreatedAt >= dayStart)
                .WhereIfNotNull(dayEnd, w => w.CreatedAt <= dayEnd)
                .GroupBy(x => 1) // 常量分组
                .Select(g => new
                {
                    AllCount = g.Count(),
                    RefundAmount = g.Where(w => w.State == (byte)RefundOrderState.STATE_SUCCESS)
                                   .Sum(w => w.RefundAmount),
                    RefundCount = g.Count(w => w.State == (byte)RefundOrderState.STATE_SUCCESS)
                })
                .FirstOrDefaultAsync() ?? new { AllCount = 0, RefundAmount = 0L, RefundCount = 0 };

            var payAmount = payStats.PayAmount;
            var payCount = payStats.PayCount;
            var refundAmount = refundStats.RefundAmount;
            var refundCount = refundStats.RefundCount;

#if DEBUG
            // 生成虚拟数据
            payAmount = payAmount <= 0 ? Random.Shared.Next(0, 1000000) : payAmount;
            payCount = payCount <= 0 ? Random.Shared.Next(0, 1000) : payCount;
            refundAmount = refundAmount <= 0 ? Random.Shared.Next(0, 500000) : refundAmount;
            refundCount = refundCount <= 0 ? Random.Shared.Next(0, 500) : refundCount;
#endif
            allCount = payCount + refundCount;
            json.Add("dayCount", JObject.FromObject(new
            {
                allCount = allCount,
                payAmount = Decimal.Round(payAmount / 100M, 2, MidpointRounding.AwayFromZero),
                payCount = payCount,
                refundAmount = Decimal.Round(refundAmount / 100M, 2, MidpointRounding.AwayFromZero),
                refundCount = refundCount,
            }));
            return json;
        }

        #region 获取所有下级代理商
        private static IEnumerable<AgentInfo> GetSons(IQueryable<AgentInfo> list, string pid)
        {
            var query = list.Where(p => p.AgentNo == pid).ToList();
            var list2 = query.Concat(GetSonList(list, pid));
            return list2;
        }

        private static IEnumerable<AgentInfo> GetSonList(IQueryable<AgentInfo> list, string pid)
        {
            var query = list.Where(p => p.Pid == pid).ToList();
            return query.ToList().Concat(query.ToList().SelectMany(t => GetSonList(list, t.AgentNo)));
        }
        #endregion

        /// <summary>
        /// 近期交易统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <param name="recentDay"></param>
        /// <returns></returns>
        public async Task<JObject> MainPagePayTrendCountAsync(string mchNo, string agentNo, int recentDay)
        {
            // 查询支付的记录
            var dayStart = DateTime.Today.AddDays(-(recentDay - 1));
            var dayEnd = DateTime.Today.AddDays(1).AddSeconds(-1);
            var payOrderList = await SelectPayOrderCountAsync(mchNo, agentNo, dayStart, dayEnd);
            // 生成前端返回参数类型
            List<string> dateList = new List<string>();
            List<string> payAmountList = new List<string>();
            for (DateTime dt = Convert.ToDateTime(dayStart); dt <= Convert.ToDateTime(dayEnd); dt = dt.AddDays(1))
            {
                var item = payOrderList.FirstOrDefault(x => x.GroupDate.Equals(dt.ToString("yyyy-MM-dd")));
                dateList.Add(dt.ToString("MM-dd"));
#if DEBUG
                // 生成虚拟数据
                item.PayAmount = item.PayAmount <= 0 ? Random.Shared.Next(0, 1000000) : item.PayAmount;
#endif
                payAmountList.Add(AmountUtil.ConvertCent2Dollar(item.PayAmount, 2, MidpointRounding.AwayFromZero));
            }
            JObject result = new JObject();
            result.Add("dateList", JToken.FromObject(dateList));
            result.Add("payAmountList", JToken.FromObject(payAmountList));
            return result;
        }
        /// <summary>
        /// 首页支付统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <param name="createdStart"></param>
        /// <param name="createdEnd"></param>
        /// <returns></returns>
        public async Task<JObject> MainPagePayCountAsync(string mchNo, string agentNo, string createdStart, string createdEnd)
        {
            int daySpace = 6; // 默认最近七天（含当天）
            if (!DateTime.TryParse(createdStart, out DateTime dayStart) || !DateTime.TryParse(createdEnd, out DateTime dayEnd))
            {
                DateTime today = DateTime.Today;
                dayStart = today.AddDays(-daySpace);
                dayEnd = today.AddDays(1).AddSeconds(-1);
            }
            else
            {
                // 计算两时间间隔天数
                daySpace = dayEnd.Subtract(dayStart).Days;
            }

            // 查询支付的记录
            var payOrderList = await SelectPayOrderCountAsync(mchNo, agentNo, dayStart, dayEnd);
            var refundOrderList = await SelectRefundOrderCountAsync(mchNo, agentNo, dayStart, dayEnd);
            // 生成前端返回参数类型
            List<string> resDateArr = new List<string>();
            List<string> resPayAmountArr = new List<string>();
            List<string> resPayCountArr = new List<string>();
            List<string> resRefAmountArr = new List<string>();
            for (DateTime dt = Convert.ToDateTime(dayStart); dt <= Convert.ToDateTime(dayEnd); dt = dt.AddDays(1))
            {
                var pay = payOrderList.FirstOrDefault(x => x.GroupDate.Equals(dt.ToString("yyyy-MM-dd")));
                var refund = refundOrderList.FirstOrDefault(x => x.GroupDate.Equals(dt.ToString("yyyy-MM-dd")));
#if DEBUG
                // 生成虚拟数据
                pay.PayAmount = pay.PayAmount <= 0 ? Random.Shared.Next(0, 1000000) : pay.PayAmount;
                pay.PayCount = pay.PayCount <= 0 ? Random.Shared.Next(0, 1000) : pay.PayCount;
                refund.RefundAmount = refund.RefundAmount <= 0 ? Random.Shared.Next(0, 500000) : refund.RefundAmount;
#endif
                resDateArr.Add(dt.ToString("yyyy-MM-dd"));
                resPayAmountArr.Add(AmountUtil.ConvertCent2Dollar(pay.PayAmount, 2, MidpointRounding.AwayFromZero));
                resPayCountArr.Add(pay.PayCount.ToString());
                resRefAmountArr.Add(AmountUtil.ConvertCent2Dollar(refund.RefundAmount, 2, MidpointRounding.AwayFromZero));
            }

            JObject result = new JObject();
            result.Add("resDateArr", JToken.FromObject(resDateArr));
            result.Add("resPayAmountArr", JToken.FromObject(resPayAmountArr));
            result.Add("resPayCountArr", JToken.FromObject(resPayCountArr));
            result.Add("resRefAmountArr", JToken.FromObject(resRefAmountArr));
            return result;
        }

        /// <summary>
        /// 首页支付类型统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <param name="createdStart"></param>
        /// <param name="createdEnd"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PayTypeCountDto>> MainPagePayTypeCountAsync(string mchNo, string agentNo, string createdStart, string createdEnd)
        {
            if (!DateTime.TryParse(createdStart, out DateTime dayStart) || !DateTime.TryParse(createdEnd, out DateTime dayEnd))
            {
                DateTime today = DateTime.Today; // 当前日期
                dayStart = today.AddDays(-6); // 一周前日期
                dayEnd = today.AddDays(1).AddSeconds(-1);
            }
            // 统计列表
            var payCountMap = await PayTypeCountAsync(mchNo, agentNo, dayStart, dayEnd);

            // 支付方式名称标注
            foreach (var payCount in payCountMap)
            {
                payCount.TypeName = payCount.WayType.ToEnum<PayWayType>().GetDescriptionOrDefault("未知");
            }

#if DEBUG
            // 生成虚拟数据
            if (payCountMap?.Count <= 0)
            {
                // 得到所有支付方式
                payCountMap = await _payWayRepository.GetAllAsNoTracking().GroupBy(g => g.WayType).Select(s => new PayTypeCountDto()
                {
                    WayType = s.Key,
                    TypeName = s.Key.ToEnum<PayWayType>().GetDescriptionOrDefault("未知"),
                    TypeCount = Random.Shared.Next(0, 100),
                    TypeAmount = Decimal.Round(Random.Shared.Next(10000, 100000) / 100M, 2, MidpointRounding.AwayFromZero),
                }).ToListAsync();
            }
#endif

            // 返回数据列
            return payCountMap.OrderBy(o => (int)Enum.Parse<PayWayType>(o.WayType));
        }
    }
}
