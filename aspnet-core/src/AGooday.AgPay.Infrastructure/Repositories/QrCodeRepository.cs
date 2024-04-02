using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class QrCodeRepository : AgPayRepository<QrCode>, IQrCodeRepository
    {
        public QrCodeRepository(AgPayDbContext context)
            : base(context)
        {
        }
    }
}
