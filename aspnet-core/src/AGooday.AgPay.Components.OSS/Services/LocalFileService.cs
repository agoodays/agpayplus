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
        private readonly ILogger<LocalFileService> logger;

        public LocalFileService(ILogger<LocalFileService> logger, ISysConfigService sysConfigService)
        {
            this.logger = logger;
            this.sysConfigService = sysConfigService;
        }

        public async Task<string> Upload2PreviewUrl(OssSavePlaceEnum ossSavePlaceEnum, List<IFormFile> multipartFile, string saveDirAndFileName)
        {
            try
            {
                string savePath = ossSavePlaceEnum == OssSavePlaceEnum.PUBLIC ? LocalOssConfig.oss.FilePublicPath : LocalOssConfig.oss.FilePrivatePath;
                savePath = savePath.Replace("/", @"\");
                foreach (var formFile in multipartFile)
                {
                    if (formFile.Length > 0)
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), savePath, saveDirAndFileName); 

                        using (var stream = File.Create(filePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, e);
            }

            // 私有文件 不返回预览文件地址
            if (ossSavePlaceEnum == OssSavePlaceEnum.PRIVATE)
            {
                return saveDirAndFileName;
            }

            return $"{sysConfigService.GetDBApplicationConfig().OssPublicSiteUrl}/{saveDirAndFileName.Replace(@"\","/")}";
        }

        public bool DownloadFile(OssSavePlaceEnum ossSavePlaceEnum, string source, string target)
        {
            return false;
        }
    }
}
