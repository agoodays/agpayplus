using System.Security.Claims;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Manager.Api.Extensions;
using AGooday.AgPay.Manager.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace AGooday.AgPay.Manager.Api.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAuthorizationRequirement>
    {
        private readonly ICacheService _cacheService;

        public PermissionAuthorizationHandler(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
        {
            if (context.User != null)
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    throw new UnauthorizeException();
                }

                if (context.User.IsInRole("Admin"))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    var cacheKey = context.User.FindFirstValue(ClaimAttributes.CacheKey);
                    var currentUser = await _cacheService.GetAsync<CurrentUser>(cacheKey);
                    if (currentUser == null)
                    {
                        throw new UnauthorizeException();
                        //throw new BizException("登录失效");
                    }
                    var userIdClaim = context.User.FindFirst(_ => _.Type == ClaimTypes.NameIdentifier).Value;
                    if (userIdClaim != null && currentUser.Authorities.Intersect(requirement.Name).Any())
                    {
                        context.Succeed(requirement);
                    }
                }
            }
        }
    }
}
