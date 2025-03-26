using System.Text;
using System.Web;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.OSS.Config;
using AGooday.AgPay.Manager.Api.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Common
{
    [ApiController, Authorize, AllowAnonymous, NoLog]
    public class StaticController : ControllerBase
    {
        protected readonly ILogger<StaticController> _logger;

        protected StaticController(ILogger<StaticController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("api/anon/localOssFiles")]
        [Route("api/anon/localOssFiles/{*path}")]
        public async Task<ActionResult> AllPurposeAsync(string path = null)
        {
            path = GetPathFromRequest(path);
            if (string.IsNullOrEmpty(path))
            {
                return BadRequest("Path is required.");
            }
            return await HandleFileRequestAsync(path);
        }

        [HttpGet, Route("api/anon/localOssFiles/{folder}/{name}.{format}")]
        public async Task<ActionResult> FileViewAsync(string folder, string name, string format)
        {
            string fullPath = $"{HttpUtility.UrlDecode(folder)}/{name}.{format}";
            return await HandleFileRequestAsync(fullPath);
        }

        //[HttpGet, Route("api/anon/localOssFiles/{path:regex(([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&=]*)?$)}")]
        //public async Task<ActionResult> FileView(string path)
        //{
        //    path = HttpUtility.UrlDecode(path);
        //    return await HandleFileRequestAsync(path);
        //}

        [HttpGet]
        [Route("api/anon/getfile")]
        [Route("api/anon/getfile/{*path}")]
        public async Task<ActionResult> GetFileAsync(string path)
        {
            path = GetPathFromRequest(path);
            if (string.IsNullOrEmpty(path))
            {
                return BadRequest("Path is required.");
            }

            var filePath = GetFullPath(path);
            var certFile = new FileInfo(filePath);

            if (certFile.Exists)
            {
                return await ReturnFileContentAsync(certFile);
            }

            var notExistsFile = new FileInfo($"{certFile.FullName}.notexists");
            if (notExistsFile.Exists)
            {
                return await ReturnFileContentAsync(notExistsFile);
            }

            using (var sw = notExistsFile.Create())
            {
                return File(Array.Empty<byte>(), MimeTypeResolver.GetMimeType(notExistsFile.FullName));
            }

            // 如果文件不存在且没有 .notexists 文件，返回 404 Not Found
            //return NotFound();
        }

        [HttpGet]
        [Route("api/anon/getfilecontent")]
        [Route("api/anon/getfilecontent/{*path}")]
        public async Task<ActionResult> GetFileContentAsync(string path)
        {
            path = GetPathFromRequest(path);
            if (string.IsNullOrEmpty(path))
            {
                return BadRequest("Path is required.");
            }

            var fullPath = GetFullPath(path);
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound();
            }

            return Content(await System.IO.File.ReadAllTextAsync(fullPath, Encoding.UTF8));
        }

        private async Task<ActionResult> HandleFileRequestAsync(string fullPath)
        {
            try
            {
                fullPath = GetFullPath(fullPath);

                if (ImageValidator.IsImage(fullPath, out string mimeType))
                {
                    return await ReturnFileContentAsync(new FileInfo(fullPath), mimeType);
                }

                return Content(fullPath);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Internal server error: {Message}", e.Message);
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        private async Task<ActionResult> ReturnFileContentAsync(FileInfo fileInfo, string mimeType = null)
        {
            mimeType ??= MimeTypeResolver.GetMimeType(fileInfo.FullName);

            using (var stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var bytes = new byte[stream.Length];
                await stream.ReadExactlyAsync(bytes, 0, bytes.Length);
                return File(bytes, mimeType);
            }
        }

        private string GetPathFromRequest(string path)
        {
            // 如果通过路由参数没有获得path，则尝试从查询字符串中获取
            if (string.IsNullOrEmpty(path))
            {
                path = Request.Query["path"].ToString();
            }
            return HttpUtility.UrlDecode(path);
        }

        private static string GetFullPath(string relativePath)
        {
            return Path.GetFullPath(Path.Combine(LocalOssConfig.Oss.FilePublicPath, relativePath));// 使用Path.Combine处理路径
        }
    }
}