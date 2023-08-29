using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS.Refund;
using AGooday.AgPay.Payment.Api.RQRS.Transfer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Payment.Api.Services
{
    /// <summary>
    /// 商户通知 service
    /// </summary>
    public class PayMchNotifyService
    {
        private readonly IMQSender mqSender;
        private readonly ILogger<PayMchNotifyService> _logger;
        private readonly ISysConfigService _sysConfigService;
        private readonly IMchNotifyRecordService _mchNotifyRecordService;
        private readonly ConfigContextQueryService _configContextQueryService;

        public PayMchNotifyService(IMQSender mqSender,
            ILogger<PayMchNotifyService> logger,
            ISysConfigService sysConfigService,
            IMchNotifyRecordService mchNotifyRecordService,
            ConfigContextQueryService configContextQueryService)
        {
            this.mqSender = mqSender;
            _logger = logger;
            _sysConfigService = sysConfigService;
            _mchNotifyRecordService = mchNotifyRecordService;
            _configContextQueryService = configContextQueryService;
        }

        /// <summary>
        /// 商户通知信息， 只有订单是终态，才会发送通知， 如明确成功和明确失败
        /// </summary>
        /// <param name="dbPayOrder"></param>
        public void PayOrderNotify(PayOrderDto dbPayOrder)
        {
            try
            {
                // 通知地址为空
                if (string.IsNullOrWhiteSpace(dbPayOrder.NotifyUrl))
                {
                    return;
                }

                //获取到通知对象
                MchNotifyRecordDto mchNotifyRecord = _mchNotifyRecordService.FindByPayOrder(dbPayOrder.PayOrderId);

                if (mchNotifyRecord != null)
                {
                    _logger.LogInformation("当前已存在通知消息， 不再发送。");
                    return;
                }

                var mchConfigList = _sysConfigService.GetByGroupKey("payOrderNotifyConfig", CS.SYS_TYPE.MCH, dbPayOrder.MchNo);
                var mchNotifyPostType = mchConfigList.FirstOrDefault(f => f.ConfigKey.Equals("mchNotifyPostType")).ConfigVal;
                var payOrderNotifyExtParams = mchConfigList.FirstOrDefault(f => f.ConfigKey.Equals("payOrderNotifyExtParams"))?.ConfigVal;

                //商户app私钥
                string appSecret = _configContextQueryService.QueryMchApp(dbPayOrder.MchNo, dbPayOrder.AppId).AppSecret;

                // 封装通知url
                string reqMethod = "POST";
                string reqMediaType = string.Empty;
                switch (mchNotifyPostType)
                {
                    case "POST_QUERYSTRING":
                        break;
                    case "POST_BODY":
                        reqMediaType = "application/x-www-form-urlencoded";
                        break;
                    case "POST_JSON":
                    default:
                        reqMediaType = "application/json";
                        break;
                }
                string notifyUrl = CreateNotifyUrl(dbPayOrder, appSecret, reqMethod, reqMediaType, payOrderNotifyExtParams, out string reqBody);
                mchNotifyRecord = new MchNotifyRecordDto();
                mchNotifyRecord.OrderId = dbPayOrder.PayOrderId;
                mchNotifyRecord.OrderType = (byte)MchNotifyRecordType.TYPE_PAY_ORDER;
                mchNotifyRecord.MchNo = dbPayOrder.MchNo;
                mchNotifyRecord.MchOrderNo = dbPayOrder.MchOrderNo; //商户订单号
                mchNotifyRecord.IsvNo = dbPayOrder.IsvNo;
                mchNotifyRecord.AppId = dbPayOrder.AppId;
                mchNotifyRecord.NotifyUrl = notifyUrl;
                mchNotifyRecord.ReqMethod = reqMethod;
                mchNotifyRecord.ReqMediaType = reqMediaType;
                mchNotifyRecord.ReqBody = reqBody;
                mchNotifyRecord.ResResult = string.Empty;
                mchNotifyRecord.NotifyCount = 0;
                mchNotifyRecord.State = (byte)MchNotifyRecordState.STATE_ING; // 通知中

                try
                {
                    _mchNotifyRecordService.Add(mchNotifyRecord);
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e, $"数据库已存在[{mchNotifyRecord.OrderId}]消息，本次不再推送。");
                    return;
                }

                //推送到MQ
                long notifyId = mchNotifyRecord.NotifyId;
                mqSender.Send(PayOrderMchNotifyMQ.Build(notifyId));
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, $"推送失败！");
            }
        }

        /// <summary>
        /// 商户通知信息，退款成功的发送通知
        /// </summary>
        /// <param name="dbRefundOrder"></param>
        public void RefundOrderNotify(RefundOrderDto dbRefundOrder)
        {
            try
            {
                // 通知地址为空
                if (string.IsNullOrWhiteSpace(dbRefundOrder.NotifyUrl))
                {
                    return;
                }

                //获取到通知对象
                MchNotifyRecordDto mchNotifyRecord = _mchNotifyRecordService.FindByRefundOrder(dbRefundOrder.RefundOrderId);

                if (mchNotifyRecord != null)
                {
                    _logger.LogInformation("当前已存在通知消息， 不再发送。");
                    return;
                }

                var mchConfigList = _sysConfigService.GetByGroupKey("payOrderNotifyConfig", CS.SYS_TYPE.MCH, dbRefundOrder.MchNo);
                var mchNotifyPostType = mchConfigList.FirstOrDefault(f => f.ConfigKey.Equals("mchNotifyPostType")).ConfigVal;
                var payOrderNotifyExtParams = mchConfigList.FirstOrDefault(f => f.ConfigKey.Equals("payOrderNotifyExtParams"))?.ConfigVal;

                //商户app私钥
                string appSecret = _configContextQueryService.QueryMchApp(dbRefundOrder.MchNo, dbRefundOrder.AppId).AppSecret;

                // 封装通知url
                string reqMethod = "POST";
                string reqMediaType = string.Empty;
                switch (mchNotifyPostType)
                {
                    case "POST_QUERYSTRING":
                        break;
                    case "POST_BODY":
                        reqMediaType = "application/x-www-form-urlencoded";
                        break;
                    case "POST_JSON":
                    default:
                        reqMediaType = "application/json";
                        break;
                }
                string notifyUrl = CreateNotifyUrl(dbRefundOrder, appSecret, reqMethod, reqMediaType, payOrderNotifyExtParams, out string reqBody);
                mchNotifyRecord = new MchNotifyRecordDto();
                mchNotifyRecord.OrderId = dbRefundOrder.RefundOrderId;
                mchNotifyRecord.OrderType = (byte)MchNotifyRecordType.TYPE_REFUND_ORDER;
                mchNotifyRecord.MchNo = dbRefundOrder.MchNo;
                mchNotifyRecord.MchOrderNo = dbRefundOrder.MchRefundNo; //商户订单号
                mchNotifyRecord.IsvNo = dbRefundOrder.IsvNo;
                mchNotifyRecord.AppId = dbRefundOrder.AppId;
                mchNotifyRecord.NotifyUrl = notifyUrl;
                mchNotifyRecord.ReqMethod = reqMethod;
                mchNotifyRecord.ReqBody = reqBody;
                mchNotifyRecord.ResResult = "";
                mchNotifyRecord.NotifyCount = 0;
                mchNotifyRecord.State = (byte)MchNotifyRecordState.STATE_ING; // 通知中

                try
                {
                    _mchNotifyRecordService.Add(mchNotifyRecord);
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e, $"数据库已存在[{mchNotifyRecord.OrderId}]消息，本次不再推送。");
                    return;
                }

                //推送到MQ
                long notifyId = mchNotifyRecord.NotifyId;
                mqSender.Send(PayOrderMchNotifyMQ.Build(notifyId));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "推送失败！");
            }
        }

        /// <summary>
        /// 商户通知信息，转账订单的通知接口
        /// </summary>
        /// <param name="dbTransferOrder"></param>
        public void TransferOrderNotify(TransferOrderDto dbTransferOrder)
        {
            try
            {
                // 通知地址为空
                if (string.IsNullOrWhiteSpace(dbTransferOrder.NotifyUrl))
                {
                    return;
                }

                //获取到通知对象
                MchNotifyRecordDto mchNotifyRecord = _mchNotifyRecordService.FindByRefundOrder(dbTransferOrder.TransferId);

                if (mchNotifyRecord != null)
                {
                    _logger.LogInformation("当前已存在通知消息， 不再发送。");
                    return;
                }

                var mchConfigList = _sysConfigService.GetByGroupKey("payOrderNotifyConfig", CS.SYS_TYPE.MCH, dbTransferOrder.MchNo);
                var mchNotifyPostType = mchConfigList.FirstOrDefault(f => f.ConfigKey.Equals("mchNotifyPostType")).ConfigVal;
                var payOrderNotifyExtParams = mchConfigList.FirstOrDefault(f => f.ConfigKey.Equals("payOrderNotifyExtParams"))?.ConfigVal;

                //商户app私钥
                string appSecret = _configContextQueryService.QueryMchApp(dbTransferOrder.MchNo, dbTransferOrder.AppId).AppSecret;

                // 封装通知url
                string reqMethod = "POST";
                string reqMediaType = string.Empty;
                switch (mchNotifyPostType)
                {
                    case "POST_QUERYSTRING":
                        break;
                    case "POST_BODY":
                        reqMediaType = "application/x-www-form-urlencoded";
                        break;
                    case "POST_JSON":
                    default:
                        reqMediaType = "application/json";
                        break;
                }
                string notifyUrl = CreateNotifyUrl(dbTransferOrder, appSecret, reqMethod, reqMediaType, payOrderNotifyExtParams, out string reqBody);
                mchNotifyRecord = new MchNotifyRecordDto();
                mchNotifyRecord.OrderId = dbTransferOrder.TransferId;
                mchNotifyRecord.OrderType = (byte)MchNotifyRecordType.TYPE_REFUND_ORDER;
                mchNotifyRecord.MchNo = dbTransferOrder.MchNo;
                mchNotifyRecord.MchOrderNo = dbTransferOrder.MchOrderNo; //商户订单号
                mchNotifyRecord.IsvNo = dbTransferOrder.IsvNo;
                mchNotifyRecord.AppId = dbTransferOrder.AppId;
                mchNotifyRecord.NotifyUrl = notifyUrl;
                mchNotifyRecord.ReqMethod = reqMethod;
                mchNotifyRecord.ReqBody = reqBody;
                mchNotifyRecord.ResResult = "";
                mchNotifyRecord.NotifyCount = 0;
                mchNotifyRecord.State = (byte)MchNotifyRecordState.STATE_ING; // 通知中

                try
                {
                    _mchNotifyRecordService.Add(mchNotifyRecord);
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e, $"数据库已存在[{mchNotifyRecord.OrderId}]消息，本次不再推送。");
                    return;
                }

                //推送到MQ
                long notifyId = mchNotifyRecord.NotifyId;
                mqSender.Send(PayOrderMchNotifyMQ.Build(notifyId));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "推送失败！");
            }
        }

        /// <summary>
        /// 创建响应URL
        /// </summary>
        /// <param name="payOrder"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public string CreateNotifyUrl(PayOrderDto payOrder, string appSecret, string method, string mediaType, string extParams, out string body)
        {
            QueryPayOrderRS queryPayOrderRS = QueryPayOrderRS.BuildByPayOrder(payOrder);
            JObject jsonObject = JObject.FromObject(queryPayOrderRS);
            JObject payOrderJson = JObject.FromObject(payOrder);
            AppendExtParams(extParams, payOrderJson, jsonObject);
            return GenNotifyUrlAndBody(jsonObject, appSecret, payOrder.NotifyUrl, method, mediaType, out body);
        }

        /// <summary>
        /// 创建响应URL
        /// </summary>
        /// <param name="refundOrder"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public string CreateNotifyUrl(RefundOrderDto refundOrder, string appSecret, string method, string mediaType, string extParams, out string body)
        {
            QueryRefundOrderRS queryRefundOrderRS = QueryRefundOrderRS.BuildByRefundOrder(refundOrder);
            JObject jsonObject = JObject.FromObject(queryRefundOrderRS);
            JObject refundOrderJson = JObject.FromObject(refundOrder);
            AppendExtParams(extParams, refundOrderJson, jsonObject);
            return GenNotifyUrlAndBody(jsonObject, appSecret, refundOrder.NotifyUrl, method, mediaType, out body);
        }

        /// <summary>
        /// 创建响应URL
        /// </summary>
        /// <param name="transferOrder"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public string CreateNotifyUrl(TransferOrderDto transferOrder, string appSecret, string method, string mediaType, string extParams, out string body)
        {
            QueryTransferOrderRS queryTransferOrderRS = QueryTransferOrderRS.BuildByRecord(transferOrder);
            JObject jsonObject = JObject.FromObject(queryTransferOrderRS);
            JObject transferOrderJson = JObject.FromObject(transferOrder);
            AppendExtParams(extParams, transferOrderJson, jsonObject);
            return GenNotifyUrlAndBody(jsonObject, appSecret, transferOrder.NotifyUrl, method, mediaType, out body);
        }

        /// <summary>
        /// 创建响应URL
        /// </summary>
        /// <param name="payOrder"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public string CreateReturnUrl(PayOrderDto payOrder, string appSecret)
        {
            if (string.IsNullOrWhiteSpace(payOrder.ReturnUrl))
            {
                return "";
            }

            QueryPayOrderRS queryPayOrderRS = QueryPayOrderRS.BuildByPayOrder(payOrder);
            JObject jsonObject = JObject.FromObject(queryPayOrderRS);
            jsonObject.Add("reqTime", DateTimeOffset.Now.ToUnixTimeSeconds()); //添加请求时间

            // 报文签名
            jsonObject.Add("sign", AgPayUtil.GetSign(jsonObject, appSecret));   // 签名

            // 生成跳转地址
            return URLUtil.AppendUrlQuery(payOrder.ReturnUrl, jsonObject);
        }

        private static void AppendExtParams(string extParams, JObject originJsonObject, JObject targetJsonObject)
        {
            JArray extParamsArray = JArray.Parse(extParams);
            foreach (var extKey in extParamsArray)
            {
                string key = extKey.ToString();
                bool exists = targetJsonObject.ContainsKey(key);
                if (!exists && originJsonObject.TryGetValue(key, out JToken value))
                {
                    targetJsonObject.Add(key, value);
                }
            }
        }

        private static string GenNotifyUrlAndBody(JObject jsonObject, string appSecret, string notifyUrl, string method, string mediaType, out string body)
        {
            jsonObject.Add("reqTime", DateTimeOffset.Now.ToUnixTimeSeconds());// 添加请求时间
            jsonObject.Add("sign", AgPayUtil.GetSign(jsonObject, appSecret));// 报文签名

            body = string.Empty;
            switch (method)
            {
                case "POST":
                    switch (mediaType)
                    {
                        case "":
                            notifyUrl = URLUtil.AppendUrlQuery(notifyUrl, jsonObject);
                            break;
                        case "application/x-www-form-urlencoded":
                            body = BuildQueryString(jsonObject);
                            break;
                        case "application/json":
                        default:
                            body = JsonConvert.SerializeObject(jsonObject);
                            break;
                    }
                    break;
                case "GET":
                    notifyUrl = URLUtil.AppendUrlQuery(notifyUrl, jsonObject);
                    break;
                default:
                    break;
            }
            return notifyUrl;
        }

        private static string BuildQueryString(JObject data)
        {
            var parameters = new List<string>();

            foreach (var property in data.Properties())
            {
                string key = property.Name;
                string value = property.Value.ToString();
                string encodedKey = Uri.EscapeDataString(key);
                string encodedValue = Uri.EscapeDataString(value);
                string parameter = $"{encodedKey}={encodedValue}";
                parameters.Add(parameter);
            }

            return string.Join("&", parameters);
        }
    }
}
