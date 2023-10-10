using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchAppService : IDisposable
    {
        bool Add(MchAppDto dto);
        bool Remove(string recordId);
        bool Update(MchAppDto dto);
        MchAppDto GetById(string recordId);
        MchAppDto GetById(string recordId, string mchNo);
        IEnumerable<MchAppDto> GetByMchNo(string mchNo);
        IEnumerable<MchAppDto> GetByMchNos(IEnumerable<string> mchNos);
        IEnumerable<MchAppDto> GetAll();
        PaginatedList<MchAppDto> GetPaginatedData(MchAppQueryDto dto, string agentNo = null);
    }
}
