using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Domain.Commands.MchInfos;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 商户信息表 服务实现类
    /// </summary>
    public class MchInfoService : IMchInfoService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IMchInfoRepository _mchInfoRepository;
        private readonly ISysUserRepository _sysUserRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public MchInfoService(IMapper mapper, IMediatorHandler bus,
            IMchInfoRepository mchInfoRepository,
            ISysUserRepository sysUserRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _mchInfoRepository = mchInfoRepository;
            _sysUserRepository = sysUserRepository;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool IsExistMchNo(string mchNo)
        {
            return _mchInfoRepository.IsExistMchNo(mchNo);
        }

        public bool IsExistMchByIsvNo(string isvNo)
        {
            return _mchInfoRepository.IsExistMchByIsvNo(isvNo);
        }

        public bool IsExistMchByAgentNo(string agentNo)
        {
            return _mchInfoRepository.IsExistMchByAgentNo(agentNo);
        }

        public bool Add(MchInfoDto dto)
        {
            var m = _mapper.Map<MchInfo>(dto);
            _mchInfoRepository.Add(m);
            return _mchInfoRepository.SaveChanges(out int _);
        }

        public async Task Create(MchInfoCreateDto dto)
        {
            var command = _mapper.Map<CreateMchInfoCommand>(dto);
            await Bus.SendCommand(command);
        }

        public async Task Remove(string recordId)
        {
            //_mchInfoRepository.Remove(recordId);
            //_mchInfoRepository.SaveChanges();
            var command = new RemoveMchInfoCommand() { MchNo = recordId };
            await Bus.SendCommand(command);
        }

        public bool Update(MchInfoDto dto)
        {
            var m = _mapper.Map<MchInfo>(dto);
            _mchInfoRepository.Update(m);
            return _mchInfoRepository.SaveChanges(out int _);
        }

        public bool UpdateById(MchInfoUpdateDto dto)
        {
            var entity = _mchInfoRepository.GetById(dto.MchNo);
            if (!string.IsNullOrWhiteSpace(dto.MchLevel))
                entity.MchLevel = dto.MchLevel;
            if (!string.IsNullOrWhiteSpace(dto.Sipw))
                entity.Sipw = dto.Sipw;
            entity.UpdatedAt = DateTime.Now;
            _mchInfoRepository.Update(entity);
            return _mchInfoRepository.SaveChanges(out int _);
        }

        public async Task Modify(MchInfoModifyDto dto)
        {
            var command = _mapper.Map<ModifyMchInfoCommand>(dto);
            await Bus.SendCommand(command);
        }

        public MchInfoDto GetById(string recordId)
        {
            var entity = _mchInfoRepository.GetById(recordId);
            var dto = _mapper.Map<MchInfoDto>(entity);
            return dto;
        }

        public IEnumerable<MchInfoDto> GetByMchNos(List<string> mchNos)
        {
            var mchInfos = _mchInfoRepository.GetAll().Where(w => mchNos.Contains(w.MchNo));
            return _mapper.Map<IEnumerable<MchInfoDto>>(mchInfos);
        }

        public IEnumerable<MchInfoDto> GetByIsvNo(string isvNo)
        {
            var mchInfos = _mchInfoRepository.GetAll().Where(w => w.IsvNo.Equals(isvNo));
            return _mapper.Map<IEnumerable<MchInfoDto>>(mchInfos);
        }

        public IEnumerable<MchInfoDto> GetAll()
        {
            var mchInfos = _mchInfoRepository.GetAll();
            return _mapper.Map<IEnumerable<MchInfoDto>>(mchInfos);
        }

        public PaginatedList<MchInfoDto> GetPaginatedData(MchInfoQueryDto dto)
        {
            var mchInfos = _mchInfoRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (string.IsNullOrWhiteSpace(dto.MchName) || w.MchName.Contains(dto.MchName) || w.MchShortName.Contains(dto.MchName))
                && (dto.Type.Equals(0) || w.Type.Equals(dto.Type))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<MchInfo>.Create<MchInfoDto>(mchInfos.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
