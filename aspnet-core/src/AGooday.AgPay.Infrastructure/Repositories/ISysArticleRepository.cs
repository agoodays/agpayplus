using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class SysArticleRepository : Repository<SysArticle, long>, ISysArticleRepository
    {
        public SysArticleRepository(AgPayDbContext context)
            : base(context)
        {
        }
    }
}
