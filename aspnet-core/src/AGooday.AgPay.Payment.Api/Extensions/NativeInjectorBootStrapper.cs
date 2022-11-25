using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Domain.CommandHandlers;
using AGooday.AgPay.Domain.Commands.MchInfos;
using AGooday.AgPay.Domain.Commands.SysUsers;
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

namespace AGooday.AgPay.Payment.Api.Extensions
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
            services.AddTransient<IIsvInfoService, IsvInfoService>();
            services.AddTransient<IMchAppService, MchAppService>();
            services.AddTransient<IMchDivisionReceiverGroupService, MchDivisionReceiverGroupService>();
            services.AddTransient<IMchDivisionReceiverService, MchDivisionReceiverService>();
            services.AddTransient<IMchInfoService, MchInfoService>();
            services.AddTransient<IMchNotifyRecordService, MchNotifyRecordService>();
            services.AddTransient<IMchPayPassageService, MchPayPassageService>();
            services.AddTransient<IPayInterfaceConfigService, PayInterfaceConfigService>();
            services.AddTransient<IPayInterfaceDefineService, PayInterfaceDefineService>();
            services.AddTransient<IPayOrderDivisionRecordService, PayOrderDivisionRecordService>();
            services.AddTransient<IPayOrderService, PayOrderService>();
            services.AddTransient<IPayWayService, PayWayService>();
            services.AddTransient<IRefundOrderService, RefundOrderService>();
            services.AddTransient<ISysConfigService, SysConfigService>();
            services.AddTransient<ISysEntitlementService, SysEntitlementService>();
            services.AddTransient<ISysLogService, SysLogService>();
            services.AddTransient<ISysRoleEntRelaService, SysRoleEntRelaService>();
            services.AddTransient<ISysRoleService, SysRoleService>();
            services.AddTransient<ISysUserAuthService, SysUserAuthService>();
            services.AddTransient<ISysUserRoleRelaService, SysUserRoleRelaService>();
            services.AddTransient<ISysUserService, SysUserService>();
            services.AddTransient<ITransferOrderService, TransferOrderService>();

            // 命令总线Domain Bus (Mediator) 中介总线接口
            services.AddTransient<IMediatorHandler, InMemoryBus>();

            // Domain - Events
            // 将事件模型和事件处理程序匹配注入
            // 领域通知
            services.AddTransient<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
            // 领域事件
            services.AddTransient<INotificationHandler<SysUserCreatedEvent>, SysUserEventHandler>();

            // 领域层 - 领域命令
            // 将命令模型和命令处理程序匹配
            services.AddTransient<IRequestHandler<CreateSysUserCommand, Unit>, SysUserCommandHandler>();
            services.AddTransient<IRequestHandler<RemoveSysUserCommand, Unit>, SysUserCommandHandler>();
            services.AddTransient<IRequestHandler<ModifySysUserCommand, Unit>, SysUserCommandHandler>();

            services.AddTransient<IRequestHandler<CreateMchInfoCommand, Unit>, MchInfoCommandHandler>();
            services.AddTransient<IRequestHandler<RemoveMchInfoCommand, Unit>, MchInfoCommandHandler>();
            services.AddTransient<IRequestHandler<ModifyMchInfoCommand, Unit>, MchInfoCommandHandler>();

            // 领域层 - Memory缓存
            services.AddSingleton<IMemoryCache>(factory =>
            {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });

            // 注入 基础设施层 - 数据层
            services.AddTransient<AgPayDbContext>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IRepository, Repository>();
            services.AddTransient<IIsvInfoRepository, IsvInfoRepository>();
            services.AddTransient<IMchAppRepository, MchAppRepository>();
            services.AddTransient<IMchDivisionReceiverGroupRepository, MchDivisionReceiverGroupRepository>();
            services.AddTransient<IMchDivisionReceiverRepository, MchDivisionReceiverRepository>();
            services.AddTransient<IMchInfoRepository, MchInfoRepository>();
            services.AddTransient<IMchNotifyRecordRepository, MchNotifyRecordRepository>();
            services.AddTransient<IMchPayPassageRepository, MchPayPassageRepository>();
            services.AddTransient<IPayInterfaceConfigRepository, PayInterfaceConfigRepository>();
            services.AddTransient<IPayInterfaceDefineRepository, PayInterfaceDefineRepository>();
            services.AddTransient<IPayOrderDivisionRecordRepository, PayOrderDivisionRecordRepository>();
            services.AddTransient<IPayOrderRepository, PayOrderRepository>();
            services.AddTransient<IPayWayRepository, PayWayRepository>();
            services.AddTransient<IRefundOrderRepository, RefundOrderRepository>();
            services.AddTransient<ISysConfigRepository, SysConfigRepository>();
            services.AddTransient<ISysEntitlementRepository, SysEntitlementRepository>();
            services.AddTransient<ISysLogRepository, SysLogRepository>();
            services.AddTransient<ISysRoleEntRelaRepository, SysRoleEntRelaRepository>();
            services.AddTransient<ISysRoleRepository, SysRoleRepository>();
            services.AddTransient<ISysUserAuthRepository, SysUserAuthRepository>();
            services.AddTransient<ISysUserRoleRelaRepository, SysUserRoleRelaRepository>();
            services.AddTransient<ISysUserRepository, SysUserRepository>();
            services.AddTransient<ITransferOrderRepository, TransferOrderRepository>();
        }
    }
}
