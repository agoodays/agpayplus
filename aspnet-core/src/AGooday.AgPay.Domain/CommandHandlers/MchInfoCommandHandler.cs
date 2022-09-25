using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Commands.MchInfos;
using AGooday.AgPay.Domain.Commands.SysUsers;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Core.Commands;
using AGooday.AgPay.Domain.Core.Notifications;
using AGooday.AgPay.Domain.Events.MchInfos;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.CommandHandlers
{
    public class MchInfoCommandHandler : CommandHandler,
        IRequestHandler<CreateMchInfoCommand, Unit>
    {
        // 注入仓储接口
        private readonly IMchInfoRepository _mchInfoRepository;
        private readonly IIsvInfoRepository _isvInfoRepository;
        private readonly ISysUserRepository _sysUserRepository;
        private readonly ISysUserAuthRepository _sysUserAuthRepository;
        private readonly IMchAppRepository _mchAppRepository;

        // 用来进行DTO
        private readonly IMapper _mapper;

        // 注入总线
        private readonly IMediatorHandler Bus;
        private IMemoryCache Cache;

        public MchInfoCommandHandler(IUnitOfWork uow, IMediatorHandler bus, IMapper mapper, IMemoryCache cache,
            IMchInfoRepository mchInfoRepository,
            IIsvInfoRepository isvInfoRepository,
            ISysUserRepository sysUserRepository,
            ISysUserAuthRepository sysUserAuthRepository,
            IMchAppRepository mchAppRepository)
            : base(uow, bus, cache)
        {
            _mapper = mapper;
            Cache = cache;
            _sysUserRepository = sysUserRepository;
            _mchInfoRepository = mchInfoRepository;
            _isvInfoRepository = isvInfoRepository;
            _mchAppRepository = mchAppRepository;
            _sysUserAuthRepository = sysUserAuthRepository;
        }

        public Task<Unit> Handle(CreateMchInfoCommand request, CancellationToken cancellationToken)
        {
            // 命令验证
            if (!request.IsValid())
            {
                // 错误信息收集
                NotifyValidationErrors(request);
                // 返回，结束当前线程
                return Task.FromResult(new Unit());
            }

            var mchInfo = _mapper.Map<MchInfo>(request);
            do
            {
                mchInfo.MchNo = $"M{DateTimeOffset.Now.ToUnixTimeSeconds()}";
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
                    return Task.FromResult(new Unit());
                }
            }
            #endregion

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
            sysUser.AvatarUrl = "https://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/defava_m.png";//默认头像
            sysUser.IsAdmin = CS.YES;
            sysUser.State = mchInfo.State;

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
            string salt = Guid.NewGuid().ToString("N").Substring(0, 6); //6位随机数
            string authPwd = CS.DEFAULT_PWD;
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

            #region 插入商户默认应用
            // 插入商户默认应用
            var hmac = new HMACSHA256();
            var key = Convert.ToBase64String(hmac.Key);
            MchApp mchApp = new MchApp();
            mchApp.AppId = Guid.NewGuid().ToString("N").Substring(0, 24);
            mchApp.MchNo = mchInfo.MchNo;
            mchApp.AppName = "默认应用";
            mchApp.AppSecret = key;
            mchApp.State = CS.YES;
            mchApp.CreatedBy = sysUser.Realname;
            mchApp.CreatedUid = sysUser.SysUserId;

            _mchAppRepository.Add(mchApp); 
            #endregion

            // 插入商户基本信息
            // 存入商户默认用户ID
            mchInfo.InitUserId = sysUser.SysUserId;
            _mchInfoRepository.Add(mchInfo);

            if (Commit())
            {
                // 提交成功后，这里需要发布领域事件
                // 比如欢迎用户注册邮件呀，短信呀等
                var createdevent = _mapper.Map<MchInfoCreatedEvent>(mchInfo);
                Bus.RaiseEvent(createdevent);
            }

            return Task.FromResult(new Unit());
        }
    }
}
