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
    public class MchDivisionReceiverService : IMchDivisionReceiverService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IMchDivisionReceiverRepository _mchDivisionReceiverRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public MchDivisionReceiverService(IMchDivisionReceiverRepository mchDivisionReceiverRepository, IMapper mapper, IMediatorHandler bus)
        {
            _mchDivisionReceiverRepository = mchDivisionReceiverRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool Add(MchDivisionReceiverDto dto)
        {
            var m = _mapper.Map<MchDivisionReceiver>(dto);
            _mchDivisionReceiverRepository.Add(m);
            return _mchDivisionReceiverRepository.SaveChanges(out int _);
        }

        public bool Remove(long recordId)
        {
            _mchDivisionReceiverRepository.Remove(recordId);
            return _mchDivisionReceiverRepository.SaveChanges(out int _);
        }

        public bool Update(MchDivisionReceiverDto dto)
        {
            var m = _mapper.Map<MchDivisionReceiver>(dto);
            _mchDivisionReceiverRepository.Update(m);
            return _mchDivisionReceiverRepository.SaveChanges(out int _);
        }

        public MchDivisionReceiverDto GetById(long recordId)
        {
            var entity = _mchDivisionReceiverRepository.GetById(recordId);
            var dto = _mapper.Map<MchDivisionReceiverDto>(entity);
            return dto;
        }

        public MchDivisionReceiverDto GetById(long recordId, string mchNo)
        {
            var entity = _mchDivisionReceiverRepository.GetAll().Where(w => w.ReceiverId.Equals(recordId) && w.MchNo.Equals(mchNo)).FirstOrDefault();
            var dto = _mapper.Map<MchDivisionReceiverDto>(entity);
            return dto;
        }

        public int GetCount(HashSet<long> receiverIds, string mchNo, string appId, string ifCode, byte state = CS.YES)
        {
            var count = _mchDivisionReceiverRepository.GetAll()
                    .Where(w => receiverIds.Contains(w.ReceiverId)
                    && w.MchNo.Equals(mchNo) && w.AppId.Equals(appId) && w.IfCode.Equals(ifCode) && w.State.Equals(state)).Count();
            return count;
        }

        public int GetCount(HashSet<long> receiverGroupIds, string mchNo)
        {
            var count = _mchDivisionReceiverRepository.GetAll()
                    .Where(w => receiverGroupIds.Contains(w.ReceiverGroupId) && w.MchNo.Equals(mchNo)).Count();
            return count;
        }

        public IEnumerable<MchDivisionReceiverDto> GetAll()
        {
            var mchDivisionReceivers = _mchDivisionReceiverRepository.GetAll();
            return _mapper.Map<IEnumerable<MchDivisionReceiverDto>>(mchDivisionReceivers);
        }

        public bool IsExistUseReceiverGroup(long receiverGroupId)
        {
            return _mchDivisionReceiverRepository.IsExistUseReceiverGroup(receiverGroupId);
        }

        public PaginatedList<MchDivisionReceiverDto> GetPaginatedData(MchDivisionReceiverQueryDto dto)
        {
            var mchInfos = _mchDivisionReceiverRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (dto.ReceiverId.Equals(0) || w.ReceiverId.Equals(dto.ReceiverId))
                && (string.IsNullOrWhiteSpace(dto.ReceiverAlias) || w.ReceiverAlias.Equals(dto.ReceiverAlias))
                && (dto.ReceiverGroupId.Equals(0) || w.ReceiverGroupId.Equals(dto.ReceiverGroupId))
                && (string.IsNullOrWhiteSpace(dto.ReceiverGroupName) || w.ReceiverGroupName.Equals(dto.ReceiverGroupName))
                && (!dto.State.HasValue || w.State.Equals(dto.State))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (string.IsNullOrWhiteSpace(dto.IfCode) || w.IfCode.Equals(dto.IfCode))
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<MchDivisionReceiver>.Create<MchDivisionReceiverDto>(mchInfos.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
