using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.AccountBill
{
    /// <summary>
    /// 账户账单
    /// </summary>
    [Route("/api/accountBill")]
    [ApiController, Authorize]
    public class AccountBillController : CommonController
    {
        private readonly IAccountBillService _accountBillService;

        public AccountBillController(ILogger<AccountBillController> logger,
            IAccountBillService accountBillService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
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
        public ApiPageRes<AccountBillDto> List([FromQuery] AccountBillQueryDto dto)
        {
            var data = _accountBillService.GetPaginatedData<AccountBillDto>(dto);
            return ApiPageRes<AccountBillDto>.Pages(data);
        }

        /// <summary>
        /// 查询账单信息
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        [HttpGet, Route("{billId}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ACCOUNT_BILL_VIEW)]
        public ApiRes Detail(string billId)
        {
            var accountBill = _accountBillService.GetById(billId);
            return ApiRes.Ok(accountBill);
        }
    }
}
