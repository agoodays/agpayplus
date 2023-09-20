using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Domain.CommandHandlers;
using AGooday.AgPay.Domain.Commands.AgentInfos;
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

namespace AGooday.AgPay.Agent.Api.Extensions
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
        /// 
        /// 在依赖注入中，Scoped 服务只能从具有相同或更短生命周期的服务中访问，而不能从 Singleton 服务中访问。这是为了避免在 Singleton 服务中保持 Scoped 服务实例的状态。
        /// </summary>
        /// <param name="services"></param>
        public static void RegisterServices(IServiceCollection services)
        {
            // 注入 应用层Application
            services.AddScoped<IAgentInfoService, AgentInfoService>();
            services.AddScoped<IIsvInfoService, IsvInfoService>();
            services.AddScoped<IMchAppService, MchAppService>();
            services.AddScoped<IMchStoreService, MchStoreService>();
            services.AddScoped<IMchDivisionReceiverGroupService, MchDivisionReceiverGroupService>();
            services.AddScoped<IMchDivisionReceiverService, MchDivisionReceiverService>();
            services.AddScoped<IMchInfoService, MchInfoService>();
            services.AddScoped<IMchNotifyRecordService, MchNotifyRecordService>();
            services.AddScoped<IMchPayPassageService, MchPayPassageService>();
            services.AddScoped<IPayInterfaceConfigService, PayInterfaceConfigService>();
            services.AddScoped<IPayInterfaceDefineService, PayInterfaceDefineService>();
            services.AddScoped<IPayOrderDivisionRecordService, PayOrderDivisionRecordService>();
            services.AddScoped<IPayOrderService, PayOrderService>();
            services.AddScoped<IPayRateConfigService, PayRateConfigService>();
            services.AddScoped<IPayWayService, PayWayService>();
            services.AddScoped<IRefundOrderService, RefundOrderService>();
            services.AddScoped<ISysArticleService, SysArticleService>();
            services.AddScoped<ISysConfigService, SysConfigService>();
            services.AddScoped<ISysEntitlementService, SysEntitlementService>();
            services.AddScoped<ISysLogService, SysLogService>();
            services.AddScoped<ISysRoleEntRelaService, SysRoleEntRelaService>();
            services.AddScoped<ISysRoleService, SysRoleService>();
            services.AddScoped<ISysUserAuthService, SysUserAuthService>();
            services.AddScoped<ISysUserRoleRelaService, SysUserRoleRelaService>();
            services.AddScoped<ISysUserService, SysUserService>();
            services.AddScoped<ISysUserTeamService, SysUserTeamService>();
            services.AddScoped<ITransferOrderService, TransferOrderService>();

            // 命令总线Domain Bus (Mediator) 中介总线接口
            services.AddScoped<IMediatorHandler, InMemoryBus>();

            // Domain - Events
            // 将事件模型和事件处理程序匹配注入
            // 领域通知
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
            // 领域事件
            services.AddScoped<INotificationHandler<SysUserCreatedEvent>, SysUserEventHandler>();

            // 领域层 - 领域命令
            // 将命令模型和命令处理程序匹配
            services.AddScoped<IRequestHandler<CreateSysUserCommand, Unit>, SysUserCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveSysUserCommand, Unit>, SysUserCommandHandler>();
            services.AddScoped<IRequestHandler<ModifySysUserCommand, Unit>, SysUserCommandHandler>();

            services.AddScoped<IRequestHandler<CreateAgentInfoCommand, Unit>, AgentInfoCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveAgentInfoCommand, Unit>, AgentInfoCommandHandler>();
            services.AddScoped<IRequestHandler<ModifyAgentInfoCommand, Unit>, AgentInfoCommandHandler>();

            services.AddScoped<IRequestHandler<CreateMchInfoCommand, Unit>, MchInfoCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveMchInfoCommand, Unit>, MchInfoCommandHandler>();
            services.AddScoped<IRequestHandler<ModifyMchInfoCommand, Unit>, MchInfoCommandHandler>();

            // 领域层 - Memory缓存
            services.AddSingleton<IMemoryCache>(factory =>
            {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });

            // 注入 基础设施层 - 数据层
            //services.AddDbContext<AgPayDbContext>(ServiceLifetime.Transient);
            services.AddScoped<AgPayDbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IAgentInfoRepository, AgentInfoRepository>();
            services.AddScoped<IIsvInfoRepository, IsvInfoRepository>();
            services.AddScoped<IMchAppRepository, MchAppRepository>();
            services.AddScoped<IMchStoreRepository, MchStoreRepository>();
            services.AddScoped<IMchDivisionReceiverGroupRepository, MchDivisionReceiverGroupRepository>();
            services.AddScoped<IMchDivisionReceiverRepository, MchDivisionReceiverRepository>();
            services.AddScoped<IMchInfoRepository, MchInfoRepository>();
            services.AddScoped<IMchNotifyRecordRepository, MchNotifyRecordRepository>();
            services.AddScoped<IMchPayPassageRepository, MchPayPassageRepository>();
            services.AddScoped<IPayInterfaceConfigRepository, PayInterfaceConfigRepository>();
            services.AddScoped<IPayInterfaceDefineRepository, PayInterfaceDefineRepository>();
            services.AddScoped<IPayOrderDivisionRecordRepository, PayOrderDivisionRecordRepository>();
            services.AddScoped<IPayOrderRepository, PayOrderRepository>();
            services.AddScoped<IPayRateConfigRepository, PayRateConfigRepository>();
            services.AddScoped<IPayRateLevelConfigRepository, PayRateLevelConfigRepository>();
            services.AddScoped<IPayWayRepository, PayWayRepository>();
            services.AddScoped<IRefundOrderRepository, RefundOrderRepository>();
            services.AddScoped<ISysArticleRepository, SysArticleRepository>();
            services.AddScoped<ISysConfigRepository, SysConfigRepository>();
            services.AddScoped<ISysEntitlementRepository, SysEntitlementRepository>();
            services.AddScoped<ISysLogRepository, SysLogRepository>();
            services.AddScoped<ISysRoleEntRelaRepository, SysRoleEntRelaRepository>();
            services.AddScoped<ISysRoleRepository, SysRoleRepository>();
            services.AddScoped<ISysUserAuthRepository, SysUserAuthRepository>();
            services.AddScoped<ISysUserRoleRelaRepository, SysUserRoleRelaRepository>();
            services.AddScoped<ISysUserRepository, SysUserRepository>();
            services.AddScoped<ISysUserTeamRepository, SysUserTeamRepository>();
            services.AddScoped<ITransferOrderRepository, TransferOrderRepository>();
        }
    }
}
