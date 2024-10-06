using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Commands.AgentInfos;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 代理商信息表 服务实现类
    /// </summary>
    public class AgentInfoService : AgPayService<AgentInfoDto, AgentInfo>, IAgentInfoService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IAgentInfoRepository _agentInfoRepository;
        private readonly ISysUserRepository _sysUserRepository;

        public AgentInfoService(IMapper mapper, IMediatorHandler bus,
            IAgentInfoRepository agentInfoRepository,
            ISysUserRepository sysUserRepository)
            : base(mapper, bus, agentInfoRepository)
        {
            _agentInfoRepository = agentInfoRepository;
            _sysUserRepository = sysUserRepository;
        }

        public bool IsExistAgentNo(string mchNo)
        {
            return _agentInfoRepository.IsExistAgentNo(mchNo);
        }

        public bool IsExistAgent(string isvNo)
        {
            return _agentInfoRepository.IsExistAgent(isvNo);
        }

        public async Task CreateAsync(AgentInfoCreateDto dto)
        {
            var command = _mapper.Map<CreateAgentInfoCommand>(dto);
            await Bus.SendCommand(command);
        }

        public async Task RemoveAsync(string recordId)
        {
            //_agentInfoRepository.Remove(recordId);
            //_agentInfoRepository.SaveChanges();
            var command = new RemoveAgentInfoCommand() { AgentNo = recordId };
            await Bus.SendCommand(command);
        }

        public async Task ModifyAsync(AgentInfoModifyDto dto)
        {
            var command = _mapper.Map<ModifyAgentInfoCommand>(dto);
            await Bus.SendCommand(command);
        }

        public bool UpdateById(AgentInfoDto dto)
        {
            var entity = _mapper.Map<AgentInfo>(dto);
            if (!string.IsNullOrWhiteSpace(dto.Sipw))
                entity.Sipw = dto.Sipw;
            entity.UpdatedAt = DateTime.Now;
            _agentInfoRepository.Update(entity, e => new { e.Sipw, e.UpdatedAt });
            return _agentInfoRepository.SaveChanges(out int _);
        }

        public IEnumerable<AgentInfoDto> GetParents(string agentNo)
        {
            var agentInfos = _agentInfoRepository.GetAllAsNoTracking();
            var source = GetParents(agentInfos.ToList(), agentNo);
            return _mapper.Map<IEnumerable<AgentInfoDto>>(source);
        }

        public PaginatedList<AgentInfoDto> GetPaginatedData(AgentInfoQueryDto dto)
        {
            var agentInfos = GetAgentInfos(dto);
            var records = PaginatedList<AgentInfo>.Create<AgentInfoDto>(agentInfos, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }

        public PaginatedList<AgentInfoDto> GetPaginatedData(string agentNo, AgentInfoQueryDto dto)
        {
            var agentInfos = GetAgentInfos(dto);
            var subAgentInfos = GetSons(agentInfos, agentNo).Where(w => w.AgentNo != agentNo);
            var records = PaginatedList<AgentInfo>.Create<AgentInfoDto>(subAgentInfos, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }

        private IOrderedQueryable<AgentInfo> GetAgentInfos(AgentInfoQueryDto dto)
        {
            var agentNos = new List<string>();
            if (!string.IsNullOrWhiteSpace(dto.LoginUsername))
            {
                agentNos = _sysUserRepository.GetAllAsNoTracking().Where(w => w.SysType.Equals(CS.SYS_TYPE.AGENT) && w.LoginUsername.Equals(dto.LoginUsername))
                    .Select(s => s.BelongInfoId).ToList();
            }

            var agentInfos = _agentInfoRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                && (agentNos.Count == 0 || agentNos.Contains(dto.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.Pid) || w.Pid.Equals(dto.Pid))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (string.IsNullOrWhiteSpace(dto.AgentName) || w.AgentName.Contains(dto.AgentName) || w.AgentShortName.Contains(dto.AgentName))
                && (string.IsNullOrWhiteSpace(dto.ContactTel) || w.IsvNo.Equals(dto.ContactTel))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                ).OrderByDescending(o => o.CreatedAt);
            return agentInfos;
        }

        #region 获取所有下级
        private static IQueryable<AgentInfo> GetSons(IQueryable<AgentInfo> source, string pid)
        {
            var query = source.Where(p => p.AgentNo == pid);
            var newSource = query.Concat(GetSonSource(source, pid));
            return newSource;
        }

        private static IQueryable<AgentInfo> GetSonSource(IQueryable<AgentInfo> source, string pid)
        {
            var query = source.Where(p => p.Pid == pid);
            return query.Concat(query.SelectMany(t => GetSonSource(source, t.AgentNo)));
        }
        #endregion

        #region 获取所有上级
        private static IEnumerable<AgentInfo> GetFatherList(IList<AgentInfo> list, string Id)
        {
            var query = list.Where(p => p.AgentNo == Id).ToList();
            return query.ToList().Concat(query.ToList().SelectMany(t => GetFatherList(list, t.Pid)));
        }
        private static IEnumerable<AgentInfo> GetParents(IEnumerable<AgentInfo> list, string agentNo)
        {
            var query = list.Where(p => p.AgentNo.Equals(agentNo));
            return query.Concat(query.SelectMany(t => GetParents(list, t.Pid)));
        }
        #endregion
    }
}
