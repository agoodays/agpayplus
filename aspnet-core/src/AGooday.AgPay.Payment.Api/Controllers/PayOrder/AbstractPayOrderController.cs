using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Payment.Api.Channel;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using static AGooday.AgPay.Application.Permissions.PermCode;

namespace AGooday.AgPay.Payment.Api.Controllers.PayOrder
{
    /// <summary>
    /// 创建支付订单抽象类
    /// </summary>
    public abstract class AbstractPayOrderController : ApiControllerBase
    {
        protected readonly Func<string, IPaymentService> _paymentServiceFactory;
        protected readonly ConfigContextQueryService _configContextQueryService;
        protected readonly PayOrderProcessService _payOrderProcessService;
        protected readonly IMchPayPassageService _mchPayPassageService;
        protected readonly IPayOrderService _payOrderService;
        protected readonly ISysConfigService sysConfigService;

        protected AbstractPayOrderController(Func<string, IPaymentService> paymentServiceFactory,
            ConfigContextQueryService configContextQueryService,
            PayOrderProcessService payOrderProcessService,
            IMchPayPassageService mchPayPassageService,
            IPayOrderService payOrderService,
            ISysConfigService sysConfigService)
        {
            _paymentServiceFactory = paymentServiceFactory;
            _configContextQueryService = configContextQueryService;
            _payOrderProcessService = payOrderProcessService;
            _mchPayPassageService = mchPayPassageService;
            _payOrderService = payOrderService;
            this.sysConfigService = sysConfigService;
        }

        /// <summary>
        /// 统一下单 (新建订单模式)
        /// </summary>
        /// <param name="wayCode"></param>
        /// <param name="bizRQ">业务请求报文</param>
        /// <returns></returns>
        protected ApiRes UnifiedOrder(string wayCode, UnifiedOrderRQ bizRQ)
        {
            return UnifiedOrder(wayCode, bizRQ, null);
        }

        /// <summary>
        /// 统一下单
        /// </summary>
        /// <param name="wayCode"></param>
        /// <param name="bizRQ"></param>
        /// <param name="payOrder"></param>
        /// <returns></returns>
        protected ApiRes UnifiedOrder(string wayCode, UnifiedOrderRQ bizRQ, PayOrderDto payOrder)
        {
            // 响应数据
            UnifiedOrderRS bizRS = null;

            //是否新订单模式 [  一般接口都为新订单模式，  由于QR_CASHIER支付方式，需要先 在DB插入一个新订单， 导致此处需要特殊判断下。 如果已存在则直接更新，否则为插入。  ]
            bool isNewOrder = payOrder == null;

            if (payOrder != null)
            { //当订单存在时，封装公共参数。

                if (payOrder.State != (sbyte)PayOrderState.STATE_INIT)
                {
                    throw new BizException("订单状态异常");
                }

                payOrder.WayCode = wayCode; // 需要将订单更新 支付方式
                payOrder.ChannelUser = bizRQ.GetChannelUserId(); //更新渠道用户信息
                bizRQ.MchNo = payOrder.MchNo;
                bizRQ.AppId = payOrder.AppId;
                bizRQ.MchOrderNo = payOrder.MchOrderNo;
                bizRQ.WayCode = wayCode;
                bizRQ.Amount = payOrder.Amount;
                bizRQ.Currency = payOrder.Currency;
                bizRQ.ClientIp = payOrder.ClientIp;
                bizRQ.Subject = payOrder.Subject;
                bizRQ.NotifyUrl = payOrder.NotifyUrl;
                bizRQ.ReturnUrl = payOrder.ReturnUrl;
                bizRQ.ChannelExtra = payOrder.ChannelExtra;
                bizRQ.ExtParam = payOrder.ExtParam;
                bizRQ.DivisionMode = payOrder.DivisionMode;
            }

            string mchNo = bizRQ.MchNo;
            string appId = bizRQ.AppId;

            // 只有新订单模式，进行校验
            if (isNewOrder && _payOrderService.IsExistOrderByMchOrderNo(mchNo, bizRQ.MchOrderNo))
            {
                throw new BizException("商户订单[" + bizRQ.MchOrderNo + "]已存在");
            }

            if (!string.IsNullOrWhiteSpace(bizRQ.NotifyUrl) && !StringUtil.IsAvailableUrl(bizRQ.NotifyUrl))
            {
                throw new BizException("异步通知地址协议仅支持http:// 或 https:// !");
            }
            if (!string.IsNullOrWhiteSpace(bizRQ.ReturnUrl) && !StringUtil.IsAvailableUrl(bizRQ.ReturnUrl))
            {
                throw new BizException("同步通知地址协议仅支持http:// 或 https:// !");
            }

            //获取支付参数 (缓存数据) 和 商户信息
            MchAppConfigContext mchAppConfigContext = _configContextQueryService.QueryMchInfoAndAppInfo(mchNo, appId);
            if (mchAppConfigContext == null)
            {
                throw new BizException("获取商户应用信息失败");
            }

            MchInfoDto mchInfo = mchAppConfigContext.MchInfo;
            MchAppDto mchApp = mchAppConfigContext.MchApp;

            //收银台支付并且只有新订单需要走这里，  收银台二次下单的wayCode应该为实际支付方式。
            if (isNewOrder && CS.PAY_WAY_CODE.QR_CASHIER.Equals(wayCode))
            {

            }

            // 根据支付方式， 查询出 该商户 可用的支付接口
            var mchPayPassage = _mchPayPassageService.FindMchPayPassage(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, wayCode);
            if (mchPayPassage == null)
            {
                throw new BizException("商户应用不支持该支付方式");
            }

            //获取支付接口
            IPaymentService paymentService = CheckMchWayCodeAndGetService(mchAppConfigContext, mchPayPassage);
            string ifCode = paymentService.GetIfCode();

            //预先校验
            payOrder = new PayOrderDto();
            payOrder.WayCode = wayCode;
            string errMsg = paymentService.PreCheck(bizRQ, payOrder);
            if (string.IsNullOrWhiteSpace(errMsg))
            {
                throw new BizException(errMsg);
            }

            return ApiRes.Ok(new UnifiedOrderRS());
        }

        private PayOrderDto genPayOrder(UnifiedOrderRQ rq, MchInfo mchInfo, MchApp mchApp, String ifCode, MchPayPassage mchPayPassage)
        {

            PayOrderDto payOrder = new PayOrderDto();
            payOrder.PayOrderId = SeqUtil.GenPayOrderId(); //生成订单ID
            payOrder.MchNo = mchInfo.MchNo; //商户号
            payOrder.IsvNo = mchInfo.IsvNo; //服务商号
            payOrder.MchName = mchInfo.MchShortName; //商户名称（简称）
            payOrder.MchType = mchInfo.Type; //商户类型
            payOrder.MchOrderNo = rq.MchOrderNo; //商户订单号
            payOrder.AppId = mchApp.AppId; //商户应用appId
            payOrder.IfCode = ifCode; //接口代码
            payOrder.WayCode = rq.WayCode; //支付方式
            payOrder.Amount = rq.Amount; //订单金额

            if (mchPayPassage != null)
            {
                payOrder.MchFeeRate= mchPayPassage.Rate; //商户手续费费率快照
            }
            else
            {
                payOrder.MchFeeRate= Decimal.Zero; //预下单模式， 按照0计算入库， 后续进行更新
            }

            payOrder.MchFeeAmount = AmountUtil.CalPercentageFee(payOrder.Amount, payOrder.MchFeeRate); //商户手续费,单位分

            payOrder.Currency= rq.Currency; //币种
            payOrder.State= (byte)PayOrderState.STATE_INIT; //订单状态, 默认订单生成状态
            payOrder.ClientIp = "";//StringUtils.defaultIfEmpty(rq.ClientIp, getClientIp()); //客户端IP
            payOrder.Subject= rq.Subject; //商品标题
            payOrder.Body= rq.Body; //商品描述信息
                                            //        payOrder.setChannelExtra(rq.getChannelExtra()); //特殊渠道发起的附件额外参数,  是否应该删除该字段了？？ 比如authCode不应该记录， 只是在传输阶段存在的吧？  之前的为了在payOrder对象需要传参。
            payOrder.ChannelUser= rq.GetChannelUserId(); //渠道用户标志
            payOrder.ExtParam= rq.ExtParam; //商户扩展参数
            payOrder.NotifyUrl=rq.NotifyUrl; //异步通知地址
            payOrder.ReturnUrl= rq.ReturnUrl; //页面跳转地址

            // 分账模式
            payOrder.DivisionMode= rq.DivisionMode ?? (byte)PayOrderDivision.DIVISION_MODE_FORBID;

            var nowDate = DateTime.Now;

            //订单过期时间 单位： 秒
            if (rq.ExpiredTime != null)
            {
                payOrder.ExpiredTime = nowDate.AddSeconds(rq.ExpiredTime.Value);
            }
            else
            {
                payOrder.ExpiredTime = nowDate.AddHours(2); //订单过期时间 默认两个小时
            }

            payOrder.CreatedAt= nowDate; //订单创建时间
            return payOrder;
        }

        /// <summary>
        /// 校验： 商户的支付方式是否可用
        /// 返回： 支付接口
        /// </summary>
        /// <param name="mchAppConfigContext"></param>
        /// <param name="mchPayPassage"></param>
        /// <returns></returns>
        private IPaymentService CheckMchWayCodeAndGetService(MchAppConfigContext mchAppConfigContext, MchPayPassageDto mchPayPassage)
        {
            // 接口代码
            string ifCode = mchPayPassage.IfCode;
            IPaymentService paymentService = _paymentServiceFactory(ifCode);
            if (paymentService == null)
            {
                throw new BizException("无此支付通道接口");
            }

            if (!paymentService.IsSupport(mchPayPassage.WayCode))
            {
                throw new BizException("接口不支持该支付方式");
            }

            if (mchAppConfigContext.MchType == (byte)MchInfoType.TYPE_NORMAL)//普通商户
            {

                if (_configContextQueryService.QueryNormalMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, ifCode) == null)
                {
                    throw new BizException("商户应用参数未配置");
                }
            }
            else if (mchAppConfigContext.MchType == (byte)MchInfoType.TYPE_ISVSUB)//特约商户
            {

                if (_configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, ifCode) == null)
                {
                    throw new BizException("特约商户参数未配置");
                }

                if (_configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, ifCode) == null)
                {
                    throw new BizException("服务商参数未配置");
                }
            }

            return paymentService;
        }
    }
}
