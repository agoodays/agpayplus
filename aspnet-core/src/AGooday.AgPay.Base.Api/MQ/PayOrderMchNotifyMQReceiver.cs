using System.Text;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using Newtonsoft.Json;

namespace AGooday.AgPay.Base.Api.MQ
{
    /// <summary>
    /// 接收MQ消息
    /// 业务：支付订单商户通知
    /// </summary>
    public class PayOrderMchNotifyMQReceiver : PayOrderMchNotifyMQ.IMQReceiver
    {
        private readonly IMQSender _mqSender;
        private readonly ILogger<PayOrderMchNotifyMQReceiver> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PayOrderMchNotifyMQReceiver(ILogger<PayOrderMchNotifyMQReceiver> logger,
            IMQSender mqSender,
            IServiceScopeFactory serviceScopeFactory)
        {
            _mqSender = mqSender;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task ReceiveAsync(PayOrderMchNotifyMQ.MsgPayload payload)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var payOrderService = scope.ServiceProvider.GetService<IPayOrderService>();
                var mchNotifyRecordService = scope.ServiceProvider.GetService<IMchNotifyRecordService>();
                try
                {
                    _logger.LogInformation("接收商户通知MQ, 消息: {payload}", JsonConvert.SerializeObject(payload));
                    //_logger.LogInformation($"接收商户通知MQ, 消息: {JsonConvert.SerializeObject(payload)}");

                    long notifyId = payload.NotifyId;
                    MchNotifyRecordDto record = await mchNotifyRecordService.GetByIdAsync(notifyId);
                    if (record == null || record.State != (byte)MchNotifyRecordState.STATE_ING)
                    {
                        _logger.LogInformation("查询通知记录不存在或状态不是通知中");
                        return;
                    }
                    if (record.NotifyCount >= record.NotifyCountLimit)
                    {
                        _logger.LogInformation("已达到最大发送次数");
                        return;
                    }

                    //1. (发送结果最多6次)
                    int currentCount = record.NotifyCount + 1;

                    string method = record.ReqMethod;
                    string mediaType = record.ReqMediaType;
                    string body = record.ReqBody;
                    string notifyUrl = record.NotifyUrl;
                    string res = "";
                    try
                    {
                        using var client = new HttpClient();
                        client.Timeout = TimeSpan.FromSeconds(20000);
                        string result = string.Empty;
                        StringContent content = null;
                        HttpResponseMessage response = null;
                        switch (method)
                        {
                            case "POST":
                                content = new StringContent(body, Encoding.UTF8, mediaType);
                                response = await client.PostAsync(notifyUrl, content);
                                res = await response.Content.ReadAsStringAsync();
                                break;
                            case "PUT":
                                break;
                            case "GET":
                                response = await client.GetAsync(notifyUrl);
                                res = await response.Content.ReadAsStringAsync();
                                break;
                            case "DELETE":
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "http error");
                        res = $"连接[{notifyUrl}]异常:【{e.Message}】";
                    }

                    //支付订单 & 第一次通知: 更新为已通知
                    if (currentCount == 1 && (byte)MchNotifyRecordType.TYPE_PAY_ORDER == record.OrderType)
                    {
                        await payOrderService.UpdateNotifySentAsync(record.OrderId);
                    }

                    //通知成功
                    if ("SUCCESS".Equals(res, StringComparison.OrdinalIgnoreCase))
                    {
                        await mchNotifyRecordService.UpdateNotifyResultAsync(notifyId, (byte)MchNotifyRecordState.STATE_SUCCESS, res);
                        return;
                    }

                    //通知次数 >= 最大通知次数时， 更新响应结果为异常， 不在继续延迟发送消息
                    if (currentCount >= record.NotifyCountLimit)
                    {
                        await mchNotifyRecordService.UpdateNotifyResultAsync(notifyId, (byte)MchNotifyRecordState.STATE_FAIL, res);
                        return;
                    }

                    // 继续发送MQ 延迟发送
                    await mchNotifyRecordService.UpdateNotifyResultAsync(notifyId, (byte)MchNotifyRecordState.STATE_ING, res);
                    // 通知延时次数
                    //        1   2   3   4   5   6
                    //        0   30  60  90  120 150
                    await _mqSender.SendAsync(PayOrderMchNotifyMQ.Build(notifyId), currentCount * 30);

                    return;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    return;
                }
            }
        }
    }
}
