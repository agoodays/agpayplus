using AGooday.AgPay.Components.OSS.Constants;
using Microsoft.AspNetCore.Http;

namespace AGooday.AgPay.Components.OSS.Services
{
    /// <summary>
    /// OSSService 接口
    /// </summary>
    public interface IOssService
    {
        /// <summary>
        /// 上传文件 & 生成下载/预览URL
        /// </summary>
        /// <param name="ossSavePlaceEnum"></param>
        /// <param name="multipartFile"></param>
        /// <param name="saveDirAndFileName"></param>
        /// <returns></returns>
        Task<string> Upload2PreviewUrl(OssSavePlaceEnum ossSavePlaceEnum, IFormFile multipartFile, string saveDirAndFileName);

        /// <summary>
        /// 将文件下载到本地
        /// 返回是否 写入成功
        /// false: 写入失败， 或者文件不存在
        /// </summary>
        /// <param name="ossSavePlaceEnum"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        bool DownloadFile(OssSavePlaceEnum ossSavePlaceEnum, string source, string target);
    }
}
