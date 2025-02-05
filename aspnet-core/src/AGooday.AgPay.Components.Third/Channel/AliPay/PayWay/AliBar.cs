using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.AliPay.Kits;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;

namespace AGooday.AgPay.Components.Third.Channel.AliPay.PayWay
{
    /// <summary>
    /// 支付宝 条码支付
    /// </summary>
    public class AliBar : AliPayPaymentService
    {
        public AliBar(ILogger<AliBar> logger,
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override async Task<AbstractRS> PayAsync(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            AliBarOrderRQ bizRQ = (AliBarOrderRQ)rq;

            AlipayTradePayRequest req = new AlipayTradePayRequest();
            AlipayTradePayModel model = new AlipayTradePayModel();
            model.OutTradeNo = payOrder.PayOrderId;
            model.Scene = "bar_code"; //条码支付 bar_code ; 声波支付 wave_code
            model.AuthCode = bizRQ.AuthCode.Trim(); //支付授权码
            model.Subject = payOrder.Subject; //订单标题
            model.Body = payOrder.Body; //订单描述信息
            model.TotalAmount = AmountUtil.ConvertCent2Dollar(payOrder.Amount);  //支付金额
            req.SetNotifyUrl(GetNotifyUrl()); // 设置异步通知地址
            req.SetBizModel(model);

            //统一放置 isv接口必传信息
            await AliPayKit.PutApiIsvInfoAsync(mchAppConfigContext, req, model);

            //调起支付宝 （如果异常， 将直接跑出   ChannelException ）
            AlipayTradePayResponse alipayResp = (await _configContextQueryService.GetAlipayClientWrapperAsync(mchAppConfigContext)).Execute(req);

            // 构造函数响应数据
            AliBarOrderRS res = ApiResBuilder.BuildSuccess<AliBarOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            //放置 响应数据
            channelRetMsg.ChannelAttach = alipayResp.Body;
            channelRetMsg.ChannelOrderId = alipayResp.TradeNo;
            channelRetMsg.ChannelUserId = alipayResp.BuyerUserId; //渠道用户标识

            // ↓↓↓↓↓↓ 调起接口成功后业务判断务必谨慎！！ 避免因代码编写bug，导致不能正确返回订单状态信息  ↓↓↓↓↓↓

            //当条码重复发起时，支付宝返回的code = 10003, subCode = null [等待用户支付], 此时需要特殊判断 = = 。
            //支付成功, 更新订单成功 || 等待支付宝的异步回调接口
            if ("10000".Equals(alipayResp.Code) && !alipayResp.IsError)
            {
                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
            }
            //10003 表示为 处理中, 例如等待用户输入密码
            else if ("10003".Equals(alipayResp.Code))
            {
                channelRetMsg.ChannelState = ChannelState.WAITING;
            }
            //其他状态, 表示下单失败
            else
            {
                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                channelRetMsg.ChannelErrCode = AliPayKit.AppendErrCode(alipayResp.Code, alipayResp.SubCode);
                channelRetMsg.ChannelErrMsg = AliPayKit.AppendErrMsg(alipayResp.Msg, alipayResp.SubMsg);
            }
            return res;
        }

        public override Task<string> PreCheckAsync(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            AliBarOrderRQ bizRQ = (AliBarOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.AuthCode))
            {
                throw new BizException("用户支付条码[authCode]不可为空");
            }

            return Task.FromResult<string>(null);
        }
    }
}
