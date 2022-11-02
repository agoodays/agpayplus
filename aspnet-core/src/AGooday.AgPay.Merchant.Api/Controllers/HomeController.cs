using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers
{
    [Route("api/home")]
    [ApiController]
    public class HomeController : Controller
    {
        // GET: HomeController
        [HttpGet, Route("index")]
        public ActionResult Index()
        {
            return View("~/Views/Home/Index.cshtml");
        }
    }
}
