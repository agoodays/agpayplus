using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.OSS.Config;
using AGooday.AgPay.Components.OSS.Constants;
using Aliyun.OSS;
using Aliyun.OSS.Util;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
            this.ossClient = new OssClient(AliyunOssConfig.Oss.Endpoint, AliyunOssConfig.Oss.AccessKeyId, AliyunOssConfig.Oss.AccessKeySecret);
        }

        public async Task<string> Upload2PreviewUrl(OssSavePlaceEnum ossSavePlaceEnum, IFormFile multipartFile, string saveDirAndFileName)
        {
            try
            {
                if (multipartFile.Length > 0)
                {
                    this.ossClient.PutObject(ossSavePlaceEnum == OssSavePlaceEnum.PUBLIC ? AliyunOssConfig.Oss.PublicBucketName : AliyunOssConfig.Oss.PrivateBucketName
                        , saveDirAndFileName, multipartFile.OpenReadStream());

                    if (ossSavePlaceEnum == OssSavePlaceEnum.PUBLIC)
                    {
                        // 文档：https://help.aliyun.com/document_detail/32084.html，https://www.alibabacloud.com/help/zh/doc-detail/39607.htm
                        // example: https://BucketName.Endpoint/ObjectName
                        return $"https://{AliyunOssConfig.Oss.PublicBucketName}.{AliyunOssConfig.Oss.Endpoint}/{saveDirAndFileName}";
                    }

                    return saveDirAndFileName;
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, e);
            }
            return null;
        }

        public bool DownloadFile(OssSavePlaceEnum ossSavePlaceEnum, string source, string target)
        {
            try
            {
                string bucket = ossSavePlaceEnum == OssSavePlaceEnum.PRIVATE ? AliyunOssConfig.Oss.PrivateBucketName : AliyunOssConfig.Oss.PublicBucketName;
                //this.ossClient.GetObject(new GetObjectRequest(bucket, source),File.Open(target, FileMode.OpenOrCreate));
                var ossObject = this.ossClient.GetObject(new GetObjectRequest(bucket, source));
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
