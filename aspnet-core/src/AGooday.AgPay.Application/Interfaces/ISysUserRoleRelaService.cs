using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysUserRoleRelaService : IAgPayService<SysUserRoleRelaDto>
    {
        /// <summary>
        /// 分配用户角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        Task<int> SaveUserRoleAsync(long userId, List<string> roleIds);
        IEnumerable<string> SelectRoleIdsByUserId(long userId);
        IEnumerable<long> SelectUserIdsByRoleId(string roleId);
        Task<PaginatedList<SysUserRoleRelaDto>> GetPaginatedDataAsync(SysUserRoleRelaQueryDto dto);
    }
}
