using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class IsvInfoRepository : AgPayRepository<IsvInfo>, IIsvInfoRepository
    {
        public IsvInfoRepository(AgPayDbContext context)
            : base(context)
        {
        }
        public bool IsExistIsvNo(string isvNo)
        {
            return GetAllAsNoTracking().Any(c => c.IsvNo.Equals(isvNo));
        }
    }
}
