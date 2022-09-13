using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Commands.SysUsers;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Core.Notifications;
using AGooday.AgPay.Domain.Events.SysUsers;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.CommandHandlers
{
    /// <summary>
    /// Users领域命令处理程序
    /// 
    /// IRequestHandler 也是通过结构类型Unit来处理不需要返回值的情况。
    /// </summary>
    public class SysUserCommandHandler : CommandHandler,
        IRequestHandler<CreateSysUserCommand, Unit>
    {
        // 注入仓储接口
        private readonly ISysUserRepository _sysUserRepository;

        // 用来进行DTO
        private readonly IMapper _mapper;

        // 注入总线
        private readonly IMediatorHandler Bus;

        public SysUserCommandHandler(IUnitOfWork uow, IMediatorHandler bus, ISysUserRepository sysUserRepository, IMapper mapper)
            : base(uow, bus)
        {
            _sysUserRepository = sysUserRepository;
            _mapper = mapper;
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

            var entity = _mapper.Map<SysUser>(request);

            #region 检查
            // 登录用户名不可重复
            // 这些业务逻辑，当然要在领域层中（领域命令处理程序中）进行处理
            if (_sysUserRepository.IsExistLoginUsername(entity.LoginUsername, entity.SysType))
            {
                // 引发错误事件
                Bus.RaiseEvent(new DomainNotification("", "该用户名已经被使用！"));
                return Task.FromResult(new Unit());
            }
            // 手机号不可重复
            if (_sysUserRepository.IsExistTelphone(entity.Telphone, entity.SysType))
            {
                Bus.RaiseEvent(new DomainNotification("", "手机号已存在！"));
                return Task.FromResult(new Unit());
            }
            // 员工号不可重复
            if (_sysUserRepository.IsExistUserNo(entity.UserNo, entity.SysType))
            {
                Bus.RaiseEvent(new DomainNotification("", "员工号已存在！"));
                return Task.FromResult(new Unit());
            }
            #endregion

            //默认头像
            switch (entity.Sex) {
                case CS.SEX_MALE:
                    entity.AvatarUrl = "https://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/defava_m.png";
                    break;
                case CS.SEX_FEMALE:
                    entity.AvatarUrl = "https://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/defava_f.png";
                    break;
            }

            _sysUserRepository.AddAsync(entity);
            if (Commit())
            {
                // 提交成功后，这里需要发布领域事件
                // 比如欢迎用户注册邮件呀，短信呀等
                var createdevent = _mapper.Map<SysUserCreatedEvent>(entity);
                Bus.RaiseEvent(createdevent);
            }

            return Task.FromResult(new Unit());
        }
    }
}
