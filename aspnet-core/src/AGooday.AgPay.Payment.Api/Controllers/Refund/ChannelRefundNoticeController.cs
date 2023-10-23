using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Payment.Api.Channel;
using AGooday.AgPay.Payment.Api.Controllers.PayOrder;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static AGooday.AgPay.Payment.Api.Channel.IChannelRefundNoticeService;

namespace AGooday.AgPay.Payment.Api.Controllers.Refund
{
    /// <summary>
    /// 渠道侧的退款通知入口【异步回调(doNotify)】
    /// </summary>
    [ApiController]
    public class ChannelRefundNoticeController : ControllerBase
    {
        private readonly ILogger<AbstractPayOrderController> log;
        private readonly IRefundOrderService refundOrderService;
        private readonly ConfigContextQueryService configContextQueryService;
        private readonly RefundOrderProcessService refundOrderProcessService;
        protected readonly Func<string, IChannelRefundNoticeService> channelRefundNoticeServiceFactory;

        public ChannelRefundNoticeController(ILogger<AbstractPayOrderController> log, 
            IRefundOrderService refundOrderService, 
            ConfigContextQueryService configContextQueryService, 
            RefundOrderProcessService refundOrderProcessService, 
            Func<string, IChannelRefundNoticeService> channelRefundNoticeServiceFactory)
        {
            this.log = log;
            this.refundOrderService = refundOrderService;
            this.configContextQueryService = configContextQueryService;
            this.refundOrderProcessService = refundOrderProcessService;
            this.channelRefundNoticeServiceFactory = channelRefundNoticeServiceFactory;
        }

        /// <summary>
        /// 异步回调入口
        /// </summary>
        /// <param name="ifCode"></param>
        /// <param name="refundOrderId"></param>
        /// <param name="urlOrderId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/refund/notify/{ifCode}")]
        [Route("/api/refund/notify/{ifCode}/{refundOrderId}")]
        public ActionResult DoNotify(string ifCode, string refundOrderId, string urlOrderId)
        {
            string logPrefix = $"进入[{ifCode}]退款回调：urlOrderId：[{urlOrderId}] ";
            log.LogInformation($"===== {logPrefix} =====");

            try
            {
                // 参数有误
                if (string.IsNullOrWhiteSpace(ifCode))
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, "ifCode is empty");
                }

                //查询退款接口是否存在
                IChannelRefundNoticeService refundNotifyService = channelRefundNoticeServiceFactory(ifCode);

                // 支付通道接口实现不存在
                if (refundNotifyService == null)
                {
                    log.LogError($"{logPrefix}, interface not exists ");
                    return BadRequest($"[{ifCode}] interface not exists");
                }

                // 解析订单号 和 请求参数
                Dictionary<string, object> mutablePair = refundNotifyService.ParseParams(Request, urlOrderId, NoticeTypeEnum.DO_NOTIFY);
                if (mutablePair == null)
                {
                    // 解析数据失败， 响应已处理
                    log.LogError($"{logPrefix}, mutablePair is null ", logPrefix);
                    throw new BizException("解析数据异常！"); //需要实现类自行抛出ResponseException, 不应该在这抛此异常。
                }

                // 解析到订单号
                refundOrderId = mutablePair.First().Key;
                log.LogInformation($"{logPrefix}, 解析数据为：refundOrderId:{refundOrderId}, params:{mutablePair.First().Value}");

                if (!string.IsNullOrWhiteSpace(urlOrderId) && !urlOrderId.Equals(refundOrderId))
                {
                    log.LogError($"{logPrefix}, 订单号不匹配. urlOrderId={urlOrderId}, refundOrderId={refundOrderId} ");
                    throw new BizException("退款单号不匹配！");
                }

                //获取订单号 和 订单数据
                RefundOrderDto refundOrder = refundOrderService.GetById(refundOrderId);

                // 订单不存在
                if (refundOrder == null)
                {
                    log.LogError($"{logPrefix}, 退款订单不存在. payOrderId={refundOrder} ");
                    return refundNotifyService.DoNotifyOrderNotExists(Request);
                }

                //查询出商户应用的配置信息
                MchAppConfigContext mchAppConfigContext = configContextQueryService.QueryMchInfoAndAppInfo(refundOrder.MchNo, refundOrder.AppId);

                //调起接口的回调判断
                ChannelRetMsg notifyResult = refundNotifyService.DoNotice(Request, mutablePair.First().Value, refundOrder, mchAppConfigContext, NoticeTypeEnum.DO_NOTIFY);

                // 返回null 表明出现异常， 无需处理通知下游等操作。
                if (notifyResult == null || notifyResult.ChannelState == null || notifyResult.ResponseEntity == null)
                {
                    log.LogError($"{logPrefix}, 处理回调事件异常  notifyResult data error, notifyResult ={notifyResult} ");
                    throw new BizException("处理回调事件异常！"); //需要实现类自行抛出ResponseException, 不应该在这抛此异常。
                }
                // 处理退款订单
                bool updateOrderSuccess = refundOrderProcessService.HandleRefundOrder4Channel(notifyResult, refundOrder);

                // 更新退款订单 异常
                if (!updateOrderSuccess)
                {
                    log.LogError($"{logPrefix}, updateOrderSuccess = {updateOrderSuccess} ");
                    return refundNotifyService.DoNotifyOrderStateUpdateFail(Request);
                }

                log.LogInformation($"===== {logPrefix}, 订单通知完成。 refundOrderId={refundOrderId}, parseState = {notifyResult.ChannelState} =====");

                return notifyResult.ResponseEntity;
            }
            catch (BizException e)
            {
                log.LogError(e, $"{logPrefix}, refundOrderId={refundOrderId}, BizException");
                return BadRequest(e.Message);
            }
            catch (ResponseException e)
            {
                log.LogError(e, $"{logPrefix}, refundOrderId={refundOrderId}, ResponseException");
                return e.ResponseEntity;
            }
            catch (Exception e)
            {
                log.LogError(e, $"{logPrefix}, refundOrderId={refundOrderId}, 系统异常");
                return BadRequest(e.Message);
            }
        }
    }
}
