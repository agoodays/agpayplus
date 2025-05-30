using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.PayConfig
{
    /// <summary>
    /// 支付方式管理类
    /// </summary>
    [Route("api/payWays")]
    [ApiController, Authorize]
    public class PayWayController : CommonController
    {
        private readonly IPayWayService _payWayService;
        private readonly IMchPayPassageService _mchPayPassageService;
        private readonly IPayOrderService _payOrderService;

        public PayWayController(ILogger<PayWayController> logger,
            ICacheService cacheService,
            IAuthService authService,
            IPayWayService payWayService,
            IMchPayPassageService mchPayPassageService,
            IPayOrderService payOrderService)
            : base(logger, cacheService, authService)
        {
            _payWayService = payWayService;
            _mchPayPassageService = mchPayPassageService;
            _payOrderService = payOrderService;
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_PC_WAY_LIST, PermCode.MGR.ENT_PAY_ORDER_SEARCH_PAY_WAY)]
        public async Task<ApiPageRes<PayWayDto>> ListAsync([FromQuery] PayWayQueryDto dto)
        {
            var data = await _payWayService.GetPaginatedDataAsync<PayWayDto>(dto);
            return ApiPageRes<PayWayDto>.Pages(data);
        }

        /// <summary>
        /// 新增支付方式
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost, Route(""), MethodLog("新增支付方式")]
        [PermissionAuth(PermCode.MGR.ENT_PC_WAY_ADD)]
        public async Task<ApiRes> AddAsync(PayWayDto dto)
        {
            if (await _payWayService.IsExistPayWayCodeAsync(dto.WayCode))
            {
                throw new BizException("支付方式代码已存在");
            }
            bool result = await _payWayService.AddAsync(dto);
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
        [HttpDelete, Route("{wayCode}"), MethodLog("删除支付方式")]
        [PermissionAuth(PermCode.MGR.ENT_PC_WAY_DEL)]
        public async Task<ApiRes> DeleteAsync(string wayCode)
        {
            // 校验该支付方式是否有商户已配置通道或者已有订单
            if (await _mchPayPassageService.IsExistMchPayPassageUseWayCodeAsync(wayCode)
                || await _payOrderService.IsExistOrderUseWayCodeAsync(wayCode))
            {
                throw new BizException("该支付方式已有商户配置通道或已发生交易，无法删除！");
            }

            bool result = await _payWayService.RemoveAsync(wayCode);
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
        [HttpPut, Route("{wayCode}"), MethodLog("更新支付方式")]
        [PermissionAuth(PermCode.MGR.ENT_PC_WAY_EDIT)]
        public async Task<ApiRes> UpdateAsync(string wayCode, PayWayDto dto)
        {
            bool result = await _payWayService.UpdateAsync(dto);
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
        [HttpGet, Route("{wayCode}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_PC_WAY_VIEW, PermCode.MGR.ENT_PC_WAY_EDIT)]
        public async Task<ApiRes> DetailAsync(string wayCode)
        {
            var payWay = await _payWayService.GetByIdAsNoTrackingAsync(wayCode);
            if (payWay == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(payWay);
        }
    }
}
