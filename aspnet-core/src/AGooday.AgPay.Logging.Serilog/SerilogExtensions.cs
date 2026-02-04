using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace AGooday.AgPay.Logging.Serilog
{
    public static class SerilogExtensions
    {
        public static IHostBuilder UseAgSerilog(this IHostBuilder hostBuilder, IConfiguration configuration, Action<SerilogOptions> configureOptions = null)
        {
            var options = new SerilogOptions();
            configuration.GetSection("AgSerilog").Bind(options);
            configureOptions?.Invoke(options);

            // 构建 Logger 配置
            return hostBuilder.UseSerilog((ctx, config) =>
            {
                // 从 appsettings.json 加载基础配置（Console 等）
                config.ReadFrom.Configuration(ctx.Configuration) // 先加载 Console 等静态配置        
                      .Enrich.FromLogContext() // 注册日志上下文
                      .Enrich.WithThreadId()
                      // 上下文
                      .Enrich.WithProperty("SystemName", options.SystemName)
                      .Enrich.WithProperty("Version", options.Version);

                // 👇 动态添加按级别分离的文件输出
                AddFileSinks(config, options);

                // 🔜 动态添加 Seq（如果配置了）
                if (options.EnableSeq && !string.IsNullOrWhiteSpace(options.SeqUrl))
                {
                    config.WriteTo.Seq(
                        serverUrl: options.SeqUrl,
                        apiKey: options.SeqApiKey,
                        restrictedToMinimumLevel: LogEventLevel.Information);
                }
            });
        }

        private static void AddFileSinks(LoggerConfiguration config, SerilogOptions options)
        {
            var levels = new[]
            {
                (Level: LogEventLevel.Debug, Folder: "debug", FilePrefix: "debug_"),
                (Level: LogEventLevel.Information, Folder: "info", FilePrefix: "info_"),
                (Level: LogEventLevel.Warning, Folder: "warn", FilePrefix: "warn_"),
                (Level: LogEventLevel.Error, Folder: "error", FilePrefix: "error_")
            };

            foreach (var (level, folder, prefix) in levels)
            {
                var path = Path.Combine(options.BaseLogPath, folder, $"{prefix}.log");

                if (level == LogEventLevel.Error)
                {
                    config.WriteTo.Logger(lc => lc
                        .Filter.ByIncludingOnly(e => e.Level >= level)
                        .WriteTo.File(
                            path: path,
                            outputTemplate: options.OutputTemplate,
                            rollingInterval: RollingInterval.Day,
                            fileSizeLimitBytes: 10 * 1024 * 1024,
                            retainedFileCountLimit: 10000,
                            shared: true));
                }
                else
                {
                    config.WriteTo.Logger(lc => lc
                        .Filter.ByIncludingOnly(e => e.Level == level)
                        .WriteTo.File(
                            path: path,
                            outputTemplate: options.OutputTemplate,
                            rollingInterval: RollingInterval.Day,
                            fileSizeLimitBytes: 10 * 1024 * 1024,
                            retainedFileCountLimit: 10000,
                            shared: true));
                }
            }
        }
    }
}
