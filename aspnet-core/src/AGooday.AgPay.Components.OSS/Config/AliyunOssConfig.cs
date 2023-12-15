namespace AGooday.AgPay.Components.OSS.Config
{
    public class AliyunOssConfig
    {
        public static OssConfig Oss { get; set; } = new OssConfig();
        /// <summary>
        /// 系统oss配置信息
        /// </summary>
        public class OssConfig
        {
            public string Endpoint { get; set; }
            public string PublicBucketName { get; set; }
            public string PrivateBucketName { get; set; }
            public string AccessKeyId { get; set; }
            public string AccessKeySecret { get; set; }
            public int? ExpireTime { get; set; }
        }
    }
}