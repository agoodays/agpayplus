using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Merchant
{
    /// <summary>
    /// 商户应用管理类
    /// </summary>
    [Route("/api/mchApps")]
    [ApiController]
    public class MchAppController : ControllerBase
    {
        private readonly ILogger<MchAppController> _logger;
        private readonly IMchAppService _mchAppService;

        public MchAppController(ILogger<MchAppController> logger, IMchAppService mchAppService)
        {
            _logger = logger;
            _mchAppService = mchAppService;
        }

        /// <summary>
        /// 应用列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public ApiRes List([FromQuery] MchAppQueryDto dto)
        {
            var data = _mchAppService.GetPaginatedData(dto);
            return ApiRes.Ok(new { records = data.ToList(), total = data.TotalCount, current = data.PageIndex, hasNext = data.HasNext });
        }

        /// <summary>
        /// 新建应用
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public ApiRes Add(MchAppDto dto)
        {
            _mchAppService.Add(dto);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 删除应用
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{appId}")]
        public ApiRes Delete(string appId)
        {
            _mchAppService.Remove(appId);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新应用信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{appId}")]
        public ApiRes Update(MchAppDto dto)
        {
            _mchAppService.Update(dto);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 应用详情
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{appId}")]
        public ApiRes Detail(string appId)
        {
            var mchApp = _mchAppService.GetById(appId);
            if (mchApp == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(mchApp);
        }
    }
}
