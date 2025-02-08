using System.Security.Claims;
using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Merchant.Api.Extensions.AuthContext
{
    public static class AuthContextService
    {
        private static IHttpContextAccessor _context;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _context = httpContextAccessor;
        }
        /// <summary>
        /// 
        /// </summary>
        public static HttpContext Current => _context?.HttpContext;
        /// <summary>
        /// 
        /// </summary>
        public static AuthContextUser CurrentUser => new AuthContextUser
        {
            SysUserId = Convert.ToInt64(Current?.User?.FindFirstValue(ClaimAttributes.SysUserId)),
            AvatarUrl = Current?.User?.FindFirstValue(ClaimAttributes.AvatarUrl),
            Realname = Current?.User?.FindFirstValue(ClaimAttributes.Realname),
            LoginUsername = Current?.User?.FindFirstValue(ClaimAttributes.LoginUsername),
            Telphone = Current?.User?.FindFirstValue(ClaimAttributes.Telphone),
            UserNo = Current?.User?.FindFirstValue(ClaimAttributes.UserNo),
            Sex = Convert.ToByte(Current?.User?.FindFirstValue(ClaimAttributes.Sex)),
            State = Convert.ToByte(Current?.User?.FindFirstValue(ClaimAttributes.State)),
            IsAdmin = Convert.ToByte(Current?.User?.FindFirstValue(ClaimAttributes.IsAdmin)),
            SysType = Current?.User?.FindFirstValue(ClaimAttributes.SysType),
            BelongInfoId = Current?.User?.FindFirstValue(ClaimAttributes.BelongInfoId),
            CreatedAt = Convert.ToDateTime(Current?.User?.FindFirstValue(ClaimAttributes.CreatedAt)),
            UpdatedAt = Convert.ToDateTime(Current?.User?.FindFirstValue(ClaimAttributes.UpdatedAt)),
            CacheKey = Current?.User?.FindFirstValue(ClaimAttributes.CacheKey)
        };

        /// <summary>
        /// 是否已授权
        /// </summary>
        public static bool IsAuthenticated => Current?.User?.Identity?.IsAuthenticated ?? false;

        /// <summary>
        /// 是否是超级管理员
        /// </summary>
        public static bool IsAdmin => (Convert.ToSByte(Current?.User?.FindFirstValue(ClaimAttributes.IsAdmin)) == CS.YES);
    }
}
