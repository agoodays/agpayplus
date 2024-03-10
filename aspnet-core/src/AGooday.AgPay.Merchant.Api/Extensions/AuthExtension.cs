using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Merchant.Api.Extensions
{
    public static class AuthExtension
    {
        public static void GetEnts(this SysUserAuthInfoDto auth, IAuthService authService,
            out List<string> authorities, out List<SysEntitlementDto> ents)
        {
            authorities = new List<string>();
            ents = new List<SysEntitlementDto>();
            if (auth.UserType.Equals(CS.USER_TYPE.ADMIN) || auth.UserType.Equals(CS.USER_TYPE.OPERATOR))
            {
                authorities = authService.GetUserRolesByUserId(auth.SysUserId).Select(s => s.RoleId).ToList();
                ents = authService.GetEntsByUserId(auth.SysUserId, auth.UserType, auth.SysType)
                    .ToList();
            }

            if (auth.UserType.Equals(CS.USER_TYPE.DIRECTOR) || auth.UserType.Equals(CS.USER_TYPE.CLERK))
            {
                ents = authService.GetEntsBySysType(auth.SysType, null)
                    .Where(w => w.MatchRule != null && w.MatchRule.UserEntRules != null && w.MatchRule.UserEntRules.Intersect(auth.EntRules).Any())
                    .ToList();
            }
            ents = ents.Where(w => w.MatchRule == null
            || (w.MatchRule.MchType == null && w.MatchRule.MchLevelArray == null)
            || (w.MatchRule.MchType != null && w.MatchRule.MchType.Equals(auth.MchType))
            || (w.MatchRule.MchLevelArray != null && w.MatchRule.MchLevelArray.Contains(auth.MchLevel))).ToList();
            authorities.AddRange(ents.Select(s => s.EntId));
        }
    }
}
