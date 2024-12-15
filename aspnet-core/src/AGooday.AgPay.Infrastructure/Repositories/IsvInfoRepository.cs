using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class IsvInfoRepository : AgPayRepository<IsvInfo>, IIsvInfoRepository
    {
        public IsvInfoRepository(AgPayDbContext context)
            : base(context)
        {
        }
        public Task<bool> IsExistIsvNoAsync(string isvNo)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.IsvNo.Equals(isvNo));
        }
    }
}
