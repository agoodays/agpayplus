using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Base.Api.Hosting
{
    /// <summary>
    /// 共享宿主编排选项。
    /// 只描述「各 API 之间真实存在的差异」，共性与默认值由 <see cref="AgPayApiHostExtensions"/> 统一提供。
    /// 新增差异项时，请同时在此处说明它对应哪个服务，避免再次出现"四个 Program.cs 各写一遍"。
    /// </summary>
    public sealed class AgPayApiHostOptions
    {
        /// <summary>
        /// 宿主程序集。用于 Serilog SystemName 兜底与 MediatR 处理器扫描。
        /// 传入 <c>typeof(Program).Assembly</c>。
        /// </summary>
        public Assembly ApplicationAssembly { get; init; }

        /// <summary>
        /// 服务标识，作为 Serilog SystemName / Version 的最终兜底值。
        /// 正常情况下值来自 appsettings.json 的 AgSerilog 节点。
        /// </summary>
        public string SystemName { get; init; }

        /// <summary>
        /// Swagger 文档标题。
        /// Manager/Agent/Merchant 使用各自的程序集名；Payment 不设置标题（保持无参 AddSwaggerGen）。
        /// </summary>
        public string SwaggerTitle { get; init; }

        /// <summary>
        /// 是否启用 JWT 认证 + 权限点授权 + Swagger 安全定义。
        /// Manager / Agent / Merchant = true；Payment = false（该服务使用商户请求验签）。
        /// </summary>
        public bool UseJwtAuthentication { get; init; }

        /// <summary>
        /// 操作日志处理器类型（IOpLogHandler 的实现）。
        /// 该类型在每个宿主项目中各有一份（Manager/Agent/Merchant），因此由宿主注入。
        /// 为 null 时不注册操作日志（Payment）。
        /// </summary>
        public Type OperationLogHandlerType { get; init; }

        /// <summary>
        /// 是否注册渠道注入与 ConfigContext（Manager / Payment = true，Agent / Merchant = false）。
        /// </summary>
        public bool UseChannelConfigContext { get; init; }

        /// <summary>
        /// 是否启用 WebSocket（Manager / Merchant = true）。
        /// 注意：WebSocket 服务实例本身由宿主项目注册。
        /// </summary>
        public bool UseWebSockets { get; init; }

        /// <summary>
        /// 是否挂载 OCR 控制器与 OCR 注入（仅 Manager）。
        /// </summary>
        public bool UseOcr { get; init; }

        /// <summary>
        /// 是否启用 .NET 9 内置 OpenAPI + Scalar UI（仅 Manager）。
        /// </summary>
        public bool UseScalarOpenApi { get; init; }

        /// <summary>
        /// 是否初始化雪花 ID 与 SeqUtil（Manager / Agent / Payment = true）。
        /// </summary>
        /// <remarks>
        /// Merchant 当前为 false —— 该服务的 Program.cs 与 appsettings.json 都没有 IdWorkerConfig，
        /// 即从未调用过 IdWorker.Initialize / SeqUtil.Initialize。
        /// 本选项如实反映现状；是否补齐属于独立决策，见分析报告。
        /// </remarks>
        public bool InitializeIdWorker { get; init; }

        /// <summary>
        /// MVC 注册方式。Merchant / Payment 使用 AddControllersWithViews，Manager / Agent 使用 AddControllers。
        /// </summary>
        public bool UseControllersWithViews { get; init; }

        /// <summary>
        /// 是否禁用默认模型验证过滤器（仅 Payment）。
        /// </summary>
        public bool SuppressModelStateInvalidFilter { get; init; }

        /// <summary>
        /// 追加的 MVC 选项配置，用于注入宿主项目本地的过滤器（Payment 的 ValidateModelAttribute）。
        /// </summary>
        public Action<MvcOptions> ConfigureMvcOptions { get; init; }

        /// <summary>
        /// 是否把非 API 路由回退到收银台 SPA（仅 Payment）。
        /// 为 true 时管线改用 UseRouting().UseEndpoints()。
        /// </summary>
        public bool MapCashierFallback { get; init; }

        /// <summary>
        /// 是否把 Quartz 健康检查并入 /health/ready（仅 Payment，该服务承载定时任务）。
        /// </summary>
        public bool EnableQuartzHealthCheck { get; init; }
    }
}
