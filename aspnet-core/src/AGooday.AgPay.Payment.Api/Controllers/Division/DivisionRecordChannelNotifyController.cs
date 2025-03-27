using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.Division
{
    public class DivisionRecordChannelNotifyController : ApiControllerBase
    {
        private readonly ILogger<DivisionRecordChannelNotifyController> _logger;
        private readonly IPayOrderDivisionRecordService _payOrderDivisionRecordService;
        private readonly IChannelServiceFactory<AbstractDivisionRecordChannelNotifyService> _divisionRecordChannelNotifyServiceFactory;

        public DivisionRecordChannelNotifyController(ILogger<DivisionRecordChannelNotifyController> logger,
            IPayOrderDivisionRecordService payOrderDivisionRecordService,
            IChannelServiceFactory<AbstractDivisionRecordChannelNotifyService> divisionRecordChannelNotifyServiceFactory,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(requestKit, configContextQueryService)
        {
            _logger = logger;
            _payOrderDivisionRecordService = payOrderDivisionRecordService;
            _divisionRecordChannelNotifyServiceFactory = divisionRecordChannelNotifyServiceFactory;
        }

        [HttpPost]
        [Route("api/divisionRecordChannelNotify/{ifCode}")]
        public async Task<IActionResult> DoNotifyAsync([FromRoute] string ifCode)
        {
            string divisionBatchId = null;
            string logPrefix = $"进入[{ifCode}]分账回调";
            _logger.LogInformation("===== {logPrefix} =====", logPrefix);
            //_logger.LogInformation($"===== {logPrefix} =====");

            try
            {
                // 参数有误
                if (string.IsNullOrEmpty(ifCode))
                {
                    return BadRequest("ifCode is empty");
                }

                //查询支付接口是否存在
                AbstractDivisionRecordChannelNotifyService divisionNotifyService = _divisionRecordChannelNotifyServiceFactory.GetService(ifCode);

                // 支付通道接口实现不存在
                if (divisionNotifyService == null)
                {
                    _logger.LogError("{logPrefix}, interface not exists", logPrefix);
                    //_logger.LogError($"{logPrefix}, interface not exists");
                    return BadRequest($"[{ifCode}] interface not exists");
                }

                // 解析批次号 和 请求参数
                Dictionary<string, object> mutablePair = await divisionNotifyService.ParseParamsAsync(Request);
                if (mutablePair == null)
                {
                    _logger.LogError("{logPrefix}, mutablePair is null", logPrefix);
                    //_logger.LogError($"{logPrefix}, mutablePair is null");
                    throw new BizException("解析数据异常！"); //需要实现类自行抛出ResponseException, 不应该在这抛此异常。
                }

                //解析到订单号
                divisionBatchId = mutablePair.First().Key;
                _logger.LogInformation("{logPrefix}, 解析数据为: divisionBatchId={divisionBatchId}, params={params}", logPrefix, divisionBatchId, mutablePair.First().Value);
                //_logger.LogInformation($"{logPrefix}, 解析数据为: divisionBatchId={divisionBatchId}, params={mutablePair.First().Value}");

                // 通过 batchId 查询出列表（ 注意：  需要按照ID 排序！！！！ ）
                List<PayOrderDivisionRecordDto> recordList = _payOrderDivisionRecordService.GetByBatchOrderId(new PayOrderDivisionRecordQueryDto()
                {
                    BatchOrderId = divisionBatchId,
                    State = (byte)PayOrderDivisionRecordState.STATE_ACCEPT,
                });

                // 订单不存在
                if (recordList == null || recordList.Count == 0)
                {
                    _logger.LogError("{logPrefix}, 待处理订单不存在. divisionBatchId={divisionBatchId}", logPrefix, divisionBatchId);
                    //_logger.LogError($"{logPrefix}, 待处理订单不存在. divisionBatchId={divisionBatchId}");
                    return divisionNotifyService.DoNotifyOrderNotExists(Request);
                }

                //查询出商户应用的配置信息
                MchAppConfigContext mchAppConfigContext = await _configContextQueryService.QueryMchInfoAndAppInfoAsync(recordList[0].MchNo, recordList[0].AppId);

                //调起接口的回调判断
                DivisionChannelNotifyModel notifyResult = await divisionNotifyService.DoNotifyAsync(Request, mutablePair.First().Value, recordList, mchAppConfigContext);

                // 返回null 表明出现异常， 无需处理通知下游等操作。
                if (notifyResult == null || notifyResult.ApiRes == null)
                {
                    _logger.LogError("{logPrefix}, 处理回调事件异常  notifyResult data error, notifyResult ={notifyResult}", logPrefix, notifyResult);
                    //_logger.LogError($"{logPrefix}, 处理回调事件异常  notifyResult data error, notifyResult ={notifyResult}");
                    throw new BizException("处理回调事件异常！"); //需要实现类自行抛出ResponseException, 不应该在这抛此异常。
                }

                if (notifyResult.RecordResultMap != null && notifyResult.RecordResultMap.Count > 0)
                {
                    foreach (long divisionId in notifyResult.RecordResultMap.Keys)
                    {
                        // 单条结果
                        ChannelRetMsg retMsgItem = notifyResult.RecordResultMap[divisionId];

                        // 明确成功
                        if (retMsgItem.ChannelState == ChannelState.CONFIRM_SUCCESS)
                        {
                            await _payOrderDivisionRecordService.UpdateRecordSuccessOrFailBySingleItemAsync(divisionId, (byte)PayOrderDivisionRecordState.STATE_SUCCESS, retMsgItem.ChannelOriginResponse);
                        }
                        else if (retMsgItem.ChannelState == ChannelState.CONFIRM_FAIL) // 明确失败
                        {
                            await _payOrderDivisionRecordService.UpdateRecordSuccessOrFailBySingleItemAsync(divisionId, (byte)PayOrderDivisionRecordState.STATE_FAIL, string.IsNullOrEmpty(retMsgItem.ChannelErrMsg) ? retMsgItem.ChannelOriginResponse : retMsgItem.ChannelErrMsg);
                        }
                    }
                }

                _logger.LogInformation("===== {logPrefix}, 通知完成。 divisionBatchId={divisionBatchId}, parseState = {notifyResult} =====", logPrefix, divisionBatchId, notifyResult);
                //_logger.LogInformation($"===== {logPrefix}, 通知完成。 divisionBatchId={divisionBatchId}, parseState = {notifyResult} =====");

                return notifyResult.ApiRes;
            }
            catch (BizException e)
            {
                _logger.LogError(e, "{logPrefix}, divisionBatchId={divisionBatchId}, BizException", logPrefix, divisionBatchId);
                //_logger.LogError(e, $"{logPrefix}, divisionBatchId={divisionBatchId}, BizException");
                return BadRequest(e.Message);
            }
            catch (ResponseException e)
            {
                _logger.LogError(e, "{logPrefix}, divisionBatchId={divisionBatchId}, ResponseException", logPrefix, divisionBatchId);
                //_logger.LogError(e, $"{logPrefix}, divisionBatchId={divisionBatchId}, ResponseException");
                return e.ResponseEntity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{logPrefix}, divisionBatchId={divisionBatchId}, 系统异常", logPrefix, divisionBatchId);
                //_logger.LogError(e, $"{logPrefix}, divisionBatchId={divisionBatchId}, 系统异常");
                return BadRequest(e.Message);
            }
        }
    }
}
