﻿using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Domain.Commands.MchInfos;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Core.Notifications;
using AGooday.AgPay.Domain.Events.MchInfos;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace AGooday.AgPay.Domain.CommandHandlers
{
    public class MchInfoCommandHandler : CommandHandler,
        IRequestHandler<CreateMchInfoCommand>,
        IRequestHandler<ModifyMchInfoCommand>,
        IRequestHandler<RemoveMchInfoCommand>
    {
        // 注入仓储接口
        private readonly IMchInfoRepository _mchInfoRepository;
        private readonly IIsvInfoRepository _isvInfoRepository;
        private readonly ISysUserRepository _sysUserRepository;
        private readonly ISysUserAuthRepository _sysUserAuthRepository;
        private readonly IMchAppRepository _mchAppRepository;
        private readonly IMchStoreRepository _mchStoreRepository;
        private readonly IPayOrderRepository _payOrderRepository;
        private readonly IMchPayPassageRepository _mchPayPassageRepository;
        private readonly IPayInterfaceConfigRepository _payInterfaceConfigRepository;

        // 用来进行DTO
        private readonly IMapper _mapper;

        // 注入总线
        private readonly IMediatorHandler Bus;
        private readonly IMQSender mqSender;
        private readonly IMemoryCache Cache;

        public MchInfoCommandHandler(IUnitOfWork uow, IMediatorHandler bus, IMapper mapper, IMQSender mqSender, IMemoryCache cache,
            IMchInfoRepository mchInfoRepository,
            IIsvInfoRepository isvInfoRepository,
            ISysUserRepository sysUserRepository,
            ISysUserAuthRepository sysUserAuthRepository,
            IMchAppRepository mchAppRepository,
            IMchStoreRepository mchStoreRepository,
            IPayOrderRepository payOrderRepository,
            IMchPayPassageRepository mchPayPassageRepository,
            IPayInterfaceConfigRepository payInterfaceConfigRepository)
            : base(uow, bus, cache)
        {
            _mapper = mapper;
            Cache = cache;
            Bus = bus;
            this.mqSender = mqSender;
            _sysUserRepository = sysUserRepository;
            _mchInfoRepository = mchInfoRepository;
            _isvInfoRepository = isvInfoRepository;
            _mchStoreRepository = mchStoreRepository;
            _mchAppRepository = mchAppRepository;
            _sysUserAuthRepository = sysUserAuthRepository;
            _payOrderRepository = payOrderRepository;
            _mchPayPassageRepository = mchPayPassageRepository;
            _payInterfaceConfigRepository = payInterfaceConfigRepository;
        }

        public Task Handle(CreateMchInfoCommand request, CancellationToken cancellationToken)
        {
            // 命令验证
            if (!request.IsValid())
            {
                // 错误信息收集
                NotifyValidationErrors(request);
                // 返回，结束当前线程
                return Task.CompletedTask;
            }

            var mchInfo = _mapper.Map<MchInfo>(request);
            do
            {
                mchInfo.MchNo = SeqUtil.GenMchNo();
            } while (_mchInfoRepository.IsExistMchNo(mchInfo.MchNo));

            #region 检查
            // 校验特邀商户信息
            if (mchInfo.Type.Equals(CS.MCH_TYPE_ISVSUB) && !string.IsNullOrWhiteSpace(mchInfo.IsvNo))
            {
                // 当前服务商状态是否正确
                var isvInfo = _isvInfoRepository.GetById(mchInfo.IsvNo);
                if (isvInfo == null || isvInfo.State == CS.NO)
                {
                    Bus.RaiseEvent(new DomainNotification("", "当前服务商不可用！"));
                    return Task.CompletedTask;
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
                sysUser.Realname = mchInfo.ContactName;
                sysUser.Telphone = mchInfo.ContactTel;
                sysUser.UserNo = mchInfo.MchNo;
                sysUser.BelongInfoId = mchInfo.MchNo;
                sysUser.SysType = CS.SYS_TYPE.MCH;
                sysUser.Sex = CS.SEX_MALE;
                sysUser.AvatarUrl = CS.DEFAULT_MALE_AVATAR_URL;//默认头像
                sysUser.InitUser = true;
                sysUser.UserType = CS.USER_TYPE.ADMIN;
                sysUser.State = mchInfo.State;

                #region 检查
                // 登录用户名不可重复
                // 这些业务逻辑，当然要在领域层中（领域命令处理程序中）进行处理
                if (_sysUserRepository.IsExistLoginUsername(sysUser.LoginUsername, sysUser.SysType))
                {
                    // 引发错误事件
                    Bus.RaiseEvent(new DomainNotification("", "登录名已经被使用！"));
                    return Task.CompletedTask;
                }
                // 手机号不可重复
                if (_sysUserRepository.IsExistTelphone(sysUser.Telphone, sysUser.SysType))
                {
                    Bus.RaiseEvent(new DomainNotification("", "联系人手机号已存在！"));
                    return Task.CompletedTask;
                }
                // 员工号不可重复
                if (_sysUserRepository.IsExistUserNo(sysUser.UserNo, sysUser.SysType))
                {
                    Bus.RaiseEvent(new DomainNotification("", "员工号已存在！"));
                    return Task.CompletedTask;
                }
                #endregion

                _sysUserRepository.Add(sysUser);
                _sysUserRepository.SaveChanges();

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

                #region 插入商户默认应用
                // 插入商户默认应用
                MchApp mchApp = new MchApp();
                mchApp.AppId = SeqUtil.GenAppId();
                mchApp.MchNo = mchInfo.MchNo;
                mchApp.AppName = "默认应用";
                mchApp.AppSignType = "[\"MD5\"]";
                mchApp.AppSecret = RandomUtil.RandomString(128);
                mchApp.State = CS.YES;
                mchApp.CreatedBy = sysUser.Realname;
                mchApp.CreatedUid = sysUser.SysUserId;

                _mchAppRepository.Add(mchApp);
                #endregion

                #region 插入商户默认店铺
                // 插入商户默认店铺
                MchStore mchStore = new MchStore();
                mchStore.StoreName = mchInfo.MchName;
                mchStore.MchNo = mchInfo.MchNo;
                mchStore.AgentNo = mchInfo.AgentNo;
                mchStore.IsvNo = mchInfo.IsvNo;
                mchStore.ContactPhone = mchInfo.ContactTel;
                mchStore.StoreLogo = string.Empty;
                mchStore.StoreOuterImg = string.Empty;
                mchStore.StoreInnerImg = string.Empty;
                mchStore.ProvinceCode = string.Empty;
                mchStore.CityCode = string.Empty;
                mchStore.DistrictCode = string.Empty;
                mchStore.Address = string.Empty;
                mchStore.Lng = string.Empty;
                mchStore.Lat = string.Empty;
                mchStore.DefaultFlag = CS.YES;
                mchStore.CreatedBy = sysUser.Realname;
                mchStore.CreatedUid = sysUser.SysUserId;

                _mchStoreRepository.Add(mchStore);
                #endregion

                // 插入商户基本信息
                // 存入商户默认用户ID
                // mchInfo.Sipw = BCryptUtil.Hash(CS.DEFAULT_SIPW, out salt);
                mchInfo.InitUserId = sysUser.SysUserId;
                _mchInfoRepository.Add(mchInfo);

                if (!Commit())
                {
                    Bus.RaiseEvent(new DomainNotification("", "添加商户失败"));
                    return Task.CompletedTask;
                }

                CommitTransaction();

                if (request.IsNotify == CS.YES)
                {
                    // 提交成功后，这里需要发布领域事件
                    // 比如欢迎用户注册邮件呀，短信呀等
                    var createdEvent = _mapper.Map<MchInfoCreatedEvent>(mchInfo);
                    createdEvent.LoginUsername = request.LoginUsername;
                    createdEvent.LoginPassword = authPwd;
                    createdEvent.IsNotify = request.IsNotify;
                    Bus.RaiseEvent(createdEvent);
                }
            }
            catch (Exception e)
            {
                RollbackTransaction();
                Bus.RaiseEvent(new DomainNotification("", e.Message));
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public Task Handle(ModifyMchInfoCommand request, CancellationToken cancellationToken)
        {
            // 命令验证
            if (!request.IsValid())
            {
                // 错误信息收集
                NotifyValidationErrors(request);
                // 返回，结束当前线程
                return Task.CompletedTask;
            }

            request.UpdatedAt = DateTime.Now;
            var mchInfo = _mapper.Map<MchInfo>(request);

            // 待删除用户登录信息的ID list
            var removeCacheUserIdList = new List<long>();

            // 如果商户状态为禁用状态，清除该商户用户登录信息
            if (mchInfo.State == CS.NO)
            {
                removeCacheUserIdList = _sysUserRepository.GetAllAsNoTracking()
                    .Where(w => w.SysType.Equals(CS.SYS_TYPE.MCH) && w.BelongInfoId.Equals(mchInfo.MchNo))
                    .Select(w => w.SysUserId).ToList();
            }

            try
            {
                BeginTransaction();
                //修改了手机号， 需要修改auth表信息
                // 获取商户超管
                long mchAdminUserId = _sysUserRepository.FindMchAdminUserId(mchInfo.MchNo);
                var sysUserAuth = _sysUserAuthRepository.GetAll()
                     .Where(w => w.UserId.Equals(mchAdminUserId) && w.SysType.Equals(CS.SYS_TYPE.MCH) && w.IdentityType.Equals(CS.AUTH_TYPE.TELPHONE))
                     .FirstOrDefault();

                if (sysUserAuth != null && !sysUserAuth.Identifier.Equals(request.ContactTel))
                {
                    if (_sysUserRepository.IsExistTelphone(request.ContactTel, request.ContactTel))
                    {
                        Bus.RaiseEvent(new DomainNotification("", "该手机号已关联其他用户！"));
                        return Task.CompletedTask;
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
                    _sysUserAuthRepository.ResetAuthInfo(mchAdminUserId, CS.SYS_TYPE.MCH, null, null, updatePwd);

                    // 删除超管登录信息
                    removeCacheUserIdList.Add(mchAdminUserId);
                }

                // 推送mq删除redis用户认证信息
                if (removeCacheUserIdList.Count != 0)
                {
                    mqSender.Send(CleanMchLoginAuthCacheMQ.Build(removeCacheUserIdList));
                }

                // 更新商户信息
                _mchInfoRepository.Update(mchInfo);

                if (!Commit())
                {
                    Bus.RaiseEvent(new DomainNotification("", "修改当前商户失败"));
                    return Task.CompletedTask;
                }

                CommitTransaction();

                // 推送mq到目前节点进行更新数据
                mqSender.Send(ResetIsvAgentMchAppInfoConfigMQ.Build(ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_MCH_INFO, null, null, mchInfo.MchNo, null));
            }
            catch (Exception e)
            {
                RollbackTransaction();
                Bus.RaiseEvent(new DomainNotification("", e.Message));
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public Task Handle(RemoveMchInfoCommand request, CancellationToken cancellationToken)
        {
            // 命令验证
            if (!request.IsValid())
            {
                // 错误信息收集
                NotifyValidationErrors(request);
                // 返回，结束当前线程
                return Task.CompletedTask;
            }

            // 0.当前商户是否存在
            var mchInfo = _mchInfoRepository.GetById(request.MchNo);
            if (mchInfo is null)
            {
                // 引发错误事件
                Bus.RaiseEvent(new DomainNotification("", "该商户不存在！"));
                return Task.CompletedTask;
            }
            // 1.查看当前商户是否存在交易数据
            if (_payOrderRepository.IsExistOrderUseMchNo(request.MchNo))
            {
                // 引发错误事件
                Bus.RaiseEvent(new DomainNotification("", "该商户已存在交易数据，不可删除！"));
                return Task.CompletedTask;
            }

            try
            {
                BeginTransaction();

                // 2.删除当前商户配置的支付通道
                _mchPayPassageRepository.RemoveByMchNo(request.MchNo);

                // 3.删除当前商户支付接口配置参数
                var appIds = _mchAppRepository.GetAll().Where(w => w.MchNo.Equals(request.MchNo)).Select(s => s.AppId).ToList();
                _payInterfaceConfigRepository.RemoveByInfoIds(appIds, CS.INFO_TYPE.MCH_APP);

                // 4.删除当前商户应用信息
                foreach (var appId in appIds)
                {
                    _mchAppRepository.Remove(appId);
                }

                var sysUsers = _sysUserRepository.GetAllAsNoTracking()
                    .Where(w => w.BelongInfoId.Equals(request.MchNo) && w.SysType.Equals(CS.SYS_TYPE.MCH));
                foreach (var sysUser in sysUsers)
                {
                    var sysUserAuths = _sysUserAuthRepository.GetAllAsNoTracking()
                        .Where(w => w.UserId.Equals(sysUser.SysUserId));
                    // 5.删除当前商户用户认证信息
                    foreach (var sysUserAuth in sysUserAuths)
                    {
                        _sysUserAuthRepository.Remove(sysUserAuth.AuthId);
                    }
                    // 6.删除当前商户的登录用户
                    _sysUserRepository.Remove(sysUser.SysUserId);
                }

                // 7.删除当前商户
                _mchInfoRepository.Remove(mchInfo.MchNo);

                if (!Commit())
                {
                    Bus.RaiseEvent(new DomainNotification("", "删除当前商户失败"));
                    return Task.CompletedTask;
                }

                CommitTransaction();

                // 推送mq删除redis用户缓存
                var userIdList = sysUsers.Select(s => s.SysUserId).ToList();
                mqSender.Send(CleanMchLoginAuthCacheMQ.Build(userIdList));

                // 推送mq到目前节点进行更新数据
                mqSender.Send(ResetIsvAgentMchAppInfoConfigMQ.Build(ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_MCH_INFO, null, null, request.MchNo, null));
            }
            catch (Exception e)
            {
                RollbackTransaction();
                Bus.RaiseEvent(new DomainNotification("", e.Message));
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
