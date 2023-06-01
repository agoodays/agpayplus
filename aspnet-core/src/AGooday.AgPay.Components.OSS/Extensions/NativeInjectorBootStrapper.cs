using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.OSS.Constants;
using AGooday.AgPay.Components.OSS.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AGooday.AgPay.Components.OSS.Extensions
{
    public class NativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IOssService, AliyunOssService>();
            services.AddScoped<IOssService, LocalFileService>(); 
            services.AddScoped<IOssServiceFactory, OssServiceFactory>();
        }
    }

    public interface IOssServiceFactory
    {
        IOssService GetService();
    }

    public class OssServiceFactory : IOssServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISysConfigService _sysConfigService;

        public OssServiceFactory(IServiceProvider serviceProvider, ISysConfigService sysConfigService)
        {
            _serviceProvider = serviceProvider;
            _sysConfigService = sysConfigService;
        }

        public IOssService GetService()
        {
            var ossUseType = _sysConfigService.GetDBOssConfig().OssUseType;
            switch (ossUseType)
            {
                case OssUseType.LOCAL_FILE:
                    return _serviceProvider.GetService<AliyunOssService>();
                case OssUseType.ALIYUN_OSS:
                    return _serviceProvider.GetService<LocalFileService>();
                default:
                    throw new Exception($"Invalid service: {ossUseType}");
            }
        }
    }
}
