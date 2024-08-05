using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.OCR.Constants;
using AGooday.AgPay.Components.OCR.Models;
using Baidu.Aip.Ocr;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.OCR.Services
{
    /// <summary>
    /// API 概览：https://cloud.baidu.com/doc/OCR/s/Hkibizy6z
    /// </summary>
    public class BaiduOcrService : AbstractOcrService
    {
        private readonly BaiduOcrConfig ocrConfig;
        private readonly JsonSerializerSettings globalSettings;
        private readonly Ocr client;

        public BaiduOcrService(ILogger<BaiduOcrService> logger, ISysConfigService sysConfigService)
            : base(logger)
        {
            var dbOcrConfig = sysConfigService.GetDBOcrConfig();
            // 获取全局默认配置
            globalSettings = JsonConvert.DefaultSettings?.Invoke() ?? new JsonSerializerSettings();
            ocrConfig = (BaiduOcrConfig)AbstractOcrConfig.GetOcrConfig(dbOcrConfig.OcrType, dbOcrConfig.TencentOcrConfig);
            client = new Ocr(ocrConfig.ApiKey, ocrConfig.SecretKey);
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
                    var resp = client.GeneralBasicUrl(imageUrl);
                    var isHaveResult = resp.TryGetValue("words_result", out JToken wordsResult);
                    if (isHaveResult)
                    {
                        detectedTexts = ((JArray)wordsResult).Select(item => item["words"]?.ToString()).ToList();
                    }
                }

                if (type.Equals(OcrTypeCS.GENERAL_ACCURATE, StringComparison.OrdinalIgnoreCase))
                {
                    var resp = client.AccurateBasicUrl(imageUrl);
                    var isHaveResult = resp.TryGetValue("words_result", out JToken wordsResult);
                    if (isHaveResult)
                    {
                        detectedTexts = ((JArray)wordsResult).Select(item => item["words"]?.ToString()).ToList();
                    }
                }

                if (type.Equals(OcrTypeCS.GENERAL_HANDWRITING, StringComparison.OrdinalIgnoreCase))
                {
                    var resp = client.HandwritingUrl(imageUrl);
                    var isHaveResult = resp.TryGetValue("words_result", out JToken wordsResult);
                    if (isHaveResult)
                    {
                        detectedTexts = ((JArray)wordsResult).Select(item => item["words"]?.ToString()).ToList();
                    }
                }

                return Task.FromResult(string.Join("\n", detectedTexts));
            }
            catch (Exception ex)
            {
                // 处理异常
                _logger.LogError(ex, $"Ocr异常");
                throw;
            }
            finally
            {
                JsonConvert.DefaultSettings = () => globalSettings;
            }
        }

        public override async Task<CardOCRResult> RecognizeCardTextAsync(string imageUrl, string type)
        {
            try
            {
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings();

                // 处理识别结果
                CardOCRResult result = new CardOCRResult();

                if (type.Equals(OcrTypeCS.ID_CARD, StringComparison.OrdinalIgnoreCase))
                {
                    // 发送请求并获取识别结果
                    var resp = client.MultiIdcardUrl(imageUrl);
                    var isHaveResult = resp.TryGetValue("words_result", out JToken wordsResult);
                    if (isHaveResult)
                    {
                        var idcardFrontResult = ((JArray)wordsResult).Where(w => ((JObject)w).GetValue("card_info").Value<string>("card_type") == "idcard_front")
                            .Select(s => ((JObject)s).GetValue("card_result")).FirstOrDefault();
                        var idcardBackResult = ((JArray)wordsResult).Where(w => ((JObject)w).GetValue("card_info").Value<string>("card_type") == "idcard_back")
                            .Select(s => ((JObject)s).GetValue("card_result")).FirstOrDefault();
                        if (idcardFrontResult != null)
                        {
                            result.IdCardName = ConvertEmptyStringToNull(GetWords(idcardFrontResult, "姓名"));
                            result.IdCardSex = ConvertEmptyStringToNull(GetWords(idcardFrontResult, "性别"));
                            result.IdCardNation = ConvertEmptyStringToNull(GetWords(idcardFrontResult, "民族"));
                            result.IdCardBirth = ConvertEmptyStringToNull(ConvertDateToFormat(GetWords(idcardFrontResult, "出生"), "yyyyMMdd"));
                            result.IdCardAddress = ConvertEmptyStringToNull(GetWords(idcardFrontResult, "住址"));
                            result.IdCardIdNum = ConvertEmptyStringToNull(GetWords(idcardFrontResult, "公民身份号码"));
                        }
                        if (idcardBackResult != null)
                        {
                            result.IdCardAuthority = ConvertEmptyStringToNull(GetWords(idcardBackResult, "签发机关"));
                            var issueDate = ConvertDateToFormat(GetWords(idcardBackResult, "签发日期"), "yyyyMMdd", "yyyy.MM.dd");
                            var expiringDate = ConvertDateToFormat(GetWords(idcardBackResult, "失效日期"), "yyyyMMdd", "yyyy.MM.dd");
                            result.IdCardValidDate = issueDate != null && expiringDate != null ? $"{issueDate}-{expiringDate}" : issueDate ?? expiringDate;
                        }
                    }
                }

                if (type.Equals(OcrTypeCS.BANK_CARD, StringComparison.OrdinalIgnoreCase))
                {
                    var image = await GetImageBytesAsync(imageUrl);
                    var resp = client.Bankcard(image);
                    var isHaveResult = resp.TryGetValue("result", out JToken _result);
                    if (isHaveResult)
                    {
                        ((JObject)_result).TryGetString("bank_card_number", out string bankCardNumber);
                        ((JObject)_result).TryGetString("bank_name", out string bankName);
                        ((JObject)_result).TryGetString("bank_card_type", out string bankCardType);
                        result.BankCardCardNo = bankCardNumber;
                        result.BankCardBankInfo = bankName;
                        result.BankCardValidDate = null;
                        result.BankCardCardType = bankCardType;
                    }
                }

                if (type.Equals(OcrTypeCS.BIZ_LICENSE, StringComparison.OrdinalIgnoreCase))
                {
                    var image = await GetImageBytesAsync(imageUrl);
                    var resp = client.BusinessLicense(image);
                    var isHaveResult = resp.TryGetValue("words_result", out JToken wordsResult);
                    if (isHaveResult)
                    {
                        result.BizLicenseRegNum = GetWords(wordsResult, "社会信用代码");
                        result.BizLicenseName = GetWords(wordsResult, "单位名称");
                        result.BizLicenseCapital = GetWords(wordsResult, "注册资本");
                        result.BizLicensePerson = GetWords(wordsResult, "法人");
                        result.BizLicenseAddress = GetWords(wordsResult, "地址");
                        result.BizLicenseBusiness = GetWords(wordsResult, "经营范围");
                        result.BizLicenseType = GetWords(wordsResult, "类型");
                        result.BizLicensePeriod = GetWords(wordsResult, "有效期");
                        result.BizLicenseComposingForm = GetWords(wordsResult, "组成形式");
                        result.BizLicenseRegistrationDate = GetWords(wordsResult, "成立日期");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                // 处理异常
                _logger.LogError(ex, $"OCR异常");
                throw;
            }
            finally
            {
                JsonConvert.DefaultSettings = () => globalSettings;
            }
        }

        public static string GetWords(JToken wordsResult, string propertyName)
        {
            if (wordsResult != null)
            {
                var isHaveResult = ((JObject)wordsResult).TryGetValue(propertyName, out JToken jToken);
                if (isHaveResult)
                {
                    ((JObject)jToken).TryGetString("words", out string words);
                    return words;
                }
            }
            return null;
        }
    }
}
