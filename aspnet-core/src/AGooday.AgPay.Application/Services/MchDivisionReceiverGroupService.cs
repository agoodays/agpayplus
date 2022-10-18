using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Domain.Commands.SysUsers;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Application.Services
{
    public class MchDivisionReceiverGroupService : IMchDivisionReceiverGroupService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IMchDivisionReceiverGroupRepository _mchDivisionReceiverGroupRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public MchDivisionReceiverGroupService(IMchDivisionReceiverGroupRepository mchDivisionReceiverGroupRepository, IMapper mapper, IMediatorHandler bus)
        {
            _mchDivisionReceiverGroupRepository = mchDivisionReceiverGroupRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool Add(MchDivisionReceiverGroupDto dto)
        {
            var m = _mapper.Map<MchDivisionReceiverGroup>(dto);
            _mchDivisionReceiverGroupRepository.Add(m);
            return _mchDivisionReceiverGroupRepository.SaveChanges(out int _);
        }

        public bool Remove(long recordId)
        {
            _mchDivisionReceiverGroupRepository.Remove(recordId);
            return _mchDivisionReceiverGroupRepository.SaveChanges(out int _);
        }

        public bool Update(MchDivisionReceiverGroupDto dto)
        {
            var m = _mapper.Map<MchDivisionReceiverGroup>(dto);
            _mchDivisionReceiverGroupRepository.Update(m);
            return _mchDivisionReceiverGroupRepository.SaveChanges(out int _);
        }

        public MchDivisionReceiverGroupDto GetById(long recordId)
        {
            var entity = _mchDivisionReceiverGroupRepository.GetById(recordId);
            var dto = _mapper.Map<MchDivisionReceiverGroupDto>(entity);
            return dto;
        }

        public MchDivisionReceiverGroupDto GetById(long recordId, string mchNo)
        {
            var entity = _mchDivisionReceiverGroupRepository.GetAll().Where(w => w.ReceiverGroupId.Equals(recordId) && w.MchNo.Equals(mchNo)).FirstOrDefault();
            return _mapper.Map<MchDivisionReceiverGroupDto>(entity);
        }

        public IEnumerable<MchDivisionReceiverGroupDto> GetAll()
        {
            var mchDivisionReceiverGroups = _mchDivisionReceiverGroupRepository.GetAll();
            return _mapper.Map<IEnumerable<MchDivisionReceiverGroupDto>>(mchDivisionReceiverGroups);
        }

        public IEnumerable<MchDivisionReceiverGroupDto> GetByMchNo(string mchNo)
        {
            var mchDivisionReceiverGroups = _mchDivisionReceiverGroupRepository.GetAll()
                .Where(w => w.MchNo.Equals(mchNo) && w.AutoDivisionFlag.Equals(CS.YES));
            return _mapper.Map<IEnumerable<MchDivisionReceiverGroupDto>>(mchDivisionReceiverGroups);
        }

        public MchDivisionReceiverGroupDto FindByIdAndMchNo(long receiverGroupId, string mchNo)
        {
            var entity = _mchDivisionReceiverGroupRepository.GetAll()
                .Where(w => w.ReceiverGroupId.Equals(receiverGroupId) && w.MchNo.Equals(mchNo));
            return _mapper.Map<MchDivisionReceiverGroupDto>(entity);
        }

        public PaginatedList<MchDivisionReceiverGroupDto> GetPaginatedData(MchDivisionReceiverGroupQueryDto dto)
        {
            var mchInfos = _mchDivisionReceiverGroupRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.ReceiverGroupName) || w.ReceiverGroupName.Equals(dto.ReceiverGroupName))
                && (dto.ReceiverGroupId.Equals(0) || w.ReceiverGroupId.Equals(dto.ReceiverGroupId))
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<MchDivisionReceiverGroup>.Create<MchDivisionReceiverGroupDto>(mchInfos.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
