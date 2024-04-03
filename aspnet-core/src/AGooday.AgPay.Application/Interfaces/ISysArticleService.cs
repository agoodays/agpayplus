using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysArticleService : IAgPayService<SysArticleDto, long>
    {
        PaginatedList<SysArticleDto> GetPaginatedData(SysArticleQueryDto dto, string agentNo = null);
    }
}
