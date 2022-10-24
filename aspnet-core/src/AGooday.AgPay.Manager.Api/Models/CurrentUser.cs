using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Manager.Api.Models
{
    public class CurrentUser
    {
        public string CacheKey { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public SysUserAuthInfoDto SysUser { get; set; }

        /// <summary>
        /// 角色+权限 集合   （角色必须以： ROLE_ 开头） 
        /// </summary>
        public IEnumerable<string> Authorities { get; set; }
    }
}
