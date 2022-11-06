using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.OSS.Models;
using AGooday.AgPay.Components.OSS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Components.OSS.Controllers
{
    [ApiController, AllowAnonymous]
    [Route("api/ossFiles")]
    public class OssFileController : ControllerBase
    {
        private readonly ILogger<OssFileController> logger;
        private IOssService ossService;

        public OssFileController(ILogger<OssFileController> logger, IOssService ossService)
        {
            this.logger = logger;
            this.ossService = ossService;
        }

        /** 上传文件 （单文件上传） */
        [HttpPost, Route("{bizType}")]
        public async Task<ApiRes> SingleFileUpload([FromForm] IFormFile file, string bizType)
        {
            if (file == null)
            {
                return ApiRes.Fail(ApiCode.SYSTEM_ERROR, "选择文件不存在");
            }
            try
            {
                OssFileConfig ossFileConfig = OssFileConfig.GetOssFileConfigByBizType(bizType);

                //1. 判断bizType 是否可用
                if (ossFileConfig == null)
                {
                    throw new BizException("类型有误");
                }

                // 2. 判断文件是否支持
                string suffix = Path.GetExtension(file.FileName);
                string fileSuffix = FileKit.GetFileSuffix(file.FileName, false);
                if (!ossFileConfig.IsAllowFileSuffix(fileSuffix))
                {
                    throw new BizException("上传文件格式不支持！");
                }

                // 3. 判断文件大小是否超限
                if (!ossFileConfig.IsMaxSizeLimit(file.Length))
                {
                    throw new BizException("上传大小请限制在[" + ossFileConfig.MaxSize / 1024 / 1024 + "M]以内！");
                }

                // 新文件地址 (xxx/xxx.jpg 格式)
                string saveDirAndFileName = Path.Combine(bizType, $"{Guid.NewGuid().ToString("N")}{Path.GetExtension(file.FileName)}");
                string url = await ossService.Upload2PreviewUrl(ossFileConfig.OssSavePlaceEnum, new List<IFormFile>() { file }, saveDirAndFileName);
                return ApiRes.Ok(url);
            }
            catch (BizException biz)
            {
                throw biz;
            }
            catch (Exception e)
            {
                logger.LogError(e, $"upload error, fileName = {file.FileName}");
                throw new BizException(ApiCode.SYSTEM_ERROR);
            }
        }

        [HttpGet, Route("get")]
        public IEnumerable<int> Get()
        {
            return Enumerable.Range(1, 5).Select(index => Random.Shared.Next(index, 55)).ToArray();
        }
    }
}
