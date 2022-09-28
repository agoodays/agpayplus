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
        private readonly IMchPayPassageService _mchPayPassageService;
        private readonly IPayOrderService _payOrderService;

        public PayWayController(ILogger<PayWayController> logger,
            IPayWayService payWayService,
            IMchPayPassageService mchPayPassageService,
            IPayOrderService payOrderService)
        {
            _logger = logger;
            _payWayService = payWayService;
            _mchPayPassageService = mchPayPassageService;
            _payOrderService = payOrderService;
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
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
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
            // 校验该支付方式是否有商户已配置通道或者已有订单
            if (_mchPayPassageService.IsExistMchPayPassageUseWayCode(wayCode)
                || _payOrderService.IsExistOrderUseWayCode(wayCode))
            {
                throw new BizException("该支付方式已有商户配置通道或已发生交易，无法删除！");
            }

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
