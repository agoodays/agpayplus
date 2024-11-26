using System.Net;
using System.Net.Mime;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.PpPay;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaypalServerSdk.Standard;
using PaypalServerSdk.Standard.Authentication;
using PaypalServerSdk.Standard.Controllers;
using PaypalServerSdk.Standard.Models;

namespace AGooday.AgPay.Components.Third.Models
{
    /// <summary>
    /// https://github.com/paypal/PayPal-Dotnet-Server-SDK
    /// </summary>
    public class PayPalWrapper
    {
        public PaypalServerSdkClient Client { get; private set; }

        public string NotifyWebhook { get; private set; }
        public string RefundWebhook { get; private set; }

        public ChannelRetMsg ProcessOrder(string token, PayOrderDto payOrder)
        {
            return ProcessOrder(token, payOrder, false);
        }

        public static List<string> ProcessOrder(string order)
        {
            return ProcessOrder(order, "null");
        }

        /// <summary>
        /// 解析拼接 ID
        /// </summary>
        /// <param name="order"></param>
        /// <param name="afterOrderId"></param>
        /// <returns></returns>
        public static List<string> ProcessOrder(string order, string afterOrderId)
        {
            string ppOrderId = "null";
            string ppCatptId = "null";
            if (order != null)
            {
                if (order.Contains(','))
                {
                    string[] split = order.Split(",");
                    if (split.Length == 2)
                    {
                        ppCatptId = split[1];
                        ppOrderId = split[0];
                    }
                }
            }
            if (afterOrderId != null && !"null".Equals(afterOrderId, StringComparison.OrdinalIgnoreCase))
            {
                ppOrderId = afterOrderId;
            }

            if ("null".Equals(ppCatptId, StringComparison.OrdinalIgnoreCase))
            {
                ppCatptId = null;
            }
            if ("null".Equals(ppOrderId, StringComparison.OrdinalIgnoreCase))
            {
                ppOrderId = null;
            }

            return new List<string> { ppOrderId, ppCatptId };
        }

        /// <summary>
        /// 处理并捕获订单
        /// 由于 Paypal 创建订单后需要进行一次 Capture(捕获) 才可以正确获取到订单的支付状态
        /// </summary>
        /// <param name="token"></param>
        /// <param name="payOrder"></param>
        /// <param name="isCapture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ChannelRetMsg ProcessOrder(string token, PayOrderDto payOrder, bool isCapture)
        {
            string ppOrderId = ProcessOrder(payOrder.ChannelOrderNo, token)[0];
            string ppCatptId = ProcessOrder(payOrder.ChannelOrderNo)[1];

            ChannelRetMsg channelRetMsg = ChannelRetMsg.Waiting();
            channelRetMsg.ResponseEntity = TextResp("ERROR");

            if (ppOrderId == null)
            {
                channelRetMsg.ChannelErrCode = "201";
                channelRetMsg.ChannelErrMsg = "捕获订单请求失败";
                return channelRetMsg;
            }
            else
            {
                OrdersController ordersController = Client.OrdersController;

                Order order;

                channelRetMsg.ChannelOrderId = $"{ppOrderId},null";

                if (ppCatptId == null && isCapture)
                {
                    OrdersCaptureInput ordersCaptureInput = new OrdersCaptureInput
                    {
                        Id = ppOrderId,
                        Prefer = "return=representation",
                    };

                    var response = ordersController.OrdersCapture(ordersCaptureInput);

                    if ((int)response.StatusCode != 201)
                    {
                        channelRetMsg.ChannelErrCode = "201";
                        channelRetMsg.ChannelErrMsg = "捕获订单请求失败";
                        return channelRetMsg;
                    }
                    order = response.Data;
                }
                else
                {
                    OrdersGetInput request = new OrdersGetInput(ppOrderId);
                    var response = ordersController.OrdersGet(request);

                    if ((int)response.StatusCode != 200)
                    {
                        channelRetMsg.ChannelOrderId = ppOrderId;
                        channelRetMsg.ChannelErrCode = "200";
                        channelRetMsg.ChannelErrMsg = "请求订单详情失败";
                        return channelRetMsg;
                    }

                    order = response.Data;
                }

                string orderJsonStr = JsonConvert.SerializeObject(order);
                var orderJson = JObject.Parse(orderJsonStr);

                foreach (PurchaseUnit purchaseUnit in order.PurchaseUnits)
                {
                    if (purchaseUnit.Payments != null)
                    {
                        foreach (Capture capture in purchaseUnit.Payments.Captures)
                        {
                            ppCatptId = capture.Id;
                            break;
                        }
                    }
                }

                string orderUserId = orderJson.SelectToken("payer.payer_id")?.ToString();
                //string orderUserId = order.Payer.PayerId;

                ChannelRetMsg result = new ChannelRetMsg();
                result.IsNeedQuery = true;
                result.ChannelOrderId = $"{ppOrderId},{ppCatptId}";
                result.ChannelUserId = orderUserId;
                result.ChannelAttach = orderJsonStr;
                result.ResponseEntity = TextResp("SUCCESS");
                result.ChannelState = ChannelState.WAITING; // 默认支付中
                result = DispatchCode(order.Status.Value, result); // 处理状态码
                return result;
            }
        }

        /// <summary>
        /// 处理 Paypal 状态码
        /// </summary>
        /// <param name="status"></param>
        /// <param name="channelRetMsg"></param>
        /// <returns></returns>
        public static ChannelRetMsg DispatchCode(OrderStatus status, ChannelRetMsg channelRetMsg)
        {
            switch (status)
            {
                case OrderStatus.Saved:
                case OrderStatus.Approved:
                case OrderStatus.PayerActionRequired:
                case OrderStatus.Created:
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                    break;
                case OrderStatus.Voided:
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    break;
                case OrderStatus.Completed:
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                    break;
                default:
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                    break;
            }
            return channelRetMsg;
        }

        public static ChannelRetMsg DispatchCode(RefundStatus status, ChannelRetMsg channelRetMsg)
        {
            switch (status)
            {
                case RefundStatus.Pending:
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                    break;
                case RefundStatus.Failed:
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    break;
                case RefundStatus.Completed:
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                    break;
                default:
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                    break;
            }
            return channelRetMsg;
        }

        /// <summary>
        /// 文本类型的响应数据
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static ActionResult TextResp(string text, int statusCode = (int)HttpStatusCode.OK)
        {
            var response = new ContentResult
            {
                Content = text,
                ContentType = MediaTypeNames.Text.Html,
                StatusCode = statusCode
            };
            return response;
        }

        public static PayPalWrapper BuildPaypalWrapper(PpPayNormalMchParams ppPayNormalMchParams)
        {
            PayPalWrapper paypalWrapper = new PayPalWrapper();
            paypalWrapper.Client = new PaypalServerSdkClient.Builder()
                .ClientCredentialsAuth(
                    new ClientCredentialsAuthModel.Builder(
                        ppPayNormalMchParams.ClientId,
                        ppPayNormalMchParams.Secret
                    )
                    .Build())
                .Environment(ppPayNormalMchParams.Sandbox == 1 ? PaypalServerSdk.Standard.Environment.Sandbox : PaypalServerSdk.Standard.Environment.Production)
                .LoggingConfig(config => config
                    .LogLevel(LogLevel.Information)
                    .RequestConfig(reqConfig => reqConfig.Body(true))
                    .ResponseConfig(respConfig => respConfig.Headers(true))
                )
                .Build();
            paypalWrapper.NotifyWebhook = ppPayNormalMchParams.NotifyWebhook;
            paypalWrapper.RefundWebhook = ppPayNormalMchParams.RefundWebhook;

            return paypalWrapper;
        }
    }
}
