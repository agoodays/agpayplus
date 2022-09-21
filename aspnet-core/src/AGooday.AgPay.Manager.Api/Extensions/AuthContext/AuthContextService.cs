using AGooday.AgPay.Common.Constants;
using System.Security.Claims;

namespace AGooday.AgPay.Manager.Api.Extensions.AuthContext
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
        public static HttpContext Current => _context.HttpContext;
        /// <summary>
        /// 
        /// </summary>
        public static AuthContextUser CurrentUser
        {
            get
            {
                var user = new AuthContextUser
                {
                    Name = Current.User.FindFirstValue(ClaimTypes.Name),
                    UserId = Current.User.FindFirstValue("userid"),
                    Avatar = Current.User.FindFirstValue("avatar"),
                    DisplayName = Current.User.FindFirstValue("displayName"),
                    LoginName = Current.User.FindFirstValue("loginName"),
                    Telphone = Current.User.FindFirstValue("Telphone"),
                    UserNo = Current.User.FindFirstValue("userNo"),
                    IdentityType = Current.User.FindFirstValue("identityType"),
                    SysType = Current.User.FindFirstValue("sysType"),
                    CacheKey = Current.User.FindFirstValue("CacheKey")
                };
                return user;
            }
        }

        /// <summary>
        /// 是否已授权
        /// </summary>
        public static bool IsAuthenticated
        {
            get
            {
                return Current.User.Identity.IsAuthenticated;
            }
        }

        /// <summary>
        /// 是否是超级管理员
        /// </summary>
        public static bool IsAdmin
        {
            get
            {
                return (Convert.ToSByte(Current.User.FindFirstValue("isAdmin")) == CS.YES);
            }
        }
    }
}
