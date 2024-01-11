using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.OSS.Config;
using AGooday.AgPay.Components.OSS.Constants;
using AGooday.AgPay.Components.OSS.Models;
using Aliyun.OSS;
using Aliyun.OSS.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace AGooday.AgPay.Components.OSS.Services
{
    public class AliyunOssService : IOssService
    {
        private readonly ISysConfigService sysConfigService;
        private readonly ILogger<AliyunOssService> logger;
        // ossClient 初始化
        private readonly OssClient ossClient = null;

        public AliyunOssService(ILogger<AliyunOssService> logger, ISysConfigService sysConfigService)
        {
            this.logger = logger;
            this.sysConfigService = sysConfigService;
            AliyunOssConfig.Oss = JsonConvert.DeserializeObject<AliyunOssConfig.OssConfig>(sysConfigService.GetDBOssConfig().AliyunOssConfig);
            ossClient = new OssClient(AliyunOssConfig.Oss.Endpoint, AliyunOssConfig.Oss.AccessKeyId, AliyunOssConfig.Oss.AccessKeySecret);
        }

        public Task<UploadFormParams> GetUploadFormParamsAsync(OssSavePlaceEnum ossSavePlaceEnum, string bizType, string saveDirAndFileName)
        {
            var result = new UploadFormParams();
            var expireTime = AliyunOssConfig.Oss.ExpireTime ?? 30000;
            DateTime localTime = DateTime.Now.AddSeconds(expireTime);
            DateTime utcTime = localTime.ToUniversalTime();
            string iso8601GmtTime = utcTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var postPolicy = $"{{\"expiration\":\"{iso8601GmtTime}\",\"conditions\":[[\"content-length-range\",0,5242880]]}}";
            // 对策略进行 Base64 编码
            byte[] policyBytes = Encoding.UTF8.GetBytes(postPolicy);
            string policy = Convert.ToBase64String(policyBytes);
            // 使用 HMAC-SHA1 算法计算签名值
            var signature = string.Empty;
            byte[] accessKeySecretBytes = Encoding.UTF8.GetBytes(AliyunOssConfig.Oss.AccessKeySecret);
            using (HMACSHA1 hmacSha1 = new HMACSHA1(accessKeySecretBytes))
            {
                byte[] signatureBytes = hmacSha1.ComputeHash(Encoding.UTF8.GetBytes(policy));
                signature = Convert.ToBase64String(signatureBytes);
            }
            saveDirAndFileName = saveDirAndFileName.Replace('\\', '/');
            result.FormParams = new FormParams()
            {
                OssAccessKeyId = AliyunOssConfig.Oss.AccessKeyId,
                SuccessActionStatus = 200,
                Signature = signature,
                Key = saveDirAndFileName,
                Policy = policy
            };
            result.FormActionUrl = $"https://{AliyunOssConfig.Oss.PublicBucketName}.{AliyunOssConfig.Oss.Endpoint}/";
            result.OssFileUrl = $"{result.FormActionUrl}{saveDirAndFileName}";
            return Task.FromResult(result);
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
                logger.LogError(e, e.Message);
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
                logger.LogError(e, e.Message);
                return false;
            }
        }

        private static void WriteToFile(string filePath, Stream stream)
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
