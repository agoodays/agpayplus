using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchAppService : IAgPayService<MchAppDto>
    {
        MchAppDto GetById(string recordId, string mchNo);
        IEnumerable<MchAppDto> GetByMchNo(string mchNo);
        IEnumerable<MchAppDto> GetByMchNos(IEnumerable<string> mchNos);
        IEnumerable<MchAppDto> GetByAppIds(IEnumerable<string> appIds);
        PaginatedList<MchAppDto> GetPaginatedData(MchAppQueryDto dto, string agentNo = null);
    }
}
