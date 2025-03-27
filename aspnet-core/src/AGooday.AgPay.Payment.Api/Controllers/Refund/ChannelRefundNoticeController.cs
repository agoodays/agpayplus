﻿using System.Net;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using Microsoft.AspNetCore.Mvc;
using static AGooday.AgPay.Components.Third.Channel.IChannelRefundNoticeService;

namespace AGooday.AgPay.Payment.Api.Controllers.Refund
{
    /// <summary>
    /// 渠道侧的退款通知入口【异步回调(doNotify)】
    /// </summary>
    [ApiController]
    public class ChannelRefundNoticeController : ControllerBase
    {
        private readonly ILogger<ChannelRefundNoticeController> _logger;
        private readonly IRefundOrderService _refundOrderService;
        private readonly ConfigContextQueryService _configContextQueryService;
        private readonly RefundOrderProcessService _refundOrderProcessService;
        private readonly IChannelServiceFactory<IChannelRefundNoticeService> _channelRefundNoticeServiceFactory;

        public ChannelRefundNoticeController(ILogger<ChannelRefundNoticeController> logger,
            IRefundOrderService refundOrderService,
            ConfigContextQueryService configContextQueryService,
            RefundOrderProcessService refundOrderProcessService,
            IChannelServiceFactory<IChannelRefundNoticeService> channelRefundNoticeServiceFactory)
        {
            _logger = logger;
            _refundOrderService = refundOrderService;
            _configContextQueryService = configContextQueryService;
            _refundOrderProcessService = refundOrderProcessService;
            _channelRefundNoticeServiceFactory = channelRefundNoticeServiceFactory;
        }

        /// <summary>
        /// 异步回调入口
        /// </summary>
        /// <param name="ifCode"></param>
        /// <param name="refundOrderId"></param>
        /// <param name="urlOrderId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/refund/notify/{ifCode}")]
        [Route("api/refund/notify/{ifCode}/{refundOrderId}")]
        public async Task<ActionResult> DoNotifyAsync(string ifCode, string refundOrderId, string urlOrderId)
        {
            string logPrefix = $"进入[{ifCode}]退款回调：urlOrderId：[{urlOrderId}] ";
            _logger.LogInformation("===== {logPrefix} =====", logPrefix);
            //_logger.LogInformation($"===== {logPrefix} =====");

            try
            {
                // 参数有误
                if (string.IsNullOrWhiteSpace(ifCode))
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, "ifCode is empty");
                }

                //查询退款接口是否存在
                IChannelRefundNoticeService refundNotifyService = _channelRefundNoticeServiceFactory.GetService(ifCode);

                // 支付通道接口实现不存在
                if (refundNotifyService == null)
                {
                    _logger.LogError("{logPrefix}, interface not exists ", logPrefix);
                    //_logger.LogError($"{logPrefix}, interface not exists ");
                    return BadRequest($"[{ifCode}] interface not exists");
                }

                // 解析订单号 和 请求参数
                Dictionary<string, object> mutablePair = await refundNotifyService.ParseParamsAsync(Request, urlOrderId, NoticeTypeEnum.DO_NOTIFY);
                if (mutablePair == null)
                {
                    // 解析数据失败， 响应已处理
                    _logger.LogError("{logPrefix}, mutablePair is null ", logPrefix);
                    //_logger.LogError($"{logPrefix}, mutablePair is null ");
                    throw new BizException("解析数据异常！"); //需要实现类自行抛出ResponseException, 不应该在这抛此异常。
                }

                // 解析到订单号
                refundOrderId = mutablePair.First().Key;
                _logger.LogInformation("{logPrefix}, 解析数据为：refundOrderId={refundOrderId}, params={params}", logPrefix, refundOrderId, mutablePair.First().Value);
                //_logger.LogInformation($"{logPrefix}, 解析数据为：refundOrderId={refundOrderId}, params={mutablePair.First().Value}");

                if (!string.IsNullOrWhiteSpace(urlOrderId) && !urlOrderId.Equals(refundOrderId))
                {
                    _logger.LogError("{logPrefix}, 订单号不匹配. urlOrderId={urlOrderId}, refundOrderId={refundOrderId} ", logPrefix, urlOrderId, refundOrderId);
                    //_logger.LogError($"{logPrefix}, 订单号不匹配. urlOrderId={urlOrderId}, refundOrderId={refundOrderId} ");
                    throw new BizException("退款单号不匹配！");
                }

                //获取订单号 和 订单数据
                RefundOrderDto refundOrder = await _refundOrderService.GetByIdAsync(refundOrderId);

                // 订单不存在
                if (refundOrder == null)
                {
                    _logger.LogError("{logPrefix}, 退款订单不存在. refundOrderId={refundOrderId} ", logPrefix, refundOrderId);
                    //_logger.LogError($"{logPrefix}, 退款订单不存在. refundOrderId={refundOrderId} ");
                    return refundNotifyService.DoNotifyOrderNotExists(Request);
                }

                //查询出商户应用的配置信息
                MchAppConfigContext mchAppConfigContext = await _configContextQueryService.QueryMchInfoAndAppInfoAsync(refundOrder.MchNo, refundOrder.AppId);

                //调起接口的回调判断
                ChannelRetMsg notifyResult = await refundNotifyService.DoNoticeAsync(Request, mutablePair.First().Value, refundOrder, mchAppConfigContext, NoticeTypeEnum.DO_NOTIFY);

                // 返回null 表明出现异常， 无需处理通知下游等操作。
                if (notifyResult == null || notifyResult.ChannelState == null || notifyResult.ResponseEntity == null)
                {
                    _logger.LogError("{logPrefix}, 处理回调事件异常  notifyResult data error, notifyResult={notifyResult} ", logPrefix, notifyResult);
                    //_logger.LogError($"{logPrefix}, 处理回调事件异常  notifyResult data error, notifyResult={notifyResult} ");
                    throw new BizException("处理回调事件异常！"); //需要实现类自行抛出ResponseException, 不应该在这抛此异常。
                }
                // 处理退款订单
                bool updateOrderSuccess = await _refundOrderProcessService.HandleRefundOrder4ChannelAsync(notifyResult, refundOrder);

                // 更新退款订单 异常
                if (!updateOrderSuccess)
                {
                    _logger.LogError("{logPrefix}, updateOrderSuccess={updateOrderSuccess} ", logPrefix, updateOrderSuccess);
                    //_logger.LogError($"{logPrefix}, updateOrderSuccess = {updateOrderSuccess} ");
                    return refundNotifyService.DoNotifyOrderStateUpdateFail(Request);
                }

                _logger.LogInformation("===== {logPrefix}, 订单通知完成。 refundOrderId={refundOrderId}, parseState={notifyResult.ChannelState} =====", logPrefix, refundOrderId, notifyResult.ChannelState);
                //_logger.LogInformation($"===== {logPrefix}, 订单通知完成。 refundOrderId={refundOrderId}, parseState={notifyResult.ChannelState} =====");

                return notifyResult.ResponseEntity;
            }
            catch (BizException e)
            {
                _logger.LogError(e, "{logPrefix}, refundOrderId={refundOrderId}, BizException", logPrefix, refundOrderId);
                //_logger.LogError(e, $"{logPrefix}, refundOrderId={refundOrderId}, BizException");
                return BadRequest(e.Message);
            }
            catch (ResponseException e)
            {
                _logger.LogError(e, "{logPrefix}, refundOrderId={refundOrderId}, ResponseException", logPrefix, refundOrderId);
                //_logger.LogError(e, $"{logPrefix}, refundOrderId={refundOrderId}, ResponseException");
                return e.ResponseEntity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{logPrefix}, refundOrderId={refundOrderId}, 系统异常", logPrefix, refundOrderId);
                //_logger.LogError(e, $"{logPrefix}, refundOrderId={refundOrderId}, 系统异常");
                return BadRequest(e.Message);
            }
        }
    }
}
