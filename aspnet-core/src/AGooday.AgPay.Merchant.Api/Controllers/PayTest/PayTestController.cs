using AGooday.AgPay.AopSdk;
using AGooday.AgPay.AopSdk.Exceptions;
using AGooday.AgPay.AopSdk.Models;
using AGooday.AgPay.AopSdk.Request;
using AGooday.AgPay.AopSdk.Response;
using AGooday.AgPay.Application;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Authorization;
using AGooday.AgPay.Merchant.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Merchant.Api.Controllers.PayTest
{
    /// <summary>
    /// 支付测试类
    /// </summary>
    [Route("api/paytest")]
    [ApiController, Authorize, NoLog]
    public class PayTestController : CommonController
    {
        private readonly ILogger<PayTestController> _logger;
        private readonly IPayOrderService _payOrderService;
        private readonly IMchAppService _mchAppService;
        private readonly IMchPayPassageService _mchPayPassageService;
        private readonly ISysConfigService _sysConfigService;

        public PayTestController(ILogger<PayTestController> logger, RedisUtil client,
            IPayOrderService payOrderService,
            IMchAppService mchAppService,
            IMchPayPassageService mchPayPassageService,
            ISysConfigService sysConfigService,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
            _payOrderService = payOrderService;
            _mchAppService = mchAppService;
            _mchPayPassageService = mchPayPassageService;
            _sysConfigService = sysConfigService;
        }

        /// <summary>
        /// 查询商户对应应用下支持的支付方式
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <returns></returns>
        [HttpGet, Route("payways/{appId}")]
        [PermissionAuth(PermCode.MCH.ENT_MCH_PAY_TEST_PAYWAY_LIST)]
        public ApiRes PayWayList(string appId)
        {
            var payWays = _mchPayPassageService.GetMchPayPassageByAppId(GetCurrentMchNo(), appId)
                .Select(s => s.WayCode);
            return ApiRes.Ok(payWays);
        }

        /// <summary>
        /// 调起下单接口
        /// </summary>
        /// <param name="payOrder"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost, Route("payOrders")]
        [PermissionAuth(PermCode.MCH.ENT_MCH_PAY_TEST_DO)]
        public ApiRes DoPay(PayOrderModel payOrder)
        {
            if (string.IsNullOrWhiteSpace(payOrder.OrderTitle))
            {
                throw new BizException("订单标题不能为空");
            }
            var mchApp = _mchAppService.GetById(payOrder.AppId);
            if (mchApp == null || mchApp.State != CS.PUB_USABLE || !mchApp.AppId.Equals(payOrder.AppId))
            {
                throw new BizException("商户应用不存在或不可用");
            }
            PayOrderCreateRequest request = new PayOrderCreateRequest();
            PayOrderCreateReqModel model = new PayOrderCreateReqModel();
            request.SetBizModel(model);
            model.MchNo = GetCurrentMchNo(); // 商户号
            model.AppId = payOrder.AppId;
            model.StoreId = payOrder.StoreId;
            model.MchOrderNo = payOrder.MchOrderNo;
            model.WayCode = payOrder.WayCode;
            model.Amount = AmountUtil.ConvertDollar2Cent(payOrder.Amount);
            // paypal通道使用USD类型货币
            if (payOrder.WayCode.Equals(CS.PAY_WAY_CODE.PP_PC.ToLower()))
            {
                model.Currency = "USD";
            }
            else
            {
                model.Currency = "CNY";
            }
            model.ClientIp = IpUtil.GetIP(Request);
            model.Subject = $"{payOrder.OrderTitle}[{model.MchNo}商户联调]";
            model.Body = $"{payOrder.OrderTitle}[{model.MchNo}商户联调]";

            DBApplicationConfig dbApplicationConfig = _sysConfigService.GetDBApplicationConfig();

            model.NotifyUrl = $"{dbApplicationConfig.MchSiteUrl}/api/anon/paytestNotify/payOrder"; //回调地址
            model.DivisionMode = payOrder.DivisionMode; //分账模式

            //设置扩展参数
            JObject extParams = new JObject();
            if (!string.IsNullOrWhiteSpace(payOrder.PayDataType))
            {
                extParams.Add("payDataType", payOrder.PayDataType.Trim());
            }
            if (!string.IsNullOrWhiteSpace(payOrder.AuthCode))
            {
                extParams.Add("authCode", payOrder.AuthCode.Trim());
            }
            model.ChannelExtra = extParams.ToString();

            var agPayClient = new AgPayClient(dbApplicationConfig.PaySiteUrl, mchApp.AppSecret);

            try
            {
                PayOrderCreateResponse response = agPayClient.Execute(request);
                if (response.code != 0)
                {
                    throw new BizException(response.msg);
                }
                return ApiRes.Ok(response.Get());
            }
            catch (AgPayException e)
            {
                throw new BizException(e.Message);
            }
        }
    }
}
