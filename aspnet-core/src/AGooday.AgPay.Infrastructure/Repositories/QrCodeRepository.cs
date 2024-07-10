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

        public QrCode GetByIdAsNoTracking(string recordId)
        {
            return GetAllAsNoTracking().FirstOrDefault(w => w.QrcId.Equals(recordId));
        }
    }
}
