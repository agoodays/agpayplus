using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.OSS.Config;
using AGooday.AgPay.Components.OSS.Constants;
using Aliyun.OSS;
using Aliyun.OSS.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AGooday.AgPay.Components.OSS.Services
{
    public class AliyunOssService : IOssService
    {
        private readonly ISysConfigService sysConfigService;
        private readonly ILogger<AliyunOssService> logger;
        // ossClient 初始化
        private OssClient ossClient = null;

        public AliyunOssService(ILogger<AliyunOssService> logger, ISysConfigService sysConfigService)
        {
            this.logger = logger;
            this.sysConfigService = sysConfigService;
            AliyunOssConfig.Oss = JsonConvert.DeserializeObject<AliyunOssConfig.OssConfig>(sysConfigService.GetDBOssConfig().AliyunOssConfig);
            ossClient = new OssClient(AliyunOssConfig.Oss.Endpoint, AliyunOssConfig.Oss.AccessKeyId, AliyunOssConfig.Oss.AccessKeySecret);
        }

        public Task<string> Upload2PreviewUrlAsync(OssSavePlaceEnum ossSavePlaceEnum, IFormFile multipartFile, string saveDirAndFileName)
        {
            try
            {
                if (multipartFile.Length > 0)
                {
                    ossClient.PutObject(ossSavePlaceEnum == OssSavePlaceEnum.PUBLIC ? AliyunOssConfig.Oss.PublicBucketName : AliyunOssConfig.Oss.PrivateBucketName
                        , saveDirAndFileName, multipartFile.OpenReadStream());

                    if (ossSavePlaceEnum == OssSavePlaceEnum.PUBLIC)
                    {
                        // 文档：https://help.aliyun.com/document_detail/32084.html，https://www.alibabacloud.com/help/zh/doc-detail/39607.htm
                        // example: https://BucketName.Endpoint/ObjectName
                        return Task.FromResult($"https://{AliyunOssConfig.Oss.PublicBucketName}.{AliyunOssConfig.Oss.Endpoint}/{saveDirAndFileName}");
                    }

                    return Task.FromResult(saveDirAndFileName);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, e);
            }
            return Task.FromResult<string>(null);
        }

        public bool DownloadFile(OssSavePlaceEnum ossSavePlaceEnum, string source, string target)
        {
            try
            {
                string bucket = ossSavePlaceEnum == OssSavePlaceEnum.PRIVATE ? AliyunOssConfig.Oss.PrivateBucketName : AliyunOssConfig.Oss.PublicBucketName;
                //this.ossClient.GetObject(new GetObjectRequest(bucket, source),File.Open(target, FileMode.OpenOrCreate));
                var ossObject = ossClient.GetObject(new GetObjectRequest(bucket, source));
                WriteToFile(target, ossObject.Content);

                return true;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, e);
                return false;
            }
        }

        private void WriteToFile(string filePath, Stream stream)
        {
            using (var requestStream = stream)
            {
                using (var fs = File.Open(filePath, FileMode.OpenOrCreate))
                {
                    IoUtils.WriteTo(stream, fs);
                }
            }
        }
    }
}
