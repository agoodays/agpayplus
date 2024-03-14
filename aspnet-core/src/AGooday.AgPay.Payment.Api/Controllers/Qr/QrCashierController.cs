using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Payment.Api.Channel;
using AGooday.AgPay.Payment.Api.Controllers.PayOrder;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Controllers.Qr
{
    /// <summary>
    /// 聚合码支付二维码收银台
    /// </summary>
    [Route("api/cashier")]
    [ApiController]
    public class QrCashierController : AbstractPayOrderController
    {
        private readonly Func<string, IChannelUserService> _channelUserServiceFactory;
        private readonly PayMchNotifyService _payMchNotifyService;

        private readonly IQrCodeService _qrCodeService;

        public QrCashierController(IMQSender mqSender,
            Func<string, IChannelUserService> channelUserServiceFactory,
            Func<string, IPaymentService> paymentServiceFactory,
            IQrCodeService qrCodeService,
            ConfigContextQueryService configContextQueryService,
            PayOrderProcessService payOrderProcessService,
            PayMchNotifyService payMchNotifyService,
            RequestKit requestKit,
            ILogger<AbstractPayOrderController> logger,
            IMchPayPassageService mchPayPassageService,
            IPayWayService payWayService,
            IPayOrderService payOrderService,
            ISysConfigService sysConfigService)
            : base(mqSender, paymentServiceFactory, configContextQueryService, payOrderProcessService, requestKit, logger, mchPayPassageService, payWayService, payOrderService, sysConfigService)
        {
            _channelUserServiceFactory = channelUserServiceFactory;
            _payMchNotifyService = payMchNotifyService;
            _qrCodeService = qrCodeService;
        }

        /// <summary>
        /// 返回 oauth2【获取uerId跳转地址】
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("redirectUrl")]
        public ApiRes RedirectUrl()
        {
            (byte type, string id) = GetTokenData();
            (string mchNo, string appId) = GetMchNoAndAppId(type, id);

            //回调地址
            string redirectUrlEncode = _sysConfigService.GetDBApplicationConfig().GenOauth2RedirectUrlEncode(CS.GetTokenData(type, id));

            //获取商户配置信息
            MchAppConfigContext mchAppConfigContext = _configContextQueryService.QueryMchInfoAndAppInfo(mchNo, appId);

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
        [HttpPost, Route("channelUserId")]
        public ApiRes ChannelUserId()
        {
            (byte type, string id) = GetTokenData();
            (string mchNo, string appId) = GetMchNoAndAppId(type, id);

            string wayCode = GetWayCode();

            //获取商户配置信息
            MchAppConfigContext mchAppConfigContext = _configContextQueryService.QueryMchInfoAndAppInfo(mchNo, appId);
            IChannelUserService channelUserService = GetServiceByWayCode(wayCode);
            return ApiRes.Ok(channelUserService.GetChannelUserId(GetReqParamJson(), mchAppConfigContext));
        }

        /// <summary>
        /// 获取订单支付信息
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("payOrderInfo")]
        public ApiRes PayOrderInfo()
        {
            (byte type, string id) = GetTokenData();
            if (!type.Equals(CS.TOKEN_DATA_TYPE.PAY_ORDER_ID))
            {
                return ApiRes.CustomFail("参数错误");
            }

            //查询订单
            PayOrderDto payOrder = GetPayOrder(id);

            PayOrderDto resOrder = new PayOrderDto();
            resOrder.PayOrderId = payOrder.PayOrderId;
            resOrder.MchOrderNo = payOrder.MchOrderNo;
            resOrder.MchName = payOrder.MchName;
            resOrder.Amount = payOrder.Amount;
            resOrder.ReturnUrl = _payMchNotifyService.CreateReturnUrl(payOrder, _configContextQueryService.QueryMchInfoAndAppInfo(payOrder.MchNo, payOrder.AppId).MchApp.AppSecret);
            return ApiRes.Ok(resOrder);
        }

        /// <summary>
        /// 调起下单接口, 返回支付数据包
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("pay")]
        public ApiRes Pay()
        {
            (byte type, string id) = GetTokenData();
            if (!type.Equals(CS.TOKEN_DATA_TYPE.PAY_ORDER_ID))
            {
                return ApiRes.CustomFail("参数错误");
            }
            //查询订单
            PayOrderDto payOrder = GetPayOrder(id);

            string wayCode = GetWayCode();

            ApiRes apiRes = null;

            if (wayCode.Equals(CS.PAY_WAY_CODE.ALI_JSAPI))
            {
                apiRes = PackageAlipayPayPackage(payOrder);
            }
            else if (wayCode.Equals(CS.PAY_WAY_CODE.WX_JSAPI))
            {
                apiRes = PackageWxpayPayPackage(payOrder);
            }

            return ApiRes.Ok(apiRes);
        }

        /// <summary>
        /// 获取支付宝的 支付参数
        /// </summary>
        /// <param name="payOrder"></param>
        /// <returns></returns>
        private ApiRes PackageAlipayPayPackage(PayOrderDto payOrder)
        {
            string channelUserId = GetReqParamJson().GetValue("channelUserId").ToString();
            AliJsapiOrderRQ rq = new AliJsapiOrderRQ();
            rq.BuyerUserId = channelUserId;
            return this.UnifiedOrder(GetWayCode(), rq, payOrder);
        }

        /// <summary>
        /// 获取微信的 支付参数
        /// </summary>
        /// <param name="payOrder"></param>
        /// <returns></returns>
        private ApiRes PackageWxpayPayPackage(PayOrderDto payOrder)
        {
            string openId = GetReqParamJson().GetValue("channelUserId").ToString();
            WxJsapiOrderRQ rq = new WxJsapiOrderRQ();
            rq.Openid = openId;
            return this.UnifiedOrder(GetWayCode(), rq, payOrder);
        }

        private (string mchNo, string appId) GetMchNoAndAppId(byte type, string id)
        {
            switch (type)
            {
                case CS.TOKEN_DATA_TYPE.PAY_ORDER_ID:
                    var payOrder = GetPayOrder(id);
                    return (payOrder.MchNo, payOrder.AppId);
                case CS.TOKEN_DATA_TYPE.QRC_ID:
                    var qrCode = GetQrCode(id);
                    return (qrCode.MchNo, qrCode.AppId);
                default:
                    throw new BizException("参数错误");
            }
        }

        private (byte type, string id) GetTokenData()
        {
            var token = GetToken();
            JObject tokenData = JObject.Parse(AgPayUtil.AesDecode(token));
            tokenData.TryGetString("type", out string type);
            tokenData.TryGetString("id", out string id);
            return (Convert.ToByte(type), id);
        }

        private string GetToken()
        {
            return GetReqParamJson().GetValue("token").ToString();
        }

        private string GetWayCode()
        {
            return GetReqParamJson().GetValue("wayCode").ToString();
        }

        private PayOrderDto GetPayOrder(string payOrderId)
        {
            //string token = GetToken();

            //string payOrderId = AgPayUtil.AesDecode(token); //解析token

            PayOrderDto payOrder = _payOrderService.GetById(payOrderId);
            if (payOrder == null || payOrder.State != (byte)PayOrderState.STATE_INIT)
            {
                throw new BizException("订单不存在或状态不正确");
            }

            return _payOrderService.GetById(payOrderId);
        }

        private QrCodeDto GetQrCode(string qrcId)
        {
            var qrCode = _qrCodeService.GetById(qrcId);
            if (qrCode == null || qrCode.State != CS.YES || qrCode.BindState != CS.YES)
            {
                throw new BizException("码牌不存在或状态不正确");
            }
            return qrCode;
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
