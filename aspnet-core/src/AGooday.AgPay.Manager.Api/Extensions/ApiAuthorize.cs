using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using StackExchange.Redis;

namespace AGooday.AgPay.Manager.Api.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ApiAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private IDatabase _redis;
        public ApiAuthorizeAttribute(string name)
        {
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.Filters.Contains(new IgnoreAuthorize()))
            {
                return;
            }

            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                throw new UnauthorizeException();
            }
            var client = ((RedisUtil)context.HttpContext.RequestServices.GetService(typeof(RedisUtil)));
            _redis = client.GetDatabase();
        }
    }
}
