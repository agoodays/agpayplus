using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 分账记录表 服务实现类
    /// </summary>
    public class PayOrderDivisionRecordService : IPayOrderDivisionRecordService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayOrderRepository _payOrderRepository;
        private readonly IPayOrderDivisionRecordRepository _payOrderDivisionRecordRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public PayOrderDivisionRecordService(IPayOrderRepository payOrderRepository, IPayOrderDivisionRecordRepository payOrderDivisionRecordRepository, IMapper mapper, IMediatorHandler bus)
        {
            _payOrderRepository = payOrderRepository;
            _payOrderDivisionRecordRepository = payOrderDivisionRecordRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool Add(PayOrderDivisionRecordDto dto)
        {
            var m = _mapper.Map<PayOrderDivisionRecord>(dto);
            _payOrderDivisionRecordRepository.Add(m);
            return _payOrderDivisionRecordRepository.SaveChanges(out int _);
        }

        public bool Remove(long recordId)
        {
            _payOrderDivisionRecordRepository.Remove(recordId);
            return _payOrderDivisionRecordRepository.SaveChanges(out int _);
        }

        public bool Update(PayOrderDivisionRecordDto dto)
        {
            var m = _mapper.Map<PayOrderDivisionRecord>(dto);
            _payOrderDivisionRecordRepository.Update(m);
            return _payOrderDivisionRecordRepository.SaveChanges(out int _);
        }

        public PayOrderDivisionRecordDto GetById(long recordId)
        {
            var entity = _payOrderDivisionRecordRepository.GetById(recordId);
            var dto = _mapper.Map<PayOrderDivisionRecordDto>(entity);
            return dto;
        }

        public PayOrderDivisionRecordDto GetById(long recordId, string mchNo)
        {
            var entity = _payOrderDivisionRecordRepository.GetAll().Where(w => w.RecordId.Equals(recordId) && w.MchNo.Equals(mchNo)).FirstOrDefault();
            return _mapper.Map<PayOrderDivisionRecordDto>(entity);
        }

        public IEnumerable<PayOrderDivisionRecordDto> GetByPayOrderId(string payOrderId)
        {
            var payOrderDivisionRecords = _payOrderDivisionRecordRepository.GetAll()
                    .Where(w => w.PayOrderId.Equals(payOrderId));
            return _mapper.Map<IEnumerable<PayOrderDivisionRecordDto>>(payOrderDivisionRecords);
        }

        public List<PayOrderDivisionRecordDto> GetByBatchOrderId(PayOrderDivisionRecordQueryDto dto)
        {
            var payOrderDivisionRecords = _payOrderDivisionRecordRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.BatchOrderId) || w.BatchOrderId.Equals(dto.BatchOrderId))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                ).OrderBy(o => o.RecordId);
            return _mapper.Map<List<PayOrderDivisionRecordDto>>(payOrderDivisionRecords);
        }

        public IEnumerable<PayOrderDivisionRecordDto> GetAll()
        {
            var payOrderDivisionRecords = _payOrderDivisionRecordRepository.GetAll();
            return _mapper.Map<IEnumerable<PayOrderDivisionRecordDto>>(payOrderDivisionRecords);
        }

        public PaginatedList<PayOrderDivisionRecordDto> GetPaginatedData(PayOrderDivisionRecordQueryDto dto)
        {
            var mchInfos = _payOrderDivisionRecordRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (dto.ReceiverId.Equals(0) || w.ReceiverId.Equals(dto.ReceiverId))
                && (dto.ReceiverGroupId.Equals(0) || w.ReceiverGroupId.Equals(dto.ReceiverGroupId))
                && (string.IsNullOrWhiteSpace(dto.BatchOrderId) || w.BatchOrderId.Equals(dto.BatchOrderId))
                && (string.IsNullOrWhiteSpace(dto.PayOrderId) || w.PayOrderId.Equals(dto.PayOrderId))
                && (string.IsNullOrWhiteSpace(dto.AccNo) || w.AccNo.Equals(dto.AccNo))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                && (dto.CreatedEnd == null || w.CreatedAt < dto.CreatedEnd)
                && (dto.CreatedStart == null || w.CreatedAt >= dto.CreatedStart)
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<PayOrderDivisionRecord>.Create<PayOrderDivisionRecordDto>(mchInfos.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }

        public PaginatedList<PayOrderDivisionRecordDto> DistinctBatchOrderIdList(PayOrderDivisionRecordQueryDto dto)
        {
            var mchInfos = _payOrderDivisionRecordRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (dto.ReceiverId.Equals(0) || w.ReceiverId.Equals(dto.ReceiverId))
                && (dto.ReceiverGroupId.Equals(0) || w.ReceiverGroupId.Equals(dto.ReceiverGroupId))
                && (string.IsNullOrWhiteSpace(dto.BatchOrderId) || w.BatchOrderId.Equals(dto.BatchOrderId))
                && (string.IsNullOrWhiteSpace(dto.PayOrderId) || w.PayOrderId.Equals(dto.PayOrderId))
                && (string.IsNullOrWhiteSpace(dto.AccNo) || w.AccNo.Equals(dto.AccNo))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                && (dto.CreatedEnd == null || w.CreatedAt < dto.CreatedEnd)
                && (dto.CreatedStart == null || w.CreatedAt >= dto.CreatedStart)
                ).DistinctBy(d => new { d.BatchOrderId, d.PayOrderId }).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<PayOrderDivisionRecord>.Create<PayOrderDivisionRecordDto>(mchInfos.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }

        /// <summary>
        /// 更新分账记录为分账成功  ( 单条 )  将：  已受理 更新为： 其他状态
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="state"></param>
        /// <param name="channelRespResult"></param>
        public void UpdateRecordSuccessOrFailBySingleItem(long recordId, byte state, string channelRespResult)
        {
            var updateRecord = _payOrderDivisionRecordRepository.GetById(recordId);
            updateRecord.State = state;
            updateRecord.ChannelRespResult = state == (byte)PayOrderDivisionRecordState.STATE_SUCCESS ? "" : channelRespResult; // 若明确成功，清空错误信息。;
            _payOrderDivisionRecordRepository.Update(updateRecord);
            _payOrderDivisionRecordRepository.SaveChanges(out int _);
        }

        /// <summary>
        /// 更新分账记录为分账成功
        /// </summary>
        /// <param name="records"></param>
        /// <param name="state"></param>
        /// <param name="channelBatchOrderId"></param>
        /// <param name="channelRespResult"></param>
        public void UpdateRecordSuccessOrFail(List<PayOrderDivisionRecordDto> records, byte state, string channelBatchOrderId, string channelRespResult)
        {
            if (records == null || !records.Any())
            {
                return;
            }
            var recordIds = records.Select(s => s.RecordId);

            var updateRecords = _payOrderDivisionRecordRepository.GetAll().Where(w => recordIds.Contains(w.RecordId) && w.State.Equals((byte)PayOrderDivisionRecordState.STATE_WAIT));
            foreach (var updateRecord in updateRecords)
            {
                updateRecord.State = state;
                updateRecord.ChannelBatchOrderId = channelBatchOrderId;
                updateRecord.ChannelRespResult = channelRespResult;
                _payOrderDivisionRecordRepository.Update(updateRecord);
            }
            _payOrderDivisionRecordRepository.SaveChanges(out int _);
        }

        /// <summary>
        /// 更新分账订单为： 等待分账中的状态
        /// </summary>
        /// <param name="payOrderId"></param>
        public void UpdateResendState(string payOrderId)
        {
            var updateRecord = _payOrderRepository.GetAll()
                .Where(w => w.PayOrderId.Equals(payOrderId) && w.DivisionState == (byte)PayOrderDivision.DIVISION_STATE_FINISH)
                .FirstOrDefault();
            updateRecord.DivisionState = (byte)PayOrderDivision.DIVISION_STATE_WAIT_TASK;
            _payOrderRepository.Update(updateRecord);
            _payOrderRepository.SaveChanges(out int payOrderUpdateRow);

            if (payOrderUpdateRow <= 0)
            {
                throw new BizException("更新订单分账状态失败");
            }

            var updateRecordByDiv = _payOrderDivisionRecordRepository.GetAll()
                .Where(w => w.PayOrderId.Equals(payOrderId) && w.State.Equals((byte)PayOrderDivisionRecordState.STATE_FAIL)).FirstOrDefault();
            updateRecordByDiv.BatchOrderId = SeqUtil.GenDivisionBatchId(); // 重新生成batchOrderId, 避免部分失败导致： out_trade_no重复。
            updateRecordByDiv.State = (byte)PayOrderDivisionRecordState.STATE_WAIT; //待分账
            updateRecordByDiv.ChannelRespResult = "";
            updateRecordByDiv.ChannelBatchOrderId = "";
            _payOrderDivisionRecordRepository.Update(updateRecordByDiv);
            var recordUpdateFlag = _payOrderDivisionRecordRepository.SaveChanges(out int _);
            if (!recordUpdateFlag)
            {
                throw new BizException("更新分账记录状态失败");
            }
        }
    }
}
