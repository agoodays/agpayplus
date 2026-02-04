using Quartz;
using Quartz.Spi;

namespace AGooday.AgPay.Payment.Api.Jobs
{
    /// <summary>
    /// 依赖注入任务工厂，从 DI 容器中解析 Job 实例
    /// 支持注入 Scoped 服务（通过创建 Scope）
    /// </summary>
    public class DependencyInjectionJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DependencyInjectionJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            // 创建一个 Scope
            var scope = _serviceProvider.CreateScope();

            try
            {
                // 从 Scoped ServiceProvider 中解析真实的 Job
                var realJob = scope.ServiceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;

                // 返回包装后的 Job，自动管理 Scope 生命周期
                return new ScopedJob(realJob, scope);
            }
            catch
            {
                // 如果创建失败，立即释放 Scope
                scope.Dispose();
                throw;
            }
        }

        public void ReturnJob(IJob job)
        {
            // ScopedJob 会在 Dispose 时自动释放 Scope
            if (job is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    /// <summary>
    /// Scoped Job 包装类，负责管理 Scope 的生命周期
    /// </summary>
    internal sealed class ScopedJob : IJob, IDisposable
    {
        private readonly IJob _innerJob;
        private readonly IServiceScope _scope;

        public ScopedJob(IJob innerJob, IServiceScope scope)
        {
            _innerJob = innerJob ?? throw new ArgumentNullException(nameof(innerJob));
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public Task Execute(IJobExecutionContext context)
        {
            return _innerJob.Execute(context);
        }

        public void Dispose()
        {
            // 先释放 Scope（会释放所有 Scoped 服务）
            _scope.Dispose();

            // 如果内部 Job 实现了 IDisposable，也释放它
            if (_innerJob is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
