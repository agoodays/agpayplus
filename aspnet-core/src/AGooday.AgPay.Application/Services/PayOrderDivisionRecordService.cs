using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Extensions;
using AutoMapper;

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
            var entity = await _payOrderDivisionRecordRepository.FirstOrDefaultAsync(w => w.RecordId.Equals(recordId) && w.MchNo.Equals(mchNo));
            return _mapper.Map<PayOrderDivisionRecordDto>(entity);
        }

        public async Task<PayOrderDivisionRecordDto> GetByIdAsNoTrackingAsync(long recordId, string mchNo)
        {
            var entity = await _payOrderDivisionRecordRepository.GetByIdAsNoTrackingAsync(recordId, mchNo);
            return _mapper.Map<PayOrderDivisionRecordDto>(entity);
        }

        public Task<List<PayOrderDivisionRecordDto>> GetByPayOrderIdAsync(string payOrderId, byte? state)
        {
            var query = _payOrderDivisionRecordRepository.GetAllAsNoTracking()
                .Where(w => w.PayOrderId.Equals(payOrderId))
                .WhereIfNotNull(state, w => w.State.Equals(state));
            return query.ToListProjectToAsync<PayOrderDivisionRecord, PayOrderDivisionRecordDto>(_mapper);
        }

        public Task<List<PayOrderDivisionRecordDto>> GetByBatchOrderIdAsync(PayOrderDivisionRecordQueryDto dto)
        {
            var query = _payOrderDivisionRecordRepository.GetAllAsNoTracking()
                .WhereIfNotEmpty(dto.BatchOrderId, w => w.BatchOrderId.Equals(dto.BatchOrderId))
                .WhereIfNotNull(dto.State, w => w.State.Equals(dto.State))
                .OrderBy(o => o.RecordId);
            return query.ToListProjectToAsync<PayOrderDivisionRecord, PayOrderDivisionRecordDto>(_mapper);
        }

        public Task<PaginatedResult<PayOrderDivisionRecordDto>> GetPaginatedDataAsync(PayOrderDivisionRecordQueryDto dto)
        {
            var query = _payOrderDivisionRecordRepository.GetAllAsNoTracking()
                .WhereIfNotEmpty(dto.MchNo, w => w.MchNo.Equals(dto.MchNo))
                .WhereIfNotEmpty(dto.IsvNo, w => w.IsvNo.Equals(dto.IsvNo))
                .WhereIfNotNull(dto.ReceiverId, w => w.ReceiverId.Equals(dto.ReceiverId))
                .WhereIfNotNull(dto.ReceiverGroupId, w => w.ReceiverGroupId.Equals(dto.ReceiverGroupId))
                .WhereIfNotEmpty(dto.BatchOrderId, w => w.BatchOrderId.Equals(dto.BatchOrderId))
                .WhereIfNotEmpty(dto.PayOrderId, w => w.PayOrderId.Equals(dto.PayOrderId))
                .WhereIfNotEmpty(dto.AccNo, w => w.AccNo.Equals(dto.AccNo))
                .WhereIfNotEmpty(dto.AppId, w => w.AppId.Equals(dto.AppId))
                .WhereIfNotEmpty(dto.IfCode, w => w.IfCode.Equals(dto.IfCode))
                .WhereIfNotNull(dto.State, w => w.State.Equals(dto.State))
                .WhereIfNotNull(dto.CreatedStart, w => w.CreatedAt >= dto.CreatedStart)
                .WhereIfNotNull(dto.CreatedEnd, w => w.CreatedAt <= dto.CreatedEnd)
                .OrderByDescending(o => o.CreatedAt);
            return query.ToPaginatedResultAsync<PayOrderDivisionRecord, PayOrderDivisionRecordDto>(_mapper, dto.PageNumber, dto.PageSize);
        }

        /// <summary>
        /// batch_order_id 去重， 查询出所有的 分账已受理状态的订单， 支持分页
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<PaginatedResult<PayOrderDivisionRecordDto>> DistinctBatchOrderIdListAsync(PayOrderDivisionRecordQueryDto dto)
        {
            // 先获取去重的组合和最新记录ID
            var distinctQuery = _payOrderDivisionRecordRepository.GetAllAsNoTracking()
                .WhereIfNotEmpty(dto.MchNo, w => w.MchNo.Equals(dto.MchNo))
                .WhereIfNotEmpty(dto.IsvNo, w => w.IsvNo.Equals(dto.IsvNo))
                .WhereIfNotNull(dto.ReceiverId, w => w.ReceiverId.Equals(dto.ReceiverId))
                .WhereIfNotNull(dto.ReceiverGroupId, w => w.ReceiverGroupId.Equals(dto.ReceiverGroupId))
                .WhereIfNotEmpty(dto.BatchOrderId, w => w.BatchOrderId.Equals(dto.BatchOrderId))
                .WhereIfNotEmpty(dto.PayOrderId, w => w.PayOrderId.Equals(dto.PayOrderId))
                .WhereIfNotEmpty(dto.AccNo, w => w.AccNo.Equals(dto.AccNo))
                .WhereIfNotEmpty(dto.AppId, w => w.AppId.Equals(dto.AppId))
                .WhereIfNotEmpty(dto.IfCode, w => w.IfCode.Equals(dto.IfCode))
                .WhereIfNotNull(dto.State, w => w.State.Equals(dto.State))
                .WhereIfNotNull(dto.CreatedStart, w => w.CreatedAt >= dto.CreatedStart)
                .WhereIfNotNull(dto.CreatedEnd, w => w.CreatedAt <= dto.CreatedEnd)
                .GroupBy(x => new { x.BatchOrderId, x.PayOrderId })
                .Select(g => new
                {
                    g.Key.BatchOrderId,
                    g.Key.PayOrderId,
                    LatestRecordId = g.OrderByDescending(x => x.CreatedAt).Select(x => x.RecordId).FirstOrDefault()
                })
                .OrderByDescending(x => x.LatestRecordId); // 按最新记录排序

            // 然后获取完整的记录信息
            var latestRecordIds = distinctQuery.Select(x => x.LatestRecordId);

            var query = _payOrderDivisionRecordRepository.GetAllAsNoTracking()
                .Where(x => latestRecordIds.Contains(x.RecordId))
                .OrderByDescending(o => o.CreatedAt);

            return query.ToPaginatedResultAsync<PayOrderDivisionRecord, PayOrderDivisionRecordDto>(_mapper, dto.PageNumber, dto.PageSize);
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
                .Where(w => recordIds.Contains(w.RecordId) && w.State.Equals((byte)PayOrderDivisionRecordState.STATE_WAIT))
                .UpdateAsync(s => s.SetProperty(p => p.State, p => state)
                    .SetProperty(p => p.ChannelBatchOrderId, p => channelBatchOrderId)
                    .SetProperty(p => p.ChannelRespResult, p => channelRespResult)
                    .SetProperty(p => p.UpdatedAt, now));
            return updatedCount;
        }

        /// <summary>
        /// 更新分账订单为： 等待分账中的状态
        /// </summary>
        /// <param name="payOrderId"></param>
        public async Task<bool> UpdateResendStateAsync(string payOrderId)
        {
            var updateRecord = await _payOrderRepository.GetAll()
                .Where(w => w.PayOrderId.Equals(payOrderId) && w.DivisionState == (byte)PayOrderDivisionState.DIVISION_STATE_FINISH)
                .SafeFirstOrDefaultAsync();
            updateRecord.DivisionState = (byte)PayOrderDivisionState.DIVISION_STATE_WAIT_TASK;
            _payOrderRepository.Update(updateRecord);
            var payOrderUpdateRow = await _payOrderRepository.SaveChangesAsync();

            if (payOrderUpdateRow <= 0)
            {
                throw new BizException("更新订单分账状态失败");
            }

            var updateRecordByDiv = await _payOrderDivisionRecordRepository.GetAll()
                .Where(w => w.PayOrderId.Equals(payOrderId) && w.State.Equals((byte)PayOrderDivisionRecordState.STATE_FAIL))
                .SafeFirstOrDefaultAsync();
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
