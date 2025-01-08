using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

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

        public async Task<MchDivisionReceiverDto> GetByIdAsync(long recordId, string mchNo)
        {
            var entity = await _mchDivisionReceiverRepository.GetAllAsNoTracking()
                .Where(w => w.ReceiverId.Equals(recordId) && w.MchNo.Equals(mchNo))
                .FirstOrDefaultAsync();
            return _mapper.Map<MchDivisionReceiverDto>(entity);
        }

        public Task<int> GetCountAsync(HashSet<long> receiverIds, string mchNo, string appId, string ifCode, byte state = CS.YES)
        {
            return _mchDivisionReceiverRepository.GetAllAsNoTracking()
                    .Where(w => receiverIds.Contains(w.ReceiverId)
                    && w.MchNo.Equals(mchNo) && w.AppId.Equals(appId)
                    && w.IfCode.Equals(ifCode) && w.State.Equals(state))
                    .CountAsync();
        }

        public Task<int> GetCountAsync(HashSet<long> receiverGroupIds, string mchNo)
        {
            return _mchDivisionReceiverRepository.GetAllAsNoTracking()
                    .Where(w => receiverGroupIds.Contains(w.ReceiverGroupId) && w.MchNo.Equals(mchNo))
                    .CountAsync();
        }

        public Task<bool> IsExistUseReceiverGroupAsync(long receiverGroupId)
        {
            return _mchDivisionReceiverRepository.IsExistUseReceiverGroupAsync(receiverGroupId);
        }

        public Task<PaginatedList<MchDivisionReceiverDto>> GetPaginatedDataAsync(MchDivisionReceiverQueryDto dto)
        {
            var query = _mchDivisionReceiverRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (dto.ReceiverId.Equals(null) || w.ReceiverId.Equals(dto.ReceiverId))
                && (string.IsNullOrWhiteSpace(dto.ReceiverAlias) || w.ReceiverAlias.Equals(dto.ReceiverAlias))
                && (dto.ReceiverGroupId.Equals(null) || w.ReceiverGroupId.Equals(dto.ReceiverGroupId))
                && (string.IsNullOrWhiteSpace(dto.ReceiverGroupName) || w.ReceiverGroupName.Equals(dto.ReceiverGroupName))
                && (!dto.State.HasValue || w.State.Equals(dto.State))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (string.IsNullOrWhiteSpace(dto.IfCode) || w.IfCode.Equals(dto.IfCode)))
                .OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<MchDivisionReceiver>.CreateAsync<MchDivisionReceiverDto>(query, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
