using AGooday.AgPay.Domain.Core.Tracker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AGooday.AgPay.Infrastructure.Interceptor
{
    /// <summary>
    /// 拦截器
    /// </summary>
    public class TimestampInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            SetTimestamps(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            SetTimestamps(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void SetTimestamps(DbContext context)
        {
            var now = DateTime.Now;

            //var entries = context.ChangeTracker.Entries<AbstractTrackableTimestamps>();
            var entries = context.ChangeTracker.Entries<ITrackableTimestamps>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    // 如果是新增记录，则设置 CreatedAt 和 UpdatedAt
                    //entry.Property(e => e.CreatedAt).CurrentValue ??= now;
                    //entry.Property(e => e.UpdatedAt).CurrentValue ??= now;
                    entry.Entity.CreatedAt ??= now;
                    entry.Entity.UpdatedAt ??= now;
                }
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt ??= now;
                    //entry.Property(e => e.UpdatedAt).CurrentValue ??= now;
                    // 确保 CreatedAt 不被修改
                    entry.Property(e => e.CreatedAt).IsModified = false;
                }
            }

            #region
            //// 获取所有具有 CreatedAt 和 UpdatedAt 属性的实体
            //var entries = context.ChangeTracker.Entries()
            //    .Where(e => e.Entity.GetType().GetProperty("CreatedAt") != null &&
            //                e.Entity.GetType().GetProperty("UpdatedAt") != null);

            //foreach (var entry in entries)
            //{
            //    if (entry.State == EntityState.Added)
            //    {
            //        entry.Property("CreatedAt").CurrentValue ??= now;
            //        entry.Property("UpdatedAt").CurrentValue ??= now;
            //    }
            //    else if (entry.State == EntityState.Modified)
            //    {
            //        entry.Property("UpdatedAt").CurrentValue ??= now;
            //        entry.Property("CreatedAt").IsModified ??= false;
            //    }
            //} 
            #endregion
        }
    }
}
