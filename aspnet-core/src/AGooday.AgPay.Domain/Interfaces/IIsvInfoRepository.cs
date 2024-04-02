using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IIsvInfoRepository : IAgPayRepository<IsvInfo>
    {
        bool IsExistIsvNo(string isvNo);
    }
}
