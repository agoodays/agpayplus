using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace AGooday.AgPay.Manager.Api.Controllers.Common
{
    [ApiController, AllowAnonymous]
    public class StaticController : ControllerBase
    {
        [HttpGet, Route("api/anon/localOssFiles/{folder}/{name}.{format}")]
        public ActionResult ImgView(string folder, string name, string format)
        {
            string path = $"{HttpUtility.UrlDecode(folder)}/{name}.{format}";
            using (var sw = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var bytes = new byte[sw.Length];
                sw.Read(bytes, 0, bytes.Length);
                sw.Close();
                return File(bytes, $"image/{format}");
            }
        }

        [HttpGet, Route("api/anon/get")]
        public IEnumerable<int> Get()
        {
            return Enumerable.Range(1, 5).Select(index => Random.Shared.Next(index, 55)).ToArray();
        }
    }
}
