using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Domain.Commands.AgentInfos;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Application.Services
{
    public class AgentInfoService : IAgentInfoService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IAgentInfoRepository _agentInfoRepository;
        private readonly ISysUserRepository _sysUserRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public AgentInfoService(IMapper mapper, IMediatorHandler bus,
            IAgentInfoRepository agentInfoRepository,
            ISysUserRepository sysUserRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _agentInfoRepository = agentInfoRepository;
            _sysUserRepository = sysUserRepository;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool IsExistMchNo(string mchNo)
        {
            return _agentInfoRepository.IsExistAgentNo(mchNo);
        }

        public bool Add(AgentInfoDto dto)
        {
            var m = _mapper.Map<AgentInfo>(dto);
            _agentInfoRepository.Add(m);
            return _agentInfoRepository.SaveChanges(out int _);
        }

        public void Create(AgentInfoCreateDto dto)
        {
            var command = _mapper.Map<CreateAgentInfoCommand>(dto);
            Bus.SendCommand(command);
        }

        public void Remove(string recordId)
        {
            //_agentInfoRepository.Remove(recordId);
            //_agentInfoRepository.SaveChanges();
            var command = new RemoveAgentInfoCommand() { AgentNo = recordId };
            Bus.SendCommand(command);
        }

        public bool Update(AgentInfoDto dto)
        {
            var m = _mapper.Map<AgentInfo>(dto);
            _agentInfoRepository.Update(m);
            return _agentInfoRepository.SaveChanges(out int _);
        }

        public async void Modify(AgentInfoModifyDto dto)
        {
            var command = _mapper.Map<ModifyAgentInfoCommand>(dto);
            await Bus.SendCommand(command);
        }

        public AgentInfoDto GetById(string recordId)
        {
            var entity = _agentInfoRepository.GetById(recordId);
            var dto = _mapper.Map<AgentInfoDto>(entity);
            return dto;
        }

        public AgentInfoDetailDto GetByMchNo(string mchNo)
        {
            var agentInfo = _agentInfoRepository.GetById(mchNo);
            var dto = _mapper.Map<AgentInfoDetailDto>(agentInfo);
            var sysUser = _sysUserRepository.GetById(agentInfo.InitUserId.Value);
            dto.LoginUsername = sysUser.LoginUsername;
            return dto;
        }

        public IEnumerable<AgentInfoDto> GetAll()
        {
            var agentInfos = _agentInfoRepository.GetAll();
            return _mapper.Map<IEnumerable<AgentInfoDto>>(agentInfos);
        }

        public PaginatedList<AgentInfoDto> GetPaginatedData(AgentInfoQueryDto dto)
        {
            var agentInfos = _agentInfoRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (string.IsNullOrWhiteSpace(dto.AgentName) || w.AgentName.Contains(dto.AgentName) || w.AgentShortName.Contains(dto.AgentName))
                && (dto.Type.Equals(0) || w.Type.Equals(dto.Type))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<AgentInfo>.Create<AgentInfoDto>(agentInfos.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
