using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Utils;
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
        private readonly IMchNotifyRecordService _mchNotifyRecordService;
        private readonly ConfigContextQueryService _configContextQueryService;
        private readonly ILogger<PayMchNotifyService> _logger;

        public PayMchNotifyService(IMchNotifyRecordService mchNotifyRecordService, ConfigContextQueryService configContextQueryService, ILogger<PayMchNotifyService> logger)
        {
            _mchNotifyRecordService = mchNotifyRecordService;
            _configContextQueryService = configContextQueryService;
            _logger = logger;
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
                string notifyUrl = CreateNotifyUrl(dbPayOrder, appSecret);
                mchNotifyRecord = new MchNotifyRecordDto();
                mchNotifyRecord.OrderId = dbPayOrder.PayOrderId;
                mchNotifyRecord.OrderType = (byte)MchNotifyRecordType.TYPE_PAY_ORDER;
                mchNotifyRecord.MchNo = dbPayOrder.MchNo;
                mchNotifyRecord.MchOrderNo = dbPayOrder.MchOrderNo; //商户订单号
                mchNotifyRecord.IsvNo = dbPayOrder.IsvNo;
                mchNotifyRecord.AppId = dbPayOrder.AppId;
                mchNotifyRecord.NotifyUrl = notifyUrl;
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
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, $"推送失败！");
            }
        }

        /// <summary>
        /// 创建响应URL
        /// </summary>
        /// <param name="payOrder"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public string CreateNotifyUrl(PayOrderDto payOrder, string appSecret)
        {

            QueryPayOrderRS queryPayOrderRS = QueryPayOrderRS.BuildByPayOrder(payOrder);
            JObject jsonObject = JObject.FromObject(queryPayOrderRS);
            jsonObject.Add("reqTime", DateTimeOffset.Now.ToUnixTimeSeconds()); //添加请求时间

            // 报文签名
            jsonObject.Add("sign", AgPayUtil.GetSign(jsonObject, appSecret));

            // 生成通知
            return URLUtil.AppendUrlQuery(payOrder.NotifyUrl, jsonObject);
        }

        /// <summary>
        /// 创建响应URL
        /// </summary>
        /// <param name="refundOrder"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public string CreateNotifyUrl(RefundOrderDto refundOrder, string appSecret)
        {

            QueryRefundOrderRS queryRefundOrderRS = QueryRefundOrderRS.BuildByRefundOrder(refundOrder);
            JObject jsonObject = JObject.FromObject(queryRefundOrderRS);
            jsonObject.Add("reqTime", DateTimeOffset.Now.ToUnixTimeSeconds()); //添加请求时间

            // 报文签名
            jsonObject.Add("sign", AgPayUtil.GetSign(jsonObject, appSecret));   // 签名

            // 生成通知
            return URLUtil.AppendUrlQuery(refundOrder.NotifyUrl, jsonObject);
        }

        /// <summary>
        /// 创建响应URL
        /// </summary>
        /// <param name="transferOrder"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public string CreateNotifyUrl(TransferOrderDto transferOrder, string appSecret)
        {

            QueryTransferOrderRS rs = QueryTransferOrderRS.BuildByRecord(transferOrder);
            JObject jsonObject = JObject.FromObject(rs);
            jsonObject.Add("reqTime", DateTimeOffset.Now.ToUnixTimeSeconds()); //添加请求时间

            // 报文签名
            jsonObject.Add("sign", AgPayUtil.GetSign(jsonObject, appSecret));   // 签名

            // 生成通知
            return URLUtil.AppendUrlQuery(transferOrder.NotifyUrl, jsonObject);
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
                string notifyUrl = CreateNotifyUrl(dbRefundOrder, appSecret);
                mchNotifyRecord = new MchNotifyRecordDto();
                mchNotifyRecord.OrderId = dbRefundOrder.RefundOrderId;
                mchNotifyRecord.OrderType = (byte)MchNotifyRecordType.TYPE_REFUND_ORDER;
                mchNotifyRecord.MchNo = dbRefundOrder.MchNo;
                mchNotifyRecord.MchOrderNo = dbRefundOrder.MchRefundNo; //商户订单号
                mchNotifyRecord.IsvNo = dbRefundOrder.IsvNo;
                mchNotifyRecord.AppId = dbRefundOrder.AppId;
                mchNotifyRecord.NotifyUrl = notifyUrl;
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
                //mqSender.send(PayOrderMchNotifyMQ.build(notifyId));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "推送失败！");
            }
        }
    }
}
