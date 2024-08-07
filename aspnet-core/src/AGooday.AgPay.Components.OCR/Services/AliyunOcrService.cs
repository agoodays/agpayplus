using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Components.OCR.Constants;
using AGooday.AgPay.Components.OCR.Models;
using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.SDK.Ocr_api20210707;
using AlibabaCloud.SDK.Ocr_api20210707.Models;
using AlibabaCloud.TeaUtil.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AGooday.AgPay.Components.OCR.Services
{
    /// <summary>
    /// API 概览：https://help.aliyun.com/document_detail/442328.html
    /// </summary>
    public class AliyunOcrService : AbstractOcrService
    {
        private readonly AliyunOcrConfig ocrConfig;
        private readonly Client client;

        public AliyunOcrService(ILogger<AliyunOcrService> logger, ISysConfigService sysConfigService)
            : base(logger)
        {
            var dbOcrConfig = sysConfigService.GetDBOcrConfig();
            ocrConfig = (AliyunOcrConfig)AbstractOcrConfig.GetOcrConfig(dbOcrConfig.OcrType, dbOcrConfig.AliOcrConfig);
            Config config = new Config
            {
                // 必填，您的 AccessKey ID
                AccessKeyId = ocrConfig.AccessKeyId,
                // 必填，您的 AccessKey Secret
                AccessKeySecret = ocrConfig.AccessKeySecret,
            };
            // Endpoint 请参考 https://api.aliyun.com/product/ocr-api
            config.Endpoint = ocrConfig.Endpoint;
            client = new Client(config);
        }

        public override Task<string> RecognizeTextAsync(string imageUrl, string type)
        {
            try
            {
                if (type.Equals(OcrTypeCS.GENERAL_BASIC, StringComparison.OrdinalIgnoreCase))
                {
                    RecognizeGeneralRequest request = new RecognizeGeneralRequest
                    {
                        Url = imageUrl,
                    };
                    // 复制代码运行请自行打印 API 的返回值
                    RecognizeGeneralResponse response = client.RecognizeGeneralWithOptions(request, new RuntimeOptions());

                    // 处理识别结果
                    var resp = JsonConvert.DeserializeObject<AliyunGeneralOCRResponse>(response.Body.Data);
                    return Task.FromResult(resp.Data.Content);
                }

                if (type.Equals(OcrTypeCS.GENERAL_ACCURATE, StringComparison.OrdinalIgnoreCase))
                {
                    RecognizeAdvancedRequest request = new RecognizeAdvancedRequest
                    {
                        Url = imageUrl,
                    };
                    // 复制代码运行请自行打印 API 的返回值
                    RecognizeAdvancedResponse response = client.RecognizeAdvancedWithOptions(request, new RuntimeOptions());

                    // 处理识别结果
                    var resp = JsonConvert.DeserializeObject<AliyunGeneralOCRResponse>(response.Body.Data);
                    return Task.FromResult(resp.Data.Content);
                }

                if (type.Equals(OcrTypeCS.GENERAL_HANDWRITING, StringComparison.OrdinalIgnoreCase))
                {
                    RecognizeHandwritingRequest request = new RecognizeHandwritingRequest
                    {
                        Url = imageUrl,
                    };
                    // 复制代码运行请自行打印 API 的返回值
                    RecognizeHandwritingResponse response = client.RecognizeHandwritingWithOptions(request, new RuntimeOptions());

                    // 处理识别结果
                    var resp = JsonConvert.DeserializeObject<AliyunGeneralOCRResponse>(response.Body.Data);
                    return Task.FromResult(resp.Data.Content);
                }

                return Task.FromResult(string.Empty);
            }
            catch (Exception ex)
            {
                // 处理异常
                _logger.LogError(ex, $"OCR异常");
                throw;
            }
        }

        public override Task<CardOCRResult> RecognizeCardTextAsync(string imageUrl, string type)
        {
            try
            {
                // 处理识别结果
                CardOCRResult result = new CardOCRResult();

                if (type.Equals(OcrTypeCS.ID_CARD, StringComparison.OrdinalIgnoreCase))
                {
                    RecognizeIdcardRequest request = new RecognizeIdcardRequest
                    {
                        Url = imageUrl,
                    };
                    // 复制代码运行请自行打印 API 的返回值
                    RecognizeIdcardResponse response = client.RecognizeIdcardWithOptions(request, new RuntimeOptions());
                    var resp = JsonConvert.DeserializeObject<AliyunIDCardOCRResponse>(response.Body.Data);
                    result.IdCardName = resp.Data.Face?.Data.Name;
                    result.IdCardSex = resp.Data.Face?.Data.Sex;
                    result.IdCardNation = resp.Data.Face?.Data.Ethnicity;
                    result.IdCardBirth = ConvertDateToFormat(resp.Data.Face?.Data.BirthDate, "yyyy年MM月dd日");
                    result.IdCardAddress = resp.Data.Face?.Data.Address;
                    result.IdCardIdNum = resp.Data.Face?.Data.IdNumber;

                    result.IdCardAuthority = resp.Data.Back?.Data.IssueAuthority;
                    result.IdCardValidDate = resp.Data.Back?.Data.ValidPeriod;
                    var validDates = result.IdCardValidDate.Split('-');
                    var issueDate = validDates.First();
                    var expiringDate = validDates.Last();
                    result.IdCardIssueDate = ConvertDateToFormat(issueDate, "yyyy.MM.dd");
                    result.IdCardExpiringDate = ConvertDateToFormat(expiringDate, "yyyy.MM.dd");
                }

                if (type.Equals(OcrTypeCS.BANK_CARD, StringComparison.OrdinalIgnoreCase))
                {
                    RecognizeBankCardRequest request = new RecognizeBankCardRequest
                    {
                        Url = imageUrl,
                    };
                    // 复制代码运行请自行打印 API 的返回值
                    RecognizeBankCardResponse response = client.RecognizeBankCardWithOptions(request, new RuntimeOptions());
                    var resp = JsonConvert.DeserializeObject<AliyunBankCardOCRResponse>(response.Body.Data);
                    result.BankCardCardNo = resp.Data.CardNumber;
                    result.BankCardBankInfo = resp.Data.BankName;
                    result.BankCardValidDate = resp.Data.ValidToDate;
                    result.BankCardCardType = resp.Data.CardType;
                }

                if (type.Equals(OcrTypeCS.BIZ_LICENSE, StringComparison.OrdinalIgnoreCase))
                {
                    RecognizeBankCardRequest request = new RecognizeBankCardRequest
                    {
                        Url = imageUrl,
                    };
                    // 复制代码运行请自行打印 API 的返回值
                    RecognizeBankCardResponse response = client.RecognizeBankCardWithOptions(request, new RuntimeOptions());
                    var resp = JsonConvert.DeserializeObject<AliyunBizLicenseOCRResponse>(response.Body.Data);
                    result.BizLicenseRegNum = resp.Data.CreditCode;
                    result.BizLicenseName = resp.Data.CompanyName;
                    result.BizLicenseCapital = resp.Data.RegisteredCapital;
                    result.BizLicensePerson = resp.Data.LegalPerson;
                    result.BizLicenseAddress = resp.Data.BusinessAddress;
                    result.BizLicenseBusiness = resp.Data.BusinessScope;
                    result.BizLicenseType = resp.Data.CompanyType;
                    result.BizLicensePeriod = (resp.Data.ValidPeriod?.EndsWith("长期") ?? true) ? "长期" : ConvertDateToFormat(resp.Data.ValidPeriod, "yyyy年MM月dd日");
                    result.BizLicenseComposingForm = resp.Data.CompanyForm;
                    result.BizLicenseRegistrationDate = ConvertDateToFormat(resp.Data.RegistrationDate, "yyyyMMdd");
                    result.BizLicenseValidFromDate = ConvertDateToFormat(resp.Data.ValidFromDate, "yyyyMMdd");
                    result.BizLicenseValidToDate = ConvertDateToFormat(resp.Data.ValidToDate.Equals("29991231") ? "长期" : resp.Data.ValidToDate, "yyyyMMdd");
                }

                return Task.FromResult(result);

            }
            catch (Exception ex)
            {
                // 处理异常
                _logger.LogError(ex, $"OCR异常");
                throw;
            }
        }
    }
}
