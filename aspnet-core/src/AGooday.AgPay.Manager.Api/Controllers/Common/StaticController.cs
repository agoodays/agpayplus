using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Common
{
    [ApiController]
    public class StaticController : ControllerBase
    {
        [HttpGet]
        [Route("api/anon/localOssFiles")]
        public ActionResult ImgView()
        {
            return Ok();
        }

        [HttpGet]
        [Route("api/anon/get")]
        public IEnumerable<int> Get()
        {
            return Enumerable.Range(1, 5).Select(index => Random.Shared.Next(index, 55)).ToArray();
        }
    }
}
