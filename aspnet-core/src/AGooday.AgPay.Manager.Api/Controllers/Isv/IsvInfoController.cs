using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Isv
{
    /// <summary>
    /// 服务商管理类
    /// </summary>
    [Route("/api/isvInfo")]
    [ApiController]
    public class IsvInfoController : ControllerBase
    {
        private readonly ILogger<IsvInfoController> _logger;
        private readonly IIsvInfoService _isvInfoService;

        public IsvInfoController(ILogger<IsvInfoController> logger, IIsvInfoService isvInfoService)
        {
            _logger = logger;
            _isvInfoService = isvInfoService;
        }

        /// <summary>
        /// 查询服务商信息列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public ApiRes List([FromQuery] IsvInfoQueryDto dto)
        {
            var data = _isvInfoService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
        }

        /// <summary>
        /// 新增服务商
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public ApiRes Add(IsvInfoDto dto)
        {
            _isvInfoService.Add(dto);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 删除服务商
        /// </summary>
        /// <param name="isvNo"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{isvNo}")]
        public ApiRes Delete(string isvNo)
        {
            _isvInfoService.Remove(isvNo);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新服务商信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{isvNo}")]
        public ApiRes Update(IsvInfoDto dto)
        {
            _isvInfoService.Update(dto);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 查看服务商信息
        /// </summary>
        /// <param name="isvNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{isvNo}")]
        public ApiRes Detail(string isvNo)
        {
            var isvInfo = _isvInfoService.GetById(isvNo);
            if (isvInfo == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(isvInfo);
        }
    }
}
