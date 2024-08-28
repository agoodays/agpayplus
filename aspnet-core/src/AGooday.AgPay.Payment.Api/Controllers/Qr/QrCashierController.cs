using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.Channel.WxPay;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using AGooday.AgPay.Payment.Api.Controllers.PayOrder;
using AGooday.AgPay.Payment.Api.Models;
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
        public async Task<ApiRes> RedirectUrlAsync()
        {
            (byte type, string id) = GetTokenData();
            (string mchNo, string appId) = await GetMchNoAndAppIdAsync(type, id);

            //回调地址
            string redirectUrlEncode = _sysConfigService.GetDBApplicationConfig().GenOauth2RedirectUrlEncode(CS.GetTokenData(type, id));
#if DEBUG
            return ApiRes.Ok(URLUtil.DecodeAll(redirectUrlEncode));
#endif

            //获取商户配置信息
            MchAppConfigContext mchAppConfigContext = _configContextQueryService.QueryMchInfoAndAppInfo(mchNo, appId);

            string wayCode = GetWayCode();
            string oauth2InfoId = GetOauth2InfoId(mchAppConfigContext, wayCode);
            //获取接口并返回数据
            IChannelUserService channelUserService = GetServiceByWayCode(wayCode);
            if (channelUserService == null)
            {
                throw new BizException("无此支付通道接口");
            }
            return ApiRes.Ok(channelUserService.BuildUserRedirectUrl(redirectUrlEncode, oauth2InfoId, wayCode, mchAppConfigContext));
        }

        /// <summary>
        /// 获取userId
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("channelUserId")]
        public async Task<ApiRes> ChannelUserIdAsync()
        {
            (byte type, string id) = GetTokenData();
            (string mchNo, string appId) = await GetMchNoAndAppIdAsync(type, id);

            string wayCode = GetWayCode();

            //获取商户配置信息
            MchAppConfigContext mchAppConfigContext = _configContextQueryService.QueryMchInfoAndAppInfo(mchNo, appId);
            string oauth2InfoId = GetOauth2InfoId(mchAppConfigContext, wayCode);
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
            return ApiRes.Ok(channelUserService.GetChannelUserId(GetReqParamJson(), oauth2InfoId, wayCode, mchAppConfigContext));
        }

        /// <summary>
        /// 获取订单支付信息
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("payOrderInfo")]
        public async Task<ApiRes> PayOrderInfoAsync()
        {
            (byte type, string id) = GetTokenData();
            PayOrderInfo resOrder = new PayOrderInfo();
            if (type.Equals(CS.TOKEN_DATA_TYPE.PAY_ORDER_ID))
            {
                //查询订单
                PayOrderDto payOrder = await GetPayOrderAsync(id);
                resOrder.PayOrderId = payOrder.PayOrderId;
                resOrder.MchOrderNo = payOrder.MchOrderNo;
                resOrder.MchName = payOrder.MchName;
                resOrder.Amount = payOrder.Amount;
                resOrder.ReturnUrl = _payMchNotifyService.CreateReturnUrl(payOrder, _configContextQueryService.QueryMchInfoAndAppInfo(payOrder.MchNo, payOrder.AppId).MchApp.AppSecret);
            }
            else if (type.Equals(CS.TOKEN_DATA_TYPE.QRC_ID))
            {
                var qrCode = await GetQrCodeAsync(id);
                MchAppConfigContext mchAppConfigContext = _configContextQueryService.QueryMchInfoAndAppInfo(qrCode.MchNo, qrCode.AppId);
                resOrder.MchName = mchAppConfigContext.MchInfo.MchName;
                resOrder.FixedFlag = qrCode.FixedFlag;
                resOrder.Amount = qrCode.FixedPayAmount;
            }
            else
            {
                return ApiRes.CustomFail("参数错误");
            }
            return ApiRes.Ok(resOrder);
        }

        /// <summary>
        /// 调起下单接口, 返回支付数据包
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("pay")]
        public async Task<ApiRes> PayAsync()
        {
            ApiRes apiRes;
            UnifiedOrderRQ rq = null;
            PayOrderDto payOrder = null;
            (byte type, string id) = GetTokenData();
            string wayCode = GetWayCode();
            if (type.Equals(CS.TOKEN_DATA_TYPE.PAY_ORDER_ID))
            {
                //查询订单
                payOrder = await GetPayOrderAsync(id);
                payOrder.WayCode = wayCode;
            }
            else if (type.Equals(CS.TOKEN_DATA_TYPE.QRC_ID))
            {
                var qrCode = await GetQrCodeAsync(id);

                string amount = GetReqParamJson().GetValue("amount").ToString();
                GetReqParamJson().TryGetString("buyerRemark", out string buyerRemark);
                rq = new UnifiedOrderRQ();
                rq.MchNo = qrCode.MchNo; // 商户号
                rq.AppId = qrCode.AppId;
                rq.StoreId = qrCode.StoreId;
                rq.MchOrderNo = SeqUtil.GenMhoOrderId();
                rq.WayCode = wayCode;
                rq.Amount = AmountUtil.ConvertDollar2Cent(amount);
                rq.Currency = "CNY";
                rq.ClientIp = IpUtil.GetIP(Request);
                rq.Subject = $"静态码支付";
                rq.Body = $"静态码支付";
                rq.BuyerRemark = buyerRemark;
            }
            else
            {
                return ApiRes.CustomFail("参数错误");
            }
            var mchNo = rq?.MchNo ?? payOrder?.MchNo;
            var appId = rq?.AppId ?? payOrder?.AppId;
            MchAppConfigContext mchAppConfigContext = _configContextQueryService.QueryMchInfoAndAppInfo(mchNo, appId);
            rq.Version = "1.0";
            rq.SignType = "MD5";
            rq.ReqTime = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            var jsonObject = JObject.FromObject(rq);
            string sign = AgPayUtil.Sign(jsonObject, rq.SignType, mchAppConfigContext.MchApp.AppSecret);
            rq.Sign = sign;

            if (wayCode.Equals(CS.PAY_WAY_CODE.ALI_JSAPI))
            {
                apiRes = await PackageAlipayPayPackageAsync(rq, payOrder, mchAppConfigContext);
            }
            else if (wayCode.Equals(CS.PAY_WAY_CODE.WX_JSAPI))
            {
                apiRes = await PackageWxpayPayPackageAsync(rq, payOrder, mchAppConfigContext);
            }
            else
            {
                throw new BizException("不支持的支付方式");
            }

            return apiRes;
        }

        /// <summary>
        /// 获取支付宝的 支付参数
        /// </summary>
        /// <param name="payOrder"></param>
        /// <returns></returns>
        private async Task<ApiRes> PackageAlipayPayPackageAsync(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            var wayCode = (rq.WayCode ?? payOrder?.WayCode) ?? GetWayCode();
            string channelUserId = GetReqParamJson().GetValue("channelUserId").ToString();
            AliJsapiOrderRQ bizRQ = new AliJsapiOrderRQ();
            if (rq != null)
            {
                JObject resJSON = new JObject();
                resJSON.Add("buyerUserId", channelUserId);
                rq.ChannelExtra = resJSON.ToString();
                bizRQ = (AliJsapiOrderRQ)rq.BuildBizRQ();
            }
            bizRQ.BuyerUserId = channelUserId;
            return await this.UnifiedOrderAsync(wayCode, bizRQ, payOrder);
        }

        /// <summary>
        /// 获取微信的 支付参数
        /// </summary>
        /// <param name="payOrder"></param>
        /// <returns></returns>
        private async Task<ApiRes> PackageWxpayPayPackageAsync(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            var wayCode = (rq.WayCode ?? payOrder?.WayCode) ?? GetWayCode();
            string openId = GetReqParamJson().GetValue("channelUserId").ToString();
            string oauth2InfoId = GetOauth2InfoId(mchAppConfigContext, wayCode);
            WxPayChannelUserService channelUserService = (WxPayChannelUserService)GetServiceByWayCode(wayCode);
            channelUserService.GetOauth2Params(oauth2InfoId, wayCode, mchAppConfigContext, out string appId, out string _, out string _);
            WxJsapiOrderRQ bizRQ = new WxJsapiOrderRQ();
            bizRQ.Openid = openId;
            bizRQ.SubAppId = appId;
            if (rq != null)
            {
                JObject resJSON = new JObject();
                resJSON.Add("openid", openId);
                resJSON.Add("subAppId", appId);
                rq.ChannelExtra = resJSON.ToString();
                bizRQ = (WxJsapiOrderRQ)rq.BuildBizRQ();
            }
            return await this.UnifiedOrderAsync(wayCode, bizRQ, payOrder);
        }

        private async Task<(string mchNo, string appId)> GetMchNoAndAppIdAsync(byte type, string id)
        {
            switch (type)
            {
                case CS.TOKEN_DATA_TYPE.PAY_ORDER_ID:
                    var payOrder = await GetPayOrderAsync(id);
                    return (payOrder.MchNo, payOrder.AppId);
                case CS.TOKEN_DATA_TYPE.QRC_ID:
                    var qrCode = await GetQrCodeAsync(id);
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

        private async Task<PayOrderDto> GetPayOrderAsync(string payOrderId)
        {
            //string token = GetToken();

            //string payOrderId = AgPayUtil.AesDecode(token); //解析token

            PayOrderDto payOrder = await _payOrderService.GetByIdAsync(payOrderId);
            if (payOrder == null || payOrder.State != (byte)PayOrderState.STATE_INIT)
            {
                throw new BizException("订单不存在或状态不正确");
            }

            return payOrder;
        }

        private async Task<QrCodeDto> GetQrCodeAsync(string qrcId)
        {
            var qrCode = await _qrCodeService.GetByIdAsync(qrcId);
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

        private string GetOauth2InfoId(MchAppConfigContext mchAppConfigContext, string wayCode)
        {
            // 根据支付方式， 查询出 该商户 可用的支付接口
            var mchPayPassage = _mchPayPassageService.FindMchPayPassage(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, wayCode);
            if (mchPayPassage == null)
            {
                throw new BizException("商户应用不支持该支付方式");
            }
            string oauth2InfoId = null;
            if (mchAppConfigContext.IsIsvSubMch())
            {
                var payInterfaceConfig = _configContextQueryService.QueryIsvPayIfConfig(mchAppConfigContext.MchInfo.IsvNo, mchPayPassage.IfCode);
                oauth2InfoId = payInterfaceConfig?.Oauth2InfoId;
            }

            return oauth2InfoId;
        }
    }
}
