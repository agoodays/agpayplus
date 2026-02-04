using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Base.Api.Attributes;
using AGooday.AgPay.Base.Api.Authorization;
using AGooday.AgPay.Base.Api.Controllers;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.Cache.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.AccountBill
{
    /// <summary>
    /// 账户账单
    /// </summary>
    [Route("api/accountBill")]
    [ApiController, Authorize]
    public class AccountBillController : CommonController
    {
        private readonly IAccountBillService _accountBillService;

        public AccountBillController(ILogger<AccountBillController> logger,
            ICacheService cacheService,
            IAuthService authService,
            IAccountBillService accountBillService)
            : base(logger, cacheService, authService)
        {
            _accountBillService = accountBillService;
        }

        /// <summary>
        /// 账户账单列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ACCOUNT_BILL_LIST)]
        public async Task<ApiPageRes<AccountBillDto>> ListAsync([FromQuery] AccountBillQueryDto dto)
        {
            var data = await _accountBillService.GetPaginatedDataAsync(dto);
            return ApiPageRes<AccountBillDto>.Pages(data);
        }

        /// <summary>
        /// 查询账单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ACCOUNT_BILL_VIEW)]
        public async Task<ApiRes> DetailAsync(long id)
        {
            var accountBill = await _accountBillService.GetByIdAsNoTrackingAsync(id);
            return ApiRes.Ok(accountBill);
        }
    }
}
