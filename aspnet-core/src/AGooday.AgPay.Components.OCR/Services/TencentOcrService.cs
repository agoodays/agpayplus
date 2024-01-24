using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.OCR.Models;
using Microsoft.Extensions.Logging;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Ocr.V20181119;
using TencentCloud.Ocr.V20181119.Models;

namespace AGooday.AgPay.Components.OCR.Services
{
    /// <summary>
    /// API 概览：https://cloud.tencent.com/document/api/866/33515
    /// </summary>
    public class TencentOcrService : IOcrService
    {
        private readonly ILogger<TencentOcrService> logger;
        private readonly TencentOcrConfig ocrConfig;

        public TencentOcrService(ILogger<TencentOcrService> logger, ISysConfigService sysConfigService)
        {
            this.logger = logger;
            var dbSmsConfig = sysConfigService.GetDBOcrConfig();
            ocrConfig = (TencentOcrConfig)AbstractOcrConfig.GetOcrConfig(dbSmsConfig.OcrType, dbSmsConfig.TencentOcrConfig);
        }

        public Task<Dictionary<string, string>> RecognizeTextAsync(string imagePath, string type)
        {
            try
            {
                // 设置腾讯云API访问密钥
                Credential cred = new Credential()
                {
                    SecretId = ocrConfig.SecretId,
                    SecretKey = ocrConfig.SecretKey
                };

                // 实例化OCR客户端
                ClientProfile clientProfile = new ClientProfile();
                HttpProfile httpProfile = new HttpProfile();
                httpProfile.Endpoint = "ocr.tencentcloudapi.com"; // OCR服务的API地址
                clientProfile.HttpProfile = httpProfile;
                OcrClient client = new OcrClient(cred, "ap-guangzhou", clientProfile);

                // 处理识别结果
                Dictionary<string, string> map = new Dictionary<string, string>();

                if (type.Equals("GeneralBasic"))
                {
                    // 构造请求对象
                    GeneralBasicOCRRequest req = new GeneralBasicOCRRequest();
                    req.ImageUrl = imagePath; // 要识别的身份证图片URL

                    // 发送请求并获取识别结果
                    GeneralBasicOCRResponse resp = client.GeneralBasicOCRSync(req);
                    resp.ToMap(map, type);
                }

                if (type.Equals("GeneralAccurate"))
                {
                    // 构造请求对象
                    GeneralAccurateOCRRequest req = new GeneralAccurateOCRRequest();
                    req.ImageUrl = imagePath; // 要识别的身份证图片URL

                    // 发送请求并获取识别结果
                    GeneralAccurateOCRResponse resp = client.GeneralAccurateOCRSync(req);
                    resp.ToMap(map, type);
                }

                if (type.Equals("GeneralHandwriting"))
                {
                    // 构造请求对象
                    GeneralHandwritingOCRRequest req = new GeneralHandwritingOCRRequest();
                    req.ImageUrl = imagePath; // 要识别的身份证图片URL

                    // 发送请求并获取识别结果
                    GeneralHandwritingOCRResponse resp = client.GeneralHandwritingOCRSync(req);
                    resp.ToMap(map, type);
                }

                return Task.FromResult(map);
            }
            catch (Exception ex)
            {
                // 处理异常
                logger.LogError(ex, $"Ocr异常");
                throw;
            }
        }

        public Task<Dictionary<string, string>> RecognizeCardTextAsync(string imagePath, string type)
        {
            try
            {
                // 设置腾讯云API访问密钥
                Credential cred = new Credential()
                {
                    SecretId = ocrConfig.SecretId,
                    SecretKey = ocrConfig.SecretKey
                };

                // 实例化OCR客户端
                ClientProfile clientProfile = new ClientProfile();
                HttpProfile httpProfile = new HttpProfile();
                httpProfile.Endpoint = "ocr.tencentcloudapi.com"; // OCR服务的API地址
                clientProfile.HttpProfile = httpProfile;
                OcrClient client = new OcrClient(cred, "ap-guangzhou", clientProfile);

                // 处理识别结果
                Dictionary<string, string> map = new Dictionary<string, string>();

                if (type.Equals("IdCard"))
                {
                    // 构造请求对象
                    IDCardOCRRequest req = new IDCardOCRRequest();
                    req.ImageUrl = imagePath; // 要识别的身份证图片URL

                    // 发送请求并获取识别结果
                    IDCardOCRResponse resp = client.IDCardOCRSync(req);
                    resp.ToMap(map, type);
                }

                if (type.Equals("BankCard"))
                {
                    // 构造请求对象
                    BankCardOCRRequest req = new BankCardOCRRequest();
                    req.ImageUrl = imagePath; // 要识别的身份证图片URL

                    // 发送请求并获取识别结果
                    BankCardOCRResponse resp = client.BankCardOCRSync(req);
                    resp.ToMap(map, type);
                }

                if (type.Equals("BizLicense"))
                {
                    // 构造请求对象
                    BizLicenseOCRRequest req = new BizLicenseOCRRequest();
                    req.ImageUrl = imagePath; // 要识别的身份证图片URL

                    // 发送请求并获取识别结果
                    BizLicenseOCRResponse resp = client.BizLicenseOCRSync(req);
                    resp.ToMap(map, type);
                }

                return Task.FromResult(map);
            }
            catch (Exception ex)
            {
                // 处理异常
                logger.LogError(ex, $"Ocr异常");
                throw;
            }
        }
    }
}
