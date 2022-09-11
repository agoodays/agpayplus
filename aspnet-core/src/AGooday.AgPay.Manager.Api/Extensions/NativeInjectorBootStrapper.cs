using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Infrastructure.Context;
using AGooday.AgPay.Infrastructure.Repositories;

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
            services.AddScoped<ISysUserService, SysUserService>();

            // 注入 基础设施层 - 数据层
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<ISysUserRepository, SysUserRepository>();
            services.AddScoped<AgPayDbContext>();
        }
    }
}
