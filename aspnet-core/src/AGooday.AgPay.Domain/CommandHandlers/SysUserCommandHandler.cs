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
            // 判断用户名是否存在
            // 这些业务逻辑，当然要在领域层中（领域命令处理程序中）进行处理
            var existingUsers = _sysUserRepository.GetByLoginUsername(entity.LoginUsername);
            if (existingUsers != null && existingUsers.SysUserId != entity.SysUserId)
            {
                //引发错误事件
                Bus.RaiseEvent(new DomainNotification("", "该用户名已经被使用！"));
                return Task.FromResult(new Unit());
            }
            #endregion

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
