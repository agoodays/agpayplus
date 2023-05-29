using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Commands.AgentInfos;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 代理商信息表 服务实现类
    /// </summary>
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

        public bool IsExistAgentNo(string mchNo)
        {
            return _agentInfoRepository.IsExistAgentNo(mchNo);
        }

        public bool IsExistAgent(string isvNo)
        {
            return _agentInfoRepository.IsExistAgent(isvNo);
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

        public bool UpdateById(AgentInfoUpdateDto dto)
        {
            var entity = _agentInfoRepository.GetById(dto.AgentNo);
            if (!string.IsNullOrWhiteSpace(dto.Sipw))
                entity.Sipw = dto.Sipw;
            entity.UpdatedAt = DateTime.Now;
            _agentInfoRepository.Update(entity);
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

        public AgentInfoDetailDto GetByAgentNo(string agentNo)
        {
            var agentInfo = _agentInfoRepository.GetById(agentNo);
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
            IOrderedQueryable<AgentInfo> agentInfos = GetAgentInfos(dto);
            var records = PaginatedList<AgentInfo>.Create<AgentInfoDto>(agentInfos.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }

        public PaginatedList<AgentInfoDto> GetPaginatedData(string agentNo, AgentInfoQueryDto dto)
        {
            IOrderedQueryable<AgentInfo> agentInfos = GetAgentInfos(dto);
            var subAgentInfos = GetSons(agentInfos.AsNoTracking(), agentNo).Where(w => w.AgentNo != agentNo);
            var records = PaginatedList<AgentInfo>.Create<AgentInfoDto>(subAgentInfos, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }

        private IOrderedQueryable<AgentInfo> GetAgentInfos(AgentInfoQueryDto dto)
        {
            var agentNos = new List<string>();
            if (!string.IsNullOrWhiteSpace(dto.LoginUsername))
            {
                agentNos = _sysUserRepository.GetAll().Where(w => w.SysType.Equals(CS.SYS_TYPE.AGENT) && w.LoginUsername.Equals(dto.LoginUsername))
                    .Select(s => s.BelongInfoId).AsNoTracking().ToList();
            }

            var agentInfos = _agentInfoRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                && (!agentNos.Any() || agentNos.Contains(dto.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.Pid) || w.IsvNo.Equals(dto.Pid))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (string.IsNullOrWhiteSpace(dto.AgentName) || w.AgentName.Contains(dto.AgentName) || w.AgentShortName.Contains(dto.AgentName))
                && (string.IsNullOrWhiteSpace(dto.ContactTel) || w.IsvNo.Equals(dto.ContactTel))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                ).OrderByDescending(o => o.CreatedAt);
            return agentInfos;
        }

        #region 获取所有下级
        private IEnumerable<AgentInfo> GetSons(IQueryable<AgentInfo> list, string pid)
        {
            var query = list.Where(p => p.AgentNo == pid).ToList();
            var list2 = query.Concat(GetSonList(list, pid));
            return list2;
        }

        private IEnumerable<AgentInfo> GetSonList(IQueryable<AgentInfo> list, string pid)
        {
            var query = list.Where(p => p.Pid == pid).ToList();
            return query.ToList().Concat(query.ToList().SelectMany(t => GetSonList(list, t.AgentNo)));
        }
        #endregion

        #region 获取所有上级
        private IEnumerable<AgentInfo> GetFatherList(IList<AgentInfo> list, string Id)
        {
            var query = list.Where(p => p.AgentNo == Id).ToList();
            return query.ToList().Concat(query.ToList().SelectMany(t => GetFatherList(list, t.Pid)));
        }
        #endregion
    }
}
