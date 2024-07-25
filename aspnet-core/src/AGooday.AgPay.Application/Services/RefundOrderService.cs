using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Newtonsoft.Json.Linq;
using System.Data;

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

        public RefundOrderService(IMapper mapper, IMediatorHandler bus,
            IRefundOrderRepository refundOrderRepository,
            IPayOrderRepository payOrderRepository)
            : base(mapper, bus, refundOrderRepository)
        {
            _refundOrderRepository = refundOrderRepository;
            _payOrderRepository = payOrderRepository;
        }

        /// <summary>
        /// 查询商户订单
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="mchRefundNo"></param>
        /// <param name="refundOrderId"></param>
        /// <returns></returns>
        public RefundOrderDto QueryMchOrder(string mchNo, string mchRefundNo, string refundOrderId)
        {
            if (string.IsNullOrEmpty(refundOrderId))
            {
                var entity = _refundOrderRepository.GetAll().Where(w => w.MchNo.Equals(mchNo) && w.RefundOrderId.Equals(refundOrderId)).FirstOrDefault();
                return _mapper.Map<RefundOrderDto>(entity);
            }
            else if (string.IsNullOrEmpty(mchRefundNo))
            {
                var entity = _refundOrderRepository.GetAll().Where(w => w.MchNo.Equals(mchNo) && w.MchRefundNo.Equals(mchRefundNo)).FirstOrDefault();
                return _mapper.Map<RefundOrderDto>(entity);
            }
            else
            {
                return null;
            }
        }

        public PaginatedList<RefundOrderDto> GetPaginatedData(RefundOrderQueryDto dto)
        {
            var refundOrders = GetRefundOrders(dto).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<RefundOrder>.Create<RefundOrderDto>(refundOrders, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }

        private IQueryable<RefundOrder> GetRefundOrders(RefundOrderQueryDto dto)
        {
            var result = _refundOrderRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (dto.MchType.Equals(null) || w.MchType.Equals(dto.MchType))
                && (string.IsNullOrWhiteSpace(dto.IfCode) || w.WayCode.Equals(dto.IfCode))
                && (string.IsNullOrWhiteSpace(dto.WayCode) || w.WayCode.Equals(dto.WayCode))
                && (string.IsNullOrWhiteSpace(dto.RefundOrderId) || w.RefundOrderId.Equals(dto.RefundOrderId))
                && (string.IsNullOrWhiteSpace(dto.MchRefundNo) || w.MchRefundNo.Equals(dto.MchRefundNo))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (string.IsNullOrWhiteSpace(dto.UnionOrderId) || w.PayOrderId.Equals(dto.UnionOrderId)
                || w.RefundOrderId.Equals(dto.UnionOrderId) || w.MchRefundNo.Equals(dto.UnionOrderId)
                || w.ChannelPayOrderNo.Equals(dto.UnionOrderId) || w.ChannelOrderNo.Equals(dto.UnionOrderId))// 三合一订单
                && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
                && (dto.CreatedEnd.Equals(null) || w.CreatedAt <= dto.CreatedEnd));
            return result;
        }

        public JObject Statistics(RefundOrderQueryDto dto)
        {
            var refundOrders = GetRefundOrders(dto);
            var allRefundAmount = refundOrders.Sum(s => s.RefundAmount);
            var allRefundCount = refundOrders.Count();
            var refund = refundOrders.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS));
            var refundFeeAmount = refund.Sum(s => s.RefundFeeAmount);
            var refundAmount = refund.Sum(s => s.RefundAmount);
            var refundCount = refund.Count();
            JObject result = new JObject();
            result.Add("allRefundAmount", Decimal.Round(allRefundAmount / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("allRefundCount", allRefundCount);
            result.Add("refundAmount", Decimal.Round(refundAmount / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("refundCount", refundCount);
            result.Add("refundFeeAmount", refundFeeAmount);
            result.Add("round", Math.Round(allRefundCount > 0 ? refundCount / Convert.ToDecimal(allRefundCount) : 0M, 2, MidpointRounding.AwayFromZero));
            return result;
        }

        public bool IsExistOrderByMchOrderNo(string mchNo, string mchRefundNo)
        {
            return _refundOrderRepository.IsExistOrderByMchOrderNo(mchNo, mchRefundNo);
        }
        public bool IsExistRefundingOrder(string payOrderId)
        {
            return _refundOrderRepository.IsExistRefundingOrder(payOrderId);
        }

        public long SumSuccessRefundAmount(string payOrderId)
        {
            return _refundOrderRepository.GetAll()
                .Where(w => w.PayOrderId.Equals(payOrderId)
                && w.State.Equals((byte)RefundOrderState.STATE_SUCCESS))
                .Sum(s => s.RefundAmount);
        }

        /// <summary>
        /// 更新退款单状态 【退款单生成】 --》 【退款中】
        /// </summary>
        /// <param name="refundOrderId"></param>
        /// <param name="channelOrderNo"></param>
        /// <returns></returns>
        public bool UpdateInit2Ing(string refundOrderId, string channelOrderNo)
        {
            var updateRecord = _refundOrderRepository.GetById(refundOrderId);
            if (updateRecord.State != (byte)RefundOrderState.STATE_INIT)
            {
                return false;
            }

            updateRecord.State = (byte)RefundOrderState.STATE_ING;
            updateRecord.ChannelOrderNo = channelOrderNo;
            _refundOrderRepository.Update(updateRecord);
            return _refundOrderRepository.SaveChanges(out int _);
        }
        /// <summary>
        /// 更新退款单状态 【退款中】 --》 【退款成功】
        /// </summary>
        /// <param name="refundOrderId"></param>
        /// <param name="channelOrderNo"></param>
        /// <returns></returns>
        public bool UpdateIng2Success(string refundOrderId, string channelOrderNo)
        {
            var updateRecord = _refundOrderRepository.GetById(refundOrderId);
            if (updateRecord.State != (byte)RefundOrderState.STATE_ING)
            {
                return false;
            }

            //1. 更新退款订单表数据
            updateRecord.State = (byte)RefundOrderState.STATE_SUCCESS;
            updateRecord.ChannelOrderNo = channelOrderNo;
            updateRecord.SuccessTime = DateTime.Now;
            _refundOrderRepository.Update(updateRecord);
            if (!_refundOrderRepository.SaveChanges(out int _))
            {
                return false;
            }
            //2. 更新订单表数据（更新退款次数,退款状态,如全额退款更新支付状态为已退款）
            if (!UpdateRefundAmountAndCount(updateRecord.PayOrderId, updateRecord.RefundAmount, updateRecord.RefundFeeAmount))
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
        public bool UpdateRefundAmountAndCount(string payOrderId, long currentRefundAmount, long currentRefundFeeAmount)
        {
            var payOrder = _payOrderRepository.GetById(payOrderId);
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
            return _payOrderRepository.SaveChanges(out int _);
        }
        /// <summary>
        /// 更新退款单状态 【退款中】 --》 【退款失败】
        /// </summary>
        /// <param name="refundOrderId"></param>
        /// <param name="channelOrderNo"></param>
        /// <param name="channelErrCode"></param>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        public bool UpdateIng2Fail(string refundOrderId, string channelOrderNo, string channelErrCode, string channelErrMsg)
        {
            var updateRecord = _refundOrderRepository.GetById(refundOrderId);
            if (updateRecord.State != (byte)RefundOrderState.STATE_ING)
            {
                return false;
            }

            updateRecord.State = (byte)RefundOrderState.STATE_FAIL;
            updateRecord.ErrCode = channelErrCode;
            updateRecord.ErrMsg = channelErrMsg;
            updateRecord.ChannelOrderNo = channelOrderNo;
            _refundOrderRepository.Update(updateRecord);
            return _refundOrderRepository.SaveChanges(out int _);
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
        public bool UpdateIng2SuccessOrFail(string refundOrderId, byte updateState, string channelOrderNo, string channelErrCode, string channelErrMsg)
        {
            if (updateState == (byte)RefundOrderState.STATE_ING)
            {
                return true;
            }
            else if (updateState == (byte)RefundOrderState.STATE_SUCCESS)
            {
                return UpdateIng2Success(refundOrderId, channelOrderNo);
            }
            else if (updateState == (byte)RefundOrderState.STATE_FAIL)
            {
                return UpdateIng2Fail(refundOrderId, channelOrderNo, channelErrCode, channelErrMsg);
            }
            return false;

        }
        /// <summary>
        /// 更新退款单为 关闭状态
        /// </summary>
        /// <returns></returns>
        public int UpdateOrderExpired()
        {
            var updateRecords = _refundOrderRepository.GetAll()
                .Where(w => (new List<byte>() { (byte)RefundOrderState.STATE_INIT, (byte)RefundOrderState.STATE_ING }).Contains(w.State)
                && w.ExpiredTime < DateTime.Now);
            foreach (var refundOrder in updateRecords)
            {
                refundOrder.State = (byte)PayOrderState.STATE_CLOSED;
                _refundOrderRepository.Update(refundOrder);
            }
            return _refundOrderRepository.SaveChanges();
        }
    }
}
