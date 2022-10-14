using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS.Refund;
using AGooday.AgPay.Payment.Api.RQRS.Transfer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;

namespace AGooday.AgPay.Payment.Api.Services
{
    /// <summary>
    /// 商户通知 service
    /// </summary>
    public class PayMchNotifyService
    {
        private readonly IMQSender mqSender;
        private readonly IMchNotifyRecordService _mchNotifyRecordService;
        private readonly ConfigContextQueryService _configContextQueryService;
        private readonly ILogger<PayMchNotifyService> _logger;

        public PayMchNotifyService(IMQSender mqSender, ILogger<PayMchNotifyService> logger, IMchNotifyRecordService mchNotifyRecordService, ConfigContextQueryService configContextQueryService)
        {
            this.mqSender = mqSender;
            _logger = logger;
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

                //商户app私钥
                string appSecret = _configContextQueryService.QueryMchApp(dbPayOrder.MchNo, dbPayOrder.AppId).AppSecret;

                // 封装通知url
                string reqMethod = "POST";
                string notifyUrl = CreateNotifyUrl(dbPayOrder, appSecret, reqMethod, out string reqBody);
                mchNotifyRecord = new MchNotifyRecordDto();
                mchNotifyRecord.OrderId = dbPayOrder.PayOrderId;
                mchNotifyRecord.OrderType = (byte)MchNotifyRecordType.TYPE_PAY_ORDER;
                mchNotifyRecord.MchNo = dbPayOrder.MchNo;
                mchNotifyRecord.MchOrderNo = dbPayOrder.MchOrderNo; //商户订单号
                mchNotifyRecord.IsvNo = dbPayOrder.IsvNo;
                mchNotifyRecord.AppId = dbPayOrder.AppId;
                mchNotifyRecord.NotifyUrl = notifyUrl;
                mchNotifyRecord.ReqMethod = reqMethod;
                mchNotifyRecord.ReqBody = "";
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

                //商户app私钥
                string appSecret = _configContextQueryService.QueryMchApp(dbRefundOrder.MchNo, dbRefundOrder.AppId).AppSecret;

                // 封装通知url
                string reqMethod = "POST";
                string notifyUrl = CreateNotifyUrl(dbRefundOrder, appSecret, reqMethod, out string reqBody);
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

                //商户app私钥
                string appSecret = _configContextQueryService.QueryMchApp(dbTransferOrder.MchNo, dbTransferOrder.AppId).AppSecret;

                // 封装通知url
                string reqMethod = "POST";
                string notifyUrl = CreateNotifyUrl(dbTransferOrder, appSecret, reqMethod, out string reqBody);
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
        public string CreateNotifyUrl(PayOrderDto payOrder, string appSecret, string method, out string body)
        {
            QueryPayOrderRS queryPayOrderRS = QueryPayOrderRS.BuildByPayOrder(payOrder);
            JObject jsonObject = JObject.FromObject(queryPayOrderRS);
            return GenNotifyUrlAndBody(jsonObject, appSecret, payOrder.NotifyUrl, method, out body);
        }

        /// <summary>
        /// 创建响应URL
        /// </summary>
        /// <param name="refundOrder"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public string CreateNotifyUrl(RefundOrderDto refundOrder, string appSecret, string method, out string body)
        {
            QueryRefundOrderRS queryRefundOrderRS = QueryRefundOrderRS.BuildByRefundOrder(refundOrder);
            JObject jsonObject = JObject.FromObject(queryRefundOrderRS);
            return GenNotifyUrlAndBody(jsonObject, appSecret, refundOrder.NotifyUrl, method, out body);
        }

        /// <summary>
        /// 创建响应URL
        /// </summary>
        /// <param name="transferOrder"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public string CreateNotifyUrl(TransferOrderDto transferOrder, string appSecret, string method, out string body)
        {
            QueryTransferOrderRS rs = QueryTransferOrderRS.BuildByRecord(transferOrder);
            JObject jsonObject = JObject.FromObject(rs);
            return GenNotifyUrlAndBody(jsonObject, appSecret, transferOrder.NotifyUrl, method, out body);
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

        private static string GenNotifyUrlAndBody(JObject jsonObject, string appSecret, string notifyUrl, string method, out string body)
        {
            jsonObject.Add("reqTime", DateTimeOffset.Now.ToUnixTimeSeconds());// 添加请求时间
            jsonObject.Add("sign", AgPayUtil.GetSign(jsonObject, appSecret));// 报文签名

            body = string.Empty;
            switch (method)
            {
                case "POST":
                    body = JsonConvert.SerializeObject(jsonObject);
                    break;
                case "GET":
                    notifyUrl = URLUtil.AppendUrlQuery(notifyUrl, jsonObject);
                    break;
                default:
                    break;
            }
            return notifyUrl;
        }
    }
}
