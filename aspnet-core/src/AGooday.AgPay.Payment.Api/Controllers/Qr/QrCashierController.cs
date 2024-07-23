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
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
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
        private readonly IChannelServiceFactory<IChannelUserService> _channelUserServiceFactory;
        private readonly PayMchNotifyService _payMchNotifyService;
        private readonly IQrCodeService _qrCodeService;

        public QrCashierController(ILogger<QrCashierController> logger,
            IChannelServiceFactory<IChannelUserService> channelUserServiceFactory,
            PayMchNotifyService payMchNotifyService,
            IQrCodeService qrCodeService,
            IChannelServiceFactory<IPaymentService> paymentServiceFactory,
            PayOrderProcessService payOrderProcessService,
            IMchPayPassageService mchPayPassageService,
            IPayRateConfigService payRateConfigService,
            IPayWayService payWayService,
            IPayOrderService payOrderService,
            IPayOrderProfitService payOrderProfitService,
            ISysConfigService sysConfigService,
            IMQSender mqSender,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, paymentServiceFactory, payOrderProcessService, mchPayPassageService, payRateConfigService, payWayService, payOrderService, payOrderProfitService, sysConfigService, mqSender, requestKit, configContextQueryService)
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
        public IActionResult RedirectUrl()
        {
            (byte type, string id) = GetTokenData();
            (string mchNo, string appId) = GetMchNoAndAppId(type, id);

            //回调地址
            string redirectUrlEncode = _sysConfigService.GetDBApplicationConfig().GenOauth2RedirectUrlEncode(CS.GetTokenData(type, id));
#if DEBUG
            return Redirect(URLUtil.DecodeAll(redirectUrlEncode));
#endif

            //获取商户配置信息
            MchAppConfigContext mchAppConfigContext = _configContextQueryService.QueryMchInfoAndAppInfo(mchNo, appId);

            string wayCode = GetWayCode();

            //获取接口并返回数据
            IChannelUserService channelUserService = GetServiceByWayCode(wayCode);
            return Ok(ApiRes.Ok(channelUserService.BuildUserRedirectUrl(redirectUrlEncode, mchAppConfigContext)));
        }

        /// <summary>
        /// 获取userId
        /// </summary>
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
#if DEBUG
            string channelUserId;
            switch (wayCode)
            {
                case CS.PAY_WAY_CODE.ALI_JSAPI:
                    channelUserId = "2088612672407456";
                    break;
                case CS.PAY_WAY_CODE.WX_JSAPI:
                    channelUserId = "oo_BZ0p305z0ZmW-eBvuuRbHrumw";
                    break;
                default:
                    channelUserId = string.Empty;
                    break;
            }
            return ApiRes.Ok(channelUserId);
#endif
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
            if (type.Equals(CS.TOKEN_DATA_TYPE.PAY_ORDER_ID))
            {
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
            else if (type.Equals(CS.TOKEN_DATA_TYPE.QRC_ID))
            {
                var qrCode = GetQrCode(id);
                MchAppConfigContext mchAppConfigContext = _configContextQueryService.QueryMchInfoAndAppInfo(qrCode.MchNo, qrCode.AppId);
                return ApiRes.Ok(new { qrCode.FixedFlag, Amount = qrCode.FixedPayAmount, mchAppConfigContext.MchInfo.MchName });
            }
            else
            {
                return ApiRes.CustomFail("参数错误");
            }
        }

        /// <summary>
        /// 调起下单接口, 返回支付数据包
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("pay")]
        public ApiRes Pay()
        {
            (byte type, string id) = GetTokenData();
            if (type.Equals(CS.TOKEN_DATA_TYPE.PAY_ORDER_ID))
            {
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
            else if (type.Equals(CS.TOKEN_DATA_TYPE.QRC_ID))
            {
                var qrCode = GetQrCode(id);

                string wayCode = GetWayCode();

                ApiRes apiRes = null;
                string channelUserId = GetReqParamJson().GetValue("channelUserId").ToString();
                string amount = GetReqParamJson().GetValue("amount").ToString();
                GetReqParamJson().TryGetString("buyerRemark", out string buyerRemark);
                UnifiedOrderRQ rq = new UnifiedOrderRQ();
                rq.MchNo = qrCode.MchNo; // 商户号
                rq.AppId = qrCode.AppId;
                rq.StoreId = qrCode.StoreId;
                rq.MchOrderNo = SeqUtil.GenMhoOrderId();
                rq.WayCode = wayCode;
                rq.Amount = Convert.ToInt64(amount);
                rq.Currency = "CNY";
                rq.ClientIp = IpUtil.GetIP(Request);
                rq.Subject = $"静态码支付";
                rq.Body = $"静态码支付";
                rq.BuyerRemark = buyerRemark;
                if (wayCode.Equals(CS.PAY_WAY_CODE.ALI_JSAPI))
                {
                    JObject resJSON = new JObject();
                    resJSON.Add("buyerUserId", channelUserId);
                    rq.ChannelExtra = resJSON.ToString();
                }
                else if (wayCode.Equals(CS.PAY_WAY_CODE.WX_JSAPI))
                {
                    JObject resJSON = new JObject();
                    resJSON.Add("openid", channelUserId);
                    rq.ChannelExtra = resJSON.ToString();
                }
                else
                {
                    throw new BizException("不支持的支付方式");
                }
                UnifiedOrderRQ bizRQ = rq.BuildBizRQ();

                apiRes = this.UnifiedOrder(GetWayCode(), bizRQ);

                return ApiRes.Ok(apiRes);
            }
            else
            {
                return ApiRes.CustomFail("参数错误");
            }
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
                return _channelUserServiceFactory.GetService(CS.IF_CODE.ALIPAY);
            }
            else if (CS.PAY_WAY_CODE.WX_JSAPI.Equals(wayCode))
            {
                return _channelUserServiceFactory.GetService(CS.IF_CODE.WXPAY);
            }
            return null;
        }
    }
}
