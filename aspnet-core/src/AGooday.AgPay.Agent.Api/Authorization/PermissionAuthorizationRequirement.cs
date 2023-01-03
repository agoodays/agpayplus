using Microsoft.AspNetCore.Authorization;

namespace AGooday.AgPay.Agent.Api.Authorization
{
    public class PermissionAuthorizationRequirement : IAuthorizationRequirement
    {
        public PermissionAuthorizationRequirement(params string[] name)
        {
            Name = name;
        }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string[] Name { get; set; }
    }
}
