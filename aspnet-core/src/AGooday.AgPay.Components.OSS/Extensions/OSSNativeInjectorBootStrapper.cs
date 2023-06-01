using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.OSS.Constants;
using AGooday.AgPay.Components.OSS.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AGooday.AgPay.Components.OSS.Extensions
{
    public class OSSNativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<LocalFileService>();
            services.AddScoped<AliyunOssService>();
            services.AddScoped<IOssServiceFactory, OssServiceFactory>();
        }
    }

    public interface IOssServiceFactory
    {
        IOssService GetService();
        string GetOssUseType();
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
            var ossUseType = GetOssUseType();
            switch (ossUseType)
            {
                case OssUseTypeCS.LOCAL_FILE:
                    return _serviceProvider.GetService<LocalFileService>();
                case OssUseTypeCS.ALIYUN_OSS:
                    return _serviceProvider.GetService<AliyunOssService>();
                default:
                    throw new Exception($"Invalid service: {ossUseType}");
            }
        }

        public string GetOssUseType()
        {
            return _sysConfigService.GetDBOssConfig().OssUseType;
        }
    }
}
