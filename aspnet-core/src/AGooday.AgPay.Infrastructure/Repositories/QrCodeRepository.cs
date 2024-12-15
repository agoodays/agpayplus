using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class QrCodeRepository : AgPayRepository<QrCode>, IQrCodeRepository
    {
        public QrCodeRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public Task<bool> IsExistBatchIdAsync(string batchId)
        {
            return GetAllAsNoTracking().AnyAsync(c => c.BatchId.Equals(batchId));
        }

        public Task<QrCode> GetByIdAsNoTrackingAsync(string recordId)
        {
            return GetAllAsNoTracking().FirstOrDefaultAsync(w => w.QrcId.Equals(recordId));
        }
    }
}
