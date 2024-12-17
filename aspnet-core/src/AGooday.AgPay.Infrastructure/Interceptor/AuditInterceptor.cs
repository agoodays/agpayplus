using System.Security.Claims;
using AGooday.AgPay.Domain.Core.Tracker;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AGooday.AgPay.Infrastructure.Interceptor
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _context;

        public AuditInterceptor(IHttpContextAccessor context)
        {
            _context = context;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            SetAuditInfo(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            SetAuditInfo(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void SetAuditInfo(DbContext context)
        {
            var (userId, realname) = GetCurrentUser();

            var entries = context.ChangeTracker.Entries<ITrackableUser>();

            foreach (var entry in entries)
            {
                if (entry.Entity is ITrackableUser trackable)
                {
                    if (entry.State == EntityState.Added)
                    {
                        trackable.CreatedUid ??= userId;
                        trackable.CreatedBy ??= realname;
                    }
                    //else if (entry.State == EntityState.Modified)
                    //{
                    //    //trackable.UpdatedUid ??= userId;
                    //    //trackable.UpdatedBy ??= realname;
                    //}
                }
            }

            // 获取所有具有 UpdatedUid 和 UpdatedBy 属性的实体
            var updatedEntries = context.ChangeTracker.Entries()
                .Where(e => e.Entity.GetType().GetProperty("UpdatedUid") != null &&
                            e.Entity.GetType().GetProperty("UpdatedBy") != null);

            foreach (var entry in updatedEntries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("UpdatedUid").CurrentValue ??= userId;
                    entry.Property("UpdatedBy").CurrentValue ??= realname;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property("UpdatedUid").CurrentValue ??= userId;
                    entry.Property("UpdatedBy").CurrentValue ??= realname;
                }
            }
        }

        private (long? userId, string realname) GetCurrentUser()
        {
            string userIdStr = _context?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            long? userId = string.IsNullOrWhiteSpace(userIdStr) ? null : Convert.ToInt64(userIdStr);
            string realname = _context?.HttpContext?.User?.FindFirst(ClaimTypes.GivenName)?.Value;
            return (userId, realname);
        }
    }
}
