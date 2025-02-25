using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Domain.Commands.AgentInfos;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Core.Notifications;
using AGooday.AgPay.Domain.Events.AgentInfos;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AGooday.AgPay.Domain.CommandHandlers
{
    public class AgentInfoCommandHandler : CommandHandler,
        IRequestHandler<CreateAgentInfoCommand>,
        IRequestHandler<ModifyAgentInfoCommand>,
        IRequestHandler<RemoveAgentInfoCommand>
    {
        // 注入仓储接口
        private readonly IAgentInfoRepository _agentInfoRepository;
        private readonly IIsvInfoRepository _isvInfoRepository;
        private readonly IMchInfoRepository _mchInfoRepository;
        private readonly ISysUserRepository _sysUserRepository;
        private readonly ISysUserAuthRepository _sysUserAuthRepository;

        // 用来进行DTO
        private readonly IMapper _mapper;

        // 注入总线
        private readonly IMQSender _mqSender;
        private readonly IMediatorHandler Bus;
        private readonly IMemoryCache Cache;

        public AgentInfoCommandHandler(IUnitOfWork uow, IMediatorHandler bus, IMapper mapper, IMQSender mqSender, IMemoryCache cache,
            IAgentInfoRepository agentInfoRepository,
            IIsvInfoRepository isvInfoRepository,
            IMchInfoRepository mchInfoRepository,
            ISysUserRepository sysUserRepository,
            ISysUserAuthRepository sysUserAuthRepository)
            : base(uow, bus, cache)
        {
            _mapper = mapper;
            Cache = cache;
            Bus = bus;
            _mqSender = mqSender;
            _sysUserRepository = sysUserRepository;
            _agentInfoRepository = agentInfoRepository;
            _isvInfoRepository = isvInfoRepository;
            _sysUserAuthRepository = sysUserAuthRepository;
            _mchInfoRepository = mchInfoRepository;
        }

        public async Task Handle(CreateAgentInfoCommand request, CancellationToken cancellationToken)
        {
            // 命令验证
            if (!request.IsValid())
            {
                // 错误信息收集
                NotifyValidationErrors(request);
                // 返回，结束当前线程
                return;
            }

            var agentInfo = _mapper.Map<AgentInfo>(request);
            do
            {
                agentInfo.AgentNo = SeqUtil.GenAgentNo();
            } while (await _agentInfoRepository.IsExistAgentNoAsync(agentInfo.AgentNo));

            #region 检查
            // 校验上级代理商信息
            if (!string.IsNullOrWhiteSpace(agentInfo.Pid))
            {
                // 当前服务商状态是否正确
                var pagentInfo = await _agentInfoRepository.GetByIdAsync(agentInfo.Pid);
                if (pagentInfo == null || pagentInfo.State == CS.NO)
                {
                    await Bus.RaiseEvent(new DomainNotification("", "上级代理商不可用！"));
                    return;
                }
                if (!agentInfo.IsvNo.Equals(pagentInfo.IsvNo))
                {
                    await Bus.RaiseEvent(new DomainNotification("", "上级代理商/服务商信息有误！"));
                    return;
                }
                agentInfo.Level = (byte)(pagentInfo.Level + 1);
            }
            else
            {
                agentInfo.Level = 1;
            }
            // 校验服务商信息
            if (!string.IsNullOrWhiteSpace(agentInfo.IsvNo))
            {
                // 当前服务商状态是否正确
                var isvInfo = await _isvInfoRepository.GetByIdAsync(agentInfo.IsvNo);
                if (isvInfo == null || isvInfo.State == CS.NO)
                {
                    await Bus.RaiseEvent(new DomainNotification("", "服务商号不可用！"));
                    return;
                }
            }
            #endregion

            try
            {
                BeginTransaction();
                #region 插入用户信息
                // 插入用户信息
                SysUser sysUser = new SysUser();
                sysUser.LoginUsername = request.LoginUsername;
                sysUser.Realname = agentInfo.ContactName;
                sysUser.Telphone = agentInfo.ContactTel;
                sysUser.UserNo = agentInfo.AgentNo;
                sysUser.BelongInfoId = agentInfo.AgentNo;
                sysUser.SysType = CS.SYS_TYPE.AGENT;
                sysUser.Sex = CS.SEX_MALE;
                sysUser.AvatarUrl = CS.DEFAULT_MALE_AVATAR_URL;//默认头像
                sysUser.InitUser = true;
                sysUser.UserType = CS.USER_TYPE.ADMIN;
                sysUser.State = agentInfo.State;

                #region 检查
                // 登录用户名不可重复
                // 这些业务逻辑，当然要在领域层中（领域命令处理程序中）进行处理
                if (await _sysUserRepository.IsExistLoginUsernameAsync(sysUser.LoginUsername, sysUser.SysType))
                {
                    // 引发错误事件
                    await Bus.RaiseEvent(new DomainNotification("", "登录名已经被使用！"));
                    return;
                }
                // 手机号不可重复
                if (await _sysUserRepository.IsExistTelphoneAsync(sysUser.Telphone, sysUser.SysType))
                {
                    await Bus.RaiseEvent(new DomainNotification("", "联系人手机号已存在！"));
                    return;
                }
                // 员工号不可重复
                if (await _sysUserRepository.IsExistUserNoAsync(sysUser.UserNo, sysUser.SysType))
                {
                    await Bus.RaiseEvent(new DomainNotification("", "员工号已存在！"));
                    return;
                }
                #endregion

                await _sysUserRepository.AddAsync(sysUser);
                await _sysUserRepository.SaveChangesAsync();

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
                #endregion

                // 插入代理商基本信息
                // 存入代理商默认用户ID
                // agentInfo.Sipw = BCryptUtil.Hash(CS.DEFAULT_SIPW, out salt);
                agentInfo.InitUserId = sysUser.SysUserId;
                await _agentInfoRepository.AddAsync(agentInfo);

                if (!await CommitAsync())
                {
                    await Bus.RaiseEvent(new DomainNotification("", "添加代理商失败"));
                    RollbackTransaction();
                    return;
                }

                CommitTransaction();

                if (request.IsNotify == CS.YES)
                {
                    // 提交成功后，这里需要发布领域事件
                    // 比如欢迎用户注册邮件呀，短信呀等
                    var createdEvent = _mapper.Map<AgentInfoCreatedEvent>(agentInfo);
                    createdEvent.LoginUsername = request.LoginUsername;
                    createdEvent.LoginPassword = authPwd;
                    createdEvent.IsNotify = request.IsNotify;
                    await Bus.RaiseEvent(createdEvent);
                }
            }
            catch (Exception e)
            {
                RollbackTransaction();
                await Bus.RaiseEvent(new DomainNotification("", e.Message));
                return;
            }
        }

        public async Task Handle(ModifyAgentInfoCommand request, CancellationToken cancellationToken)
        {
            // 命令验证
            if (!request.IsValid())
            {
                // 错误信息收集
                NotifyValidationErrors(request);
                // 返回，结束当前线程
                return;
            }

            request.UpdatedAt = DateTime.Now;
            var agentInfo = _mapper.Map<AgentInfo>(request);

            // 待删除用户登录信息的ID list
            var removeCacheUserIdList = new List<long>();

            // 如果代理商状态为禁用状态，清除该代理商用户登录信息
            if (agentInfo.State == CS.NO)
            {
                removeCacheUserIdList = _sysUserRepository.GetAllAsNoTracking()
                    .Where(w => w.SysType.Equals(CS.SYS_TYPE.AGENT) && w.BelongInfoId.Equals(agentInfo.AgentNo))
                    .Select(w => w.SysUserId).ToList();
            }

            try
            {
                BeginTransaction();
                //修改了手机号， 需要修改auth表信息
                // 获取代理商超管
                long agentAdminUserId = await _sysUserRepository.FindAgentAdminUserIdAsync(agentInfo.AgentNo);
                var sysUserAuth = await _sysUserAuthRepository.GetAllAsNoTracking()
                    .Where(w => w.UserId.Equals(agentAdminUserId) && w.SysType.Equals(CS.SYS_TYPE.AGENT) && w.IdentityType.Equals(CS.AUTH_TYPE.TELPHONE))
                    .FirstOrDefaultAsync(cancellationToken);

                if (sysUserAuth != null && !sysUserAuth.Identifier.Equals(request.ContactTel))
                {
                    if (await _sysUserRepository.IsExistTelphoneAsync(request.ContactTel, request.ContactTel))
                    {
                        await Bus.RaiseEvent(new DomainNotification("", "该手机号已关联其他用户！"));
                        return;
                    }
                    sysUserAuth.Identifier = request.ContactTel;
                    _sysUserAuthRepository.Update(sysUserAuth);
                }

                // 判断是否重置密码
                if (request.ResetPass)
                {
                    // 待更新的密码
                    string updatePwd = request.DefaultPass ? CS.DEFAULT_PWD : Base64Util.DecodeBase64(request.ConfirmPwd);

                    // 重置超管密码
                    await _sysUserAuthRepository.ResetAuthInfoAsync(agentAdminUserId, CS.SYS_TYPE.AGENT, null, null, updatePwd);
                    await _sysUserAuthRepository.SaveChangesAsync();

                    // 删除超管登录信息
                    removeCacheUserIdList.Add(agentAdminUserId);
                }

                // 推送mq删除redis用户认证信息
                if (removeCacheUserIdList.Count != 0)
                {
                    await _mqSender.SendAsync(CleanAgentLoginAuthCacheMQ.Build(removeCacheUserIdList));
                }

                // 更新代理商信息
                _agentInfoRepository.Update(agentInfo);
                //_agentInfoRepository.SaveChanges();

                if (!Commit())
                {
                    await Bus.RaiseEvent(new DomainNotification("", "修改当前代理商失败"));
                    RollbackTransaction();
                    return;
                }

                CommitTransaction();

                // 推送mq到目前节点进行更新数据
                await _mqSender.SendAsync(ResetIsvAgentMchAppInfoConfigMQ.Build(ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_AGENT_INFO, null, agentInfo.AgentNo, null, null));
            }
            catch (Exception e)
            {
                RollbackTransaction();
                await Bus.RaiseEvent(new DomainNotification("", e.Message));
                return;
            }
        }

        public async Task Handle(RemoveAgentInfoCommand request, CancellationToken cancellationToken)
        {
            // 命令验证
            if (!request.IsValid())
            {
                // 错误信息收集
                NotifyValidationErrors(request);
                // 返回，结束当前线程
                return;
            }

            // 0.当前代理商是否存在
            var agentInfo = await _agentInfoRepository.GetByIdAsync(request.AgentNo);
            if (agentInfo is null)
            {
                // 引发错误事件
                await Bus.RaiseEvent(new DomainNotification("", "该代理商不存在！"));
                return;
            }

            // 1.查询当前服务商下是否存在商户
            if (await _mchInfoRepository.IsExistMchByAgentNoAsync(request.AgentNo))
            {
                await Bus.RaiseEvent(new DomainNotification("", "该代理商下存在商户，不可删除"));
                return;
            }

            try
            {
                BeginTransaction();
                var sysUsers = _sysUserRepository.GetAllAsNoTracking()
                    .Where(w => w.BelongInfoId.Equals(request.AgentNo) && w.SysType.Equals(CS.SYS_TYPE.AGENT));
                var sysUserIds = sysUsers.Select(s => s.SysUserId);
                var sysUserAuths = _sysUserAuthRepository.GetAllAsNoTracking()
                    .Where(w => sysUserIds.Contains(w.UserId));
                // 删除当前代理商用户认证信息
                _sysUserAuthRepository.RemoveRange(sysUserAuths);
                // 删除当前代理商的登录用户
                _sysUserRepository.RemoveRange(sysUsers);

                // 2.删除当前代理商信息
                _agentInfoRepository.Remove(agentInfo);

                if (!Commit())
                {
                    await Bus.RaiseEvent(new DomainNotification("", "删除当前代理商失败"));
                    RollbackTransaction();
                    return;
                }

                CommitTransaction();

                // 推送mq删除redis用户缓存
                var userIdList = sysUsers.Select(s => s.SysUserId).ToList();
                await _mqSender.SendAsync(CleanAgentLoginAuthCacheMQ.Build(userIdList));

                // 推送mq到目前节点进行更新数据
                await _mqSender.SendAsync(ResetIsvAgentMchAppInfoConfigMQ.Build(ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_AGENT_INFO, null, agentInfo.AgentNo, null, null));
            }
            catch (Exception e)
            {
                RollbackTransaction();
                await Bus.RaiseEvent(new DomainNotification("", e.Message));
                return;
            }
        }
    }
}
