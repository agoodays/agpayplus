using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Domain.Models;
using System;

namespace AGooday.AgPay.Manager.Api.Controllers.PayConfig
{
    /// <summary>
    /// 支付方式管理类
    /// </summary>
    [Route("/api/payWays")]
    [ApiController]
    public class PayWayController : ControllerBase
    {
        private readonly ILogger<PayWayController> _logger;
        private readonly IPayWayService _payWayService;

        public PayWayController(ILogger<PayWayController> logger, IPayWayService payWayService)
        {
            _logger = logger;
            _payWayService = payWayService;
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public ApiRes List([FromQuery] PayWayQueryDto dto)
        {
            var data = _payWayService.GetPaginatedData<PayWayDto>(dto);
            return ApiRes.Ok(new { records = data.ToList(), total = data.TotalCount, current = data.PageIndex, hasNext = data.HasNext });
        }

        /// <summary>
        /// 新增支付方式
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost]
        [Route("")]
        public ApiRes Add(PayWayDto dto)
        {
            if (_payWayService.IsExistPayWayCode(dto.WayCode))
            {
                throw new BizException("支付方式代码已存在");
            }
            bool result = _payWayService.Add(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_CREATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 删除支付方式
        /// </summary>
        /// <param name="wayCode"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{wayCode}")]
        public ApiRes Delete(string wayCode)
        {
            bool result = _payWayService.Remove(wayCode);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_DELETE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新支付方式
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{wayCode}")]
        public ApiRes Update(PayWayDto dto)
        {
            _payWayService.Update(dto);
            bool result = _payWayService.Update(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_UPDATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 查看支付方式
        /// </summary>
        /// <param name="wayCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{wayCode}")]
        public ApiRes Detail(string wayCode)
        {
            var payWay = _payWayService.GetById(wayCode);
            if (payWay == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(payWay);
        }
    }
}
