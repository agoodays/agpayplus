using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class SysEntitlementRepository : AgPayRepository<SysEntitlement>, ISysEntitlementRepository
    {
        public SysEntitlementRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public Task<SysEntitlement> GetByKeyAsNoTrackingAsync(string entId, string sysType)
        {
            return GetAllAsNoTracking().FirstOrDefaultAsync(w => w.SysType.Equals(sysType) && w.EntId.Equals(entId));
        }

        public Task<SysEntitlement> GetByKeyAsync(string entId, string sysType)
        {
            return DbSet.FirstOrDefaultAsync(w => w.SysType.Equals(sysType) && w.EntId.Equals(entId));
        }

        #region Sql
        public IEnumerable<SysEntitlement> GetSubSysEntitlementsFromSqlAsNoTracking(string entId, string sysType)
        {
            FormattableString sql = $@"WITH RECURSIVE sub_sys_entitlements AS (
              SELECT * FROM t_sys_entitlement WHERE ent_id = {entId} AND sys_type = {sysType}
              UNION ALL
              SELECT t.* FROM t_sys_entitlement t INNER JOIN sub_sys_entitlements sa ON t.pid = sa.ent_id AND t.sys_type = {sysType}
            )
            SELECT * FROM sub_sys_entitlements;";
            return FromSqlAsNoTracking(sql);
        }

        public IEnumerable<SysEntitlement> GetParentSysEntitlementsFromSqlAsNoTracking(string entId, string sysType)
        {
            FormattableString sql = $@"WITH RECURSIVE parent_sys_entitlements AS (
              SELECT * FROM t_sys_entitlement WHERE ent_id = {entId} AND sys_type = {sysType}
              UNION ALL
              SELECT t.* FROM t_sys_entitlement t INNER JOIN parent_sys_entitlements p ON t.ent_id = p.pid AND t.sys_type = {sysType}
            )
            SELECT * FROM parent_sys_entitlements;";
            return FromSqlAsNoTracking(sql);
        }
        #endregion
    }
}
