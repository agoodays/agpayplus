using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Manager.Api.Controllers.PayConfig
{
    /// <summary>
    /// 支付接口定义管理类
    /// </summary>
    [Route("api/payIfDefines")]
    [ApiController, Authorize]
    public class PayInterfaceDefineController : CommonController
    {
        private readonly IPayInterfaceDefineService _payIfDefineService;
        private readonly IPayInterfaceConfigService _payIfConfigService;
        private readonly IPayOrderService _payOrderService;

        public PayInterfaceDefineController(ILogger<PayInterfaceDefineController> logger,
            IPayInterfaceDefineService payIfDefineService,
            IPayInterfaceConfigService payIfConfigService,
            IPayOrderService payOrderService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            _payIfDefineService = payIfDefineService;
            _payIfConfigService = payIfConfigService;
            _payOrderService = payOrderService;
        }

        /// <summary>
        /// 支付接口
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_PC_IF_DEFINE_LIST)]
        public ApiRes List(byte? state)
        {
            var data = _payIfDefineService.GetAllAsNoTracking()
                .Where(w => !state.HasValue || w.State.Equals(state))
                .OrderByDescending(o => o.CreatedAt);
            return ApiRes.Ok(data);
        }

        /// <summary>
        /// 新增支付接口
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("新增支付接口")]
        [PermissionAuth(PermCode.MGR.ENT_PC_IF_DEFINE_ADD)]
        public async Task<ApiRes> AddAsync(PayInterfaceDefineAddOrEditDto dto)
        {
            JArray jsonArray = new JArray();
            var wayCodes = dto.WayCodeStrs.Split(",");
            foreach (var wayCode in wayCodes)
            {
                JObject value = new JObject();
                value.Add("wayCode", wayCode);
                jsonArray.Add(value);
            }
            dto.WayCodes = jsonArray;
            var result = await _payIfDefineService.AddAsync(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_CREATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 删除支付接口
        /// </summary>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpDelete, Route("{ifCode}"), MethodLog("删除支付接口")]
        [PermissionAuth(PermCode.MGR.ENT_PC_IF_DEFINE_DEL)]
        public async Task<ApiRes> DeleteAsync(string ifCode)
        {
            // 校验该支付方式是否有服务商或商户配置参数或者已有订单
            if (await _payIfConfigService.IsExistUseIfCodeAsync(ifCode) || await _payOrderService.IsExistOrderUseIfCodeAsync(ifCode))
            {
                throw new BizException("该支付接口已有服务商或商户配置参数或已发生交易，无法删除！");
            }
            var result = _payIfDefineService.Remove(ifCode);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_DELETE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新支付接口
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{ifCode}"), MethodLog("更新支付接口")]
        [PermissionAuth(PermCode.MGR.ENT_PC_IF_DEFINE_EDIT)]
        public ApiRes Update(string ifCode, PayInterfaceDefineAddOrEditDto dto)
        {
            JArray jsonArray = new JArray();
            var wayCodes = dto.WayCodeStrs.Split(",");
            foreach (var wayCode in wayCodes)
            {
                JObject jsonObject = new JObject();
                jsonObject.Add("wayCode", wayCode);
                jsonArray.Add(jsonObject);
            }
            dto.WayCodes = jsonArray;
            var result = _payIfDefineService.Update(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_UPDATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 查看支付接口
        /// </summary>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        [HttpGet, Route("{ifCode}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_PC_IF_DEFINE_VIEW, PermCode.MGR.ENT_PC_IF_DEFINE_EDIT)]
        public async Task<ApiRes> DetailAsync(string ifCode)
        {
            var payIfDefine = await _payIfDefineService.GetByIdAsync(ifCode);
            if (payIfDefine == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(payIfDefine);
        }
    }
}
