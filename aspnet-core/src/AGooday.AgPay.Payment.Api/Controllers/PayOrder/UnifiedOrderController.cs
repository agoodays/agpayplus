﻿using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using AGooday.AgPay.Payment.Api.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.PayOrder
{
    /// <summary>
    /// 统一下单
    /// </summary>
    [ApiController]
    public class UnifiedOrderController : AbstractPayOrderController
    {
        public UnifiedOrderController(ILogger<UnifiedOrderController> logger,
            IServiceProvider serviceProvider,
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
        }

        /// <summary>
        /// 统一下单接口
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("api/pay/unifiedOrder")]
        [PermissionAuth(PermCode.PAY.API_PAY_ORDER)]
        public async Task<ActionResult<ApiRes>> UnifiedOrderAsync()
        {
            //获取参数 & 验签
            UnifiedOrderRQ rq = await this.GetRQByWithMchSignAsync<UnifiedOrderRQ>();

            UnifiedOrderRQ bizRQ = await this.BuildBizRQAsync(rq);

            //实现子类的res
            ApiRes apiRes = await this.UnifiedOrderAsync(bizRQ.WayCode, bizRQ);
            if (apiRes.Data == null)
            {
                return apiRes;
            }

            UnifiedOrderRS bizRes = (UnifiedOrderRS)apiRes.Data;

            //聚合接口，返回的参数
            UnifiedOrderRS res = new UnifiedOrderRS();
            CopyUtil.CopyProperties(bizRes, res);

            //只有 订单生成（QR_CASHIER） || 支付中 || 支付成功返回该数据
            if (bizRes.OrderState != null && (bizRes.OrderState == (byte)PayOrderState.STATE_INIT || bizRes.OrderState == (byte)PayOrderState.STATE_ING || bizRes.OrderState == (byte)PayOrderState.STATE_SUCCESS))
            {
                res.PayDataType = bizRes.BuildPayDataType();
                res.PayData = bizRes.BuildPayData();
            }

            return ApiRes.OkWithSign(res, rq.SignType, (await _configContextQueryService.QueryMchAppAsync(rq.MchNo, rq.AppId)).AppSecret);
        }
        private async Task<UnifiedOrderRQ> BuildBizRQAsync(UnifiedOrderRQ rq)
        {
            //支付方式  比如： ali_bar
            string wayCode = rq.WayCode;

            //jsapi 收银台聚合支付场景 (不校验是否存在payWayCode)
            if (CS.PAY_WAY_CODE.QR_CASHIER.Equals(wayCode))
            {
                return rq.BuildBizRQ();
            }

            //如果是自动分类条码
            if (CS.PAY_WAY_CODE.AUTO_BAR.Equals(wayCode))
            {
                AutoBarOrderRQ bizRQ = (AutoBarOrderRQ)rq.BuildBizRQ();
                wayCode = AgPayUtil.GetPayWayCodeByBarCode(bizRQ.AuthCode);
                rq.WayCode = wayCode.Trim();
            }

            if (!await _payWayService.IsExistPayWayCodeAsync(wayCode))
            {
                throw new BizException("不支持的支付方式");
            }

            //转换为 bizRQ
            return rq.BuildBizRQ();
        }
    }
}
