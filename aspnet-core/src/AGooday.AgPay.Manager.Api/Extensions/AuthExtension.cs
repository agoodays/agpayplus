using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Manager.Api.Extensions
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

            if (auth.UserType.Equals(CS.USER_TYPE.Expand))
            {
                ents = authService.GetEntsBySysType(auth.SysType, null)
                    .Where(w => w.MatchRule != null && w.MatchRule.EpUserEnt.Value)
                    .ToList();
            }
            authorities.AddRange(ents.Select(s => s.EntId));
        }
    }
}
