using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Config
{
    public class DBOssConfig
    {
        /// <summary>
        /// 阿里云oss配置
        /// </summary>
        public string AliyunOssConfig { get; set; }

        /// <summary>
        /// 文件上传服务类型
        /// </summary>
        public string OssUseType { get; set; }

        /// <summary>
        /// oss公共读文件地址
        /// </summary>
        public string OssPublicSiteUrl { get; set; }

        /// <summary>
        /// 获取阿里云oss配置
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GenAliyunOssConfig()
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(AliyunOssConfig);
        }
    }
}
