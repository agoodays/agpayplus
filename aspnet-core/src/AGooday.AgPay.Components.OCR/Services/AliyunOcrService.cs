using AGooday.AgPay.Application.Interfaces;
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
    public class AliyunOcrService : IOcrService
    {
        private readonly ILogger<AliyunOcrService> logger;
        private readonly AliyunOcrConfig ocrConfig;
        private readonly Client client;

        public AliyunOcrService(ILogger<AliyunOcrService> logger, ISysConfigService sysConfigService)
        {
            this.logger = logger;
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
        }

        public Task<string> RecognizeTextAsync(string imagePath, string type)
        {
            RecognizeGeneralRequest request = new RecognizeGeneralRequest
            {
                Url = imagePath,
            };
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                RecognizeGeneralResponse response = client.RecognizeGeneralWithOptions(request, new RuntimeOptions());

                // 处理识别结果
                var resp = JsonConvert.DeserializeObject<AliyunGeneralOCRResponse>(response.Body.Data);
                return Task.FromResult(resp.Data.Content);

            }
            catch (Exception ex)
            {
                // 处理异常
                logger.LogError(ex, $"OCR异常，请求报文：{JsonConvert.SerializeObject(request)}");
                throw;
            }
        }

        public Task<Dictionary<string, string>> RecognizeCardTextAsync(string imagePath, string type)
        {
            try
            {
                // 处理识别结果
                Dictionary<string, string> map = new Dictionary<string, string>();

                if (type.Equals("IdCard"))
                {
                    RecognizeIdcardRequest request = new RecognizeIdcardRequest
                    {
                        Url = imagePath,
                    };
                    // 复制代码运行请自行打印 API 的返回值
                    RecognizeIdcardResponse response = client.RecognizeIdcardWithOptions(request, new RuntimeOptions());
                    var resp = JsonConvert.DeserializeObject<AliyunIDCardOCRResponse>(response.Body.Data);
                    map.Add("IdCardName", resp.Face.Data.Name);
                    map.Add("IdCardSex", resp.Face.Data.Sex);
                    map.Add("IdCardNation", resp.Face.Data.Ethnicity);
                    map.Add("IdCardBirth", resp.Face.Data.BirthDate);
                    map.Add("IdCardAddress", resp.Face.Data.Address);
                    map.Add("IdCardIdNum", resp.Face.Data.IdNumber);

                    map.Add("IdCardAuthority", resp.Back.Data.IssueAuthority);
                    map.Add("IdCardValidDate", resp.Back.Data.ValidPeriod);
                }

                if (type.Equals("BankCard"))
                {
                    RecognizeBankCardRequest request = new RecognizeBankCardRequest
                    {
                        Url = imagePath,
                    };
                    // 复制代码运行请自行打印 API 的返回值
                    RecognizeBankCardResponse response = client.RecognizeBankCardWithOptions(request, new RuntimeOptions());
                    var resp = JsonConvert.DeserializeObject<AliyunBankCardOCRResponse>(response.Body.Data);
                    map.Add("BankCardCardNo", resp.Data.CardNumber);
                    map.Add("BankCardBankInfo", resp.Data.BankName);
                    map.Add("BankCardValidDate", resp.Data.ValidToDate);
                    map.Add("BankCardCardType", resp.Data.CardType);
                }

                if (type.Equals("BizLicense"))
                {
                    RecognizeBankCardRequest request = new RecognizeBankCardRequest
                    {
                        Url = imagePath,
                    };
                    // 复制代码运行请自行打印 API 的返回值
                    RecognizeBankCardResponse response = client.RecognizeBankCardWithOptions(request, new RuntimeOptions());
                    var resp = JsonConvert.DeserializeObject<AliyunBizLicenseOCRResponse>(response.Body.Data);
                    map.Add("BizLicenseRegNum", resp.Data.CreditCode);
                    map.Add("BizLicenseName", resp.Data.CompanyName);
                    map.Add("BizLicenseCapital", resp.Data.RegisteredCapital);
                    map.Add("BizLicensePerson", resp.Data.LegalPerson);
                    map.Add("BizLicenseAddress", resp.Data.BusinessAddress);
                    map.Add("BizLicenseBusiness", resp.Data.BusinessScope);
                    map.Add("BizLicenseType", resp.Data.CompanyType);
                    map.Add("BizLicensePeriod", resp.Data.ValidPeriod);
                    map.Add("BizLicenseComposingForm", resp.Data.CompanyForm);
                    map.Add("BizLicenseRegistrationDate", resp.Data.RegistrationDate);
                    map.Add("BizLicenseValidFromDate", resp.Data.ValidFromDate);
                    map.Add("BizLicenseValidToDate", resp.Data.ValidToDate);
                }

                return Task.FromResult(map);

            }
            catch (Exception ex)
            {
                // 处理异常
                logger.LogError(ex, $"OCR异常");
                throw;
            }
        }
    }
}
