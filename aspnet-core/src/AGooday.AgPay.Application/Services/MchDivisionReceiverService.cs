using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Extensions;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 商户分账接收者账号绑定关系表 服务实现类
    /// </summary>
    public class MchDivisionReceiverService : AgPayService<MchDivisionReceiverDto, MchDivisionReceiver, long>, IMchDivisionReceiverService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IMchDivisionReceiverRepository _mchDivisionReceiverRepository;

        public MchDivisionReceiverService(IMapper mapper, IMediatorHandler bus,
            IMchDivisionReceiverRepository mchDivisionReceiverRepository)
            : base(mapper, bus, mchDivisionReceiverRepository)
        {
            _mchDivisionReceiverRepository = mchDivisionReceiverRepository;
        }

        public override async Task<bool> AddAsync(MchDivisionReceiverDto dto)
        {
            var entity = _mapper.Map<MchDivisionReceiver>(dto);
            await _mchDivisionReceiverRepository.AddAsync(entity);
            var (result, _) = await _mchDivisionReceiverRepository.SaveChangesWithResultAsync();
            dto.ReceiverId = entity.ReceiverId;
            return result;
        }

        public async Task<MchDivisionReceiverDto> GetByIdAsNoTrackingAsync(long recordId, string mchNo)
        {
            var query = _mchDivisionReceiverRepository.GetAllAsNoTracking()
                .Where(w => w.ReceiverId.Equals(recordId) && w.MchNo.Equals(mchNo));
            return await query.FirstOrDefaultProjectToAsync<MchDivisionReceiver, MchDivisionReceiverDto>(_mapper);
        }

        public Task<int> GetCountAsync(HashSet<long> receiverIds, string mchNo, string appId, string ifCode, byte state = CS.YES)
        {
            return _mchDivisionReceiverRepository.GetAllAsNoTracking()
                    .SafeCountAsync(w => receiverIds.Contains(w.ReceiverId)
                    && w.MchNo.Equals(mchNo) && w.AppId.Equals(appId)
                    && w.IfCode.Equals(ifCode) && w.State.Equals(state));
        }

        public Task<int> GetCountAsync(HashSet<long> receiverGroupIds, string mchNo)
        {
            return _mchDivisionReceiverRepository.GetAllAsNoTracking()
                    .SafeCountAsync(w => receiverGroupIds.Contains(w.ReceiverGroupId) && w.MchNo.Equals(mchNo));
        }

        public Task<bool> IsExistUseReceiverGroupAsync(long receiverGroupId)
        {
            return _mchDivisionReceiverRepository.IsExistUseReceiverGroupAsync(receiverGroupId);
        }

        public Task<PaginatedResult<MchDivisionReceiverDto>> GetPaginatedDataAsync(MchDivisionReceiverQueryDto dto)
        {
            var query = _mchDivisionReceiverRepository.GetAllAsNoTracking()
                .WhereIfNotEmpty(dto.MchNo, w => w.MchNo.Equals(dto.MchNo))
                .WhereIfNotEmpty(dto.IsvNo, w => w.IsvNo.Equals(dto.IsvNo))
                .WhereIfNotNull(dto.ReceiverId, w => w.ReceiverId.Equals(dto.ReceiverId))
                .WhereIfNotEmpty(dto.ReceiverAlias, w => w.ReceiverAlias.Equals(dto.ReceiverAlias))
                .WhereIfNotNull(dto.ReceiverGroupId, w => w.ReceiverGroupId.Equals(dto.ReceiverGroupId))
                .WhereIfNotEmpty(dto.ReceiverGroupName, w => w.ReceiverGroupName.Equals(dto.ReceiverGroupName))
                .WhereIfNotNull(dto.State, w => w.State.Equals(dto.State))
                .WhereIfNotEmpty(dto.AppId, w => w.AppId.Equals(dto.AppId))
                .WhereIfNotEmpty(dto.IfCode, w => w.IfCode.Equals(dto.IfCode))
                .OrderByDescending(o => o.CreatedAt);
            return query.ToPaginatedResultAsync<MchDivisionReceiver, MchDivisionReceiverDto>(_mapper, dto.PageNumber, dto.PageSize);
        }
    }
}
