using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Manager.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AGooday.AgPay.Manager.Api.Extensions
{
    public static class JwtBearerAuthenticationExtension
    {
        /// <summary>
        /// 注册JWT Bearer认证服务的静态扩展方法
        /// </summary>
        /// <param name="services"></param>
        /// <param name="appSettings">JWT授权的配置项</param>
        public static void AddJwtBearerAuthentication(this IServiceCollection services, JwtSettings appSettings)
        {
            //使用应用密钥得到一个加密密钥字节数组
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(cfg => cfg.SlidingExpiration = true)
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,//是否验证Issuer
                    ValidateAudience = true,//是否验证Audience
                    ValidateLifetime = true,//是否验证失效时间
                    ValidateIssuerSigningKey = true,//是否验证SecurityKey
                    ValidIssuer = appSettings.Issuer,//Issuer，这两项和前面签发jwt的设置一致
                    ValidAudience = appSettings.Audience,//订阅者
                    IssuerSigningKey = new SymmetricSecurityKey(key)//密钥
                };
            });
        }

        public static string IssueJwt(JwtSettings jwtSettings, TokenModelJwt tokenModel)
        {
            // 返回前端 accessToken
            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, tokenModel.SysUserId.ToString()),
                new Claim(ClaimTypes.Name, tokenModel.LoginUsername),
                new Claim(ClaimAttributes.SysUserId,tokenModel.SysUserId.ToString()),
                new Claim(ClaimAttributes.AvatarUrl,tokenModel.AvatarUrl),
                new Claim(ClaimAttributes.Realname,tokenModel.Realname),
                new Claim(ClaimAttributes.LoginUsername,tokenModel.LoginUsername),
                new Claim(ClaimAttributes.Telphone,tokenModel.Telphone),
                new Claim(ClaimAttributes.UserNo,tokenModel.UserNo.ToString()),
                new Claim(ClaimAttributes.Sex,tokenModel.Sex.ToString()),
                new Claim(ClaimAttributes.State,tokenModel.State.ToString()),
                new Claim(ClaimAttributes.IsAdmin,tokenModel.IsAdmin.ToString()),
                new Claim(ClaimAttributes.SysType,tokenModel.SysType),
                new Claim(ClaimAttributes.BelongInfoId,tokenModel.BelongInfoId),
                new Claim(ClaimAttributes.CreatedAt,tokenModel.CreatedAt),
                new Claim(ClaimAttributes.UpdatedAt,tokenModel.UpdatedAt),
                new Claim(ClaimAttributes.CacheKey,tokenModel.CacheKey)
            });
            if (tokenModel.IsAdmin.Equals(CS.YES))
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            }
            return GetJwtAccessToken(jwtSettings, claimsIdentity);
        }

        private static string GetJwtAccessToken(JwtSettings appSettings, ClaimsIdentity claimsIdentity)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = appSettings.Issuer,
                Audience = appSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public static TokenModelJwt SerializeJwt(string jwtStr)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            TokenModelJwt tokenModelJwt = new TokenModelJwt();

            // token校验
            if (jwtStr.IsNotEmptyOrNull() && jwtHandler.CanReadToken(jwtStr))
            {
                JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);

                Claim[] claimArr = jwtToken?.Claims?.ToArray();
                if (claimArr != null && claimArr.Length > 0)
                {
                    tokenModelJwt.SysUserId = claimArr.FirstOrDefault(a => a.Type == ClaimAttributes.SysUserId)?.Value;
                    tokenModelJwt.AvatarUrl = claimArr.FirstOrDefault(a => a.Type == ClaimAttributes.AvatarUrl)?.Value;
                    tokenModelJwt.Realname = claimArr.FirstOrDefault(a => a.Type == ClaimAttributes.Realname)?.Value;
                    tokenModelJwt.LoginUsername = claimArr.FirstOrDefault(a => a.Type == ClaimAttributes.LoginUsername)?.Value;
                    tokenModelJwt.Telphone = claimArr.FirstOrDefault(a => a.Type == ClaimAttributes.Telphone)?.Value;
                    tokenModelJwt.UserNo = claimArr.FirstOrDefault(a => a.Type == ClaimAttributes.UserNo)?.Value;
                    tokenModelJwt.Sex = claimArr.FirstOrDefault(a => a.Type == ClaimAttributes.Sex)?.Value;
                    tokenModelJwt.State = claimArr.FirstOrDefault(a => a.Type == ClaimAttributes.State)?.Value;
                    tokenModelJwt.IsAdmin = claimArr.FirstOrDefault(a => a.Type == ClaimAttributes.IsAdmin)?.Value;
                    tokenModelJwt.SysType = claimArr.FirstOrDefault(a => a.Type == ClaimAttributes.SysType)?.Value;
                    tokenModelJwt.BelongInfoId = claimArr.FirstOrDefault(a => a.Type == ClaimAttributes.BelongInfoId)?.Value;
                    tokenModelJwt.CreatedAt = claimArr.FirstOrDefault(a => a.Type == ClaimAttributes.CreatedAt)?.Value;
                    tokenModelJwt.UpdatedAt = claimArr.FirstOrDefault(a => a.Type == ClaimAttributes.UpdatedAt)?.Value;
                    tokenModelJwt.CacheKey = claimArr.FirstOrDefault(a => a.Type == ClaimAttributes.CacheKey)?.Value;
                }
            }
            return tokenModelJwt;
        }
    }

    /// <summary>
    /// 令牌
    /// </summary>
    public class TokenModelJwt
    {
        public string SysUserId { get; set; }
        public string AvatarUrl { get; set; }
        public string Realname { get; set; }
        public string LoginUsername { get; set; }
        public string Telphone { get; set; }
        public string UserNo { get; set; }
        public string Sex { get; set; }
        public string State { get; set; }
        public string IsAdmin { get; set; }
        public string SysType { get; set; }
        public string BelongInfoId { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string CacheKey { get; set; }
    }

    /// <summary>
    /// Claim属性
    /// </summary>
    public static class ClaimAttributes
    {
        public const string SysUserId = "sui";
        public const string AvatarUrl = "au";
        public const string Realname = "rn";
        public const string LoginUsername = "lun";
        public const string Telphone = "tel";
        public const string UserNo = "un";
        public const string Sex = "sex";
        public const string State = "sta";
        public const string IsAdmin = "ia";
        public const string SysType = "st";
        public const string BelongInfoId = "bii";
        public const string CreatedAt = "cat";
        public const string UpdatedAt = "uat";
        public const string CacheKey = "ck";
        public const string RefreshExpires = "re";
    }
}
