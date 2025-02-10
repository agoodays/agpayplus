using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Cache.Extensions;
using AGooday.AgPay.Components.Cache.Options;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ;
using AGooday.AgPay.Components.MQ.Vender.RabbitMQ.Receive;
using AGooday.AgPay.Components.OSS.Config;
using AGooday.AgPay.Components.OSS.Extensions;
using AGooday.AgPay.Components.SMS.Extensions;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using AGooday.AgPay.Payment.Api.Extensions;
using AGooday.AgPay.Payment.Api.FilterAttributes;
using AGooday.AgPay.Payment.Api.Jobs;
using AGooday.AgPay.Payment.Api.Middlewares;
using AGooday.AgPay.Payment.Api.MQ;
using AGooday.AgPay.Payment.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

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

//// ע����־
//services.AddLogging(config =>
//{
//    //Microsoft.Extensions.Logging.Log4Net.AspNetCore
//    config.AddLog4Net();
//});
services.AddSingleton<ILoggerProvider, Log4NetLoggerProvider>();

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

services.AddMemoryCache();

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

services.AddControllersWithViews()
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
services.AddSwaggerGen();

// Adding MediatR for Domain Events
// ������������¼���ע��
// ���ð� MediatR.Extensions.Microsoft.DependencyInjection
//services.AddMediatR(typeof(MyxxxHandler));//����ע��ĳһ���������
//��
services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());//Ŀ����Ϊ��ɨ��Handler��ʵ�ֶ�����ӵ�IOC��������

services.Configure<ApiBehaviorOptions>(options =>
{
    // ����Ĭ��ģ����֤������
    options.SuppressModelStateInvalidFilter = true;
});
services.Configure<MvcOptions>(options =>
{
    // ȫ������Զ���ģ����֤������
    options.Filters.Add<ValidateModelAttribute>();
});

services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//services.AddSingleton<RequestIpUtil>();
services.AddSingleton<RequestKit>();

// .NET Core ԭ������ע��
// ��дһ����������������չʾ�� Presentation �и���
NativeInjectorBootStrapper.RegisterServices(services);

services.AddNotice(builder.Configuration);

#region RabbitMQ
services.AddTransient<RabbitMQSender>();
services.AddSingleton<IMQSender>(provider =>
{
    var mqSenderFactory = new MQSenderFactory(builder.Configuration, provider);
    return mqSenderFactory.CreateSender();
});
services.AddSingleton<IMQMsgReceiver, PayOrderDivisionRabbitMQReceiver>();
services.AddSingleton<IMQMsgReceiver, PayOrderMchNotifyRabbitMQReceiver>();
services.AddSingleton<IMQMsgReceiver, PayOrderReissueRabbitMQReceiver>();
services.AddSingleton<IMQMsgReceiver, ResetAppConfigRabbitMQReceiver>();
services.AddSingleton<IMQMsgReceiver, ResetIsvAgentMchAppInfoRabbitMQReceiver>();
services.AddSingleton<PayOrderDivisionMQ.IMQReceiver, PayOrderDivisionMQReceiver>();
services.AddSingleton<PayOrderMchNotifyMQ.IMQReceiver, PayOrderMchNotifyMQReceiver>();
services.AddSingleton<PayOrderReissueMQ.IMQReceiver, PayOrderReissueMQReceiver>();
services.AddSingleton<ResetAppConfigMQ.IMQReceiver, ResetAppConfigMQReceiver>();
services.AddSingleton<ResetIsvAgentMchAppInfoConfigMQ.IMQReceiver, ResetIsvAgentMchAppInfoMQReceiver>();
services.AddHostedService<MQReceiverHostedService>();
#endregion

#region Quartz
/*
 ��˵һ��Cron���ʽ�ɣ�

 ��7�ι��ɣ��� �� ʱ �� �� ���� �꣨��ѡ��

 "-" ����ʾ��Χ  MON-WED��ʾ����һ��������
 "," ����ʾ�о� MON,WEB��ʾ����һ��������
 "*" �����ǡ�ÿ����ÿ�£�ÿ�죬ÿ�ܣ�ÿ���
 "/" :��ʾ������0/15�����ڷ��Ӷ����棩 ÿ15���ӣ���0���Ժ�ʼ��3/20 ÿ20���ӣ���3�����Ժ�ʼ
 "?" :ֻ�ܳ������գ����ڶ����棬��ʾ��ָ�������ֵ
 "L" :ֻ�ܳ������գ����ڶ����棬��Last����д��һ���µ����һ�죬һ�����ڵ����һ�죨��������
 "W" :��ʾ�����գ��������ֵ����Ĺ�����
 "#" :��ʾһ���µĵڼ������ڼ������磺"6#3"��ʾÿ���µĵ����������壨1=SUN...6=FRI,7=SAT��

 ���Minutes����ֵ�� '0/15' ����ʾ��0��ʼÿ15����ִ��

 ���Minutes����ֵ�� '3/20' ����ʾ��3��ʼÿ20����ִ�У�Ҳ���ǡ�3/23/43��
*/
// https://andrewlock.net/creating-a-quartz-net-hosted-service-with-asp-net-core/
// ���Quartz����
services.AddSingleton<IJobFactory, SingletonJobFactory>();
services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

// �������
services.AddSingleton<PayOrderExpiredJob>();
services.AddSingleton<PayOrderReissueJob>();
services.AddSingleton<RefundOrderExpiredJob>();
services.AddSingleton<RefundOrderReissueJob>();
services.AddSingleton<TransferOrderReissueJob>();
services.AddSingleton(new JobSchedule(
    jobType: typeof(PayOrderExpiredJob),
    cronExpression: "0 0/1 * * * ?")); // ÿ����ִ��һ��
services.AddSingleton(new JobSchedule(
    jobType: typeof(PayOrderReissueJob),
    cronExpression: "0 0/1 * * * ?")); // ÿ����ִ��һ��
services.AddSingleton(new JobSchedule(
    jobType: typeof(RefundOrderExpiredJob),
    cronExpression: "0 0/1 * * * ?")); // ÿ����ִ��һ��
services.AddSingleton(new JobSchedule(
    jobType: typeof(RefundOrderReissueJob),
    cronExpression: "0 0/1 * * * ?")); // ÿ����ִ��һ��
services.AddSingleton(new JobSchedule(
    jobType: typeof(TransferOrderReissueJob),
    cronExpression: "0 0/1 * * * ?")); // ÿ����ִ��һ��

services.AddHostedService<QuartzHostedService>();
#endregion

#region OSS
OSSNativeInjectorBootStrapper.RegisterServices(services);
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

#region SMS
SMSNativeInjectorBootStrapper.RegisterServices(services);
#endregion

services.AddSingleton<ChannelCertConfigKit>(serviceProvider =>
{
    return new ChannelCertConfigKit(serviceProvider);
});

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
services.AddScoped<ChannelOrderReissueService>();
services.AddScoped<PayMchNotifyService>();
services.AddScoped<PayOrderDivisionProcessService>();
services.AddScoped<PayOrderProcessService>();
services.AddScoped<RefundOrderProcessService>();

services.AddSingleton<IQRCodeService, QRCodeService>();

ChannelNativeInjectorBootStrapper.RegisterServices(services);

AgPayUtil.AES_KEY = builder.Configuration["AesKey"];
AgPayUtil.RSA2_PRIVATE_KEY = builder.Configuration["SysRSA2:PrivateKey"];

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

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseNdc();

app.UseCalculateExecutionTime();

app.UseRequestResponseLogging();

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}
// ǿ��HTTPS���ã����ڽ�HTTP�����ض���HTTPS
app.UseHttpsRedirection();
app.UseStaticFiles();

// ���� CORS �м��
app.UseCors("CorsPolicy");

// ��Ȩ �����û��Ȩ�޷��ʺ���ҳ��
app.UseAuthorization();

app.UseExceptionHandling();

app.UseRouting().UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

    // ����Ĭ��·�ɣ������з� API ·���ض��� Vue ��ҳ��Ӧ�ó�������ҳ
    endpoints.MapFallbackToFile("/cashier/index.html");

    //// ���� Vue ��ҳ��Ӧ�ó����·��
    //endpoints.MapGet("/", async context =>
    //{
    //    context.Response.Redirect("/cashier/index.html");
    //});
});

app.Run();
