using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaypalServerSdk.Standard.Models;

namespace AGooday.AgPay.Components.Third.Channel.PpPay.PayWay
{
    public class PpPc : PpPayPaymentService
    {
        public PpPc(ILogger<PpPc> logger, 
            IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(logger, serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            PpPcOrderRQ bizRQ = (PpPcOrderRQ)rq;

            OrderRequest orderRequest = new OrderRequest();

            // 配置 Paypal ApplicationContext 也就是支付页面信息
            OrderApplicationContext applicationContext = new OrderApplicationContext()
            {
                BrandName = mchAppConfigContext.MchApp.AppName,
                LandingPage = OrderApplicationContextLandingPage.NoPreference,
                ReturnUrl = GetReturnUrl(payOrder.PayOrderId),
                UserAction = OrderApplicationContextUserAction.PayNow,
                ShippingPreference = OrderApplicationContextShippingPreference.NoShipping
            };

            if (!string.IsNullOrWhiteSpace(bizRQ.CancelUrl))
            {
                applicationContext.CancelUrl = bizRQ.CancelUrl;
            }
            orderRequest.ApplicationContext = applicationContext;
            orderRequest.Intent = CheckoutPaymentIntent.Capture;

            List<PurchaseUnitRequest> purchaseUnitRequests = new List<PurchaseUnitRequest>();

            // 金额换算
            string amountStr = AmountUtil.ConvertCent2Dollar(payOrder.Amount.ToString());
            string currency = payOrder.Currency.ToUpper();

            // 由于 Paypal 是支持订单多商品的，这里值添加一个
            PurchaseUnitRequest purchaseUnitRequest = new PurchaseUnitRequest();
            // 绑定 订单 ID 否则回调和异步较难处理
            purchaseUnitRequest.CustomId = payOrder.PayOrderId;
            purchaseUnitRequest.InvoiceId = payOrder.PayOrderId;
            purchaseUnitRequest.Amount = new AmountWithBreakdown()
            {
                CurrencyCode = currency,
                MValue = amountStr,
                Breakdown = new AmountBreakdown()
                {
                    ItemTotal = new Money()
                    {
                        CurrencyCode = currency,
                        MValue = amountStr,
                    }
                }
            };
            purchaseUnitRequest.Items = new List<Item>()
            {
                new Item()
                {
                    Name = payOrder.Subject,
                    Description = payOrder.Body,
                    Sku = payOrder.PayOrderId,
                    UnitAmount = new Money()
                    {
                        CurrencyCode = currency,
                        MValue = amountStr,
                    },
                    Quantity="1",
                }
            };


            purchaseUnitRequests.Add(purchaseUnitRequest);
            orderRequest.PurchaseUnits = purchaseUnitRequests;

            // 从缓存获取 Paypal 操作工具
            PayPalWrapper paypalWrapper = _configContextQueryService.GetPaypalWrapper(mchAppConfigContext);

            OrdersCreateInput request = new OrdersCreateInput
            {
                Body = orderRequest,
                Prefer = "return=representation",
            };

            // 构造函数响应数据
            PpPcOrderRS res = ApiResBuilder.BuildSuccess<PpPcOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();

            try
            {
                var response = paypalWrapper.Client.OrdersController.OrdersCreate(request);
                // 标准返回 HttpPost 需要为 201
                if ((int)response.StatusCode == 201)
                {
                    Order order = response.Data;

                    // 从返回数据里读取出支付链接
                    LinkDescription paypalLink = order.Links.Find(l => l.Rel.Equals("approve", StringComparison.OrdinalIgnoreCase)
                    && l.Method == LinkHttpMethod.Get);

                    // 设置返回实体
                    channelRetMsg.ChannelAttach = JsonConvert.SerializeObject(order);
                    channelRetMsg.ChannelOrderId = $"{order.Id},null"; // 拼接订单ID
                    channelRetMsg = PayPalWrapper.DispatchCode(order.Status.Value, channelRetMsg); // 处理状态码

                    // 设置支付链接
                    res.PayUrl = paypalLink.Href;
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = "201";
                    channelRetMsg.ChannelErrMsg = "Request failed, Paypal response is not 201";
                }

                res.ChannelRetMsg = channelRetMsg;
                return res;
            }
            catch (HttpRequestException e)
            {
                string message = e.Message;
                var messageObj = JObject.Parse(message);
                string issue = messageObj.SelectToken("details[0].issue")?.ToString();
                string description = messageObj.SelectToken("details[0].description")?.ToString();
                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                channelRetMsg.ChannelErrCode = issue;
                channelRetMsg.ChannelErrMsg = description;
                res.ChannelRetMsg = channelRetMsg;
                return res;
            }
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            return null;
        }
    }
}
