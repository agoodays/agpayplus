using System.Security.Claims;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Manager.Api.Extensions;
using AGooday.AgPay.Manager.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AGooday.AgPay.Manager.Api.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAuthorizationRequirement>
    {
        private readonly IDatabase _redis;

        public PermissionAuthorizationHandler(RedisUtil client)
        {
            _redis = client.GetDatabase();
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
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
                    string currentUserJson = _redis.StringGet(cacheKey);
                    if (string.IsNullOrWhiteSpace(currentUserJson))
                    {
                        throw new UnauthorizeException();
                        //throw new BizException("登录失效");
                    }
                    var currentUser = JsonConvert.DeserializeObject<CurrentUser>(currentUserJson);
                    var userIdClaim = context.User.FindFirst(_ => _.Type == ClaimTypes.NameIdentifier).Value;
                    if (userIdClaim != null && currentUser.Authorities.Intersect(requirement.Name).Any())
                    {
                        context.Succeed(requirement);
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
