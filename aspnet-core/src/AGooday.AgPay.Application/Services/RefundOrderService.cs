using System.Data;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
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
    /// 退款订单表 服务实现类
    /// </summary>
    public class RefundOrderService : AgPayService<RefundOrderDto, RefundOrder>, IRefundOrderService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IRefundOrderRepository _refundOrderRepository;

        private readonly IPayOrderRepository _payOrderRepository;

        private readonly IPayOrderProfitRepository _payOrderProfitRepository;
        private readonly IAccountBillRepository _accountBillRepository;

        public RefundOrderService(IMapper mapper, IMediatorHandler bus,
            IRefundOrderRepository refundOrderRepository,
            IPayOrderRepository payOrderRepository,
            IPayOrderProfitRepository payOrderProfitRepository,
            IAccountBillRepository accountBillRepository)
            : base(mapper, bus, refundOrderRepository)
        {
            _refundOrderRepository = refundOrderRepository;
            _payOrderRepository = payOrderRepository;
            _payOrderProfitRepository = payOrderProfitRepository;
            _accountBillRepository = accountBillRepository;
        }

        public Task<bool> IsExistOrderByMchOrderNoAsync(string mchNo, string mchRefundNo)
        {
            return _refundOrderRepository.IsExistOrderByMchOrderNoAsync(mchNo, mchRefundNo);
        }
        public Task<bool> IsExistRefundingOrderAsync(string payOrderId)
        {
            return _refundOrderRepository.IsExistRefundingOrderAsync(payOrderId);
        }

        /// <summary>
        /// 查询商户订单
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="mchRefundNo"></param>
        /// <param name="refundOrderId"></param>
        /// <returns></returns>
        public async Task<RefundOrderDto> QueryMchOrderAsync(string mchNo, string mchRefundNo, string refundOrderId)
        {
            var query = _refundOrderRepository.GetAllAsNoTracking()
                .Where(w => w.MchNo.Equals(mchNo)
                && ((!string.IsNullOrEmpty(refundOrderId) && w.RefundOrderId.Equals(refundOrderId))
                || (!string.IsNullOrEmpty(mchRefundNo) && w.MchRefundNo.Equals(mchRefundNo))));
            return await query.FirstOrDefaultProjectToAsync<RefundOrder, RefundOrderDto>(_mapper);
        }

        public Task<PaginatedResult<RefundOrderDto>> GetPaginatedDataAsync(RefundOrderQueryDto dto)
        {
            var query = GetRefundOrders(dto).OrderByDescending(o => o.CreatedAt);
            return query.ToPaginatedResultAsync<RefundOrder, RefundOrderDto>(_mapper, dto.PageNumber, dto.PageSize);
        }

        private IQueryable<RefundOrder> GetRefundOrders(RefundOrderQueryDto dto)
        {
            var result = _refundOrderRepository.GetAllAsNoTracking()
                .WhereIfNotEmpty(dto.MchNo, w => w.MchNo.Equals(dto.MchNo))
                .WhereIfNotEmpty(dto.AgentNo, w => w.AgentNo.Equals(dto.AgentNo))
                .WhereIfNotEmpty(dto.IsvNo, w => w.IsvNo.Equals(dto.IsvNo))
                .WhereIfNotNull(dto.MchType, w => w.MchType.Equals(dto.MchType))
                .WhereIfNotEmpty(dto.IfCode, w => w.IfCode.Equals(dto.IfCode))
                .WhereIfNotEmpty(dto.WayCode, w => w.WayCode.Equals(dto.WayCode))
                .WhereIfNotEmpty(dto.RefundOrderId, w => w.RefundOrderId.Equals(dto.RefundOrderId))
                .WhereIfNotEmpty(dto.MchRefundNo, w => w.MchRefundNo.Equals(dto.MchRefundNo))
                .WhereIfNotNull(dto.State, w => w.State.Equals(dto.State))
                .WhereIfNotEmpty(dto.AppId, w => w.AppId.Equals(dto.AppId))
                .WhereIfNotEmpty(dto.UnionOrderId, w => w.PayOrderId.Equals(dto.UnionOrderId) || w.RefundOrderId.Equals(dto.UnionOrderId) || w.MchRefundNo.Equals(dto.UnionOrderId) || w.ChannelPayOrderNo.Equals(dto.UnionOrderId) || w.ChannelOrderNo.Equals(dto.UnionOrderId))// 三合一订单
                .WhereIfNotNull(dto.CreatedStart, w => w.CreatedAt >= dto.CreatedStart)
                .WhereIfNotNull(dto.CreatedEnd, w => w.CreatedAt <= dto.CreatedEnd);
            return result;
        }

        public async Task<JObject> StatisticsAsync(RefundOrderQueryDto dto)
        {
            // 获取所有统计数据
            var statistics = await GetRefundOrders(dto)
                .GroupBy(x => 1) // 常量分组
                .Select(g => new
                {
                    // 总退款统计
                    AllRefundAmount = g.Sum(o => o.RefundAmount),
                    AllRefundCount = g.Count(),

                    // 成功退款统计
                    RefundFeeAmount = g.Where(o => o.State == (byte)RefundOrderState.STATE_SUCCESS)
                                     .Sum(o => o.RefundFeeAmount),
                    RefundAmount = g.Where(o => o.State == (byte)RefundOrderState.STATE_SUCCESS)
                                  .Sum(o => o.RefundAmount),
                    RefundCount = g.Count(o => o.State == (byte)RefundOrderState.STATE_SUCCESS)
                })
                .FirstOrDefaultAsync();

            // 处理空结果
            statistics ??= new
            {
                AllRefundAmount = 0L,
                AllRefundCount = 0,
                RefundFeeAmount = 0L,
                RefundAmount = 0L,
                RefundCount = 0
            };

            // 计算成功率
            var successRate = statistics.AllRefundCount > 0
                ? Math.Round(statistics.RefundCount / Convert.ToDecimal(statistics.AllRefundCount), 2, MidpointRounding.AwayFromZero)
                : 0M;

            JObject result = new JObject();
            result.Add("allRefundAmount", Decimal.Round(statistics.AllRefundAmount / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("allRefundCount", statistics.AllRefundCount);
            result.Add("refundAmount", Decimal.Round(statistics.RefundAmount / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("refundCount", statistics.RefundCount);
            result.Add("refundFeeAmount", statistics.RefundFeeAmount);
            result.Add("round", successRate);
            return result;
        }

        public Task<long> SumSuccessRefundAmountAsync(string payOrderId)
        {
            return _refundOrderRepository.GetAllAsNoTracking()
                .Where(w => w.PayOrderId.Equals(payOrderId) && w.State.Equals((byte)RefundOrderState.STATE_SUCCESS))
                .SafeSumAsync(s => s.RefundAmount);
        }

        /// <summary>
        /// 更新退款单状态 【退款单生成】 --》 【退款中】
        /// </summary>
        /// <param name="refundOrderId"></param>
        /// <param name="channelOrderNo"></param>
        /// <returns></returns>
        public async Task<bool> UpdateInit2IngAsync(string refundOrderId, string channelOrderNo)
        {
            var updateRecord = await _refundOrderRepository.GetByIdAsync(refundOrderId);
            if (updateRecord.State != (byte)RefundOrderState.STATE_INIT)
            {
                return false;
            }

            updateRecord.State = (byte)RefundOrderState.STATE_ING;
            updateRecord.ChannelOrderNo = channelOrderNo;
            _refundOrderRepository.Update(updateRecord);
            var (result, _) = await _refundOrderRepository.SaveChangesWithResultAsync();
            return result;
        }
        /// <summary>
        /// 更新退款单状态 【退款中】 --》 【退款成功】
        /// </summary>
        /// <param name="refundOrderId"></param>
        /// <param name="channelOrderNo"></param>
        /// <returns></returns>
        public async Task<bool> UpdateIng2SuccessAsync(string refundOrderId, string channelOrderNo)
        {
            var updateRecord = await _refundOrderRepository.GetByIdAsync(refundOrderId);
            if (updateRecord.State != (byte)RefundOrderState.STATE_ING)
            {
                return false;
            }

            //1. 更新退款订单表数据
            updateRecord.State = (byte)RefundOrderState.STATE_SUCCESS;
            updateRecord.ChannelOrderNo = channelOrderNo;
            updateRecord.SuccessTime = DateTime.Now;
            _refundOrderRepository.Update(updateRecord);
            if (!(await _refundOrderRepository.SaveChangesAsync() > 0))
            {
                return false;
            }
            //2. 更新订单表数据（更新退款次数,退款状态,如全额退款更新支付状态为已退款）
            if (!await UpdateRefundAmountAndCountAsync(updateRecord.PayOrderId, updateRecord.RefundAmount, updateRecord.RefundFeeAmount))
            {
                throw new BizException("更新订单数据异常");
            }
            return true;
        }
        /// <summary>
        /// 更新订单退款金额和次数
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <param name="currentRefundAmount"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        public async Task<bool> UpdateRefundAmountAndCountAsync(string payOrderId, long currentRefundAmount, long currentRefundFeeAmount)
        {
            var payOrder = await _payOrderRepository.GetByIdAsync(payOrderId);
            // 成功状态的可退款
            if (payOrder.State != (byte)PayOrderState.STATE_SUCCESS)
            {
                throw new BizException("成功状态才可退款");
            }
            // 已退款金额 + 本次退款金额 小于等于订单金额
            if (payOrder.RefundAmount + currentRefundAmount > payOrder.Amount)
            {
                throw new BizException("已退款金额 + 本次退款金额必须小于等于订单金额");
            }
            payOrder.RefundTimes = ++payOrder.RefundTimes; // 退款次数 +1
            payOrder.RefundState = (byte)(payOrder.RefundAmount + currentRefundAmount >= payOrder.Amount ? PayOrderRefund.REFUND_STATE_ALL : PayOrderRefund.REFUND_STATE_SUB); // 更新是否已全额退款。 此更新需在refund_amount更新之前，否则需要去掉累加逻辑
            payOrder.RefundAmount = payOrder.RefundAmount + currentRefundAmount; // 退款金额累加
            payOrder.MchFeeAmount = payOrder.MchFeeAmount - currentRefundFeeAmount;
            payOrder.State = payOrder.RefundState.Equals((byte)PayOrderRefund.REFUND_STATE_ALL) ? (byte)PayOrderState.STATE_REFUND : payOrder.State; // 更新支付状态是否已退款。 此更新需在refund_state更新之后，如果全额退款则修改支付状态为已退款
            _payOrderRepository.Update(payOrder);
            var (result, _) = await _refundOrderRepository.SaveChangesWithResultAsync();
            return result;
        }
        /// <summary>
        /// 更新退款单状态 【退款中】 --》 【退款失败】
        /// </summary>
        /// <param name="refundOrderId"></param>
        /// <param name="channelOrderNo"></param>
        /// <param name="channelErrCode"></param>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        public async Task<bool> UpdateIng2FailAsync(string refundOrderId, string channelOrderNo, string channelErrCode, string channelErrMsg)
        {
            var updateRecord = await _refundOrderRepository.GetByIdAsync(refundOrderId);
            if (updateRecord.State != (byte)RefundOrderState.STATE_ING)
            {
                return false;
            }

            updateRecord.State = (byte)RefundOrderState.STATE_FAIL;
            updateRecord.ErrCode = channelErrCode;
            updateRecord.ErrMsg = channelErrMsg;
            updateRecord.ChannelOrderNo = channelOrderNo;
            _refundOrderRepository.Update(updateRecord);
            var (result, _) = await _refundOrderRepository.SaveChangesWithResultAsync();
            return result;
        }
        /// <summary>
        /// 更新退款单状态 【退款中】 --》 【退款成功/退款失败】
        /// </summary>
        /// <param name="refundOrderId"></param>
        /// <param name="updateState"></param>
        /// <param name="channelOrderNo"></param>
        /// <param name="channelErrCode"></param>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        public Task<bool> UpdateIng2SuccessOrFailAsync(string refundOrderId, byte updateState, string channelOrderNo, string channelErrCode, string channelErrMsg)
        {
            if (updateState == (byte)RefundOrderState.STATE_ING)
            {
                return Task.FromResult(true);
            }
            else if (updateState == (byte)RefundOrderState.STATE_SUCCESS)
            {
                return UpdateIng2SuccessAsync(refundOrderId, channelOrderNo);
            }
            else if (updateState == (byte)RefundOrderState.STATE_FAIL)
            {
                return UpdateIng2FailAsync(refundOrderId, channelOrderNo, channelErrCode, channelErrMsg);
            }
            return Task.FromResult(false);

        }
        /// <summary>
        /// 更新退款单为 关闭状态
        /// </summary>
        /// <returns></returns>
        public Task<int> UpdateOrderExpiredAsync()
        {
            // 使用 ExecuteUpdate 直接在数据库中批量更新
            var now = DateTime.Now;
            var updatedCount = _refundOrderRepository.GetAll()
                .Where(w => (new List<byte>() { (byte)RefundOrderState.STATE_INIT, (byte)RefundOrderState.STATE_ING }).Contains(w.State)
                && w.ExpiredTime < DateTime.Now)
                .UpdateAsync(s => s.SetProperty(p => p.State, p => (byte)RefundOrderState.STATE_CLOSED)
                    .SetProperty(p => p.UpdatedAt, now));
            return updatedCount;
        }
        /// <summary>
        /// 更新支付订单分润并生成账单
        /// </summary>
        /// <param name="payOrderProfitDtos"></param>
        /// <param name="accountBillDtos"></param>
        /// <returns></returns>
        public async Task<int> UpdatePayOrderProfitAndGenAccountBillAsync(List<PayOrderProfitDto> payOrderProfitDtos, List<AccountBillDto> accountBillDtos)
        {
            var payOrderProfits = _mapper.Map<IEnumerable<PayOrderProfit>>(payOrderProfitDtos);
            _payOrderProfitRepository.UpdateRange(payOrderProfits);
            if (accountBillDtos.Count > 0)
            {
                var accountBills = _mapper.Map<IEnumerable<AccountBill>>(accountBillDtos);
                await _accountBillRepository.AddRangeAsync(accountBills);
            }
            return await _refundOrderRepository.SaveChangesAsync();
        }
    }
}
