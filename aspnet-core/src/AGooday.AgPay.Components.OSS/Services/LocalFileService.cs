using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.OSS.Config;
using AGooday.AgPay.Components.OSS.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Components.OSS.Services
{
    public class LocalFileService : IOssService
    {
        private readonly ISysConfigService sysConfigService;
        private readonly OssYmlConfig ossYmlConfig;
        private readonly ILogger<LocalFileService> logger;

        public LocalFileService(ISysConfigService sysConfigService, OssYmlConfig ossYmlConfig, ILogger<LocalFileService> logger)
        {
            this.sysConfigService = sysConfigService;
            this.ossYmlConfig = ossYmlConfig;
            this.logger = logger;
        }

        public bool DownloadFile(OssSavePlaceEnum ossSavePlaceEnum, string source, string target)
        {
            throw new NotImplementedException();
        }

        public async Task<string> Upload2PreviewUrl(OssSavePlaceEnum ossSavePlaceEnum, List<IFormFile> multipartFile, string saveDirAndFileName)
        {
            try
            {
                string savePath = ossSavePlaceEnum == OssSavePlaceEnum.PUBLIC ? ossYmlConfig.Oss.FilePublicPath : ossYmlConfig.Oss.FilePrivatePath;

                foreach (var formFile in multipartFile)
                {
                    if (formFile.Length > 0)
                    {
                        var filePath = $"{savePath}/{saveDirAndFileName}";

                        using (var stream = File.Create(filePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError("", e);
            }

            // 私有文件 不返回预览文件地址
            if (ossSavePlaceEnum == OssSavePlaceEnum.PRIVATE)
            {
                return saveDirAndFileName;
            }

            return sysConfigService.GetDBApplicationConfig().OssPublicSiteUrl + "/" + saveDirAndFileName;
        }
    }
}
