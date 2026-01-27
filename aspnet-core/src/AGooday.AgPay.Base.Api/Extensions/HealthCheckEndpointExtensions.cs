using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace AGooday.AgPay.Base.Api.Extensions
{
    /// <summary>
    /// 健康检查端点扩展方法
    /// </summary>
    public static class HealthCheckEndpointExtensions
    {
        /// <summary>
        /// 映射标准的健康检查端点
        /// 包括完整健康检查、就绪探针和存活探针
        /// </summary>
        /// <param name="app">应用程序构建器</param>
        public static void MapStandardHealthChecks(this WebApplication app)
        {
            // 完整健康检查（所有检查项）
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var result = JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            description = e.Value.Description,
                            data = e.Value.Data,
                            duration = e.Value.Duration.TotalMilliseconds
                        }),
                        totalDuration = report.TotalDuration.TotalMilliseconds
                    });
                    await context.Response.WriteAsync(result);
                }
            });

            // Kubernetes 就绪探针（检查应用是否准备好接收流量）
            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready")
            });

            // Kubernetes 存活探针（检查应用是否还活着）
            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("live")
            });
        }
    }
}
