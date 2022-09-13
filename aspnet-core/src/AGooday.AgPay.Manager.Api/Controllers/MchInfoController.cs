using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.ViewModels;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers
{
    [Route("/api/mchInfo")]
    [ApiController]
    public class MchInfoController : ControllerBase
    {
        private readonly ILogger<MchInfoController> _logger;
        private readonly IMchInfoService _mchInfoService;

        public MchInfoController(ILogger<MchInfoController> logger, IMchInfoService mchInfoService)
        {
            _logger = logger;
            _mchInfoService = mchInfoService;
        }

        [HttpGet]
        [Route("list")]
        public ApiRes List()
        {
            var data = _mchInfoService.GetAll();
            return ApiRes.Ok(data);
        }

        [HttpPost]
        [Route("add")]
        public ApiRes Add(MchInfoVM vm)
        {
            _mchInfoService.Add(vm);
            return ApiRes.Ok();
        }

        [HttpDelete]
        [Route("delete/{mchNo}")]
        public ApiRes Delete(string mchNo)
        {
            _mchInfoService.Remove(mchNo);
            return ApiRes.Ok();
        }

        [HttpPut]
        [Route("update/{mchNo}")]
        public ApiRes Update(MchInfoVM vm)
        {
            _mchInfoService.Update(vm);
            return ApiRes.Ok();
        }

        [HttpGet]
        [Route("detail/{mchNo}")]
        public ApiRes Detail(string mchNo)
        {
            return ApiRes.Ok(_mchInfoService.GetById(mchNo));
        }
    }
}
