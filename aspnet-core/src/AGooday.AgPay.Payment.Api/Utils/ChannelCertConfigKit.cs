using AGooday.AgPay.Components.OSS.Config;
using AGooday.AgPay.Components.OSS.Constants;
using AGooday.AgPay.Components.OSS.Extensions;
using AGooday.AgPay.Components.OSS.Services;

namespace AGooday.AgPay.Payment.Api.Utils
{
    /// <summary>
    /// 支付平台 获取系统文件工具类
    /// </summary>
    public class ChannelCertConfigKit
    {
        public static string OssUseType { get; set; }
        private static IOssService OssService { get; set; }

        public ChannelCertConfigKit(IOssServiceFactory ossServiceFactory)
        {
            OssService = ossServiceFactory.GetService();
            OssUseType = ossServiceFactory.GetOssUseType();
        }

        public static string GetCertFilePath(string certFilePath)
        {
            return GetCertFile(certFilePath).FullName;
        }

        public static FileInfo GetCertFile(string certFilePath)
        {
            var filePath = Path.Combine(LocalOssConfig.Oss.FilePrivatePath, certFilePath);
            FileInfo certFile = new FileInfo(filePath);
            if (certFile.Exists)
            {
                // 本地存在直接返回
                return certFile;
            }

            // 以下为 文件不存在的处理方式

            // 是否本地存储
            bool isLocalSave = OssUseType.Equals(OssUseTypeCS.LOCAL_FILE);

            // 本地存储 & 文件不存在
            if (isLocalSave)
            {
                return certFile;
            }

            // 已经向oss请求并且返回了空文件时
            if (File.Exists(filePath + ".notexists"))
            {
                return certFile;
            }

            // 请求下载并返回 新File
            return DownloadFile(certFilePath, certFile);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="dbCertFilePath"></param>
        /// <param name="certFile"></param>
        /// <returns></returns>
        private static FileInfo DownloadFile(string dbCertFilePath, FileInfo certFile)
        {
            //请求文件并写入
            bool isSuccess = OssService.DownloadFile(OssSavePlaceEnum.PRIVATE, dbCertFilePath, certFile.FullName);

            // 下载成功 返回新的File对象
            if (isSuccess)
            {
                return certFile;
            }

            // 下载失败， 写入.notexists文件， 避免那下次再次下载影响效率。
            try
            {
                new FileInfo(certFile.FullName + ".notexists").Create();
            }
            catch (IOException)
            {
            }

            return certFile;
        }
    }
}
