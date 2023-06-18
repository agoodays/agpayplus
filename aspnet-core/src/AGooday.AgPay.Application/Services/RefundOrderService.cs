using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 退款订单表 服务实现类
    /// </summary>
    public class RefundOrderService : IRefundOrderService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IRefundOrderRepository _refundOrderRepository;
        private readonly IPayOrderRepository _payOrderRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public RefundOrderService(IRefundOrderRepository refundOrderRepository, IPayOrderRepository payOrderRepository, IMapper mapper, IMediatorHandler bus)
        {
            _refundOrderRepository = refundOrderRepository;
            _payOrderRepository = payOrderRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(RefundOrderDto dto)
        {
            var m = _mapper.Map<RefundOrder>(dto);
            _refundOrderRepository.Add(m);
            _refundOrderRepository.SaveChanges();
        }

        public void Remove(string recordId)
        {
            _refundOrderRepository.Remove(recordId);
            _refundOrderRepository.SaveChanges();
        }

        public void Update(RefundOrderDto dto)
        {
            var m = _mapper.Map<RefundOrder>(dto);
            _refundOrderRepository.Update(m);
            _refundOrderRepository.SaveChanges();
        }

        public RefundOrderDto GetById(string recordId)
        {
            var entity = _refundOrderRepository.GetById(recordId);
            var dto = _mapper.Map<RefundOrderDto>(entity);
            return dto;
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

        public IEnumerable<RefundOrderDto> GetAll()
        {
            var refundOrders = _refundOrderRepository.GetAll();
            return _mapper.Map<IEnumerable<RefundOrderDto>>(refundOrders);
        }

        public PaginatedList<RefundOrderDto> GetPaginatedData(RefundOrderQueryDto dto)
        {
            var refundOrders = _refundOrderRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (dto.MchType.Equals(0) || w.MchType.Equals(dto.MchType))
                && (string.IsNullOrWhiteSpace(dto.WayCode) || w.WayCode.Equals(dto.WayCode))
                && (string.IsNullOrWhiteSpace(dto.RefundOrderId) || w.RefundOrderId.Equals(dto.RefundOrderId))
                && (string.IsNullOrWhiteSpace(dto.MchRefundNo) || w.MchRefundNo.Equals(dto.MchRefundNo))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (string.IsNullOrWhiteSpace(dto.UnionOrderId) || w.PayOrderId.Equals(dto.UnionOrderId)
                || w.RefundOrderId.Equals(dto.UnionOrderId) || w.MchRefundNo.Equals(dto.UnionOrderId)
                || w.ChannelPayOrderNo.Equals(dto.UnionOrderId) || w.ChannelOrderNo.Equals(dto.UnionOrderId))// 三合一订单
                && (dto.CreatedEnd == null || w.CreatedAt < dto.CreatedEnd)
                && (dto.CreatedStart == null || w.CreatedAt >= dto.CreatedStart)
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<RefundOrder>.Create<RefundOrderDto>(refundOrders.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
        public long SumSuccessRefundAmount(string payOrderId)
        {
            return _refundOrderRepository.GetAll().Where(w => w.PayOrderId.Equals(payOrderId) && w.State.Equals((byte)RefundOrderState.STATE_SUCCESS)).Sum(s => s.RefundAmount);
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
            var payOrder = _payOrderRepository.GetById(updateRecord.PayOrderId);
            payOrder.RefundTimes = ++payOrder.RefundTimes; // 退款次数 +1
            payOrder.RefundAmount = payOrder.RefundAmount + updateRecord.RefundAmount; // 退款金额累加
            payOrder.RefundState = (byte)(payOrder.RefundAmount + updateRecord.RefundAmount >= payOrder.Amount ? PayOrderRefund.REFUND_STATE_ALL : PayOrderRefund.REFUND_STATE_SUB); // 更新是否已全额退款。 此更新需在refund_amount更新之前，否则需要去掉累加逻辑
            payOrder.State = payOrder.RefundState.Equals(PayOrderRefund.REFUND_STATE_ALL) ? (byte)PayOrderState.STATE_REFUND : payOrder.State;
            _payOrderRepository.Update(payOrder);
            if (!_payOrderRepository.SaveChanges(out int _))
            {
                throw new BizException("更新订单数据异常");
            }
            return true;
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
            var updateRecords = _refundOrderRepository.GetAll().Where(
                w => (new List<byte>() { (byte)RefundOrderState.STATE_INIT, (byte)RefundOrderState.STATE_ING }).Contains(w.State)
                && w.ExpiredTime < DateTime.Now);
            foreach (var refundOrder in updateRecords)
            {
                refundOrder.State = (byte)PayOrderState.STATE_CLOSED;
                _refundOrderRepository.Update(refundOrder);
            }
            return _refundOrderRepository.SaveChanges();
        }
        public bool IsExistOrderByMchOrderNo(string mchNo, string mchRefundNo)
        {
            return _refundOrderRepository.IsExistOrderByMchOrderNo(mchNo, mchRefundNo);
        }
        public bool IsExistRefundingOrder(string payOrderId)
        {
            return _refundOrderRepository.IsExistRefundingOrder(payOrderId);
        }
    }
}
