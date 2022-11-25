using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IIsvInfoRepository : IRepository<IsvInfo>
    {
        bool IsExistIsvNo(string isvNo);
    }
}
