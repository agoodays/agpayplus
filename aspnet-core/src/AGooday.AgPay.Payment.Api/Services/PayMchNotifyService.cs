using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using log4net;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;

namespace AGooday.AgPay.Payment.Api.Services
{
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

        /** 商户通知信息， 只有订单是终态，才会发送通知， 如明确成功和明确失败 **/
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

        /**
         * 创建响应URL
         */
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
    }
}
