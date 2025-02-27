﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Merchant.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AGooday.AgPay.Merchant.Api.Extensions
{
    public static class JwtBearerAuthenticationExtension
    {
        /// <summary>
        /// 注册JWT Bearer认证服务的静态扩展方法
        /// </summary>
        /// <param name="services"></param>
        /// <param name="jwtSettings">JWT授权的配置项</param>
        public static void AddJwtBearerAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
        {
            //设置secret，使用应用密钥得到一个加密密钥字节数组
            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
            //添加认证服务
            services.AddAuthentication(options =>
            {
                //设置默认架构
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(options => options.SlidingExpiration = true)
            //添加Jwt自定义配置
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,//是否验证颁发者
                    ValidateAudience = true,//是否验证订阅者
                    ValidateLifetime = true,//是否验证失效时间
                    ValidateIssuerSigningKey = true,//是否验证SecurityKey
                    ValidIssuer = jwtSettings.Issuer,//颁发者，这两项和前面签发jwt的设置一致
                    ValidAudience = jwtSettings.Audience,//订阅者
                    IssuerSigningKey = new SymmetricSecurityKey(key)//认证秘钥
                };
            });
        }

        public static string IssueJwt(JwtSettings jwtSettings, JwtTokenModel tokenModel)
        {
            // 返回前端 accessToken
            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, tokenModel.SysUserId.ToString()),
                new Claim(ClaimTypes.Name, tokenModel.LoginUsername),
                new Claim(ClaimTypes.GivenName, tokenModel.Realname),
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

        private static string GetJwtAccessToken(JwtSettings jwtSettings, ClaimsIdentity claimsIdentity)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = jwtSettings.Issuer,
                Audience = jwtSettings.Audience,
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
        public static JwtTokenModel SerializeJwt(string jwtStr)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            JwtTokenModel tokenModelJwt = new JwtTokenModel();

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

        public static string GetTokenFromAuthorizationHeader(string authorizationHeader)
        {
            var jwtBearer = $"{JwtBearerDefaults.AuthenticationScheme} ";
            if (authorizationHeader.StartsWith(jwtBearer))
            {
                // 移除 "Bearer " 前缀以获取实际的 JWT
                return authorizationHeader.Substring(jwtBearer.Length);
            }

            // 无效的授权头格式
            throw new Exception("Invalid Authorization header format");
        }
    }

    /// <summary>
    /// 令牌
    /// </summary>
    public class JwtTokenModel
    {
        /// <summary>
        /// 系统用户ID
        /// </summary>
        public string SysUserId { get; set; }
        /// <summary>
        /// 头像地址
        /// </summary>
        public string AvatarUrl { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string Realname { get; set; }
        /// <summary>
        /// 预留信息
        /// </summary>
        public string SafeWord { get; set; }
        /// <summary>
        /// 登录用户名
        /// </summary>
        public string LoginUsername { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Telphone { get; set; }
        /// <summary>
        /// 员工编号
        /// </summary>
        public string UserNo { get; set; }
        /// <summary>
        /// 性别 0-未知, 1-男, 2-女
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 状态 0-停用 1-启用
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// 是否超管（超管拥有全部权限） 0-否 1-是
        /// </summary>
        public string IsAdmin { get; set; }
        /// <summary>
        /// 所属系统： MGR-运营平台, AGENT-代理商中心, MCH-商户中心
        /// </summary>
        public string SysType { get; set; }
        /// <summary>
        /// 所属商户ID / 代理商ID / 0(平台)
        /// </summary>
        public string BelongInfoId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreatedAt { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string UpdatedAt { get; set; }
        /// <summary>
        /// 缓存Key
        /// </summary>
        public string CacheKey { get; set; }
    }

    /// <summary>
    /// Claim属性
    /// </summary>
    public static class ClaimAttributes
    {
        /// <summary>
        /// 系统用户ID
        /// </summary>
        public const string SysUserId = "sui";
        /// <summary>
        /// 头像地址
        /// </summary>
        public const string AvatarUrl = "au";
        /// <summary>
        /// 真实姓名
        /// </summary>
        public const string Realname = "rn";
        /// <summary>
        /// 登录用户名
        /// </summary>
        public const string LoginUsername = "lun";
        /// <summary>
        /// 手机号
        /// </summary>
        public const string Telphone = "tel";
        /// <summary>
        /// 员工编号
        /// </summary>
        public const string UserNo = "un";
        /// <summary>
        /// 性别 0-未知, 1-男, 2-女
        /// </summary>
        public const string Sex = "sex";
        /// <summary>
        /// 状态 0-停用 1-启用
        /// </summary>
        public const string State = "sta";
        /// <summary>
        /// 是否超管（超管拥有全部权限） 0-否 1-是
        /// </summary>
        public const string IsAdmin = "ia";
        /// <summary>
        /// 所属系统： MGR-运营平台, AGENT-代理商中心, MCH-商户中心
        /// </summary>
        public const string SysType = "st";
        /// <summary>
        /// 所属商户ID / 0(平台)
        /// </summary>
        public const string BelongInfoId = "bii";
        /// <summary>
        /// 创建时间
        /// </summary>
        public const string CreatedAt = "cat";
        /// <summary>
        /// 更新时间
        /// </summary>
        public const string UpdatedAt = "uat";
        /// <summary>
        /// 缓存Key
        /// </summary>
        public const string CacheKey = "ck";
        /// <summary>
        /// 刷新过期时间
        /// </summary>
        public const string RefreshExpires = "re";
    }
}
