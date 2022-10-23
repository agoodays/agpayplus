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
                    SysUserId = Convert.ToInt64(Current.User.FindFirstValue("sysUserId")),
                    AvatarUrl = Current.User.FindFirstValue("avatarUrl"),
                    Realname = Current.User.FindFirstValue("realname"),
                    LoginUsername = Current.User.FindFirstValue("loginUsername"),
                    Telphone = Current.User.FindFirstValue("telphone"),
                    UserNo = Current.User.FindFirstValue("userNo"),
                    Sex = Convert.ToByte( Current.User.FindFirstValue("sex")),
                    State = Convert.ToByte(Current.User.FindFirstValue("state")),
                    IsAdmin = Convert.ToByte(Current.User.FindFirstValue("isAdmin")),
                    SysType = Current.User.FindFirstValue("sysType"),
                    BelongInfoId = Current.User.FindFirstValue("belongInfoId"),
                    CreatedAt = Convert.ToDateTime(Current.User.FindFirstValue("createdAt")),
                    UpdatedAt = Convert.ToDateTime(Current.User.FindFirstValue("updatedAt")),
                    CacheKey = Current.User.FindFirstValue("cacheKey")
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
