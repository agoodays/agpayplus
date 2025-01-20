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
    /// 分账账号组 服务实现类
    /// </summary>
    public class MchDivisionReceiverGroupService : AgPayService<MchDivisionReceiverGroupDto, MchDivisionReceiverGroup, long>, IMchDivisionReceiverGroupService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IMchDivisionReceiverGroupRepository _mchDivisionReceiverGroupRepository;

        public MchDivisionReceiverGroupService(IMapper mapper, IMediatorHandler bus,
            IMchDivisionReceiverGroupRepository mchDivisionReceiverGroupRepository)
            : base(mapper, bus, mchDivisionReceiverGroupRepository)
        {
            _mchDivisionReceiverGroupRepository = mchDivisionReceiverGroupRepository;
        }

        public override async Task<bool> AddAsync(MchDivisionReceiverGroupDto dto)
        {
            var entity = _mapper.Map<MchDivisionReceiverGroup>(dto);
            await _mchDivisionReceiverGroupRepository.AddAsync(entity);
            var (result, _) = await _mchDivisionReceiverGroupRepository.SaveChangesWithResultAsync();
            dto.ReceiverGroupId = entity.ReceiverGroupId;
            return result;
        }

        public async Task UpdateAutoDivisionFlagAsync(MchDivisionReceiverGroupDto dto)
        {
            if (dto.AutoDivisionFlag == CS.YES)
            {
                var mchDivisionReceiverGroups = _mchDivisionReceiverGroupRepository.GetAllAsNoTracking()
                .Where(w => w.MchNo.Equals(dto.MchNo) && !w.ReceiverGroupId.Equals(dto.ReceiverGroupId));
                await mchDivisionReceiverGroups.ForEachAsync(a =>
                {
                    a.AutoDivisionFlag = CS.NO;
                });
                _mchDivisionReceiverGroupRepository.UpdateRange(mchDivisionReceiverGroups);
                await _mchDivisionReceiverGroupRepository.SaveChangesAsync();
            }
        }

        public async Task<MchDivisionReceiverGroupDto> GetByIdAsync(long recordId, string mchNo)
        {
            var entity = await _mchDivisionReceiverGroupRepository.GetAllAsNoTracking()
                .Where(w => w.ReceiverGroupId.Equals(recordId) && w.MchNo.Equals(mchNo))
                .FirstOrDefaultAsync();
            return _mapper.Map<MchDivisionReceiverGroupDto>(entity);
        }

        public IEnumerable<MchDivisionReceiverGroupDto> GetByMchNo(string mchNo)
        {
            var records = _mchDivisionReceiverGroupRepository.GetAllAsNoTracking()
                .Where(w => w.MchNo.Equals(mchNo));
            return _mapper.Map<IEnumerable<MchDivisionReceiverGroupDto>>(records);
        }

        public async Task<MchDivisionReceiverGroupDto> FindByIdAndMchNoAsync(long receiverGroupId, string mchNo)
        {
            var entity = await _mchDivisionReceiverGroupRepository.GetAllAsNoTracking()
                .Where(w => w.ReceiverGroupId.Equals(receiverGroupId) && w.MchNo.Equals(mchNo))
                .FirstOrDefaultAsync();
            return _mapper.Map<MchDivisionReceiverGroupDto>(entity);
        }

        public Task<PaginatedList<MchDivisionReceiverGroupDto>> GetPaginatedDataAsync(MchDivisionReceiverGroupQueryDto dto)
        {
            var query = _mchDivisionReceiverGroupRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.ReceiverGroupName) || w.ReceiverGroupName.Equals(dto.ReceiverGroupName))
                && (dto.ReceiverGroupId.Equals(null) || w.ReceiverGroupId.Equals(dto.ReceiverGroupId))
                && (!dto.AutoDivisionFlag.HasValue || w.AutoDivisionFlag.Equals(dto.AutoDivisionFlag)))
                .OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<MchDivisionReceiverGroup>.CreateAsync<MchDivisionReceiverGroupDto>(query, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
