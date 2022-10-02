using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace AGooday.AgPay.Merchant.Api.Controllers
{
    /// <summary>
    /// 主页数据
    /// </summary>
    [Route("api/mainChart")]
    [ApiController]
    public class MainChartController : CommonController
    {
        private readonly ILogger<MainChartController> _logger;
        private readonly ISysUserService _sysUserService;
        private readonly IMchInfoService _mchInfoService;

        public MainChartController(ILogger<MainChartController> logger, RedisUtil client,
            IMchInfoService mchInfoService,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
            _mchInfoService = mchInfoService;
            _sysUserService = sysUserService;
        }

        /// <summary>
        /// 周交易总金额
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payAmountWeek")]
        public ApiRes PayAmountWeek()
        {
            return ApiRes.Ok();
        }

        /// <summary>
        /// 商户总数量、服务商总数量、总交易金额、总交易笔数
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("numCount")]
        public ApiRes NumCount()
        {
            return ApiRes.Ok();
        }

        /// <summary>
        /// 交易统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payCount")]
        public ApiRes PayCount()
        {
            return ApiRes.Ok();
        }

        /// <summary>
        /// 支付方式统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payTypeCount")]
        public ApiRes PayWayCount()
        {
            return ApiRes.Ok();
        }

        /// <summary>
        /// 商户基本信息、用户基本信息
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("")]
        public ApiRes UserDetail()
        {
            var sysUser = _sysUserService.GetById(GetCurrentUser().User.SysUserId);
            var mchInfo = _mchInfoService.GetById(GetCurrentUser().User.BelongInfoId);
            var jobj = JObject.FromObject(mchInfo);
            jobj.Add("loginUsername", sysUser.LoginUsername);
            jobj.Add("realname", sysUser.Realname);
            return ApiRes.Ok(jobj);
        }
    }
}
