using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Domain.CommandHandlers;
using AGooday.AgPay.Domain.Commands.MchInfos;
using AGooday.AgPay.Domain.Commands.SysUsers;
using AGooday.AgPay.Domain.Communication;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Core.Notifications;
using AGooday.AgPay.Domain.EventHandlers;
using AGooday.AgPay.Domain.Events.SysUsers;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Infrastructure.Bus;
using AGooday.AgPay.Infrastructure.Context;
using AGooday.AgPay.Infrastructure.Repositories;
using AGooday.AgPay.Infrastructure.UoW;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace AGooday.AgPay.Manager.Api.Extensions
{
    public class NativeInjectorBootStrapper
    {
        /// <summary>
        /// services.AddTransient<IApplicationService,ApplicationService>//服务在每次请求时被创建，它最好被用于轻量级无状态服务（如我们的Repository和ApplicationService服务）
        /// services.AddScoped<IApplicationService, ApplicationService>//服务在每次请求时被创建，生命周期横贯整次请求
        /// services.AddSingleton<IApplicationService, ApplicationService>//Singleton（单例） 服务在第一次请求时被创建（或者当我们在ConfigureServices中指定创建某一实例并运行方法），其后的每次请求将沿用已创建服务。如果开发者的应用需要单例服务情景，请设计成允许服务容器来对服务生命周期进行操作，而不是手动实现单例设计模式然后由开发者在自定义类中进行操作。
        /// 
        /// 权重：AddSingleton→AddTransient→AddScoped
        /// AddSingleton的生命周期：项目启动-项目关闭 相当于静态类  只会有一个
        /// AddScoped的生命周期：请求开始-请求结束 在这次请求中获取的对象都是同一个
        /// AddTransient的生命周期：请求获取-（GC回收-主动释放） 每一次获取的对象都不是同一个
        /// </summary>
        /// <param name="services"></param>
        public static void RegisterServices(IServiceCollection services)
        {
            // 注入 应用层Application
            services.AddSingleton<IIsvInfoService, IsvInfoService>();
            services.AddSingleton<IMchAppService, MchAppService>();
            services.AddSingleton<IMchDivisionReceiverGroupService, MchDivisionReceiverGroupService>();
            services.AddSingleton<IMchDivisionReceiverService, MchDivisionReceiverService>();
            services.AddSingleton<IMchInfoService, MchInfoService>();
            services.AddSingleton<IMchNotifyRecordService, MchNotifyRecordService>();
            services.AddSingleton<IMchPayPassageService, MchPayPassageService>();
            services.AddSingleton<IPayInterfaceConfigService, PayInterfaceConfigService>();
            services.AddSingleton<IPayInterfaceDefineService, PayInterfaceDefineService>();
            services.AddSingleton<IPayOrderDivisionRecordService, PayOrderDivisionRecordService>();
            services.AddSingleton<IPayOrderService, PayOrderService>();
            services.AddSingleton<IPayWayService, PayWayService>();
            services.AddSingleton<IRefundOrderService, RefundOrderService>();
            services.AddSingleton<ISysConfigService, SysConfigService>();
            services.AddSingleton<ISysEntitlementService, SysEntitlementService>();
            services.AddSingleton<ISysLogService, SysLogService>();
            services.AddSingleton<ISysRoleEntRelaService, SysRoleEntRelaService>();
            services.AddSingleton<ISysRoleService, SysRoleService>();
            services.AddSingleton<ISysUserAuthService, SysUserAuthService>();
            services.AddSingleton<ISysUserRoleRelaService, SysUserRoleRelaService>();
            services.AddSingleton<ISysUserService, SysUserService>();
            services.AddSingleton<ITransferOrderService, TransferOrderService>();

            // 命令总线Domain Bus (Mediator) 中介总线接口
            services.AddSingleton<IMediatorHandler, InMemoryBus>();

            // Domain - Events
            // 将事件模型和事件处理程序匹配注入
            // 领域通知
            services.AddSingleton<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
            // 领域事件
            services.AddSingleton<INotificationHandler<SysUserCreatedEvent>, SysUserEventHandler>();

            // 领域层 - 领域命令
            // 将命令模型和命令处理程序匹配
            services.AddSingleton<IRequestHandler<CreateSysUserCommand, Unit>, SysUserCommandHandler>();
            services.AddSingleton<IRequestHandler<RemoveSysUserCommand, Unit>, SysUserCommandHandler>();
            services.AddSingleton<IRequestHandler<ModifySysUserCommand, Unit>, SysUserCommandHandler>();

            services.AddSingleton<IRequestHandler<CreateMchInfoCommand, Unit>, MchInfoCommandHandler>();
            services.AddSingleton<IRequestHandler<RemoveMchInfoCommand, Unit>, MchInfoCommandHandler>();
            services.AddSingleton<IRequestHandler<ModifyMchInfoCommand, Unit>, MchInfoCommandHandler>();

            // 领域层 - Memory缓存
            services.AddSingleton<IMemoryCache>(factory =>
            {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });

            // 注入 基础设施层 - 数据层
            services.AddSingleton<AgPayDbContext>();
            services.AddSingleton<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IRepository, Repository>();
            services.AddSingleton<IIsvInfoRepository, IsvInfoRepository>();
            services.AddSingleton<IMchAppRepository, MchAppRepository>();
            services.AddSingleton<IMchDivisionReceiverGroupRepository, MchDivisionReceiverGroupRepository>();
            services.AddSingleton<IMchDivisionReceiverRepository, MchDivisionReceiverRepository>();
            services.AddSingleton<IMchInfoRepository, MchInfoRepository>();
            services.AddSingleton<IMchNotifyRecordRepository, MchNotifyRecordRepository>();
            services.AddSingleton<IMchPayPassageRepository, MchPayPassageRepository>();
            services.AddSingleton<IPayInterfaceConfigRepository, PayInterfaceConfigRepository>();
            services.AddSingleton<IPayInterfaceDefineRepository, PayInterfaceDefineRepository>();
            services.AddSingleton<IPayOrderDivisionRecordRepository, PayOrderDivisionRecordRepository>();
            services.AddSingleton<IPayOrderRepository, PayOrderRepository>();
            services.AddSingleton<IPayWayRepository, PayWayRepository>();
            services.AddSingleton<IRefundOrderRepository, RefundOrderRepository>();
            services.AddSingleton<ISysConfigRepository, SysConfigRepository>();
            services.AddSingleton<ISysEntitlementRepository, SysEntitlementRepository>();
            services.AddSingleton<ISysLogRepository, SysLogRepository>();
            services.AddSingleton<ISysRoleEntRelaRepository, SysRoleEntRelaRepository>();
            services.AddSingleton<ISysRoleRepository, SysRoleRepository>();
            services.AddSingleton<ISysUserAuthRepository, SysUserAuthRepository>();
            services.AddSingleton<ISysUserRoleRelaRepository, SysUserRoleRelaRepository>();
            services.AddSingleton<ISysUserRepository, SysUserRepository>();
            services.AddSingleton<ITransferOrderRepository, TransferOrderRepository>();
        }
    }
}
