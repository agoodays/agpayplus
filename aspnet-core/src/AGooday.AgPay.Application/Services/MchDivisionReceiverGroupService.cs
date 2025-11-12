using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;

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
                //await mchDivisionReceiverGroups.ForEachAsync(item =>
                //{
                //    item.AutoDivisionFlag = CS.NO;
                //    item.UpdatedAt = DateTime.Now;
                //});
                //await _mchDivisionReceiverGroupRepository.SaveChangesAsync();
                await mchDivisionReceiverGroups
                    .UpdateAsync(s => s.SetProperty(p => p.AutoDivisionFlag, p => CS.NO)
                    .SetProperty(p => p.UpdatedAt, p => DateTime.Now));
            }
        }

        public async Task<MchDivisionReceiverGroupDto> GetByIdAsNoTrackingAsync(long recordId, string mchNo)
        {
            var entity = await _mchDivisionReceiverGroupRepository.FirstOrDefaultAsNoTrackingAsync(w => w.ReceiverGroupId.Equals(recordId) && w.MchNo.Equals(mchNo));
            return _mapper.Map<MchDivisionReceiverGroupDto>(entity);
        }

        public IEnumerable<MchDivisionReceiverGroupDto> GetByMchNo(string mchNo)
        {
            return _mchDivisionReceiverGroupRepository.GetAllAsNoTracking()
                .Where(w => w.MchNo.Equals(mchNo))
                .ProjectTo<MchDivisionReceiverGroupDto>(_mapper.ConfigurationProvider);
        }

        public async Task<MchDivisionReceiverGroupDto> FindByIdAndMchNoAsync(long receiverGroupId, string mchNo)
        {
            var entity = await _mchDivisionReceiverGroupRepository.FirstOrDefaultAsNoTrackingAsync(w => w.ReceiverGroupId.Equals(receiverGroupId) && w.MchNo.Equals(mchNo));
            return _mapper.Map<MchDivisionReceiverGroupDto>(entity);
        }

        public Task<PaginatedResult<MchDivisionReceiverGroupDto>> GetPaginatedDataAsync(MchDivisionReceiverGroupQueryDto dto)
        {
            var query = _mchDivisionReceiverGroupRepository.GetAllAsNoTracking()
                .WhereIfNotEmpty(dto.MchNo, w => w.MchNo.Equals(dto.MchNo))
                .WhereIfNotEmpty(dto.ReceiverGroupName, w => w.ReceiverGroupName.Equals(dto.ReceiverGroupName))
                .WhereIfNotNull(dto.ReceiverGroupId, w => w.ReceiverGroupId.Equals(dto.ReceiverGroupId))
                .WhereIfNotNull(dto.AutoDivisionFlag, w => w.AutoDivisionFlag.Equals(dto.AutoDivisionFlag))
                .OrderByDescending(o => o.CreatedAt);
            return query.ToPaginatedResultAsync<MchDivisionReceiverGroup, MchDivisionReceiverGroupDto>(_mapper, dto.PageNumber, dto.PageSize);
        }
    }
}
