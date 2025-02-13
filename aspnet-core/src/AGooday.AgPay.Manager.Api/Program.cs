using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Cache.Extensions;
using AGooday.AgPay.Components.Cache.Options;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ.Receive;
using AGooday.AgPay.Components.OCR.Controllers;
using AGooday.AgPay.Components.OCR.Extensions;
using AGooday.AgPay.Components.OSS.Config;
using AGooday.AgPay.Components.OSS.Controllers;
using AGooday.AgPay.Components.OSS.Extensions;
using AGooday.AgPay.Components.SMS.Extensions;
using AGooday.AgPay.Manager.Api.Authorization;
using AGooday.AgPay.Manager.Api.Extensions;
using AGooday.AgPay.Manager.Api.Extensions.AuthContext;
using AGooday.AgPay.Manager.Api.Filter;
using AGooday.AgPay.Manager.Api.Middlewares;
using AGooday.AgPay.Manager.Api.Models;
using AGooday.AgPay.Manager.Api.MQ;
using AGooday.AgPay.Manager.Api.OpLog;
using AGooday.AgPay.Manager.Api.WebSockets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var logging = builder.Logging;
// ���� ClearProviders �Դ���������ɾ������ ILoggerProvider ʵ��
logging.ClearProviders();
//// ͨ������־����Ӧ��������ָ�����������ڴ�����ָ����
//logging.AddFilter("Microsoft", LogLevel.Warning);
// ��ӿ���̨��־��¼�ṩ����
logging.AddConsole();

// Add services to the container.
var services = builder.Services;
var Env = builder.Environment;

services.AddSingleton(new Appsettings(Env.ContentRootPath));

//�û���Ϣ
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//// ע����־
//services.AddLogging(config =>
//{
//    //Microsoft.Extensions.Logging.Log4Net.AspNetCore
//    config.AddLog4Net();
//});
services.AddSingleton<ILoggerProvider, Log4NetLoggerProvider>();

services.AddScoped<IOpLogHandler, OpLogHandler>();

#region Redis
var redisSettingsSection = builder.Configuration.GetSection("Redis:Default");
var redisOptions = redisSettingsSection.Get<RedisOptions>();

//services.AddSingleton(new RedisUtil(redisOptions.Connection, redisOptions.InstanceName, redisOptions.DefaultDB));

CacheNativeInjectorBootStrapper.RegisterServices(services, redisOptions);
#endregion

#region OSS
builder.Configuration.GetSection("OSS").Bind(LocalOssConfig.Oss);
builder.Configuration.GetSection("OSS:AliyunOss").Bind(AliyunOssConfig.Oss);
#endregion

#region CORS
// �� appsettings.json �ж�ȡ CORS ����
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
// ��� CORS ����
services.AddCors(o =>
    o.AddPolicy("CorsPolicy",
        builder => builder
            .WithOrigins(allowedOrigins)
            //.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    ));
#endregion

services.AddMemoryCache();
services.AddHttpContextAccessor();
services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
var jwtSettingsSection = builder.Configuration.GetSection("JWT");
services.Configure<JwtSettings>(jwtSettingsSection);
// JWT ��֤
var appSettings = jwtSettingsSection.Get<JwtSettings>();
services.AddJwtBearerAuthentication(appSettings);

services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

var sysRSA2Section = builder.Configuration.GetSection("SysRSA2");
services.Configure<SysRSA2Config>(sysRSA2Section);

AgPayUtil.AES_KEY = builder.Configuration["AesKey"];
var sysRSA2Config = sysRSA2Section.Get<SysRSA2Config>();
AgPayUtil.RSA2_PRIVATE_KEY = sysRSA2Config.PrivateKey;

// Automapper ע��
services.AddAutoMapperSetup();

// Newtonsoft.Json ȫ������ 
JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    Formatting = Formatting.None,//��ʽ��
    DateFormatString = "yyyy-MM-dd HH:mm:ss",
    ContractResolver = new CamelCasePropertyNamesContractResolver(),
    NullValueHandling = NullValueHandling.Ignore
};

services.AddControllers(options =>
    {
        ////���ȫ���쳣������
        //options.Filters.Add<GlobalExceptionsFilter>();
        //��־������
        options.Filters.Add<OpLogActionFilter>();
    })
    .AddApplicationPart(typeof(OssFileController).Assembly)
    .AddApplicationPart(typeof(OcrController).Assembly)
    //.AddNewtonsoftJson();
    .AddNewtonsoftJson(options =>
    {
        //https://blog.poychang.net/using-newtonsoft-json-in-asp-net-core-projects/
        options.SerializerSettings.Formatting = Formatting.None;
        //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();//Json key ���ַ�Сд�����շ�תС�շ壩
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        options.SerializerSettings.Converters.Add(new BaseModelJsonConverter<BaseModel>());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "AGooday.AgPay.Manager.Api", Version = "1.0" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = $"JWT Authorization header using the Bearer scheme. \r\n\r\nEnter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
    });
    options.OperationFilter<SwaggerSecurityScheme>();
    //ע��ȫ����֤�����еĽӿڶ�����ʹ����֤��
    //options.AddSecurityRequirement(new OpenApiSecurityRequirement()
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

// Adding MediatR for Domain Events
// ������������¼���ע��
// ���ð� MediatR.Extensions.Microsoft.DependencyInjection
//services.AddMediatR(typeof(MyxxxHandler));//����ע��ĳһ���������
//��
services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());//Ŀ����Ϊ��ɨ��Handler��ʵ�ֶ�����ӵ�IOC��������

// .NET Core ԭ������ע��
// ��дһ����������������չʾ�� Presentation �и���
NativeInjectorBootStrapper.RegisterServices(services);

services.AddNotice(builder.Configuration);

#region RabbitMQ
services.AddTransient<RabbitMQSender>();
services.AddSingleton<IMQSenderFactory, MQSenderFactory>();
services.AddSingleton<IMQSender>(provider =>
{
    var factory = provider.GetRequiredService<IMQSenderFactory>();
    return factory.CreateSender();
});

// ��̬ע�� Receiver
var receiverTypes = new[]
{
    typeof(ResetAppConfigRabbitMQReceiver)
};

foreach (var type in receiverTypes)
{
    services.AddSingleton(typeof(IMQMsgReceiver), type);
}

var specificReceiverTypes = new[]
{
    (typeof(ResetAppConfigMQ.IMQReceiver), typeof(ResetAppConfigMQReceiver))
};

foreach (var (serviceType, implementationType) in specificReceiverTypes)
{
    services.AddSingleton(serviceType, implementationType);
}
//services.AddSingleton<IMQMsgReceiver, ResetAppConfigRabbitMQReceiver>();
//services.AddSingleton<ResetAppConfigMQ.IMQReceiver, ResetAppConfigMQReceiver>();
// ע�� HostedService
services.AddHostedService<MQReceiverHostedService>();
#endregion

#region OSS
OSSNativeInjectorBootStrapper.RegisterServices(services);
#endregion

#region OCR
OCRNativeInjectorBootStrapper.RegisterServices(services);
#endregion

#region SMS
SMSNativeInjectorBootStrapper.RegisterServices(services);
#endregion

// �������ж�ȡ IdWorkerConfig
var idWorkerConfig = builder.Configuration.GetSection("IdWorkerConfig");
bool isUseSnowflakeId = idWorkerConfig.GetValue<bool>("IsUseSnowflakeId", false);
long dataCenterId = idWorkerConfig.GetValue<long>("DataCenterId", 0);
long machineId = idWorkerConfig.GetValue<long>("MachineId", 0);
if (isUseSnowflakeId)
{
    // ��ʼ�� IdWorker
    //var idWorker = new IdWorker(dataCenterId, machineId);
    //typeof(IdWorker).GetField("lazy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
    //    .SetValue(null, new Lazy<IdWorker>(() => idWorker));
    IdWorker.Initialize(dataCenterId, machineId);
}
// ��ʼ�� SeqUtil
SeqUtil.Initialize(isUseSnowflakeId);

//���� WebSocket �������
services.AddSingleton<WsChannelUserIdServer>();

var app = builder.Build();

// ���� WebSocket ����
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(30)
});

// �Զ����м��
app.UseNdc();
app.UseCalculateExecutionTime();
app.UseRequestResponseLogging();

// Swagger �ĵ������������£�
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

// ǿ�� HTTPS �ض���
app.UseHttpsRedirection();

// ��̬�ļ�����
app.UseStaticFiles();

// ���� CORS �м��
app.UseCors("CorsPolicy");

// ��֤�м��������û��Ƿ��¼��
app.UseAuthentication();

// ���� HttpContext ������
var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
AuthContextService.Configure(httpContextAccessor);

// ��Ȩ�м��������û��Ƿ���Ȩ�޷�����Դ��
app.UseAuthorization();

// �쳣�����м��������·���м��֮ǰ��
app.UseExceptionHandling();

// ·��ӳ��
app.MapControllers();

app.Run();
