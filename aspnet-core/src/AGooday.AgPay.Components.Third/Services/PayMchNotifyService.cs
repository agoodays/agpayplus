using System.Net.Mime;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Components.Third.RQRS.PayOrder;
using AGooday.AgPay.Components.Third.RQRS.Refund;
using AGooday.AgPay.Components.Third.RQRS.Transfer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Services
{
    /// <summary>
    /// 商户通知 service
    /// </summary>
    public class PayMchNotifyService
    {
        private readonly IMQSender _mqSender;
        private readonly ILogger<PayMchNotifyService> _logger;
        private readonly ISysConfigService _sysConfigService;
        private readonly IMchNotifyRecordService _mchNotifyRecordService;
        private readonly ConfigContextQueryService _configContextQueryService;

        public PayMchNotifyService(ILogger<PayMchNotifyService> logger,
            IMQSender mqSender,
            ISysConfigService sysConfigService,
            IMchNotifyRecordService mchNotifyRecordService,
            ConfigContextQueryService configContextQueryService)
        {
            _mqSender = mqSender;
            _logger = logger;
            _sysConfigService = sysConfigService;
            _mchNotifyRecordService = mchNotifyRecordService;
            _configContextQueryService = configContextQueryService;
        }

        /// <summary>
        /// 商户通知信息， 只有订单是终态，才会发送通知， 如明确成功和明确失败
        /// </summary>
        /// <param name="dbPayOrder"></param>
        public async Task PayOrderNotifyAsync(PayOrderDto dbPayOrder)
        {
            try
            {
                // 通知地址为空
                if (string.IsNullOrWhiteSpace(dbPayOrder.NotifyUrl))
                {
                    return;
                }

                //获取到通知对象
                MchNotifyRecordDto mchNotifyRecord = await _mchNotifyRecordService.FindByPayOrderAsync(dbPayOrder.PayOrderId);

                if (mchNotifyRecord != null)
                {
                    _logger.LogInformation("当前已存在通知消息，不再发送。");
                    return;
                }

                //商户app私钥
                string appSecret = (await _configContextQueryService.QueryMchAppAsync(dbPayOrder.MchNo, dbPayOrder.AppId)).AppSecret;

                // 封装通知url
                string reqMethod = HttpMethod.Post.Method;
                string extParams = GetExtParams(dbPayOrder.MchNo, out string reqMediaType);
                string notifyUrl = CreateNotifyUrl(dbPayOrder, appSecret, reqMethod, reqMediaType, extParams, out string reqBody);
                var nowDate = DateTime.Now;
                mchNotifyRecord = new MchNotifyRecordDto();
                mchNotifyRecord.OrderId = dbPayOrder.PayOrderId;
                mchNotifyRecord.OrderType = (byte)MchNotifyRecordType.TYPE_PAY_ORDER;
                mchNotifyRecord.MchOrderNo = dbPayOrder.MchOrderNo; //商户订单号
                mchNotifyRecord.MchNo = dbPayOrder.MchNo;
                mchNotifyRecord.AgentNo = dbPayOrder.AgentNo;
                mchNotifyRecord.IsvNo = dbPayOrder.IsvNo;
                mchNotifyRecord.AppId = dbPayOrder.AppId;
                mchNotifyRecord.NotifyUrl = notifyUrl;
                mchNotifyRecord.ReqMethod = reqMethod;
                mchNotifyRecord.ReqMediaType = reqMediaType;
                mchNotifyRecord.ReqBody = reqBody;
                mchNotifyRecord.ResResult = string.Empty;
                mchNotifyRecord.NotifyCount = 0;
                mchNotifyRecord.NotifyCountLimit = 6;
                mchNotifyRecord.State = (byte)MchNotifyRecordState.STATE_ING; // 通知中
                mchNotifyRecord.CreatedAt = nowDate;
                mchNotifyRecord.UpdatedAt = nowDate;

                try
                {
                    await _mchNotifyRecordService.AddAsync(mchNotifyRecord);
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e, "数据库已存在[{OrderId}]消息，本次不再推送。", mchNotifyRecord.OrderId);
                    //_logger.LogInformation(e, $"数据库已存在[{mchNotifyRecord.OrderId}]消息，本次不再推送。");
                    return;
                }

                //推送到MQ
                long notifyId = mchNotifyRecord.NotifyId;
                await _mqSender.SendAsync(PayOrderMchNotifyMQ.Build(notifyId));
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "推送失败！");
            }
        }

        /// <summary>
        /// 商户通知信息，退款成功的发送通知
        /// </summary>
        /// <param name="dbRefundOrder"></param>
        public async Task RefundOrderNotifyAsync(RefundOrderDto dbRefundOrder)
        {
            try
            {
                // 通知地址为空
                if (string.IsNullOrWhiteSpace(dbRefundOrder.NotifyUrl))
                {
                    return;
                }

                //获取到通知对象
                MchNotifyRecordDto mchNotifyRecord = await _mchNotifyRecordService.FindByRefundOrderAsync(dbRefundOrder.RefundOrderId);

                if (mchNotifyRecord != null)
                {
                    _logger.LogInformation("当前已存在通知消息，不再发送。");
                    return;
                }

                //商户app私钥
                string appSecret = (await _configContextQueryService.QueryMchAppAsync(dbRefundOrder.MchNo, dbRefundOrder.AppId)).AppSecret;

                // 封装通知url
                string reqMethod = HttpMethod.Post.Method;
                string extParams = GetExtParams(dbRefundOrder.MchNo, out string reqMediaType);
                string notifyUrl = CreateNotifyUrl(dbRefundOrder, appSecret, reqMethod, reqMediaType, extParams, out string reqBody);
                var nowDate = DateTime.Now;
                mchNotifyRecord = new MchNotifyRecordDto();
                mchNotifyRecord.OrderId = dbRefundOrder.RefundOrderId;
                mchNotifyRecord.OrderType = (byte)MchNotifyRecordType.TYPE_REFUND_ORDER;
                mchNotifyRecord.MchOrderNo = dbRefundOrder.MchRefundNo; //商户订单号
                mchNotifyRecord.MchNo = dbRefundOrder.MchNo;
                mchNotifyRecord.AgentNo = dbRefundOrder.AgentNo;
                mchNotifyRecord.IsvNo = dbRefundOrder.IsvNo;
                mchNotifyRecord.AppId = dbRefundOrder.AppId;
                mchNotifyRecord.NotifyUrl = notifyUrl;
                mchNotifyRecord.ReqMethod = reqMethod;
                mchNotifyRecord.ReqBody = reqBody;
                mchNotifyRecord.ResResult = string.Empty;
                mchNotifyRecord.NotifyCount = 0;
                mchNotifyRecord.NotifyCountLimit = 6;
                mchNotifyRecord.State = (byte)MchNotifyRecordState.STATE_ING; // 通知中
                mchNotifyRecord.CreatedAt = nowDate;
                mchNotifyRecord.UpdatedAt = nowDate;

                try
                {
                    await _mchNotifyRecordService.AddAsync(mchNotifyRecord);
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e, "数据库已存在[{OrderId}]消息，本次不再推送。", mchNotifyRecord.OrderId);
                    //_logger.LogInformation(e, $"数据库已存在[{mchNotifyRecord.OrderId}]消息，本次不再推送。");
                    return;
                }

                //推送到MQ
                long notifyId = mchNotifyRecord.NotifyId;
                await _mqSender.SendAsync(PayOrderMchNotifyMQ.Build(notifyId));
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
        public async Task TransferOrderNotifyAsync(TransferOrderDto dbTransferOrder)
        {
            try
            {
                // 通知地址为空
                if (string.IsNullOrWhiteSpace(dbTransferOrder.NotifyUrl))
                {
                    return;
                }

                //获取到通知对象
                MchNotifyRecordDto mchNotifyRecord = await _mchNotifyRecordService.FindByRefundOrderAsync(dbTransferOrder.TransferId);

                if (mchNotifyRecord != null)
                {
                    _logger.LogInformation("当前已存在通知消息，不再发送。");
                    return;
                }

                //商户app私钥
                string appSecret = (await _configContextQueryService.QueryMchAppAsync(dbTransferOrder.MchNo, dbTransferOrder.AppId)).AppSecret;

                // 封装通知url
                string reqMethod = HttpMethod.Post.Method;
                string extParams = GetExtParams(dbTransferOrder.MchNo, out string reqMediaType);
                string notifyUrl = CreateNotifyUrl(dbTransferOrder, appSecret, reqMethod, reqMediaType, extParams, out string reqBody);
                var nowDate = DateTime.Now;
                mchNotifyRecord = new MchNotifyRecordDto();
                mchNotifyRecord.OrderId = dbTransferOrder.TransferId;
                mchNotifyRecord.OrderType = (byte)MchNotifyRecordType.TYPE_TRANSFER_ORDER;
                mchNotifyRecord.MchOrderNo = dbTransferOrder.MchOrderNo; //商户订单号
                mchNotifyRecord.MchNo = dbTransferOrder.MchNo;
                mchNotifyRecord.AgentNo = dbTransferOrder.AgentNo;
                mchNotifyRecord.IsvNo = dbTransferOrder.IsvNo;
                mchNotifyRecord.AppId = dbTransferOrder.AppId;
                mchNotifyRecord.NotifyUrl = notifyUrl;
                mchNotifyRecord.ReqMethod = reqMethod;
                mchNotifyRecord.ReqBody = reqBody;
                mchNotifyRecord.ResResult = string.Empty;
                mchNotifyRecord.NotifyCount = 0;
                mchNotifyRecord.NotifyCountLimit = 6;
                mchNotifyRecord.State = (byte)MchNotifyRecordState.STATE_ING; // 通知中
                mchNotifyRecord.CreatedAt = nowDate;
                mchNotifyRecord.UpdatedAt = nowDate;

                try
                {
                    await _mchNotifyRecordService.AddAsync(mchNotifyRecord);
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e, "数据库已存在[{OrderId}]消息，本次不再推送。", mchNotifyRecord.OrderId);
                    //_logger.LogInformation(e, $"数据库已存在[{mchNotifyRecord.OrderId}]消息，本次不再推送。");
                    return;
                }

                //推送到MQ
                long notifyId = mchNotifyRecord.NotifyId;
                await _mqSender.SendAsync(PayOrderMchNotifyMQ.Build(notifyId));
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
        public static string CreateNotifyUrl(PayOrderDto payOrder, string appSecret, string method, string mediaType, string extParams, out string body)
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
        public static string CreateNotifyUrl(RefundOrderDto refundOrder, string appSecret, string method, string mediaType, string extParams, out string body)
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
        public static string CreateNotifyUrl(TransferOrderDto transferOrder, string appSecret, string method, string mediaType, string extParams, out string body)
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
        public static string CreateReturnUrl(PayOrderDto payOrder, string appSecret)
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

        private string GetExtParams(string mhNo, out string reqMediaType)
        {
            reqMediaType = string.Empty;
            var mchConfigList = _sysConfigService.GetByGroupKey("payOrderNotifyConfig", CS.SYS_TYPE.MCH, mhNo);
            var mchNotifyPostType = mchConfigList.FirstOrDefault(f => f.ConfigKey.Equals("mchNotifyPostType")).ConfigVal;
            var payOrderNotifyExtParams = mchConfigList.FirstOrDefault(f => f.ConfigKey.Equals("payOrderNotifyExtParams"))?.ConfigVal;
            switch (mchNotifyPostType)
            {
                case "POST_QUERYSTRING":
                    break;
                case "POST_BODY":
                    reqMediaType = MediaTypeNames.Application.FormUrlEncoded;
                    break;
                case "POST_JSON":
                default:
                    reqMediaType = MediaTypeNames.Application.Json;
                    break;
            }

            return payOrderNotifyExtParams;
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
                        case MediaTypeNames.Application.FormUrlEncoded:
                            body = BuildQueryString(jsonObject);
                            break;
                        case MediaTypeNames.Application.Json:
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
