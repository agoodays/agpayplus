using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IAgentInfoService : IDisposable
    {
        bool IsExistMchNo(string mchNo);
        bool Add(AgentInfoDto dto);
        void Create(AgentInfoCreateDto dto);
        void Remove(string recordId);
        bool Update(AgentInfoDto dto);
        void Modify(AgentInfoModifyDto dto);
        AgentInfoDto GetById(string recordId);
        AgentInfoDetailDto GetByMchNo(string mchNo);
        IEnumerable<AgentInfoDto> GetAll();
        PaginatedList<AgentInfoDto> GetPaginatedData(AgentInfoQueryDto dto);
    }
}
