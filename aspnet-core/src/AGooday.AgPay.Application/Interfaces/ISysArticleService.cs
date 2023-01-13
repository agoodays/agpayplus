using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysArticleService : IDisposable
    {
        bool Add(SysArticleDto dto);
        bool Remove(long recordId);
        bool Update(SysArticleDto dto);
        SysArticleDto GetById(long recordId);
        IEnumerable<SysArticleDto> GetAll();
        PaginatedList<SysArticleDto> GetPaginatedData(SysArticleQueryDto dto, string agentNo = null);
    }
}
