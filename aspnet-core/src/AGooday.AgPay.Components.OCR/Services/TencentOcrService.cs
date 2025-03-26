using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.OCR.Constants;
using AGooday.AgPay.Components.OCR.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Ocr.V20181119;
using TencentCloud.Ocr.V20181119.Models;

namespace AGooday.AgPay.Components.OCR.Services
{
    /// <summary>
    /// API 概览：https://cloud.tencent.com/document/api/866/33515
    /// </summary>
    public class TencentOcrService : AbstractOcrService
    {
        private readonly TencentOcrConfig ocrConfig;
        private readonly JsonSerializerSettings globalSettings;
        private readonly OcrClient client;

        public TencentOcrService(ILogger<TencentOcrService> logger, ISysConfigService sysConfigService)
            : base(logger)
        {
            var dbOcrConfig = sysConfigService.GetDBOcrConfig();
            // 获取全局默认配置
            globalSettings = JsonConvert.DefaultSettings?.Invoke() ?? new JsonSerializerSettings();
            ocrConfig = (TencentOcrConfig)AbstractOcrConfig.GetOcrConfig(dbOcrConfig.OcrType, dbOcrConfig.TencentOcrConfig);
            // 设置腾讯云API访问密钥
            Credential cred = new Credential()
            {
                SecretId = ocrConfig.SecretId,
                SecretKey = ocrConfig.SecretKey
            };

            // 实例化OCR客户端
            ClientProfile clientProfile = new ClientProfile();
            HttpProfile httpProfile = new HttpProfile();
            httpProfile.Endpoint = ocrConfig.Endpoint; // OCR服务的API地址
            clientProfile.HttpProfile = httpProfile;
            client = new OcrClient(cred, "ap-guangzhou", clientProfile);
        }

        public override Task<string> RecognizeTextAsync(string imageUrl, string type)
        {
            try
            {
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings();

                // 处理识别结果
                List<string> detectedTexts = new List<string>();

                if (type.Equals(OcrTypeCS.GENERAL_BASIC, StringComparison.OrdinalIgnoreCase))
                {
                    // 构造请求对象
                    GeneralBasicOCRRequest req = new GeneralBasicOCRRequest();
                    req.ImageUrl = imageUrl; // 要识别的图片URL

                    // 发送请求并获取识别结果
                    GeneralBasicOCRResponse resp = client.GeneralBasicOCRSync(req);
                    detectedTexts = resp.TextDetections.Select(s => s.DetectedText).ToList();
                }

                if (type.Equals(OcrTypeCS.GENERAL_ACCURATE, StringComparison.OrdinalIgnoreCase))
                {
                    // 构造请求对象
                    GeneralAccurateOCRRequest req = new GeneralAccurateOCRRequest();
                    req.ImageUrl = imageUrl; // 要识别的图片URL

                    // 发送请求并获取识别结果
                    GeneralAccurateOCRResponse resp = client.GeneralAccurateOCRSync(req);
                    detectedTexts = resp.TextDetections.Select(s => s.DetectedText).ToList();
                }

                if (type.Equals(OcrTypeCS.GENERAL_HANDWRITING, StringComparison.OrdinalIgnoreCase))
                {
                    // 构造请求对象
                    GeneralHandwritingOCRRequest req = new GeneralHandwritingOCRRequest();
                    req.ImageUrl = imageUrl; // 要识别的图片URL

                    // 发送请求并获取识别结果
                    GeneralHandwritingOCRResponse resp = client.GeneralHandwritingOCRSync(req);
                    detectedTexts = resp.TextDetections.Select(s => s.DetectedText).ToList();
                }

                return Task.FromResult(string.Join("\n", detectedTexts));
            }
            catch (Exception e)
            {
                // 处理异常
                _logger.LogError(e, "OCR异常");
                throw;
            }
            finally
            {
                JsonConvert.DefaultSettings = () => globalSettings;
            }
        }

        public override Task<CardOCRResult> RecognizeCardTextAsync(string imageUrl, string type)
        {
            try
            {
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings();

                // 处理识别结果
                CardOCRResult result = new CardOCRResult();

                if (type.Equals(OcrTypeCS.ID_CARD, StringComparison.OrdinalIgnoreCase))
                {
                    // 构造请求对象
                    IDCardOCRRequest req = new IDCardOCRRequest();
                    req.ImageUrl = imageUrl; // 要识别的身份证图片URL

                    // 发送请求并获取识别结果
                    IDCardOCRResponse resp = client.IDCardOCRSync(req);
                    result.IdCardName = ConvertEmptyStringToNull(resp.Name);
                    result.IdCardSex = ConvertEmptyStringToNull(resp.Sex);
                    result.IdCardNation = ConvertEmptyStringToNull(resp.Nation);
                    result.IdCardBirth = ConvertEmptyStringToNull(ConvertDateToFormat(resp.Birth, "yyyy/MM/dd"));
                    result.IdCardAddress = ConvertEmptyStringToNull(resp.Address);
                    result.IdCardIdNum = ConvertEmptyStringToNull(resp.IdNum);
                    result.IdCardAuthority = ConvertEmptyStringToNull(resp.Authority);
                    result.IdCardValidDate = ConvertEmptyStringToNull(resp.ValidDate);
                    var validDates = result.IdCardValidDate.Split('-');
                    var issueDate = validDates.First();
                    var expiringDate = validDates.Last();
                    result.IdCardIssueDate = ConvertDateToFormat(issueDate, "yyyy.MM.dd");
                    result.IdCardExpiringDate = ConvertDateToFormat(expiringDate, "yyyy.MM.dd");
                }

                if (type.Equals(OcrTypeCS.BANK_CARD, StringComparison.OrdinalIgnoreCase))
                {
                    // 构造请求对象
                    BankCardOCRRequest req = new BankCardOCRRequest();
                    req.ImageUrl = imageUrl; // 要识别的银行卡图片URL

                    // 发送请求并获取识别结果
                    BankCardOCRResponse resp = client.BankCardOCRSync(req);
                    result.BankCardCardNo = resp.CardNo;
                    result.BankCardBankInfo = resp.BankInfo;
                    result.BankCardValidDate = resp.ValidDate;
                    result.BankCardCardType = resp.CardType;
                }

                if (type.Equals(OcrTypeCS.BIZ_LICENSE, StringComparison.OrdinalIgnoreCase))
                {
                    // 构造请求对象
                    BizLicenseOCRRequest req = new BizLicenseOCRRequest();
                    req.ImageUrl = imageUrl; // 要识别的营业执照图片URL

                    // 发送请求并获取识别结果
                    BizLicenseOCRResponse resp = client.BizLicenseOCRSync(req);
                    result.BizLicenseRegNum = resp.RegNum;
                    result.BizLicenseName = resp.Name;
                    result.BizLicenseCapital = resp.Capital;
                    result.BizLicensePerson = resp.Person;
                    result.BizLicenseAddress = resp.Address;
                    result.BizLicenseBusiness = resp.Business;
                    result.BizLicenseType = resp.Type;
                    result.BizLicensePeriod = (resp.Period?.EndsWith("长期") ?? true) ? "长期" : ConvertDateToFormat(resp.Period, "yyyy年MM月dd日");
                    result.BizLicenseComposingForm = resp.ComposingForm;
                    result.BizLicenseRegistrationDate = ConvertDateToFormat(resp.RegistrationDate, "yyyy年MM月dd日");
                    result.BizLicenseValidFromDate = result.BizLicenseRegistrationDate;
                    result.BizLicenseValidToDate = result.BizLicensePeriod;
                }

                return Task.FromResult(result);
            }
            catch (Exception e)
            {
                // 处理异常
                _logger.LogError(e, "OCR异常");
                throw;
            }
            finally
            {
                JsonConvert.DefaultSettings = () => globalSettings;
            }
        }
    }
}
