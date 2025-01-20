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
    public class PayOrderDivisionRecordService : AgPayService<PayOrderDivisionRecordDto, PayOrderDivisionRecord, long>, IPayOrderDivisionRecordService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayOrderDivisionRecordRepository _payOrderDivisionRecordRepository;

        private readonly IPayOrderRepository _payOrderRepository;

        public PayOrderDivisionRecordService(IMapper mapper, IMediatorHandler bus,
            IPayOrderDivisionRecordRepository payOrderDivisionRecordRepository,
            IPayOrderRepository payOrderRepository)
            : base(mapper, bus, payOrderDivisionRecordRepository)
        {
            _payOrderDivisionRecordRepository = payOrderDivisionRecordRepository;
            _payOrderRepository = payOrderRepository;
        }

        public override async Task<bool> AddAsync(PayOrderDivisionRecordDto dto)
        {
            var entity = _mapper.Map<PayOrderDivisionRecord>(dto);
            await _payOrderDivisionRecordRepository.AddAsync(entity);
            var (result, _) = await _payOrderDivisionRecordRepository.SaveChangesWithResultAsync();
            dto.RecordId = entity.RecordId;
            return result;
        }

        public async Task<PayOrderDivisionRecordDto> GetByIdAsync(long recordId, string mchNo)
        {
            var entity = await _payOrderDivisionRecordRepository.GetAllAsNoTracking()
                .Where(w => w.RecordId.Equals(recordId) && w.MchNo.Equals(mchNo)).FirstOrDefaultAsync();
            return _mapper.Map<PayOrderDivisionRecordDto>(entity);
        }

        public async Task<PayOrderDivisionRecordDto> GetByIdAsNoTrackingAsync(long recordId, string mchNo)
        {
            var entity = await _payOrderDivisionRecordRepository.GetByIdAsNoTrackingAsync(recordId, mchNo);
            return _mapper.Map<PayOrderDivisionRecordDto>(entity);
        }

        public IEnumerable<PayOrderDivisionRecordDto> GetByPayOrderId(string payOrderId)
        {
            var records = _payOrderDivisionRecordRepository.GetAllAsNoTracking()
                .Where(w => w.PayOrderId.Equals(payOrderId));
            return _mapper.Map<IEnumerable<PayOrderDivisionRecordDto>>(records);
        }

        public List<PayOrderDivisionRecordDto> GetByBatchOrderId(PayOrderDivisionRecordQueryDto dto)
        {
            var records = _payOrderDivisionRecordRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.BatchOrderId) || w.BatchOrderId.Equals(dto.BatchOrderId)) && (dto.State.Equals(null) || w.State.Equals(dto.State)))
                .OrderBy(o => o.RecordId);
            return _mapper.Map<List<PayOrderDivisionRecordDto>>(records);
        }

        public async Task<PaginatedList<PayOrderDivisionRecordDto>> GetPaginatedDataAsync(PayOrderDivisionRecordQueryDto dto)
        {
            var query = _payOrderDivisionRecordRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (dto.ReceiverId.Equals(null) || w.ReceiverId.Equals(dto.ReceiverId))
                && (dto.ReceiverGroupId.Equals(null) || w.ReceiverGroupId.Equals(dto.ReceiverGroupId))
                && (string.IsNullOrWhiteSpace(dto.BatchOrderId) || w.BatchOrderId.Equals(dto.BatchOrderId))
                && (string.IsNullOrWhiteSpace(dto.PayOrderId) || w.PayOrderId.Equals(dto.PayOrderId))
                && (string.IsNullOrWhiteSpace(dto.AccNo) || w.AccNo.Equals(dto.AccNo))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (string.IsNullOrWhiteSpace(dto.IfCode) || w.IfCode.Equals(dto.IfCode))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
                && (dto.CreatedEnd.Equals(null) || w.CreatedAt <= dto.CreatedEnd))
                .OrderByDescending(o => o.CreatedAt);
            var records = await PaginatedList<PayOrderDivisionRecord>.CreateAsync<PayOrderDivisionRecordDto>(query, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }

        /// <summary>
        /// batch_order_id 去重， 查询出所有的 分账已受理状态的订单， 支持分页
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public PaginatedList<PayOrderDivisionRecordDto> DistinctBatchOrderIdList(PayOrderDivisionRecordQueryDto dto)
        {
            var query = _payOrderDivisionRecordRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (dto.ReceiverId.Equals(null) || w.ReceiverId.Equals(dto.ReceiverId))
                && (dto.ReceiverGroupId.Equals(null) || w.ReceiverGroupId.Equals(dto.ReceiverGroupId))
                && (string.IsNullOrWhiteSpace(dto.BatchOrderId) || w.BatchOrderId.Equals(dto.BatchOrderId))
                && (string.IsNullOrWhiteSpace(dto.PayOrderId) || w.PayOrderId.Equals(dto.PayOrderId))
                && (string.IsNullOrWhiteSpace(dto.AccNo) || w.AccNo.Equals(dto.AccNo))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
                && (dto.CreatedEnd.Equals(null) || w.CreatedAt <= dto.CreatedEnd)
                ).DistinctBy(d => new { d.BatchOrderId, d.PayOrderId }).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<PayOrderDivisionRecord>.Create<PayOrderDivisionRecordDto>(query, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }

        /// <summary>
        /// 更新分账记录为分账成功  ( 单条 )  将：  已受理 更新为： 其他状态
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="state"></param>
        /// <param name="channelRespResult"></param>
        public async Task<bool> UpdateRecordSuccessOrFailBySingleItemAsync(long recordId, byte state, string channelRespResult)
        {
            var updateRecord = await _payOrderDivisionRecordRepository.GetByIdAsync(recordId);
            updateRecord.State = state;
            updateRecord.ChannelRespResult = state == (byte)PayOrderDivisionRecordState.STATE_SUCCESS ? "" : channelRespResult; // 若明确成功，清空错误信息。;
            _payOrderDivisionRecordRepository.Update(updateRecord);
            var (result, _) = await _payOrderDivisionRecordRepository.SaveChangesWithResultAsync();
            return result;
        }

        /// <summary>
        /// 更新分账记录为分账成功
        /// </summary>
        /// <param name="records"></param>
        /// <param name="state"></param>
        /// <param name="channelBatchOrderId"></param>
        /// <param name="channelRespResult"></param>
        public Task<int> UpdateRecordSuccessOrFailAsync(List<PayOrderDivisionRecordDto> records, byte state, string channelBatchOrderId, string channelRespResult)
        {
            if (records == null || records.Count == 0)
            {
                return Task.FromResult(0);
            }
            var recordIds = records.Select(s => s.RecordId);

            // 使用 ExecuteUpdate 直接在数据库中批量更新
            var now = DateTime.Now;
            var updatedCount = _payOrderDivisionRecordRepository.GetAll()
                .Where(w => recordIds.Contains(w.RecordId)
                && w.State.Equals((byte)PayOrderDivisionRecordState.STATE_WAIT))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.State, p => state)
                    .SetProperty(p => p.ChannelBatchOrderId, p => channelBatchOrderId)
                    .SetProperty(p => p.ChannelRespResult, p => channelRespResult)
                    .SetProperty(p => p.UpdatedAt, now));
            return updatedCount;

            //var updateRecords = _payOrderDivisionRecordRepository.GetAll()
            //    .Where(w => recordIds.Contains(w.RecordId)
            //    && w.State.Equals((byte)PayOrderDivisionRecordState.STATE_WAIT));
            //if (updateRecords.Any())
            //{
            //    foreach (var updateRecord in updateRecords)
            //    {
            //        updateRecord.State = state;
            //        updateRecord.ChannelBatchOrderId = channelBatchOrderId;
            //        updateRecord.ChannelRespResult = channelRespResult;
            //    }
            //    _payOrderDivisionRecordRepository.UpdateRange(updateRecords);
            //    return _payOrderDivisionRecordRepository.SaveChangesAsync();
            //}
            //return Task.FromResult(0);
        }

        /// <summary>
        /// 更新分账订单为： 等待分账中的状态
        /// </summary>
        /// <param name="payOrderId"></param>
        public async Task<bool> UpdateResendStateAsync(string payOrderId)
        {
            var updateRecord = _payOrderRepository.GetAll()
                .Where(w => w.PayOrderId.Equals(payOrderId) && w.DivisionState == (byte)PayOrderDivisionState.DIVISION_STATE_FINISH)
                .FirstOrDefault();
            updateRecord.DivisionState = (byte)PayOrderDivisionState.DIVISION_STATE_WAIT_TASK;
            _payOrderRepository.Update(updateRecord);
            var payOrderUpdateRow = await _payOrderRepository.SaveChangesAsync();

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
            var (recordUpdateFlag, _) = await _payOrderDivisionRecordRepository.SaveChangesWithResultAsync();
            if (!recordUpdateFlag)
            {
                throw new BizException("更新分账记录状态失败");
            }
            return recordUpdateFlag;
        }
    }
}
