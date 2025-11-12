using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Extensions;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 商户通知表 服务实现类
    /// </summary>
    public class MchNotifyRecordService : AgPayService<MchNotifyRecordDto, MchNotifyRecord, long>, IMchNotifyRecordService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IMchNotifyRecordRepository _mchNotifyRecordRepository;

        public MchNotifyRecordService(IMapper mapper, IMediatorHandler bus,
            IMchNotifyRecordRepository mchNotifyRecordRepository)
            : base(mapper, bus, mchNotifyRecordRepository)
        {
            _mchNotifyRecordRepository = mchNotifyRecordRepository;
        }

        public override async Task<bool> AddAsync(MchNotifyRecordDto dto)
        {
            var entity = _mapper.Map<MchNotifyRecord>(dto);
            await _mchNotifyRecordRepository.AddAsync(entity);
            var (result, _) = await _mchNotifyRecordRepository.SaveChangesWithResultAsync();
            dto.NotifyId = entity.NotifyId;
            return result;
        }

        public Task<PaginatedResult<MchNotifyRecordDto>> GetPaginatedDataAsync(MchNotifyQueryDto dto)
        {
            var query = _mchNotifyRecordRepository.GetAllAsNoTracking()
                .WhereIfNotEmpty(dto.MchNo, w => w.MchNo.Equals(dto.MchNo))
                .WhereIfNotEmpty(dto.IsvNo, w => w.IsvNo.Equals(dto.IsvNo))
                .WhereIfNotEmpty(dto.OrderId, w => w.OrderId.Equals(dto.OrderId))
                .WhereIfNotEmpty(dto.MchOrderNo, w => w.MchOrderNo.Equals(dto.MchOrderNo))
                .WhereIfNotEmpty(dto.AppId, w => w.AppId.Equals(dto.AppId))
                .WhereIfNotNull(dto.OrderType, w => w.OrderType.Equals(dto.OrderType))
                .WhereIfNotNull(dto.State, w => w.State.Equals(dto.State))
                .WhereIfNotNull(dto.CreatedStart, w => w.CreatedAt >= dto.CreatedStart)
                .WhereIfNotNull(dto.CreatedEnd, w => w.CreatedAt <= dto.CreatedEnd)
                .OrderByDescending(o => o.CreatedAt);
            return query.ToPaginatedResultAsync<MchNotifyRecord, MchNotifyRecordDto>(_mapper, dto.PageNumber, dto.PageSize);
        }

        /// <summary>
        /// 根据订单号和类型查询
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public async Task<MchNotifyRecordDto> FindByOrderAndTypeAsync(string orderId, byte orderType)
        {
            var entity = await _mchNotifyRecordRepository.FirstOrDefaultAsNoTrackingAsync(w => w.OrderId.Equals(orderId) && w.OrderType.Equals(orderType));
            return _mapper.Map<MchNotifyRecordDto>(entity);
        }

        /// <summary>
        /// 查询支付订单
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <returns></returns>
        public Task<MchNotifyRecordDto> FindByPayOrderAsync(string payOrderId)
        {
            return FindByOrderAndTypeAsync(payOrderId, (byte)MchNotifyRecordType.TYPE_PAY_ORDER);
        }

        /// <summary>
        /// 查询退款订单订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Task<MchNotifyRecordDto> FindByRefundOrderAsync(string orderId)
        {
            return FindByOrderAndTypeAsync(orderId, (byte)MchNotifyRecordType.TYPE_REFUND_ORDER);
        }

        /// <summary>
        /// 查询转账订单订单
        /// </summary>
        /// <param name="transferId"></param>
        /// <returns></returns>
        public Task<MchNotifyRecordDto> FindByTransferOrder(string transferId)
        {
            return FindByOrderAndTypeAsync(transferId, (byte)MchNotifyRecordType.TYPE_TRANSFER_ORDER);
        }

        /// <summary>
        /// 更新商户回调的结果即状态
        /// </summary>
        /// <param name="notifyId"></param>
        /// <param name="state"></param>
        /// <param name="resResult"></param>
        /// <returns></returns>
        public async Task<int> UpdateNotifyResultAsync(long notifyId, byte state, string resResult)
        {
            var notify = await _mchNotifyRecordRepository.GetByIdAsync(notifyId);
            notify.State = state;
            notify.NotifyCount += 1;
            notify.ResResult = resResult;
            notify.LastNotifyTime = DateTime.Now;
            _mchNotifyRecordRepository.Update(notify);
            return await _mchNotifyRecordRepository.SaveChangesAsync();
        }

        /// <summary>
        /// 更改为通知中 & 增加允许重发通知次数
        /// </summary>
        /// <param name="notifyId"></param>
        public async Task<int> UpdateIngAndAddNotifyCountLimitAsync(long notifyId)
        {
            var notify = await _mchNotifyRecordRepository.GetByIdAsync(notifyId);
            notify.NotifyCountLimit += 1;
            notify.State = (byte)MchNotifyRecordState.STATE_ING;
            _mchNotifyRecordRepository.Update(notify);
            return await _mchNotifyRecordRepository.SaveChangesAsync();
        }
    }
}
