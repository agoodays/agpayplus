using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Config
{
    public class DBOcrConfig
    {
        /// <summary>
        /// OCR识别使用类型 1-腾讯OCR 2-阿里OCR
        /// </summary>
        public byte OcrType { get; set; }

        /// <summary>
        /// OCR识别使用状态 0-关闭 1-开启
        /// </summary>
        public byte OcrState { get; set; }

        /// <summary>
        /// 腾讯OCR识别参数配置
        /// </summary>
        public string TencentOcrConfig { get; set; }

        /// <summary>
        /// 阿里OCR识别参数配置
        /// </summary>
        public string AliOcrConfig { get; set; }

        /// <summary>
        /// 百度OCR识别参数配置
        /// </summary>
        public string BaiduOcrConfig { get; set; }

        /// <summary>
        /// 获取腾讯云OCR配置
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GenTencentOcrConfig()
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(TencentOcrConfig);
        }

        /// <summary>
        /// 获取阿里云OCR配置
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GenAliOcrConfig()
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(AliOcrConfig);
        }

        /// <summary>
        /// 获取百度智能云OCR配置
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GenBaiduOcrConfig()
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(BaiduOcrConfig);
        }
    }
}
