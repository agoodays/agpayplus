using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysArticleService : IAgPayService<SysArticleDto, long>
    {
        Task<PaginatedList<SysArticleDto>> GetPaginatedDataAsync(SysArticleQueryDto dto, string agentNo = null);
    }
}
