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
using Microsoft.Extensions.Caching.Memory;

namespace AGooday.AgPay.Domain.CommandHandlers
{
    public class AgentInfoCommandHandler : CommandHandler,
        IRequestHandler<CreateAgentInfoCommand, Unit>,
        IRequestHandler<ModifyAgentInfoCommand, Unit>,
        IRequestHandler<RemoveAgentInfoCommand, Unit>
    {
        // 注入仓储接口
        private readonly IAgentInfoRepository _agentInfoRepository;
        private readonly IIsvInfoRepository _isvInfoRepository;
        private readonly ISysUserRepository _sysUserRepository;
        private readonly ISysUserAuthRepository _sysUserAuthRepository;

        // 用来进行DTO
        private readonly IMapper _mapper;

        // 注入总线
        private readonly IMediatorHandler Bus;
        private readonly IMQSender mqSender;
        private IMemoryCache Cache;

        public AgentInfoCommandHandler(IUnitOfWork uow, IMediatorHandler bus, IMapper mapper, IMQSender mqSender, IMemoryCache cache,
            IAgentInfoRepository agentInfoRepository,
            IIsvInfoRepository isvInfoRepository,
            ISysUserRepository sysUserRepository,
            ISysUserAuthRepository sysUserAuthRepository)
            : base(uow, bus, cache)
        {
            _mapper = mapper;
            Cache = cache;
            this.mqSender = mqSender;
            _sysUserRepository = sysUserRepository;
            _agentInfoRepository = agentInfoRepository;
            _isvInfoRepository = isvInfoRepository;
            _sysUserAuthRepository = sysUserAuthRepository;
        }

        public Task<Unit> Handle(CreateAgentInfoCommand request, CancellationToken cancellationToken)
        {
            // 命令验证
            if (!request.IsValid())
            {
                // 错误信息收集
                NotifyValidationErrors(request);
                // 返回，结束当前线程
                return Task.FromResult(new Unit());
            }

            var agentInfo = _mapper.Map<AgentInfo>(request);
            do
            {
                agentInfo.AgentNo = SeqUtil.GenAgentNo();
            } while (_agentInfoRepository.IsExistAgentNo(agentInfo.AgentNo));

            #region 检查
            // 校验特邀代理商信息
            if (agentInfo.Type.Equals(CS.AGENT_TYPE_ISVSUB) && !string.IsNullOrWhiteSpace(agentInfo.IsvNo))
            {
                // 当前服务商状态是否正确
                var isvInfo = _isvInfoRepository.GetById(agentInfo.IsvNo);
                if (isvInfo == null || isvInfo.State == CS.NO)
                {
                    Bus.RaiseEvent(new DomainNotification("", "当前服务商不可用！"));
                    return Task.FromResult(new Unit());
                }
            }
            #endregion

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
            sysUser.AvatarUrl = "https://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/defava_m.png";//默认头像
            sysUser.IsAdmin = CS.YES;
            sysUser.State = agentInfo.State;

            #region 检查
            // 登录用户名不可重复
            // 这些业务逻辑，当然要在领域层中（领域命令处理程序中）进行处理
            if (_sysUserRepository.IsExistLoginUsername(sysUser.LoginUsername, sysUser.SysType))
            {
                // 引发错误事件
                Bus.RaiseEvent(new DomainNotification("", "登录名已经被使用！"));
                return Task.FromResult(new Unit());
            }
            // 手机号不可重复
            if (_sysUserRepository.IsExistTelphone(sysUser.Telphone, sysUser.SysType))
            {
                Bus.RaiseEvent(new DomainNotification("", "联系人手机号已存在！"));
                return Task.FromResult(new Unit());
            }
            // 员工号不可重复
            if (_sysUserRepository.IsExistUserNo(sysUser.UserNo, sysUser.SysType))
            {
                Bus.RaiseEvent(new DomainNotification("", "员工号已存在！"));
                return Task.FromResult(new Unit());
            }
            #endregion

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
            #endregion

            // 插入代理商基本信息
            // 存入代理商默认用户ID
            agentInfo.InitUserId = sysUser.SysUserId;
            _agentInfoRepository.Add(agentInfo);

            if (Commit())
            {
                // 提交成功后，这里需要发布领域事件
                // 比如欢迎用户注册邮件呀，短信呀等
                var createdEvent = _mapper.Map<AgentInfoCreatedEvent>(agentInfo);
                createdEvent.LoginUsername = request.LoginUsername;
                createdEvent.LoginPassword = authPwd;
                createdEvent.IsNotify = request.IsNotify;
                Bus.RaiseEvent(createdEvent);
            }

            return Task.FromResult(new Unit());
        }

        public Task<Unit> Handle(ModifyAgentInfoCommand request, CancellationToken cancellationToken)
        {
            // 命令验证
            if (!request.IsValid())
            {
                // 错误信息收集
                NotifyValidationErrors(request);
                // 返回，结束当前线程
                return Task.FromResult(new Unit());
            }

            request.UpdatedAt = DateTime.Now;
            var agentInfo = _mapper.Map<AgentInfo>(request);

            // 待删除用户登录信息的ID list
            var removeCacheUserIdList = new List<long>();

            // 如果代理商状态为禁用状态，清除该代理商用户登录信息
            if (agentInfo.State == CS.NO)
            {
                removeCacheUserIdList = _sysUserRepository.GetAll().Where(w => w.SysType.Equals(CS.SYS_TYPE.AGENT) && w.BelongInfoId.Equals(agentInfo.AgentNo))
                    .Select(w => w.SysUserId).ToList();
            }

            // 判断是否重置密码
            if (request.ResetPass)
            {
                // 待更新的密码
                string updatePwd = request.DefaultPass ? CS.DEFAULT_PWD : Base64Util.DecodeBase64(request.ConfirmPwd);
                // 获取代理商超管
                long mchAdminUserId = _sysUserRepository.FindAgentAdminUserId(agentInfo.AgentNo);

                // 重置超管密码
                _sysUserAuthRepository.ResetAuthInfo(mchAdminUserId, null, null, updatePwd, CS.SYS_TYPE.AGENT);
                _sysUserAuthRepository.SaveChanges();

                // 删除超管登录信息
                removeCacheUserIdList.Add(mchAdminUserId);
            }

            // 推送mq删除redis用户认证信息
            if (removeCacheUserIdList.Any())
            {
                mqSender.Send(CleanAgentLoginAuthCacheMQ.Build(removeCacheUserIdList));
            }

            // 更新代理商信息
            _agentInfoRepository.Update(agentInfo);
            _agentInfoRepository.SaveChanges();

            Commit();

            return Task.FromResult(new Unit());
        }

        public Task<Unit> Handle(RemoveAgentInfoCommand request, CancellationToken cancellationToken)
        {
            // 命令验证
            if (!request.IsValid())
            {
                // 错误信息收集
                NotifyValidationErrors(request);
                // 返回，结束当前线程
                return Task.FromResult(new Unit());
            }

            // 0.当前代理商是否存在
            var agentInfo = _agentInfoRepository.GetById(request.AgentNo);
            if (agentInfo is null)
            {
                // 引发错误事件
                Bus.RaiseEvent(new DomainNotification("", "该代理商不存在！"));
                return Task.FromResult(new Unit());
            }

            var sysUsers = _sysUserRepository.GetAll().Where(w => w.BelongInfoId.Equals(request.AgentNo) && w.SysType.Equals(CS.SYS_TYPE.AGENT));
            foreach (var sysUser in sysUsers)
            {
                var sysUserAuths = _sysUserAuthRepository.GetAll().Where(w => w.UserId.Equals(sysUser.SysUserId));
                // 删除当前代理商用户认证信息
                foreach (var sysUserAuth in sysUserAuths)
                {
                    _sysUserAuthRepository.Remove(sysUserAuth.AuthId);
                }
                // 删除当前代理商的登录用户
                _sysUserRepository.Remove(sysUser.SysUserId);
            }

            // 1.删除当前代理商应用信息
            _agentInfoRepository.Remove(agentInfo.AgentNo);

            // 推送mq删除redis用户缓存
            var userIdList = sysUsers.Select(s => s.SysUserId).ToList();
            mqSender.Send(CleanAgentLoginAuthCacheMQ.Build(userIdList));

            Commit();

            return Task.FromResult(new Unit());
        }
    }
}
