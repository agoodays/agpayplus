using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class QrCodeShellRepository : Repository<QrCodeShell>, IQrCodeShellRepository
    {
        public QrCodeShellRepository(AgPayDbContext context)
            : base(context)
        {
        }
    }
}
