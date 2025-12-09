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
        IRequestHandler<CreateSysUserCommand>,
        IRequestHandler<RemoveSysUserCommand>,
        IRequestHandler<ModifySysUserCommand>
    {
        // 注入仓储接口
        private readonly ISysUserRepository _sysUserRepository;
        private readonly ISysUserAuthRepository _sysUserAuthRepository;
        private readonly ISysUserRoleRelaRepository _sysUserRoleRelaRepository;

        // 用来进行DTO
        private readonly IMapper _mapper;

        // 注入总线
        private readonly IMediatorHandler Bus;
        private readonly IMemoryCache Cache;

        public SysUserCommandHandler(IUnitOfWork uow, IMediatorHandler bus, IMapper mapper, IMemoryCache cache,
            ISysUserRepository sysUserRepository,
            ISysUserAuthRepository sysUserAuthRepository,
            ISysUserRoleRelaRepository sysUserRoleRelaRepository)
            : base(uow, bus, cache)
        {
            _mapper = mapper;
            Cache = cache;
            Bus = bus;
            _sysUserRepository = sysUserRepository;
            _sysUserAuthRepository = sysUserAuthRepository;
            _sysUserRoleRelaRepository = sysUserRoleRelaRepository;
        }

        public async Task Handle(CreateSysUserCommand request, CancellationToken cancellationToken)
        {
            // 命令验证
            if (!request.IsValid())
            {
                // 错误信息收集
                NotifyValidationErrors(request);
                // 返回，结束当前线程
                return;
            }

            var sysUser = _mapper.Map<SysUser>(request);

            #region 检查
            // 登录用户名不可重复
            // 这些业务逻辑，当然要在领域层中（领域命令处理程序中）进行处理
            if (await _sysUserRepository.IsExistLoginUsernameAsync(sysUser.LoginUsername, sysUser.SysType))
            {
                // 引发错误事件
                await Bus.RaiseEvent(new DomainNotification("", "该用户名已经被使用！"));
                return;
            }
            // 手机号不可重复
            if (await _sysUserRepository.IsExistTelphoneAsync(sysUser.Telphone, sysUser.SysType))
            {
                await Bus.RaiseEvent(new DomainNotification("", "手机号已存在！"));
                return;
            }
            // 员工号不可重复
            if (await _sysUserRepository.IsExistUserNoAsync(sysUser.UserNo, sysUser.SysType))
            {
                await Bus.RaiseEvent(new DomainNotification("", "员工号已存在！"));
                return;
            }
            #endregion

            //默认头像
            switch (sysUser.Sex)
            {
                case CS.SEX_MALE:
                    sysUser.AvatarUrl = CS.DEFAULT_MALE_AVATAR_URL;
                    break;
                case CS.SEX_FEMALE:
                    sysUser.AvatarUrl = CS.DEFAULT_FEMALE_AVATAR_URL;
                    break;
            }

            if (sysUser.UserType.Equals(CS.USER_TYPE.Expand) && string.IsNullOrWhiteSpace(sysUser.InviteCode))
            {
                do
                {
                    sysUser.InviteCode = StringUtil.GetUUID(6); //6位随机数
                } while (await _sysUserRepository.IsExistInviteCodeAsync(sysUser.InviteCode));
            }

            await BeginTransactionAsync();
            try
            {
                await _sysUserRepository.AddAsync(sysUser);
                await _sysUserRepository.SaveChangesAsync(); // 显式提交用户数据

                #region 添加默认用户认证表
                //string salt = StringUtil.GetUUID(6); //6位随机数
                string authPwd = request.PasswordType.Equals(CS.PASSWORD_TYPE.CUSTOM) ? request.LoginPassword : CS.DEFAULT_PWD;
                string userPwd = BCryptUtil.Hash(authPwd, out string salt);
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
                await _sysUserAuthRepository.AddAsync(sysUserAuthByLoginUsername);

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
                await _sysUserAuthRepository.AddAsync(sysUserAuthByTelphone);
                #endregion

                if (!await CommitAsync())
                {
                    await Bus.RaiseEvent(new DomainNotification("", "添加用户失败"));
                    await RollbackTransactionAsync();
                    return;
                }

                await CommitTransactionAsync();

                if (request.IsNotify == CS.YES)
                {
                    // 提交成功后，这里需要发布领域事件
                    // 比如欢迎用户注册邮件呀，短信呀等
                    var createdEvent = _mapper.Map<SysUserCreatedEvent>(sysUser);
                    createdEvent.LoginPassword = authPwd;
                    createdEvent.IsNotify = request.IsNotify;
                    await Bus.RaiseEvent(createdEvent);
                }
            }
            catch (Exception e)
            {
                await RollbackTransactionAsync();
                await Bus.RaiseEvent(new DomainNotification("", e.Message));
                return;
            }
        }

        public async Task Handle(RemoveSysUserCommand request, CancellationToken cancellationToken)
        {
            // 命令验证
            if (!request.IsValid())
            {
                // 错误信息收集
                NotifyValidationErrors(request);
                // 返回，结束当前线程
                return;
            }
            //查询该操作员信息
            SysUser sysUser;
            if (string.IsNullOrWhiteSpace(request.SysType))
            {
                sysUser = await _sysUserRepository.GetByUserIdAsync(request.SysUserId);
            }
            else
            {
                sysUser = await _sysUserRepository.GetByUserIdAsync(request.SysUserId, request.SysType);
            }
            if (sysUser is null)
            {
                // 引发错误事件
                await Bus.RaiseEvent(new DomainNotification("", "该操作员不存在！"));
                return;
            }
            //判断是否自己删除自己
            if (sysUser.SysUserId.Equals(request.CurrentSysUserId))
            {
                // 引发错误事件
                await Bus.RaiseEvent(new DomainNotification("", "系统不允许删除当前登陆用户！"));
                return;
            }
            //判断是否删除商户默认超管（初始用户）
            if (sysUser != null && sysUser.SysType == CS.SYS_TYPE.MCH && sysUser.InitUser)
            {
                await Bus.RaiseEvent(new DomainNotification("", "系统不允许删除商户默认用户！"));
                return;
            }
            //判断是否删除代理商默认超管（初始用户）
            if (sysUser != null && sysUser.SysType == CS.SYS_TYPE.AGENT && sysUser.InitUser)
            {
                await Bus.RaiseEvent(new DomainNotification("", "系统不允许删除代理商默认用户！"));
                return;
            }

            await BeginTransactionAsync();
            try
            {
                // 删除用户登录信息
                _sysUserAuthRepository.RemoveByUserId(sysUser.SysUserId, sysUser.SysType);

                // 删除用户角色信息
                _sysUserRoleRelaRepository.RemoveByUserId(sysUser.SysUserId);

                // 删除用户
                _sysUserRepository.Remove(sysUser);

                if (!await CommitAsync())
                {
                    await Bus.RaiseEvent(new DomainNotification("", "删除用户失败"));
                    await RollbackTransactionAsync();
                    return;
                }
                await CommitTransactionAsync();
            }
            catch (Exception e)
            {
                await RollbackTransactionAsync();
                await Bus.RaiseEvent(new DomainNotification("", e.Message));
                return;
            }
        }

        public async Task Handle(ModifySysUserCommand request, CancellationToken cancellationToken)
        {
            // 命令验证
            if (!request.IsValid())
            {
                // 错误信息收集
                NotifyValidationErrors(request);
                // 返回，结束当前线程
                return;
            }

            //查询该操作员信息
            var sysUser = await _sysUserRepository.GetByUserIdAsync(request.SysUserId, request.SysType);
            if (sysUser is null)
            {
                // 引发错误事件
                await Bus.RaiseEvent(new DomainNotification("", "该用户不存在！"));
                return;
            }

            //判断是否自己禁用自己
            if (request.SysUserId.Equals(request.CurrentSysUserId) && request.State == CS.PUB_DISABLE)
            {
                // 引发错误事件
                await Bus.RaiseEvent(new DomainNotification("", "系统不允许禁用当前登陆用户！"));
                return;
            }

            await BeginTransactionAsync();
            try
            {
                //判断是否重置密码
                if (request.ResetPass)
                {
                    string updatePwd = request.DefaultPass ? CS.DEFAULT_PWD : Base64Util.DecodeBase64(request.ConfirmPwd);
                    await _sysUserAuthRepository.ResetAuthInfoAsync(request.SysUserId, request.SysType, null, null, updatePwd);
                }

                //修改了手机号， 需要修改auth表信息
                if (!sysUser.Telphone.Equals(request.Telphone))
                {
                    if (await _sysUserRepository.IsExistTelphoneAsync(request.Telphone, request.SysType))
                    {
                        await Bus.RaiseEvent(new DomainNotification("", "该手机号已关联其他用户！"));
                        await RollbackTransactionAsync();
                        return;
                    }
                    await _sysUserAuthRepository.ResetAuthInfoAsync(request.SysUserId, request.SysType, null, request.Telphone, null);
                }

                //修改了用户名， 需要修改auth表信息
                if (!sysUser.LoginUsername.Equals(request.LoginUsername))
                {
                    if (await _sysUserRepository.IsExistLoginUsernameAsync(request.LoginUsername, request.SysType))
                    {
                        await Bus.RaiseEvent(new DomainNotification("", "该登录用户名已关联其他用户！"));
                        await RollbackTransactionAsync();
                        return;
                    }
                    await _sysUserAuthRepository.ResetAuthInfoAsync(request.SysUserId, request.SysType, request.LoginUsername, null, null);
                }

                //修改了编号
                if (!sysUser.UserNo.Equals(request.UserNo))
                {
                    if (await _sysUserRepository.IsExistUserNoAsync(request.UserNo, request.SysType))
                    {
                        await Bus.RaiseEvent(new DomainNotification("", "该员工编号已关联其他用户！"));
                        await RollbackTransactionAsync();
                        return;
                    }
                }

                _mapper.Map(request, sysUser);
                sysUser.UpdatedAt = DateTime.Now;

                if (sysUser.UserType.Equals(CS.USER_TYPE.Expand) && string.IsNullOrWhiteSpace(sysUser.InviteCode))
                {
                    do
                    {
                        sysUser.InviteCode = StringUtil.GetUUID(6); //6位随机数
                    } while (await _sysUserRepository.IsExistInviteCodeAsync(sysUser.InviteCode));
                }

                _sysUserRepository.Update(sysUser);

                if (!await CommitAsync())
                {
                    await Bus.RaiseEvent(new DomainNotification("", "修改当前用户失败"));
                    await RollbackTransactionAsync();
                    return;
                }

                await CommitTransactionAsync();
            }
            catch (Exception e)
            {
                await RollbackTransactionAsync();
                await Bus.RaiseEvent(new DomainNotification("", e.Message));
                return;
            }
        }
    }
}
