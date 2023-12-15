using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.OSS.Config;
using AGooday.AgPay.Components.OSS.Constants;
using AGooday.AgPay.Components.OSS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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

        public Task<UploadFormParams> GetUploadFormParamsAsync(OssSavePlaceEnum ossSavePlaceEnum, string bizType, string saveDirAndFileName)
        {
            var result = new UploadFormParams();
            result.FormParams = new FormParams()
            {
                BizType = bizType
            };
            result.FormActionUrl = "LOCAL_SINGLE_FILE_URL";
            return Task.FromResult(result);
        }

        public async Task<string> Upload2PreviewUrlAsync(OssSavePlaceEnum ossSavePlaceEnum, IFormFile multipartFile, string saveDirAndFileName)
        {
            try
            {
                string savePath = ossSavePlaceEnum == OssSavePlaceEnum.PUBLIC ? LocalOssConfig.Oss.FilePublicPath : LocalOssConfig.Oss.FilePrivatePath;
                savePath = savePath.Replace("/", @"\");

                if (multipartFile.Length > 0)
                {
                    var filePath = Path.Combine(savePath, saveDirAndFileName); //Directory.GetCurrentDirectory(), 
                    //var directorys = filePath.Split(@"\");
                    //var lastdir = directorys.Last();
                    //var directoryPath = string.Join(@"\", directorys.Where(dir => dir != lastdir));

                    /** 根据文件完整路径 获取文件名 文件夹名
                     * string fileFullPath = @"D:\Temp\aa.txt";
                     * string fileName = Path.GetFileName(fileFullPath);  //aa.txt
                     * string fileNameNoExt = Path.GetFileNameWithoutExtension(fileFullPath);  //aa
                     * string fileExt = Path.GetExtension(fileFullPath);  //.txt
                     * 
                     * string filePathOnly = Path.GetDirectoryName(fileFullPath);  //D:\Temp
                     * string folderName = Path.GetFileName(filePathOnly);  //Temp
                     * 
                     **/

                    var directoryPath = Path.GetDirectoryName(filePath);

                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    using (var stream = File.Create(filePath))
                    {
                        await multipartFile.CopyToAsync(stream);
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
            }

            saveDirAndFileName = saveDirAndFileName.Replace(@"\", "/");

            // 私有文件 不返回预览文件地址
            if (ossSavePlaceEnum == OssSavePlaceEnum.PRIVATE)
            {
                return saveDirAndFileName;
            }

            return $"{sysConfigService.GetDBOssConfig().OssPublicSiteUrl}/{saveDirAndFileName}";
        }

        public bool DownloadFile(OssSavePlaceEnum ossSavePlaceEnum, string source, string target)
        {
            return false;
        }
    }
}
