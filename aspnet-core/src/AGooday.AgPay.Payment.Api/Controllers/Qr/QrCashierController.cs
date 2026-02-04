using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Base.Api.Models;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using AGooday.AgPay.Payment.Api.Controllers.PayOrder;
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
            IServiceProvider serviceProvider,
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
            : base(logger, serviceProvider, paymentServiceFactory, payOrderProcessService, mchPayPassageService, payRateConfigService, payWayService, payOrderService, payOrderProfitService, sysConfigService, mqSender, requestKit, configContextQueryService)
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
            var token = await this.GetTokenAsync();
            //回调地址
            string redirectUrlEncode = _sysConfigService.GetDBApplicationConfig().GenOauth2RedirectUrlEncode(token);
#if DEBUG
            return ApiRes.Ok(URLUtil.DecodeAll(redirectUrlEncode));
#endif

            //获取商户配置信息
            MchAppConfigContext mchAppConfigContext = await this.CommonQueryInfoMchAppConfigContextAsync();

            string wayCode = await this.GetWayCodeAsync();
            //获取接口并返回数据
            IChannelUserService channelUserService = this.GetServiceByWayCode(wayCode);
            return ApiRes.Ok(await channelUserService.BuildUserRedirectUrlAsync(redirectUrlEncode, wayCode, mchAppConfigContext));
        }

        /// <summary>
        /// 获取userId
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("channelUserId")]
        public async Task<ApiRes> ChannelUserIdAsync()
        {
            string wayCode = await this.GetWayCodeAsync();
#if DEBUG
            string channelUserId = wayCode switch
            {
                CS.PAY_WAY_CODE.ALI_JSAPI => "2088612672407456",
                CS.PAY_WAY_CODE.WX_JSAPI => "oo_BZ0p305z0ZmW-eBvuuRbHrumw",
                _ => string.Empty,
            };
            return ApiRes.Ok(channelUserId);
#endif

            //获取商户配置信息
            MchAppConfigContext mchAppConfigContext = await this.CommonQueryInfoMchAppConfigContextAsync();
            IChannelUserService channelUserService = this.GetServiceByWayCode(wayCode);
            var param = await this.GetReqParamToJsonAsync();
            return ApiRes.Ok(await channelUserService.GetChannelUserIdAsync(param, wayCode, mchAppConfigContext));
        }

        /// <summary>
        /// 获取订单支付信息
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("payOrderInfo")]
        public async Task<ApiRes> PayOrderInfoAsync()
        {
            (byte type, string id) = await this.TokenConvertAsync();

            PayOrderInfo resOrder = new PayOrderInfo();
            if (type.Equals(CS.TOKEN_DATA_TYPE.PAY_ORDER_ID))
            {
                //查询订单
                PayOrderDto payOrder = await this.GetPayOrderAsync(id);
                resOrder.PayOrderId = payOrder.PayOrderId;
                resOrder.MchOrderNo = payOrder.MchOrderNo;
                resOrder.MchName = payOrder.MchName;
                resOrder.Amount = payOrder.Amount;
                resOrder.ReturnUrl = PayMchNotifyService.CreateReturnUrl(payOrder, (await _configContextQueryService.QueryMchInfoAndAppInfoAsync(payOrder.MchNo, payOrder.AppId)).MchApp.AppSecret);
            }
            else if (type.Equals(CS.TOKEN_DATA_TYPE.QRC_ID))
            {
                var qrCode = await this.GetQrCodeAsync(id);
                MchAppConfigContext mchAppConfigContext = await _configContextQueryService.QueryMchInfoAndAppInfoAsync(qrCode.MchNo, qrCode.AppId);
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
            //查询订单
            PayOrderDto payOrder = await this.CommonQueryPayOrderAsync();

            string wayCode = await this.GetWayCodeAsync();

            ApiRes apiRes;
            if (wayCode.Equals(CS.PAY_WAY_CODE.ALI_JSAPI))
            {
                apiRes = await this.PackageAlipayPayPackageAsync(payOrder);
            }
            else if (wayCode.Equals(CS.PAY_WAY_CODE.WX_JSAPI))
            {
                apiRes = await this.PackageWxpayPayPackageAsync(payOrder);
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
        private async Task<ApiRes> PackageAlipayPayPackageAsync(PayOrderDto payOrder)
        {
            var wayCode = await this.GetWayCodeAsync();
            string buyerUserId = await this.GetChannelUserIdAsync();
            AliJsapiOrderRQ rq = new AliJsapiOrderRQ();
            await this.CommonSetRQAsync(rq, payOrder);
            rq.BuyerUserId = buyerUserId;
            return await this.UnifiedOrderAsync(wayCode, rq, payOrder);
        }

        /// <summary>
        /// 获取微信的 支付参数
        /// </summary>
        /// <param name="payOrder"></param>
        /// <returns></returns>
        private async Task<ApiRes> PackageWxpayPayPackageAsync(PayOrderDto payOrder)
        {
            var wayCode = await this.GetWayCodeAsync();
            string openId = await this.GetChannelUserIdAsync();
            WxJsapiOrderRQ rq = new WxJsapiOrderRQ();
            await this.CommonSetRQAsync(rq, payOrder);
            rq.Openid = openId;
            return await this.UnifiedOrderAsync(wayCode, rq, payOrder);
        }

        /// <summary>
        /// 赋值通用字段
        /// </summary>
        /// <param name="rq"></param>
        /// <param name="payOrder"></param>
        private async Task CommonSetRQAsync(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            // 存在订单数据， 不需要处理
            if (payOrder != null && !string.IsNullOrWhiteSpace(payOrder.PayOrderId))
            {
                return;
            }
            var json = await this.GetReqParamJsonAsync();
            string amount = json.GetValue("amount").ToString();
            json.TryGetString("buyerRemark", out string buyerRemark);

            rq.MchNo = payOrder.MchNo; // 商户号
            rq.AppId = payOrder.AppId;
            rq.StoreId = payOrder.StoreId;
            rq.QrcId = payOrder.QrcId;
            rq.MchOrderNo = SeqUtil.GenMhoOrderId();
            //rq.WayCode = wayCode;
            rq.Amount = AmountUtil.ConvertDollar2Cent(amount);
            rq.Currency = "CNY";
            rq.ClientIp = IpUtil.GetIP(Request);
            rq.Subject = $"静态码支付";
            rq.Body = $"静态码支付";
            rq.BuyerRemark = buyerRemark;
            rq.SignType = "MD5"; // 设置默认签名方式为MD5
        }

        private async Task<string> GetTokenAsync()
        {
            return (await this.GetReqParamJsonAsync()).GetValue("token").ToString();
        }

        private async Task<string> GetWayCodeAsync()
        {
            return (await this.GetReqParamJsonAsync()).GetValue("wayCode").ToString();
        }

        private async Task<string> GetChannelUserIdAsync()
        {
            return (await this.GetReqParamJsonAsync()).GetValue("channelUserId").ToString();
        }

        private async Task<PayOrderDto> GetPayOrderAsync(string payOrderId)
        {
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
            else
            {
                throw new BizException("无此支付通道接口");
            }
        }

        private async Task<(byte type, string id)> TokenConvertAsync()
        {
            var token = await this.GetTokenAsync();
            JObject tokenData = JObject.Parse(AgPayUtil.AesDecode(token));
            tokenData.TryGetString("type", out string type);
            tokenData.TryGetString("id", out string id);
            return (Convert.ToByte(type), id);
        }

        /// <summary>
        /// 通用查询订单信息
        /// </summary>
        /// <returns></returns>
        private async Task<PayOrderDto> CommonQueryPayOrderAsync()
        {
            (byte type, string id) = await this.TokenConvertAsync();

            if (type == CS.TOKEN_DATA_TYPE.PAY_ORDER_ID)
            {
                //查询订单
                return await this.GetPayOrderAsync(id);
            }
            else if (type.Equals(CS.TOKEN_DATA_TYPE.QRC_ID))
            {
                var qrCode = await this.GetQrCodeAsync(id);

                var payOrder = new PayOrderDto();
                payOrder.MchNo = qrCode.MchNo;
                payOrder.QrcId = qrCode.QrcId;
                payOrder.AppId = qrCode.AppId;
                payOrder.StoreId = qrCode.StoreId;
                return payOrder;
            }
            else
            {
                throw new BizException("token类型不正确");
            }
        }

        /// <summary>
        /// 查询配置信息
        /// </summary>
        /// <returns></returns>
        private async Task<MchAppConfigContext> CommonQueryInfoMchAppConfigContextAsync()
        {
            var payOrder = await this.CommonQueryPayOrderAsync();
            return await _configContextQueryService.QueryMchInfoAndAppInfoAsync(payOrder.MchNo, payOrder.AppId);
        }
    }
}
