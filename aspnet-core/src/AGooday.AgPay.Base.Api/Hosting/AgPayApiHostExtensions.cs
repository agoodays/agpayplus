using System.Reflection;
using AGooday.AgPay.Base.Api.Authorization;
using AGooday.AgPay.Base.Api.Extensions;
using AGooday.AgPay.Base.Api.Extensions.AuthContext;
using AGooday.AgPay.Base.Api.Filter;
using AGooday.AgPay.Base.Api.Middlewares;
using AGooday.AgPay.Base.Api.Models;
using AGooday.AgPay.Base.Api.OpLog;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Cache.Extensions;
using AGooday.AgPay.Components.Cache.Options;
using AGooday.AgPay.Components.OCR.Controllers;
using AGooday.AgPay.Components.OCR.Extensions;
using AGooday.AgPay.Components.OSS.Config;
using AGooday.AgPay.Components.OSS.Controllers;
using AGooday.AgPay.Components.OSS.Extensions;
using AGooday.AgPay.Components.SMS.Extensions;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Logging.Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Scalar.AspNetCore;

namespace AGooday.AgPay.Base.Api.Hosting
{
    /// <summary>
    /// AgPay API 共享宿主。
    /// 把原先复制在 4 个 Program.cs 中的启动逻辑收敛为单一实现：
    /// 各服务只需声明 <see cref="AgPayApiHostOptions"/>，并注册自身独有的服务（MQ Receiver、WebSocket 服务、支付链路服务等）。
    /// </summary>
    /// <remarks>
    /// 收敛原则（供后续维护者遵循）：
    /// <list type="number">
    /// <item>共性逻辑只能写在这里，不要在某个 Program.cs 里另起一份。</item>
    /// <item>新增服务间差异时，在 <see cref="AgPayApiHostOptions"/> 增加一个带注释的选项，而不是在 Program.cs 里复制代码。</item>
    /// <item>
    /// <b>原 Program.cs 中的说明性注释与"被注释掉的备选实现"同样属于信息资产，收敛时必须一并迁移。</b>
    /// 不要因为"代码没被执行"就丢弃——它们记录了可选方案、历史取舍与启用步骤
    /// （例如 Swagger 的 XML 注释启用方式、HealthChecks 的备选写法、ConfigContextService 的
    /// 服务定位器反例）。本文件中的注释即为原 4 份文件注释的合并结果。
    /// </item>
    /// <item>
    /// <b>注释里的代码片段必须按新位置的作用域与程序集身份重新校验，不能机械照搬。</b>
    /// <see cref="System.Reflection.Assembly.GetExecutingAssembly"/>、<c>GetCallingAssembly</c>、
    /// 以及对原文件局部变量的引用都属于"位置敏感"写法：原在某个 API 的 Program.cs 中成立，
    /// 移入本类库后语义即改变（例如 GetExecutingAssembly 会从宿主 API 变成 Base.Api）。
    /// 本文件已按此原则改写了若干处，详见各处注释说明。
    /// </item>
    /// </list>
    /// </remarks>
    public static class AgPayApiHostExtensions
    {
        /// <summary>
        /// 注册共享服务与横切配置。应在注册宿主独有服务之前调用。
        /// </summary>
        public static WebApplicationBuilder AddAgPayApiHost(this WebApplicationBuilder builder, AgPayApiHostOptions options)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(options);

            // 最先加载共享配置：它必须成为【最低优先级】的默认值来源。
            AddSharedJsonConfiguration(builder);

            // Add services to the container.
            var services = builder.Services;
            var configuration = builder.Configuration;
            // 宿主程序集：用于 MediatR 处理器扫描、Serilog SystemName 兜底、以及 Swagger XML 注释文件名。
            // 一旦取错程序集，MediatR 会扫不到处理器、XML 注释也会指向不存在的文件，
            // 因此这里选择“取不到就明确报错”，而不是静默退化成 Base.Api 程序集。
            var applicationAssembly = options.ApplicationAssembly ?? Assembly.GetEntryAssembly();
            if (applicationAssembly == null)
            {
                throw new InvalidOperationException(
                    "无法确定宿主程序集。请为 AgPayApiHostOptions.ApplicationAssembly 赋值（通常是 typeof(Program).Assembly）。" +
                    "该值用于 MediatR 处理器扫描与 Swagger XML 注释文件名解析，取错程序集会导致处理器扫不到或文档路径不存在。");
            }

            #region 日志
            // 调用 ClearProviders 以从生成器中删除所有 ILoggerProvider 实例
            builder.Logging.ClearProviders();
            //// 通常，日志级别应在配置中指定，而不是在代码中指定。
            // 原 Program.cs 此处为 logging.AddFilter(...)（logging = builder.Logging）；本文件未声明该局部变量
            //builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
            // 添加控制台日志记录提供程序。
            builder.Logging.AddConsole();

            // 构建 Logger 配置
            builder.Host.UseAgSerilog(configuration, serilogOptions =>
            {
                // AgSerilog 节点已绑定配置；此处仅提供兜底，保持与原有行为一致
                serilogOptions.SystemName ??= applicationAssembly?.GetName().Name ?? options.SystemName;
                serilogOptions.Version ??= applicationAssembly?.GetName().Version?.ToString() ?? "1.0.0";
            });
            #endregion

            // 必须传入宿主的 configuration：
            // 原 Program.cs 此处曾用 new Appsettings(Env.ContentRootPath)（按 contentPath 自行构建配置），
            // 该重载只读 appsettings.json，会丢掉共享配置（appsettings.Shared*.json）与环境变量，
            // 从而使 docker-compose 的注入失效、生产静默连错库。该重载已从 Appsettings 中删除。
            services.AddSingleton(new Appsettings(configuration));

            #region Redis
            var redisOptions = configuration.GetSection("Redis:Default").Get<RedisOptions>();

            //services.AddSingleton(new RedisUtil(redisOptions.Connection, redisOptions.InstanceName, redisOptions.DefaultDB));

            CacheNativeInjectorBootStrapper.RegisterServices(services, redisOptions);
            #endregion

            #region OSS
            configuration.GetSection("OSS").Bind(LocalOssConfig.Oss);
            configuration.GetSection("OSS:AliyunOss").Bind(AliyunOssConfig.Oss);
            OSSNativeInjectorBootStrapper.RegisterServices(services);
            #endregion

            #region SMS
            SMSNativeInjectorBootStrapper.RegisterServices(services);
            #endregion

            #region CORS
            // 1. 读取生产环境的白名单配置
            var prodOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

            services.AddCors(o =>
                o.AddPolicy("CorsPolicy", policyBuilder =>
                {
                    policyBuilder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials(); // 统一开启凭证支持

#if DEBUG
                    // 开发环境：动态放行所有来源（完美实现“不限制”）
                    policyBuilder.SetIsOriginAllowed(origin => true);
#else
                    // 生产环境：严格校验白名单
                    policyBuilder.WithOrigins(prodOrigins);
#endif
                }));
            #endregion

            services.AddMemoryCache();

            #region 认证 / 授权 / 加密密钥
            //用户信息
            // 四个服务都需要 IHttpContextAccessor（AgPayDbContext 依赖它）
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // 说明：原 Payment.Api 此处另有一行 //services.AddSingleton<RequestIpUtil>();
            //      但 RequestIpUtil 是 public static class，不能作为泛型类型实参，该行无法编译；
            //      静态工具类本就无需注册 DI，故收敛后不再保留。

            AgPayUtil.AES_KEY = configuration["AesKey"];

            if (options.UseJwtAuthentication)
            {
                services.AddHttpContextAccessor();
                services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

                // JWT 认证
                var jwtSettingsSection = configuration.GetSection("JWT");
                services.Configure<JwtSettings>(jwtSettingsSection);
                services.AddJwtBearerAuthentication(jwtSettingsSection.Get<JwtSettings>());

                services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

                var sysRSA2Section = configuration.GetSection("SysRSA2");
                services.Configure<SysRSA2Config>(sysRSA2Section);
                AgPayUtil.RSA2_PRIVATE_KEY = sysRSA2Section.Get<SysRSA2Config>()?.PrivateKey;
            }
            else
            {
                // Payment.Api 走扁平配置键
                AgPayUtil.RSA2_PRIVATE_KEY = configuration["SysRSA2:PrivateKey"];
            }
            #endregion

            #region 操作日志
            if (options.OperationLogHandlerType != null)
            {
                services.AddScoped(typeof(IOpLogHandler), options.OperationLogHandlerType);
            }
            #endregion

            #region AutoMapper / Newtonsoft 全局设置
            // Automapper 注入
            services.AddAutoMapperSetup();

            // Newtonsoft.Json 全部配置 
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.None,//格式化
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            #endregion

            #region MVC / JSON
            IMvcBuilder mvcBuilder = options.UseControllersWithViews
                ? services.AddControllersWithViews(mvcOptions => ApplyMvcOptions(mvcOptions, options))
                : services.AddControllers(mvcOptions => ApplyMvcOptions(mvcOptions, options));

            // OSS 控制器：Manager / Agent / Merchant 挂载；Payment 注册了注入但不暴露端点（保持现状）
            if (options.UseJwtAuthentication)
            {
                mvcBuilder.AddApplicationPart(typeof(OssFileController).Assembly);
            }

            if (options.UseOcr)
            {
                mvcBuilder.AddApplicationPart(typeof(OcrController).Assembly);
                OCRNativeInjectorBootStrapper.RegisterServices(services);
            }

            //.AddNewtonsoftJson();
            mvcBuilder.AddNewtonsoftJson(jsonOptions =>
            {
                //https://blog.poychang.net/using-newtonsoft-json-in-asp-net-core-projects/
                jsonOptions.SerializerSettings.Formatting = Formatting.None;
                //jsonOptions.SerializerSettings.ContractResolver = new DefaultContractResolver();
                jsonOptions.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                jsonOptions.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();//Json key 首字符小写（大驼峰转小驼峰）
                jsonOptions.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                jsonOptions.SerializerSettings.Converters.Add(new BaseModelJsonConverter<BaseModel>());
            });

            if (options.SuppressModelStateInvalidFilter)
            {
                // 禁用默认模型验证过滤器
                services.Configure<ApiBehaviorOptions>(apiOptions => apiOptions.SuppressModelStateInvalidFilter = true);
            }
            #endregion

            #region Swagger / OpenAPI
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            if (options.UseJwtAuthentication)
            {
                services.AddSwaggerGen(swaggerOptions =>
                {
                    swaggerOptions.SwaggerDoc("v1", new OpenApiInfo { Title = options.SwaggerTitle, Version = "1.0" });
                    swaggerOptions.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                    {
                        Description = $"JWT Authorization header using the Bearer scheme. \r\n\r\nEnter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: 'Bearer 12345abcdef'",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                    });
                    swaggerOptions.OperationFilter<SwaggerSecurityScheme>();

                    /**
                     * 修改项目文件 .csproj
                     * 生成XML注释文件，以便Swagger可以读取
                     * <PropertyGroup>
                     *     <GenerateDocumentationFile>true</GenerateDocumentationFile>
                     *     <NoWarn>$(NoWarn);1591</NoWarn>
                     * </PropertyGroup>
                     * 
                     * 配置 Swagger 注释路径
                     * 
                     * ⚠️ 收敛时必须改写的两点（原 Program.cs 中的写法已不适用于本文件）：
                     *   1) 不能用 Assembly.GetExecutingAssembly()：它返回“包含本段代码的程序集”，
                     *      而本方法位于 AGooday.AgPay.Base.Api，于是会拼出不存在的
                     *      AGooday.AgPay.Base.Api.xml，导致 Swashbuckle 抛出
                     *      “XML comments file could not be found”并中断启动。
                     *      必须改用本方法已解析好的宿主程序集 applicationAssembly
                     *      （即各 API 传入的 typeof(Program).Assembly）。
                     *   2) AppContext.BaseDirectory 可以原样保留：它始终指向“正在运行的应用”
                     *      的基础目录，与代码写在哪个程序集无关。
                     *
                     * var xmlFilename = $"{applicationAssembly.GetName().Name}.xml";
                     * swaggerOptions.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                     *
                     * 注：启用前需先在 4 个 API 的 .csproj 打开 GenerateDocumentationFile
                     *     （当前仓库中未启用，各 API 输出目录下也没有任何 .xml）。
                     * **/

                    //注册全局认证（所有的接口都可以使用认证）
                    //swaggerOptions.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    //{
                    //    {
                    //        new OpenApiSecurityScheme
                    //        {
                    //            Reference = new OpenApiReference
                    //            {
                    //                Type = ReferenceType.SecurityScheme,
                    //                Id = JwtBearerDefaults.AuthenticationScheme
                    //            },
                    //            Scheme = "oauth2",
                    //            Name = JwtBearerDefaults.AuthenticationScheme,
                    //            In = ParameterLocation.Header,
                    //        },
                    //        new List<string>()
                    //    }
                    //});
                });
            }
            else
            {
                services.AddSwaggerGen();
            }

            if (options.UseScalarOpenApi)
            {
                // .NET 9 内置 OpenAPI — 供 Scalar 使用
                services.AddOpenApi();
            }
            #endregion

            #region MediatR / 原生依赖注入
            // Adding MediatR for Domain Events
            // 领域命令、领域事件等注入
            // 引用包 MediatR.Extensions.Microsoft.DependencyInjection
            //services.AddMediatR(typeof(MyxxxHandler));//单单注入某一个处理程序
            //或
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));//目的是为了扫描Handler的实现对象并添加到IOC的容器中

            // .NET Core 原生依赖注入
            // 单写一层用来添加依赖项，从展示层 Presentation 中隔离
            NativeInjectorBootStrapper.RegisterServices(services);
            #endregion

            #region 渠道上下文
            if (options.UseChannelConfigContext)
            {
                // 以下为收敛前 Payment.Api / Manager.Api 中保留的备选实现（服务定位器写法已被注释，勿再启用）。
                // ⚠️ 这些类型的命名空间为 AGooday.AgPay.Application.Interfaces，本文件未 using；
                //    若确要启用其中某一行，需先补上该 using。
                //var provider = services.BuildServiceProvider();
                //var mchAppService = (IMchAppService)provider.GetService(typeof(IMchAppService));
                //var mchInfoService = (IMchInfoService)provider.GetService(typeof(IMchInfoService));
                //var isvInfoService = (IIsvInfoService)provider.GetService(typeof(IIsvInfoService));
                //var payInterfaceConfigService = (IPayInterfaceConfigService)provider.GetService(typeof(IPayInterfaceConfigService));
                //services.AddSingleton(new ConfigContextService(mchAppService, mchInfoService, isvInfoService, payInterfaceConfigService));
                //services.AddScoped<ConfigContextService>(provider =>
                //{
                //    var mchStoreService = (IMchStoreService)provider.GetService(typeof(IMchStoreService));
                //    var mchAppService = (IMchAppService)provider.GetService(typeof(IMchAppService));
                //    var mchInfoService = (IMchInfoService)provider.GetService(typeof(IMchInfoService));
                //    var agentInfoService = (IAgentInfoService)provider.GetService(typeof(IAgentInfoService));
                //    var isvInfoService = (IIsvInfoService)provider.GetService(typeof(IIsvInfoService));
                //    var payInterfaceConfigService = (IPayInterfaceConfigService)provider.GetService(typeof(IPayInterfaceConfigService));
                //    return new ConfigContextService(mchStoreService, mchAppService, mchInfoService, agentInfoService, isvInfoService, payInterfaceConfigService);
                //});
                //services.AddSingleton(typeof(ConfigContextService));
                //services.AddSingleton(typeof(ConfigContextQueryService));
                services.AddSingleton<ConfigContextService>();
                services.AddScoped<ConfigContextQueryService>();
                ChannelNativeInjectorBootStrapper.RegisterServices(services);
            }
            #endregion

            #region 分布式 ID
            if (options.InitializeIdWorker)
            {
                // 从配置中读取 IdWorkerConfig
                var idWorkerConfig = configuration.GetSection("IdWorkerConfig");
                bool isUseSnowflakeId = idWorkerConfig.GetValue<bool>("IsUseSnowflakeId", false);
                long dataCenterId = idWorkerConfig.GetValue<long>("DataCenterId", 0);
                long machineId = idWorkerConfig.GetValue<long>("MachineId", 0);
                if (isUseSnowflakeId)
                {
                    // 初始化 IdWorker
                    //var idWorker = new IdWorker(dataCenterId, machineId);
                    //typeof(IdWorker).GetField("lazy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                    //    .SetValue(null, new Lazy<IdWorker>(() => idWorker));
                    IdWorker.Initialize(dataCenterId, machineId);
                }
                // 初始化 SeqUtil
                SeqUtil.Initialize(isUseSnowflakeId);
            }
            #endregion

            // 绑定配置
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            // 注册统一健康检查（Redis + MQ），并按需追加 Quartz
            // 注：原 Manager/Merchant 的注释为「注册统一健康检查」，Agent/Payment 为「注册健康检查」，此处措辞统一
            var healthChecks = services.AddBaseHealthChecks();
            if (options.EnableQuartzHealthCheck)
            {
                healthChecks.AddQuartzHealthCheck();
            }

            return builder;
        }

        /// <summary>
        /// 装配共享中间件管线。顺序与收敛前保持一致。
        /// </summary>
        public static WebApplication UseAgPayApiHost(this WebApplication app, AgPayApiHostOptions options)
        {
            ArgumentNullException.ThrowIfNull(app);
            ArgumentNullException.ThrowIfNull(options);

            // 读取配置
            var appSettings = app.Configuration.GetSection("AppSettings").Get<AppSettings>();

            // 加入 WebSocket 功能
            if (options.UseWebSockets)
            {
                app.UseWebSockets(new WebSocketOptions
                {
                    KeepAliveInterval = TimeSpan.FromSeconds(30)
                });
            }

            // 自定义中间件
            app.UseAgSerilogRequestContext();
            app.UseCalculateExecutionTime();
            app.UseRequestResponseLogging();

            // Swagger 文档（开发环境下）
            //if (app.Environment.IsDevelopment())
            //{
            // 说明：原代码就把 IsDevelopment 判断注释掉了，因此 Swagger 在生产环境同样暴露 —— 这是保留的现状，不是遗漏。
            // 如需改为仅开发环境启用，取消下一行与本节末尾 "//}" 的注释即可。
            app.UseSwagger();
            app.UseSwaggerUI();

            if (options.UseScalarOpenApi)
            {
                app.MapOpenApi();
                app.MapScalarApiReference();

                //dotnet add package IGeekFan.AspNetCore.Knife4jUI
                //app.UseKnife4UI(c =>
                //{
                //    c.RoutePrefix = ""; // serve the UI at root --knife4j
                //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
                //});
            }
            //}

            // 根据配置决定是否启用 HTTPS 重定向
            if (appSettings.ForceHttpsRedirection)
            {
                // 强制 HTTPS 重定向
                app.UseHttpsRedirection();
            }

            // 静态文件服务
            app.UseStaticFiles();

            // 启用 CORS 中间件
            app.UseCors("CorsPolicy");

            if (options.UseJwtAuthentication)
            {
                // 认证中间件（检测用户是否登录）
                app.UseAuthentication();

                // 配置 HttpContext 访问器
                var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
                AuthContextService.Configure(httpContextAccessor);
            }

            // 授权中间件（检测用户是否有权限访问资源）
            app.UseAuthorization();

            // 异常处理中间件（放在路由中间件之前）
            app.UseExceptionHandling();

            // 路由映射
            if (options.MapCashierFallback)
            {
                app.UseRouting().UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();

                    // 配置默认路由，将所有非 API 路由重定向到 Vue 单页面应用程序的入口页
                    endpoints.MapFallbackToFile("/cashier/index.html");

                    //// 配置 Vue 单页面应用程序的路由
                    //endpoints.MapGet("/", async context =>
                    //{
                    //    context.Response.Redirect("/cashier/index.html");
                    //});
                });
            }
            else
            {
                app.MapControllers();
            }

            // 映射标准健康检查端点
            app.MapStandardHealthChecks();

            return app;
        }

        private static void ApplyMvcOptions(MvcOptions mvcOptions, AgPayApiHostOptions options)
        {
            if (options.OperationLogHandlerType != null)
            {
                ////添加全局异常过滤器
                //mvcOptions.Filters.Add<GlobalExceptionsFilter>();
                //日志过滤器
                mvcOptions.Filters.Add<OpLogActionFilter>();
            }

            // 全局添加自定义模型验证过滤器（Payment.Api 的 ValidateModelAttribute）
            options.ConfigureMvcOptions?.Invoke(mvcOptions);
        }

        /// <summary>
        /// 把共享配置文件插入到配置源链的【最前面】，使其成为最低优先级的默认值来源。
        /// </summary>
        /// <remarks>
        /// ⚠️ 必须用 <c>Sources.Insert</c>，不能用 <c>builder.Configuration.AddJsonFile(...)</c>。
        /// <para>
        /// .NET 配置的规则是<b>后加载的提供程序覆盖先加载的</b>。<see cref="WebApplicationBuilder"/> 在构造时
        /// 已经按 appsettings.json → appsettings.{Environment}.json → 环境变量 → 命令行的顺序建好了源链；
        /// 若此处只是 <c>AddJsonFile</c> 追加，共享文件会排到<b>最后</b>，从而反过来覆盖
        /// docker-compose 注入的环境变量（<c>ConnectionStrings__Default</c>、<c>JWT__Secret</c>、
        /// <c>Redis__Default__Connection</c>、<c>MQ__RabbitMQ__*</c> 等），导致生产静默连到错误的数据库/中间件。
        /// </para>
        /// <para>
        /// 插入到最前面后，优先级为（低 → 高）：
        /// appsettings.Shared.json ＜ appsettings.Shared.{Environment}.json ＜ appsettings.json
        /// ＜ appsettings.{Environment}.json ＜ 环境变量 ＜ 命令行。现有覆盖链完全不变。
        /// </para>
        /// </remarks>
        private static void AddSharedJsonConfiguration(WebApplicationBuilder builder)
        {
            // 共享配置文件会链接到 API 输出目录；使用 dotnet run 时，ContentRoot 仍可能指向项目目录。
            var fileProvider = new PhysicalFileProvider(AppContext.BaseDirectory);
            var sources = ((IConfigurationBuilder)builder.Configuration).Sources;

            // 先插入基础共享文件，再插入环境共享文件：
            // 后者索引更大 => 在源链中更靠后 => 优先级更高，正好实现 Shared.{Env} 覆盖 Shared。
            var index = 0;
            index = InsertIfExists(sources, fileProvider, "appsettings.Shared.json", index, optional: false);
            InsertIfExists(sources, fileProvider, $"appsettings.Shared.{builder.Environment.EnvironmentName}.json", index, optional: true);
        }

        private static int InsertIfExists(IList<IConfigurationSource> sources, IFileProvider fileProvider, string fileName, int index, bool optional)
        {
            if (optional && !fileProvider.GetFileInfo(fileName).Exists)
            {
                // 该环境没有对应的共享文件（例如 Production 无 appsettings.Shared.Production.json）时静默跳过
                return index;
            }

            sources.Insert(index, new JsonConfigurationSource
            {
                FileProvider = fileProvider,
                Path = fileName,
                Optional = optional,
                ReloadOnChange = false
            });

            return index + 1;
        }
    }
}
