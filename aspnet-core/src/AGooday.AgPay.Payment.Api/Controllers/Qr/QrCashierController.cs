using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel;
using AGooday.AgPay.Payment.Api.Controllers.PayOrder;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Controllers.Qr
{
    [Route("api/cashier")]
    [ApiController]
    public class QrCashierController : AbstractPayOrderController
    {
        private readonly Func<string, IChannelUserService> _channelUserServiceFactory;
        public QrCashierController(Func<string, IChannelUserService> channelUserServiceFactory,
            Func<string, IPaymentService> paymentServiceFactory,
            ConfigContextQueryService configContextQueryService,
            PayOrderProcessService payOrderProcessService,
            RequestIpUtil requestIpUtil,
            ILogger<AbstractPayOrderController> logger,
            IMchPayPassageService mchPayPassageService,
            IPayOrderService payOrderService,
            ISysConfigService sysConfigService)
            : base(paymentServiceFactory, configContextQueryService, payOrderProcessService, requestIpUtil, logger, mchPayPassageService, payOrderService, sysConfigService)
        {
            _channelUserServiceFactory = channelUserServiceFactory;
        }
        /// <summary>
        /// 返回 oauth2【获取uerId跳转地址】
        /// </summary>
        /// <param name="token"></param>
        /// <param name="wayCode"></param>
        /// <returns></returns>
        [HttpPost, Route("/redirectUrl")]
        public ApiRes RedirectUrl()
        {
            //查询订单
            PayOrderDto payOrder = GetPayOrder();

            //回调地址
            string redirectUrlEncode = _sysConfigService.GetDBApplicationConfig().GenOauth2RedirectUrlEncode(payOrder.PayOrderId);

            //获取商户配置信息
            MchAppConfigContext mchAppConfigContext = _configContextQueryService.QueryMchInfoAndAppInfo(payOrder.MchNo, payOrder.AppId);

            string wayCode = GetWayCode();

            //获取接口并返回数据
            IChannelUserService channelUserService = GetServiceByWayCode(wayCode);
            return ApiRes.Ok(channelUserService.BuildUserRedirectUrl(redirectUrlEncode, mchAppConfigContext));
        }
        /// <summary>
        /// 获取userId
        /// </summary>
        /// <param name="token"></param>
        /// <param name="wayCode"></param>
        /// <returns></returns>
        [HttpPost, Route("/channelUserId")]
        public ApiRes ChannelUserId()
        {
            //查询订单
            PayOrderDto payOrder = GetPayOrder();

            string wayCode = GetWayCode();

            //获取商户配置信息
            MchAppConfigContext mchAppConfigContext = _configContextQueryService.QueryMchInfoAndAppInfo(payOrder.MchNo, payOrder.AppId);
            IChannelUserService channelUserService = GetServiceByWayCode(wayCode);
            return ApiRes.Ok(channelUserService.GetChannelUserId(GetReqParamJson(), mchAppConfigContext));
        }

        private string GetToken()
        {
            return GetReqParamJson().GetValue("token").ToString();
        }

        private string GetWayCode()
        {
            return GetReqParamJson().GetValue("wayCode").ToString();
        }

        private PayOrderDto GetPayOrder()
        {
            string token = GetToken();

            string payOrderId = AgPayUtil.AesDecode(token); //解析token

            PayOrderDto payOrder = _payOrderService.GetById(payOrderId);
            if (payOrder == null || payOrder.State != (byte)PayOrderState.STATE_INIT)
            {
                throw new BizException("订单不存在或状态不正确");
            }

            return _payOrderService.GetById(payOrderId);
        }

        private IChannelUserService GetServiceByWayCode(string wayCode)
        {
            if (CS.PAY_WAY_CODE.ALI_JSAPI.Equals(wayCode))
            {
                return _channelUserServiceFactory(CS.IF_CODE.ALIPAY);
            }
            else if (CS.PAY_WAY_CODE.WX_JSAPI.Equals(wayCode))
            {
                return _channelUserServiceFactory(CS.IF_CODE.WXPAY);
            }
            return null;
        }
    }
}
