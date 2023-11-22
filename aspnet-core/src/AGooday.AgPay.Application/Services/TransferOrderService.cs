using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 转账订单表 服务实现类
    /// </summary>
    public class TransferOrderService : ITransferOrderService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ITransferOrderRepository _transferOrderRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public TransferOrderService(ITransferOrderRepository transferOrderRepository, IMapper mapper, IMediatorHandler bus)
        {
            _transferOrderRepository = transferOrderRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(TransferOrderDto dto)
        {
            var m = _mapper.Map<TransferOrder>(dto);
            _transferOrderRepository.Add(m);
            _transferOrderRepository.SaveChanges();
        }

        public void Remove(string recordId)
        {
            _transferOrderRepository.Remove(recordId);
            _transferOrderRepository.SaveChanges();
        }

        public void Update(TransferOrderDto dto)
        {
            var m = _mapper.Map<TransferOrder>(dto);
            _transferOrderRepository.Update(m);
            _transferOrderRepository.SaveChanges();
        }

        public TransferOrderDto GetById(string recordId)
        {
            var entity = _transferOrderRepository.GetById(recordId);
            var dto = _mapper.Map<TransferOrderDto>(entity);
            return dto;
        }

        public TransferOrderDto QueryMchOrder(string mchNo, string mchOrderNo, string transferId)
        {
            if (string.IsNullOrEmpty(transferId))
            {
                var entity = _transferOrderRepository.GetAll().Where(w => w.MchNo.Equals(mchNo) && w.TransferId.Equals(transferId)).FirstOrDefault();
                return _mapper.Map<TransferOrderDto>(entity);
            }
            else if (string.IsNullOrEmpty(mchOrderNo))
            {
                var entity = _transferOrderRepository.GetAll().Where(w => w.MchNo.Equals(mchNo) && w.MchOrderNo.Equals(mchOrderNo)).FirstOrDefault();
                return _mapper.Map<TransferOrderDto>(entity);
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<TransferOrderDto> GetAll()
        {
            var transferOrders = _transferOrderRepository.GetAll();
            return _mapper.Map<IEnumerable<TransferOrderDto>>(transferOrders);
        }

        public PaginatedList<TransferOrderDto> GetPaginatedData(TransferOrderQueryDto dto)
        {
            var transferOrders = _transferOrderRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (dto.MchType.Equals(0) || w.MchType.Equals(dto.MchType))
                && (string.IsNullOrWhiteSpace(dto.TransferId) || w.TransferId.Equals(dto.TransferId))
                && (string.IsNullOrWhiteSpace(dto.MchOrderNo) || w.MchOrderNo.Equals(dto.MchOrderNo))
                && (string.IsNullOrWhiteSpace(dto.ChannelOrderNo) || w.ChannelOrderNo.Equals(dto.ChannelOrderNo))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (string.IsNullOrWhiteSpace(dto.UnionOrderId) || w.TransferId.Equals(dto.UnionOrderId)
                || w.MchOrderNo.Equals(dto.UnionOrderId) || w.MchOrderNo.Equals(dto.UnionOrderId) || w.ChannelOrderNo.Equals(dto.UnionOrderId))
                && (dto.CreatedEnd == null || w.CreatedAt < dto.CreatedEnd)
                && (dto.CreatedStart == null || w.CreatedAt >= dto.CreatedStart)
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<TransferOrder>.Create<TransferOrderDto>(transferOrders, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
        /// <summary>
        /// 更新转账订单状态 【转账订单生成】 --》 【转账中】
        /// </summary>
        /// <param name="transferId"></param>
        /// <returns></returns>
        public bool UpdateInit2Ing(string transferId)
        {
            var updateRecord = _transferOrderRepository.GetById(transferId);
            if (updateRecord.State != (byte)TransferOrderState.STATE_INIT)
            {
                return false;
            }
            updateRecord.State = (byte)TransferOrderState.STATE_ING;
            _transferOrderRepository.Update(updateRecord);
            return _transferOrderRepository.SaveChanges(out int _);
        }
        /// <summary>
        /// 更新转账订单状态 【转账中】 --》 【转账成功】
        /// </summary>
        /// <param name="transferId"></param>
        /// <param name="channelOrderNo"></param>
        /// <returns></returns>
        public bool UpdateIng2Success(string transferId, string channelOrderNo)
        {
            var updateRecord = _transferOrderRepository.GetById(transferId);
            if (updateRecord.State != (byte)TransferOrderState.STATE_ING)
            {
                return false;
            }
            updateRecord.State = (byte)TransferOrderState.STATE_SUCCESS;
            updateRecord.ChannelOrderNo = channelOrderNo;
            updateRecord.SuccessTime = DateTime.Now;
            _transferOrderRepository.Update(updateRecord);
            return _transferOrderRepository.SaveChanges(out int _);
        }
        /// <summary>
        /// 更新转账订单状态 【转账中】 --》 【转账失败】
        /// </summary>
        /// <param name="transferId"></param>
        /// <param name="channelOrderNo"></param>
        /// <param name="channelErrCode"></param>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        public bool UpdateIng2Fail(string transferId, string channelOrderNo, string channelErrCode, string channelErrMsg)
        {
            var updateRecord = _transferOrderRepository.GetById(transferId);
            if (updateRecord.State != (byte)TransferOrderState.STATE_ING)
            {
                return false;
            }
            updateRecord.State = (byte)TransferOrderState.STATE_FAIL;
            updateRecord.ErrCode = channelErrCode;
            updateRecord.ErrMsg = channelErrMsg;
            updateRecord.ChannelOrderNo = channelOrderNo;
            _transferOrderRepository.Update(updateRecord);
            return _transferOrderRepository.SaveChanges(out int _);
        }
        /// <summary>
        /// 更新转账订单状态 【转账中】 --》 【转账成功/转账失败】
        /// </summary>
        /// <param name="transferId"></param>
        /// <param name="state"></param>
        /// <param name="channelOrderId"></param>
        /// <param name="channelErrCode"></param>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        public bool UpdateIng2SuccessOrFail(string transferId, byte updateState, string channelOrderNo, string channelErrCode, string channelErrMsg)
        {
            if (updateState == (byte)TransferOrderState.STATE_ING)
            {
                return true;
            }
            else if (updateState == (byte)TransferOrderState.STATE_SUCCESS)
            {
                return UpdateIng2Success(transferId, channelOrderNo);
            }
            else if (updateState == (byte)TransferOrderState.STATE_FAIL)
            {
                return UpdateIng2Fail(transferId, channelOrderNo, channelErrCode, channelErrMsg);
            }
            return false;
        }
        public bool IsExistOrderByMchOrderNo(string mchNo, string mchOrderNo)
        {
            return _transferOrderRepository.IsExistOrderByMchOrderNo(mchNo, mchOrderNo);
        }
    }
}
