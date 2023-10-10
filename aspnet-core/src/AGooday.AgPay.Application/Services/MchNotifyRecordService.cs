using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 商户通知表 服务实现类
    /// </summary>
    public class MchNotifyRecordService : IMchNotifyRecordService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IMchNotifyRecordRepository _mchNotifyRecordRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public MchNotifyRecordService(IMchNotifyRecordRepository mchNotifyRecordRepository, IMapper mapper, IMediatorHandler bus)
        {
            _mchNotifyRecordRepository = mchNotifyRecordRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(MchNotifyRecordDto dto)
        {
            var m = _mapper.Map<MchNotifyRecord>(dto);
            _mchNotifyRecordRepository.Add(m);
            _mchNotifyRecordRepository.SaveChanges();
        }

        public void Remove(long recordId)
        {
            _mchNotifyRecordRepository.Remove(recordId);
            _mchNotifyRecordRepository.SaveChanges();
        }

        public void Update(MchNotifyRecordDto dto)
        {
            var m = _mapper.Map<MchNotifyRecord>(dto);
            _mchNotifyRecordRepository.Update(m);
            _mchNotifyRecordRepository.SaveChanges();
        }

        public MchNotifyRecordDto GetById(long recordId)
        {
            var entity = _mchNotifyRecordRepository.GetById(recordId);
            var dto = _mapper.Map<MchNotifyRecordDto>(entity);
            return dto;
        }

        public IEnumerable<MchNotifyRecordDto> GetAll()
        {
            var mchNotifyRecords = _mchNotifyRecordRepository.GetAll();
            return _mapper.Map<IEnumerable<MchNotifyRecordDto>>(mchNotifyRecords);
        }
        public PaginatedList<MchNotifyRecordDto> GetPaginatedData(MchNotifyQueryDto dto)
        {
            var mchInfos = _mchNotifyRecordRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (string.IsNullOrWhiteSpace(dto.OrderId) || w.OrderId.Equals(dto.OrderId))
                && (string.IsNullOrWhiteSpace(dto.MchOrderNo) || w.MchOrderNo.Equals(dto.MchOrderNo))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (dto.OrderType.Equals(0) || w.OrderType.Equals(dto.OrderType))
                && (dto.State.Equals(0) || w.State.Equals(dto.State))
                && (dto.CreatedStart == null || w.CreatedAt >= dto.CreatedStart)
                && (dto.CreatedEnd == null || w.CreatedAt < dto.CreatedEnd))
                .OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<MchNotifyRecord>.Create<MchNotifyRecordDto>(mchInfos.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
        /// <summary>
        /// 更改为通知中 & 增加允许重发通知次数
        /// </summary>
        /// <param name="notifyId"></param>
        public void UpdateIngAndAddNotifyCountLimit(long notifyId)
        {
            var notify = _mchNotifyRecordRepository.GetById(notifyId);
            notify.NotifyCountLimit += 1;
            notify.State = (byte)MchNotifyRecordState.STATE_ING;
            _mchNotifyRecordRepository.Update(notify);
            _mchNotifyRecordRepository.SaveChanges();
        }

        /// <summary>
        /// 根据订单号和类型查询
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public MchNotifyRecordDto FindByOrderAndType(string orderId, byte orderType)
        {
            var entity = _mchNotifyRecordRepository.GetAll().Where(w => w.OrderId.Equals(orderId) && w.OrderType.Equals(orderType)).FirstOrDefault();
            return _mapper.Map<MchNotifyRecordDto>(entity);
        }

        /// <summary>
        /// 查询支付订单
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <returns></returns>
        public MchNotifyRecordDto FindByPayOrder(string payOrderId)
        {
            return FindByOrderAndType(payOrderId, (byte)MchNotifyRecordType.TYPE_PAY_ORDER);
        }

        /// <summary>
        /// 查询退款订单订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public MchNotifyRecordDto FindByRefundOrder(string orderId)
        {
            return FindByOrderAndType(orderId, (byte)MchNotifyRecordType.TYPE_REFUND_ORDER);
        }

        /// <summary>
        /// 查询转账订单订单
        /// </summary>
        /// <param name="transferId"></param>
        /// <returns></returns>
        public MchNotifyRecordDto FindByTransferOrder(string transferId)
        {
            return FindByOrderAndType(transferId, (byte)MchNotifyRecordType.TYPE_TRANSFER_ORDER);
        }

        /// <summary>
        /// 更新商户回调的结果即状态
        /// </summary>
        /// <param name="notifyId"></param>
        /// <param name="state"></param>
        /// <param name="resResult"></param>
        /// <returns></returns>
        public int UpdateNotifyResult(long notifyId, byte state, string resResult)
        {
            var notify = _mchNotifyRecordRepository.GetById(notifyId);
            notify.State = state;
            notify.NotifyCount += 1;
            notify.ResResult = resResult;
            _mchNotifyRecordRepository.Update(notify);
            return _mchNotifyRecordRepository.SaveChanges();
        }
    }
}
