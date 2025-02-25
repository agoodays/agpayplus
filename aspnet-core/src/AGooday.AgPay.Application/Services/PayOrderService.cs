﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
            var entity = await _payOrderRepository.GetAllAsNoTracking().Where(w => w.MchNo.Equals(mchNo)
            && (w.PayOrderId.Equals(payOrderId) || w.MchOrderNo.Equals(mchOrderNo))).FirstOrDefaultAsync();
            return _mapper.Map<PayOrderDto>(entity);
        }

        public Task<PaginatedList<PayOrderDto>> GetPaginatedDataAsync(PayOrderQueryDto dto)
        {
            var query = GetPayOrders(dto).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<PayOrder>.CreateAsync<PayOrderDto>(query, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }

        private IQueryable<PayOrder> GetPayOrders(PayOrderQueryDto dto)
        {
            var result = _payOrderRepository.GetAllAsNoTracking()
                 .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                 && (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                 && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                 && (dto.MchType.Equals(null) || w.MchType.Equals(dto.MchType))
                 && (string.IsNullOrWhiteSpace(dto.IfCode) || w.IfCode.Equals(dto.IfCode))
                 && (string.IsNullOrWhiteSpace(dto.WayCode) || w.WayCode.Equals(dto.WayCode))
                 && (string.IsNullOrWhiteSpace(dto.MchOrderNo) || w.MchOrderNo.Equals(dto.MchOrderNo))
                 && (dto.State.Equals(null) || w.State.Equals(dto.State))
                 && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                 && (dto.StoreId.Equals(null) || w.StoreId.Equals(dto.StoreId))
                 && (string.IsNullOrWhiteSpace(dto.StoreName) || w.StoreName.Equals(dto.StoreName))
                 && (dto.DivisionState.Equals(null) || w.DivisionState.Equals(dto.DivisionState))
                 && (string.IsNullOrWhiteSpace(dto.PayOrderId) || w.PayOrderId.Equals(dto.PayOrderId))
                 && (string.IsNullOrWhiteSpace(dto.MchOrderNo) || w.MchOrderNo.Equals(dto.MchOrderNo))
                 && (string.IsNullOrWhiteSpace(dto.ChannelOrderNo) || w.ChannelOrderNo.Equals(dto.ChannelOrderNo))
                 && (string.IsNullOrWhiteSpace(dto.PlatformOrderNo) || w.PlatformOrderNo.Equals(dto.PlatformOrderNo))
                 && (string.IsNullOrWhiteSpace(dto.PlatformMchOrderNo) || w.PlatformMchOrderNo.Equals(dto.PlatformMchOrderNo))
                 && (string.IsNullOrWhiteSpace(dto.UnionOrderId) || w.PayOrderId.Equals(dto.UnionOrderId) || w.MchOrderNo.Equals(dto.UnionOrderId) || w.ChannelOrderNo.Equals(dto.UnionOrderId))
                 && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
                 && (dto.CreatedEnd.Equals(null) || w.CreatedAt <= dto.CreatedEnd));
            return result;
        }

        public async Task<JObject> StatisticsAsync(PayOrderQueryDto dto)
        {
            var payOrders = GetPayOrders(dto);
            var allPayAmount = await payOrders.SumAsync(s => s.Amount);
            var allPayCount = await payOrders.CountAsync();
            var failPay = payOrders.Where(w => !(w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND)));
            var failPayAmount = await failPay.SumAsync(s => s.Amount);
            var failPayCount = await failPay.CountAsync();
            // 成交金额: 支付成功的订单金额，包含部分退款及全额退款的订单
            var pay = payOrders.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));
            var mchFeeAmount = await pay.SumAsync(s => s.MchFeeAmount);
            var payAmount = await pay.SumAsync(s => s.Amount);
            var payCount = await pay.CountAsync();
            var refund = payOrders.Where(w => w.RefundState.Equals((byte)PayOrderRefund.REFUND_STATE_SUB) || w.RefundState.Equals((byte)PayOrderRefund.REFUND_STATE_ALL));
            var refundAmount = await refund.SumAsync(s => s.Amount);
            var refundCount = await refund.CountAsync();
            JObject result = new JObject();
            result.Add("allPayAmount", Decimal.Round(allPayAmount / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("allPayCount", allPayCount);
            result.Add("failPayAmount", Decimal.Round(failPayAmount / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("failPayCount", failPayCount);
            result.Add("mchFeeAmount", Decimal.Round(mchFeeAmount / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("payAmount", Decimal.Round(payAmount / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("payCount", payCount);
            result.Add("refundAmount", Decimal.Round(refundAmount / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("refundCount", refundCount);
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
                    && w.ExpiredTime < DateTime.Now)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.State, p => (byte)PayOrderState.STATE_CLOSED)
                    .SetProperty(p => p.UpdatedAt, now));
            return updatedCount;

            //// 使用 ExecuteUpdate 直接在数据库中批量更新
            //var now = DateTime.Now;
            //var updatedCount = RelationalQueryableExtensions.ExecuteUpdateAsync(
            //    _payOrderRepository.GetAll()
            //        .Where(w => new byte[] { (byte)PayOrderState.STATE_INIT, (byte)PayOrderState.STATE_ING }.Contains(w.State)
            //            && w.ExpiredTime < DateTime.Now),
            //    s => s
            //        .SetProperty(p => p.State, p => (byte)PayOrderState.STATE_CLOSED)
            //        .SetProperty(p => p.UpdatedAt, now));
            //return updatedCount;

            //var updateRecords = _payOrderRepository.GetAll()
            //    .Where(w => (new List<byte>() { (byte)PayOrderState.STATE_INIT, (byte)PayOrderState.STATE_ING }).Contains(w.State)
            //    && w.ExpiredTime < DateTime.Now);
            //if (updateRecords.Any())
            //{
            //    foreach (var payOrder in updateRecords)
            //    {
            //        payOrder.State = (byte)PayOrderState.STATE_CLOSED;
            //    }
            //    _payOrderRepository.UpdateRange(updateRecords);
            //    return _payOrderRepository.SaveChangesAsync();
            //}
            //return Task.FromResult(0);
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
        /// <param name="state"></param>
        /// <param name="refundState"></param>
        /// <param name="dayStart"></param>
        /// <param name="dayEnd"></param>
        /// <returns></returns>
        private async Task<List<PayTypeCountDto>> PayTypeCountAsync(string mchNo, string agentNo, DateTime? dayStart, DateTime? dayEnd)
        {
            var payOrders = await _payOrderRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(mchNo) || w.MchNo.Equals(mchNo))
                && (string.IsNullOrWhiteSpace(agentNo) || w.AgentNo.Equals(agentNo))
                && (new List<byte> { (byte)PayOrderState.STATE_SUCCESS, (byte)PayOrderState.STATE_REFUND }).Contains(w.State)
                && (dayStart.Equals(null) || w.CreatedAt >= dayStart)
                && (dayEnd.Equals(null) || w.CreatedAt <= dayEnd)).ToListAsync();
            var result = payOrders.GroupBy(g => g.WayType, (key, group) => new { WayType = key, Items = group.AsEnumerable() })
               .Select(s => new PayTypeCountDto
               {
                   WayType = s.WayType,
                   TypeCount = s.Items.Count(),
                   TypeAmount = Decimal.Round(s.Items.Sum(s => s.Amount) / 100M, 2, MidpointRounding.AwayFromZero)
               }).ToList();
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
        private IEnumerable<(string GroupDate, decimal PayAmount, int PayCount, decimal RefundAmount)> SelectOrderCount(string mchNo, string agentNo, DateTime? dayStart, DateTime? dayEnd)
        {
            var ordercounts = _payOrderRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(mchNo) || w.MchNo.Equals(mchNo))
                && (string.IsNullOrWhiteSpace(agentNo) || w.AgentNo.Equals(agentNo))
                && (new List<byte> { (byte)PayOrderState.STATE_SUCCESS, (byte)PayOrderState.STATE_REFUND }).Contains(w.State)
                && (dayStart.Equals(null) || w.CreatedAt >= dayStart)
                && (dayEnd.Equals(null) || w.CreatedAt <= dayEnd))
                .GroupBy(g => g.CreatedAt.Value.ToString("yyyy-MM-dd"), (key, group) => new { GroupDate = key, Items = group.AsEnumerable() })
                .Select(s => new
                {
                    GroupDate = s.GroupDate,
                    PayAmount = (s.Items.Sum(s => s.Amount) - s.Items.Sum(s => s.RefundAmount)),
                    PayCount = s.Items.Count(),
                    RefundAmount = s.Items.Sum(s => s.RefundAmount)
                });
            var result = ordercounts.AsEnumerable().Select(s => (s.GroupDate, Decimal.Round(s.PayAmount / 100M, 2, MidpointRounding.AwayFromZero), s.PayCount, Decimal.Round(s.RefundAmount / 100M, 2, MidpointRounding.AwayFromZero)));
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
                .Where(w => (string.IsNullOrWhiteSpace(mchNo) || w.MchNo.Equals(mchNo))
                && (string.IsNullOrWhiteSpace(agentNo) || w.AgentNo.Equals(agentNo))
                && (new List<byte> { (byte)PayOrderState.STATE_SUCCESS, (byte)PayOrderState.STATE_REFUND }).Contains(w.State)
                && (dayStart.Equals(null) || w.CreatedAt >= dayStart)
                && (dayEnd.Equals(null) || w.CreatedAt <= dayEnd)).ToListAsync();
            var result = payOrders.GroupBy(g => g.CreatedAt.Value.ToString("yyyy-MM-dd"), (key, group) => new { GroupDate = key, Items = group.AsEnumerable() })
                .Select(s => new
                {
                    GroupDate = s.GroupDate,
                    PayAmount = s.Items.Sum(s => s.Amount),
                    PayCount = s.Items.Count()
                }).Select(s => (s.GroupDate, s.PayAmount, s.PayCount)).ToList();
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
                .Where(w => (string.IsNullOrWhiteSpace(mchNo) || w.MchNo.Equals(mchNo))
                && (string.IsNullOrWhiteSpace(agentNo) || w.AgentNo.Equals(agentNo))
                && (new List<byte> { (byte)RefundOrderState.STATE_SUCCESS }).Contains(w.State)
                && (dayStart.Equals(null) || w.CreatedAt >= dayStart)
                && (dayEnd.Equals(null) || w.CreatedAt <= dayEnd)).ToListAsync();
            var result = refundOrders.GroupBy(g => g.CreatedAt.Value.ToString("yyyy-MM-dd"), (key, group) => new { GroupDate = key, Items = group.AsEnumerable() })
                .Select(s => new
                {
                    GroupDate = s.GroupDate,
                    RefundCount = s.Items.Count(),
                    RefundAmount = s.Items.Sum(s => s.RefundAmount)
                }).Select(s => (s.GroupDate, s.RefundAmount, s.RefundCount)).ToList();

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
            var mchInfos = _mchInfoRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(mchNo) || w.MchNo.Equals(mchNo))
                && (string.IsNullOrWhiteSpace(agentNo) || w.AgentNo.Equals(agentNo)));
            int isvSubMchCount = await mchInfos.Where(w => w.Type.Equals(CS.MCH_TYPE_ISVSUB)).CountAsync();
            int normalMchCount = await mchInfos.Where(w => w.Type.Equals(CS.MCH_TYPE_NORMAL)).CountAsync();
            int mchCount = await mchInfos.CountAsync();

            int agentCount = 0;

            if (string.IsNullOrWhiteSpace(agentNo))
            {
                // 代理商总数
                var agentInfos = _agentInfoRepository.GetAllAsNoTracking()
                    .Where(w => (string.IsNullOrWhiteSpace(agentNo) || w.AgentNo.Equals(agentNo)));
                agentCount = await agentInfos.CountAsync();
            }
            else
            {
                var subAgentInfos = GetSons(_agentInfoRepository.GetAllAsNoTracking(), agentNo);
                agentCount = subAgentInfos.Count();
            }

            // 服务商总数
            var isvInfos = _isvInfoRepository.GetAllAsNoTracking();
            int isvCount = await isvInfos.CountAsync();
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
            var payorders = _payOrderRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(mchNo) || w.MchNo.Equals(mchNo))
                && (string.IsNullOrWhiteSpace(agentNo) || w.AgentNo.Equals(agentNo))
                //&& w.State.Equals((byte)PayOrderState.STATE_SUCCESS)
                && (dayStart.Equals(null) || w.CreatedAt >= dayStart)
                && (dayEnd.Equals(null) || w.CreatedAt <= dayEnd));
            allCount += await payorders.CountAsync();
            var pay = payorders.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS) || w.State.Equals((byte)PayOrderState.STATE_REFUND));
            var payAmount = await pay.SumAsync(s => s.Amount);
            var payCount = await pay.CountAsync();

            var refundOrder = _refundOrderRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(mchNo) || w.MchNo.Equals(mchNo))
                && (string.IsNullOrWhiteSpace(agentNo) || w.AgentNo.Equals(agentNo))
                //&& w.State.Equals((byte)RefundOrderState.STATE_SUCCESS)
                && (dayStart.Equals(null) || w.CreatedAt >= dayStart)
                && (dayEnd.Equals(null) || w.CreatedAt <= dayEnd));
            allCount += await refundOrder.CountAsync();
            var refund = refundOrder.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS));
            var refundAmount = await refund.SumAsync(s => s.RefundAmount);
            var refundCount = await refund.CountAsync();

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
