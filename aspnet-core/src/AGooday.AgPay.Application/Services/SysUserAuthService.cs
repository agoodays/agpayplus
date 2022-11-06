using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 操作员认证表 服务实现类
    /// </summary>
    public class SysUserAuthService : ISysUserAuthService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysUserAuthRepository _sysUserAuthRepository;
        private readonly ISysUserRepository _sysUserRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public SysUserAuthService(IMapper mapper, IMediatorHandler bus,
            ISysUserAuthRepository sysUserAuthRepository,
            ISysUserRepository sysUserRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _sysUserAuthRepository = sysUserAuthRepository;
            _sysUserRepository = sysUserRepository;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(SysUserAuthDto dto)
        {
            var m = _mapper.Map<SysUserAuth>(dto);
            _sysUserAuthRepository.Add(m);
            _sysUserAuthRepository.SaveChanges();
        }

        public void Remove(long recordId)
        {
            _sysUserAuthRepository.Remove(recordId);
            _sysUserAuthRepository.SaveChanges();
        }

        public void Update(SysUserAuthDto dto)
        {
            var m = _mapper.Map<SysUserAuth>(dto);
            _sysUserAuthRepository.Update(m);
            _sysUserAuthRepository.SaveChanges();
        }

        public SysUserAuthDto GetById(long recordId)
        {
            var entity = _sysUserAuthRepository.GetById(recordId);
            var dto = _mapper.Map<SysUserAuthDto>(entity);
            return dto;
        }

        public IEnumerable<SysUserAuthDto> GetAll()
        {
            var sysUserAuths = _sysUserAuthRepository.GetAll();
            return _mapper.Map<IEnumerable<SysUserAuthDto>>(sysUserAuths);
        }

        public SysUserAuthInfoDto GetUserAuthInfoById(long userId)
        {
            var entity = _sysUserAuthRepository.GetAll()
                .Join(_sysUserAuthRepository.GetAll<SysUser>(),
                ua => ua.UserId, ur => ur.SysUserId,
                (ua, ur) => new { ua, ur })
                .Where(w => w.ua.UserId == userId)
                .Select(s => new SysUserAuthInfoDto
                {
                    SysUserId = s.ur.SysUserId,
                    LoginUsername = s.ur.LoginUsername,
                    Realname = s.ur.Realname,
                    Telphone = s.ur.Telphone,
                    Sex = s.ur.Sex,
                    AvatarUrl = s.ur.AvatarUrl,
                    UserNo = s.ur.UserNo,
                    IsAdmin = s.ur.IsAdmin,
                    SysType = s.ur.SysType,
                    IdentityType = s.ua.IdentityType,
                    Identifier = s.ua.Identifier,
                    Credential = s.ua.Credential
                })
                .FirstOrDefault();
            return entity;
        }

        public SysUserAuthInfoDto SelectByLogin(string identifier, byte identityType, string sysType)
        {
            var entity = _sysUserAuthRepository.GetAll()
                .Join(_sysUserAuthRepository.GetAll<SysUser>(),
                ua => ua.UserId, ur => ur.SysUserId,
                (ua, ur) => new { ua, ur })
                .Where(w => w.ua.IdentityType == identityType && w.ua.Identifier.Equals(identifier) && w.ua.SysType.Equals(sysType) && w.ur.State == CS.PUB_USABLE)
                .Select(s => new SysUserAuthInfoDto
                {
                    SysUserId = s.ur.SysUserId,
                    LoginUsername = s.ur.LoginUsername,
                    Realname = s.ur.Realname,
                    Telphone = s.ur.Telphone,
                    Sex = s.ur.Sex,
                    AvatarUrl = s.ur.AvatarUrl,
                    UserNo = s.ur.UserNo,
                    IsAdmin = s.ur.IsAdmin,
                    State = s.ur.State,
                    SysType = s.ur.SysType,
                    IdentityType = s.ua.IdentityType,
                    Identifier = s.ua.Identifier,
                    Credential = s.ua.Credential,
                    BelongInfoId = s.ur.BelongInfoId,
                    CreatedAt = s.ur.CreatedAt,
                    UpdatedAt = s.ur.UpdatedAt
                })
                .FirstOrDefault();
            return entity;
        }

        public void ResetAuthInfo(long resetUserId, string authLoginUserName, string telphone, string newPwd, string sysType)
        {
            _sysUserAuthRepository.ResetAuthInfo(resetUserId, authLoginUserName, telphone, newPwd, sysType);
            _sysUserAuthRepository.SaveChanges();
        }
    }
}
