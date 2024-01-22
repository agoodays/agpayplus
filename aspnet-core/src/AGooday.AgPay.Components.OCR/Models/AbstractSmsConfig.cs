using AGooday.AgPay.Components.OCR.Constants;
using Newtonsoft.Json;

namespace AGooday.AgPay.Components.OCR.Models
{
    public abstract class AbstractOcrConfig
    {
        public static AbstractOcrConfig GetOcrConfig(byte ocrType, string configVal)
        {
            return (OcrTypeEnum)ocrType switch
            {
                OcrTypeEnum.Tencent => JsonConvert.DeserializeObject<TencentOcrConfig>(configVal),
                OcrTypeEnum.Aliyun => JsonConvert.DeserializeObject<AliyunOcrConfig>(configVal),
                _ => throw new NotImplementedException()
            };
        }
    }
}
