using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchAppService : IAgPayService<MchAppDto>
    {
        Task<MchAppDto> GetByIdAsync(string recordId, string mchNo);
        Task<MchAppDto> GetByIdAsNoTrackingAsync(string recordId, string mchNo);
        Task<List<MchAppDto>> GetByMchNoAsNoTrackingAsync(string mchNo);
        Task<List<MchAppDto>> GetByMchNosAsNoTrackingAsync(IEnumerable<string> mchNos);
        Task<List<MchAppDto>> GetByAppIdsAsNoTrackingAsync(IEnumerable<string> appIds);
        Task<PaginatedResult<MchAppDto>> GetPaginatedDataAsync(MchAppQueryDto dto, string agentNo = null);
    }
}
