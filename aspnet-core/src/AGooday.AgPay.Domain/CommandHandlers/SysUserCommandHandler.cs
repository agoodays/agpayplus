using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Commands.SysUsers;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Core.Notifications;
using AGooday.AgPay.Domain.Events.SysUsers;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace AGooday.AgPay.Domain.CommandHandlers
{
    /// <summary>
    /// Users领域命令处理程序
    /// 
    /// IRequestHandler 也是通过结构类型Unit来处理不需要返回值的情况。
    /// </summary>
    public class SysUserCommandHandler : CommandHandler,
        IRequestHandler<CreateSysUserCommand, Unit>,
        IRequestHandler<RemoveSysUserCommand, Unit>,
        IRequestHandler<ModifySysUserCommand, Unit>
    {
        // 注入仓储接口
        private readonly ISysUserRepository _sysUserRepository;
        private readonly ISysUserAuthRepository _sysUserAuthRepository;
        private readonly ISysUserRoleRelaRepository _sysUserRoleRelaRepository;

        // 用来进行DTO
        private readonly IMapper _mapper;

        // 注入总线
        private readonly IMediatorHandler Bus;
        private IMemoryCache Cache;

        public SysUserCommandHandler(IUnitOfWork uow, IMediatorHandler bus, IMapper mapper, IMemoryCache cache,
            ISysUserRepository sysUserRepository,
            ISysUserAuthRepository sysUserAuthRepository,
            ISysUserRoleRelaRepository sysUserRoleRelaRepository)
            : base(uow, bus, cache)
        {
            _mapper = mapper;
            Cache = cache;
            _sysUserRepository = sysUserRepository;
            _sysUserAuthRepository = sysUserAuthRepository;
            _sysUserRoleRelaRepository = sysUserRoleRelaRepository;
        }

        public Task<Unit> Handle(CreateSysUserCommand request, CancellationToken cancellationToken)
        {
            // 命令验证
            if (!request.IsValid())
            {
                // 错误信息收集
                NotifyValidationErrors(request);
                // 返回，结束当前线程
                return Task.FromResult(new Unit());
            }

            var sysUser = _mapper.Map<SysUser>(request);

            #region 检查
            // 登录用户名不可重复
            // 这些业务逻辑，当然要在领域层中（领域命令处理程序中）进行处理
            if (_sysUserRepository.IsExistLoginUsername(sysUser.LoginUsername, sysUser.SysType))
            {
                // 引发错误事件
                Bus.RaiseEvent(new DomainNotification("", "该用户名已经被使用！"));
                return Task.FromResult(new Unit());
            }
            // 手机号不可重复
            if (_sysUserRepository.IsExistTelphone(sysUser.Telphone, sysUser.SysType))
            {
                Bus.RaiseEvent(new DomainNotification("", "手机号已存在！"));
                return Task.FromResult(new Unit());
            }
            // 员工号不可重复
            if (_sysUserRepository.IsExistUserNo(sysUser.UserNo, sysUser.SysType))
            {
                Bus.RaiseEvent(new DomainNotification("", "员工号已存在！"));
                return Task.FromResult(new Unit());
            }
            #endregion

            //默认头像
            switch (sysUser.Sex)
            {
                case CS.SEX_MALE:
                    sysUser.AvatarUrl = "https://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/defava_m.png";
                    break;
                case CS.SEX_FEMALE:
                    sysUser.AvatarUrl = "https://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/defava_f.png";
                    break;
            }

            sysUser.InviteCode = StringUtil.GetUUID(6); //6位随机数

            _sysUserRepository.Add(sysUser);

            #region 添加默认用户认证表
            string salt = StringUtil.GetUUID(6); //6位随机数
            string authPwd = request.PasswordType.Equals("custom") ? request.LoginPassword : CS.DEFAULT_PWD;
            string userPwd = BCrypt.Net.BCrypt.HashPassword(authPwd);
            //用户名登录方式
            var sysUserAuthByLoginUsername = new SysUserAuth()
            {
                UserId = sysUser.SysUserId,
                IdentityType = CS.AUTH_TYPE.LOGIN_USER_NAME,
                Identifier = sysUser.LoginUsername,
                Credential = userPwd,
                Salt = salt,
                SysType = sysUser.SysType
            };
            _sysUserAuthRepository.Add(sysUserAuthByLoginUsername);

            //手机号登录方式
            var sysUserAuthByTelphone = new SysUserAuth()
            {
                UserId = sysUser.SysUserId,
                IdentityType = CS.AUTH_TYPE.TELPHONE,
                Identifier = sysUser.Telphone,
                Credential = userPwd,
                Salt = salt,
                SysType = sysUser.SysType
            };
            _sysUserAuthRepository.Add(sysUserAuthByTelphone);
            #endregion

            if (Commit())
            {
                // 提交成功后，这里需要发布领域事件
                // 比如欢迎用户注册邮件呀，短信呀等
                var createdEvent = _mapper.Map<SysUserCreatedEvent>(sysUser);
                createdEvent.LoginPassword = authPwd;
                createdEvent.IsNotify = request.IsNotify;
                Bus.RaiseEvent(createdEvent);
            }

            return Task.FromResult(new Unit());
        }

        public Task<Unit> Handle(RemoveSysUserCommand request, CancellationToken cancellationToken)
        {
            // 命令验证
            if (!request.IsValid())
            {
                // 错误信息收集
                NotifyValidationErrors(request);
                // 返回，结束当前线程
                return Task.FromResult(new Unit());
            }
            //查询该操作员信息
            var sysUser = _sysUserRepository.GetByUserId(request.SysUserId, request.SysType);
            if (sysUser is null)
            {
                // 引发错误事件
                Bus.RaiseEvent(new DomainNotification("", "该操作员不存在！"));
                return Task.FromResult(new Unit());
            }
            //判断是否自己删除自己
            if (sysUser.SysUserId.Equals(request.CurrentSysUserId))
            {
                // 引发错误事件
                Bus.RaiseEvent(new DomainNotification("", "系统不允许删除当前登陆用户！"));
                return Task.FromResult(new Unit());
            }

            // 删除用户登录信息
            _sysUserAuthRepository.RemoveByUserId(sysUser.SysUserId, sysUser.SysType);

            // 删除用户角色信息
            _sysUserRoleRelaRepository.RemoveByUserId(sysUser.SysUserId);

            // 删除用户
            _sysUserRepository.Remove(sysUser);

            Commit();

            return Task.FromResult(new Unit());
        }

        public Task<Unit> Handle(ModifySysUserCommand request, CancellationToken cancellationToken)
        {
            // 命令验证
            if (!request.IsValid())
            {
                // 错误信息收集
                NotifyValidationErrors(request);
                // 返回，结束当前线程
                return Task.FromResult(new Unit());
            }

            //查询该操作员信息
            var sysUser = _sysUserRepository.GetByUserId(request.SysUserId, request.SysType);
            if (sysUser is null)
            {
                // 引发错误事件
                Bus.RaiseEvent(new DomainNotification("", "该用户不存在！"));
                return Task.FromResult(new Unit());
            }

            //判断是否自己禁用自己
            if (request.SysUserId.Equals(request.CurrentSysUserId) && request.State == CS.PUB_DISABLE)
            {
                // 引发错误事件
                Bus.RaiseEvent(new DomainNotification("", "系统不允许禁用当前登陆用户！"));
                return Task.FromResult(new Unit());
            }

            //判断是否重置密码
            if (request.ResetPass)
            {
                string updatePwd = request.DefaultPass ? CS.DEFAULT_PWD : Base64Util.DecodeBase64(request.ConfirmPwd);
                _sysUserAuthRepository.ResetAuthInfo(request.SysUserId, request.SysType, null, null, updatePwd);
            }

            //修改了手机号， 需要修改auth表信息
            if (!sysUser.Telphone.Equals(request.Telphone))
            {
                if (_sysUserRepository.IsExistTelphone(request.Telphone, request.SysType))
                {
                    Bus.RaiseEvent(new DomainNotification("", "该手机号已关联其他用户！"));
                    return Task.FromResult(new Unit());
                }
                _sysUserAuthRepository.ResetAuthInfo(request.SysUserId, request.SysType, null, request.Telphone, null);
            }

            //修改了用户名， 需要修改auth表信息
            if (!sysUser.LoginUsername.Equals(request.LoginUsername))
            {
                if (_sysUserRepository.IsExistLoginUsername(request.LoginUsername, request.SysType))
                {
                    Bus.RaiseEvent(new DomainNotification("", "该登录用户名已关联其他用户！"));
                    return Task.FromResult(new Unit());
                }
                _sysUserAuthRepository.ResetAuthInfo(request.SysUserId, request.SysType, request.LoginUsername, null, null);
            }

            _mapper.Map(request, sysUser);
            sysUser.UpdatedAt = DateTime.Now;
            _sysUserRepository.Update(sysUser);

            Commit();

            return Task.FromResult(new Unit());
        }
    }
}
