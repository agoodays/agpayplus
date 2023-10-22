using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.PpPay;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System.Net;
using System.Net.Mime;

namespace AGooday.AgPay.Payment.Api.Models
{
    public class PayPalWrapper
    {
        public PayPalEnvironment Environment { get; private set; }
        public PayPalHttpClient Client { get; private set; }

        public string NotifyWebhook { get; private set; }
        public string RefundWebhook { get; private set; }

        public ChannelRetMsg ProcessOrder(string token, PayOrderDto payOrder)
        {
            return ProcessOrder(token, payOrder, false);
        }

        public List<string> ProcessOrder(string order)
        {
            return ProcessOrder(order, "null");
        }

        /// <summary>
        /// 解析拼接 ID
        /// </summary>
        /// <param name="order"></param>
        /// <param name="afterOrderId"></param>
        /// <returns></returns>
        public List<string> ProcessOrder(string order, string afterOrderId)
        {
            string ppOrderId = "null";
            string ppCatptId = "null";
            if (order != null)
            {
                if (order.Contains(","))
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
                Order order;

                channelRetMsg.ChannelOrderId = ppOrderId + "," + "null";

                if (ppCatptId == null && isCapture)
                {
                    OrderActionRequest orderRequest = new OrderActionRequest();
                    OrdersCaptureRequest ordersCaptureRequest = new OrdersCaptureRequest(ppOrderId);
                    ordersCaptureRequest.RequestBody(orderRequest);

                    var response = Client.Execute(ordersCaptureRequest).Result;

                    if ((int)response.StatusCode != 201)
                    {
                        channelRetMsg.ChannelErrCode = "201";
                        channelRetMsg.ChannelErrMsg = "捕获订单请求失败";
                        return channelRetMsg;
                    }
                    order = response.Result<Order>();
                }
                else
                {
                    OrdersGetRequest request = new OrdersGetRequest(ppOrderId);
                    var response = Client.Execute(request).Result;

                    if ((int)response.StatusCode != 200)
                    {
                        channelRetMsg.ChannelOrderId = ppOrderId;
                        channelRetMsg.ChannelErrCode = "200";
                        channelRetMsg.ChannelErrMsg = "请求订单详情失败";
                        return channelRetMsg;
                    }

                    order = response.Result<Order>();
                }

                string status = order.Status;
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
                result.ChannelOrderId = ppOrderId + "," + ppCatptId;
                result.ChannelUserId = orderUserId;
                result.ChannelAttach = orderJsonStr;
                result.ResponseEntity = TextResp("SUCCESS");
                result.ChannelState = ChannelState.WAITING; // 默认支付中
                result = DispatchCode(status, result); // 处理状态码
                return result;
            }
        }

        /// <summary>
        /// 处理 Paypal 状态码
        /// </summary>
        /// <param name="status"></param>
        /// <param name="channelRetMsg"></param>
        /// <returns></returns>
        public ChannelRetMsg DispatchCode(string status, ChannelRetMsg channelRetMsg)
        {
            if ("SAVED".Equals(status, StringComparison.OrdinalIgnoreCase))
            {
                channelRetMsg.ChannelState = ChannelState.WAITING;
            }
            else if ("APPROVED".Equals(status, StringComparison.OrdinalIgnoreCase))
            {
                channelRetMsg.ChannelState = ChannelState.WAITING;
            }
            else if ("VOIDED".Equals(status, StringComparison.OrdinalIgnoreCase))
            {
                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
            }
            else if ("COMPLETED".Equals(status, StringComparison.OrdinalIgnoreCase))
            {
                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
            }
            else if ("PAYER_ACTION_REQUIRED".Equals(status, StringComparison.OrdinalIgnoreCase))
            {
                channelRetMsg.ChannelState = ChannelState.WAITING;
            }
            else if ("CREATED".Equals(status, StringComparison.OrdinalIgnoreCase))
            {
                channelRetMsg.ChannelState = ChannelState.WAITING;
            }
            else
            {
                channelRetMsg.ChannelState = ChannelState.UNKNOWN;
            }
            return channelRetMsg;
        }

        /// <summary>
        /// 文本类型的响应数据
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public ActionResult TextResp(string text)
        {
            var response = new ContentResult
            {
                Content = text,
                ContentType = MediaTypeNames.Text.Html,
                StatusCode = (int)HttpStatusCode.OK
            };
            return response;
        }

        public static PayPalWrapper BuildPaypalWrapper(PpPayNormalMchParams ppPayNormalMchParams)
        {
            PayPalWrapper paypalWrapper = new PayPalWrapper();
            PayPalEnvironment environment = new LiveEnvironment(ppPayNormalMchParams.ClientId, ppPayNormalMchParams.Secret);

            if (ppPayNormalMchParams.Sandbox == 1)
            {
                environment = new SandboxEnvironment(ppPayNormalMchParams.ClientId, ppPayNormalMchParams.Secret);
            }

            paypalWrapper.Environment = environment;
            paypalWrapper.Client = new PayPalHttpClient(environment);
            paypalWrapper.NotifyWebhook = ppPayNormalMchParams.NotifyWebhook;
            paypalWrapper.RefundWebhook = ppPayNormalMchParams.RefundWebhook;

            return paypalWrapper;
        }
    }
}
