using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;

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

        public override bool Add(MchDivisionReceiverGroupDto dto)
        {
            var entity = _mapper.Map<MchDivisionReceiverGroup>(dto);
            _mchDivisionReceiverGroupRepository.Add(entity);
            var result = _mchDivisionReceiverGroupRepository.SaveChanges(out int _);
            dto.ReceiverGroupId = entity.ReceiverGroupId;
            return result;
        }

        public MchDivisionReceiverGroupDto GetById(long recordId, string mchNo)
        {
            var entity = _mchDivisionReceiverGroupRepository.GetAllAsNoTracking()
                .Where(w => w.ReceiverGroupId.Equals(recordId) && w.MchNo.Equals(mchNo))
                .FirstOrDefault();
            return _mapper.Map<MchDivisionReceiverGroupDto>(entity);
        }

        public IEnumerable<MchDivisionReceiverGroupDto> GetByMchNo(string mchNo)
        {
            var mchDivisionReceiverGroups = _mchDivisionReceiverGroupRepository.GetAllAsNoTracking()
                .Where(w => w.MchNo.Equals(mchNo));
            return _mapper.Map<IEnumerable<MchDivisionReceiverGroupDto>>(mchDivisionReceiverGroups);
        }

        public MchDivisionReceiverGroupDto FindByIdAndMchNo(long receiverGroupId, string mchNo)
        {
            var entity = _mchDivisionReceiverGroupRepository.GetAllAsNoTracking()
                .Where(w => w.ReceiverGroupId.Equals(receiverGroupId) && w.MchNo.Equals(mchNo));
            return _mapper.Map<MchDivisionReceiverGroupDto>(entity);
        }

        public PaginatedList<MchDivisionReceiverGroupDto> GetPaginatedData(MchDivisionReceiverGroupQueryDto dto)
        {
            var mchDivisionReceiverGroups = _mchDivisionReceiverGroupRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.ReceiverGroupName) || w.ReceiverGroupName.Equals(dto.ReceiverGroupName))
                && (dto.ReceiverGroupId.Equals(null) || w.ReceiverGroupId.Equals(dto.ReceiverGroupId))
                && (!dto.AutoDivisionFlag.HasValue || w.AutoDivisionFlag.Equals(dto.AutoDivisionFlag))
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<MchDivisionReceiverGroup>.Create<MchDivisionReceiverGroupDto>(mchDivisionReceiverGroups, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
