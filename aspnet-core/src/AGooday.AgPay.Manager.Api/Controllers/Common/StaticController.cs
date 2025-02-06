using System.Text;
using System.Web;
using AGooday.AgPay.Components.OSS.Config;
using AGooday.AgPay.Manager.Api.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Common
{
    [ApiController, Authorize, AllowAnonymous, NoLog]
    public class StaticController : ControllerBase
    {
        [HttpGet, Route("api/anon/localOssFiles/{*path}")]
        public ActionResult AllPurpose(string path)
        {
            try
            {
                path = HttpUtility.UrlDecode(path);
                var format = GetFormat(path);
                if (IsImage(format))
                {
                    return ImgView(path, format);
                }
                else
                {
                    return Content(path);
                }
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        [HttpGet, Route("api/anon/localOssFiles/{folder}/{name}.{format}")]
        public ActionResult ImgView(string folder, string name, string format)
        {
            try
            {
                string path = $"{HttpUtility.UrlDecode(folder)}/{name}.{format}";
                if (IsImage(format))
                {
                    return ImgView(path, format);
                }
                else
                {
                    return Content(path);
                }
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        //[HttpGet, Route("api/anon/localOssFiles/{path:regex(([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&=]*)?$)}")]
        //public ActionResult FileView(string path)
        //{
        //    try
        //    {
        //        path = HttpUtility.UrlDecode(path);
        //        var format = GetFormat(path);
        //        if (IsImage(format))
        //        {
        //            return ImgView(path, format);
        //        }
        //        else
        //        {
        //            return Content(path);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(e.Message);
        //    }
        //}

        private FileContentResult ImgView(string path, string format)
        {
            path = Path.Combine(LocalOssConfig.Oss.FilePublicPath, path); // 使用Path.Combine处理路径
            using (var sw = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var bytes = new byte[sw.Length];
                sw.Read(bytes, 0, bytes.Length);
                sw.Close();
                return File(bytes, GetContentType(format));
            }
        }

        private static string GetFormat(string path)
        {
            return path.Split('.').Length > 0 ? path.Split('.').Last() : string.Empty;
        }

        private static bool IsImage(string format)
        {
            var formats = new List<string> { "jpg", "tiff", "gif", "jfif", "png", "tif", "ico", "jpeg", "wbmp", "fax", "net", "jpe" };
            return !string.IsNullOrEmpty(format) && formats.Contains(format.ToLower());
        }

        private static string GetContentType(string format)
        {
            var contentType = $"image/{format}";
            switch (format)
            {
                case "jpeg":
                case "jfif":
                case "jpe":
                case "jpg": contentType = "image/jpeg"; break;
                case "tiff": contentType = "image/tiff"; break;
                case "gif": contentType = "image/gif"; break;
                case "png": contentType = "image/png"; break;
                case "tif": contentType = "image/tiff"; break;
                case "ico": contentType = "image/x-icon"; break;
                case "wbmp": contentType = "image/vnd.wap.wbmp"; break;
                case "fax": contentType = "image/fax"; break;
                case "net": contentType = "image/pnetvue"; break;
                case "rp": contentType = "image/vnd.rn-realpix​"; break;
            }
            return contentType;
        }

        [HttpGet, Route("api/anon/get")]
        public IEnumerable<int> Get()
        {
            return Enumerable.Range(1, 5).Select(index => Random.Shared.Next(index, 55)).ToArray();
        }

        [HttpGet, Route("api/anon/getfile")]
        public ActionResult GetFile(string path)
        {
            path = HttpUtility.UrlDecode(path);
            var format = GetFormat(path);
            var filePath = Path.Combine(LocalOssConfig.Oss.FilePublicPath, path);

            FileInfo certFile = new FileInfo(filePath);
            if (certFile.Exists)
            {
                using (var sw = certFile.Open(FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                {
                    var bytes = new byte[sw.Length];
                    sw.Read(bytes, 0, bytes.Length);
                    sw.Close();
                    return File(bytes, GetContentType(format));
                }
            }
            else
            {
                var notexists = new FileInfo(certFile.FullName + ".notexists");
                if (notexists.Exists)
                {
                    using (var sw = notexists.Open(FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                    {
                        var bytes = new byte[sw.Length];
                        sw.Read(bytes, 0, bytes.Length);
                        sw.Close();
                        return File(bytes, GetContentType(format));
                    }
                }
                using (var sw = notexists.Create())
                {
                    var bytes = new byte[sw.Length];
                    sw.Read(bytes, 0, bytes.Length);
                    sw.Close();
                    return File(bytes, GetContentType(format));
                }
            }
        }

        [HttpGet, Route("api/anon/getfilecontent")]
        public string GetFileContent(string path)
        {
            string merchantCertificatePrivateKey = System.IO.File.ReadAllText(path, Encoding.UTF8);
            return merchantCertificatePrivateKey;
        }
    }
}
