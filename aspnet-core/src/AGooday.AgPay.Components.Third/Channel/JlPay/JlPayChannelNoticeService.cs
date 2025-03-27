using System.Net;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.JlPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.JlPay.Enumerator;
using AGooday.AgPay.Components.Third.Channel.JlPay.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static AGooday.AgPay.Components.Third.Channel.IChannelNoticeService;

namespace AGooday.AgPay.Components.Third.Channel.JlPay
{
    /// <summary>
    /// 嘉联回调
    /// </summary>
    public class JlPayChannelNoticeService : AbstractChannelNoticeService
    {
        public JlPayChannelNoticeService(ILogger<JlPayChannelNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public JlPayChannelNoticeService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.JLPAY;
        }

        public override async Task<Dictionary<string, object>> ParseParamsAsync(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                JObject @params = await GetReqParamJSONAsync();
                string payOrderId = @params?.GetValue("out_trade_no")?.ToString();
                return new Dictionary<string, object>() { { payOrderId, @params } };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public override async Task<ChannelRetMsg> DoNoticeAsync(HttpRequest request, object @params, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                ChannelRetMsg result = ChannelRetMsg.ConfirmSuccess(null);

                string logPrefix = "【处理嘉联支付回调】";
                // 获取请求参数
                JObject jsonParams = JObject.FromObject(@params);
                _logger.LogInformation("{logPrefix} 回调参数, 报文: {jsonParams}", logPrefix, jsonParams);
                //_logger.LogInformation($"{logPrefix} 回调参数, 报文: {jsonParams}");

                // 校验支付回调
                bool verifyResult = await VerifyParamsAsync(jsonParams, payOrder, mchAppConfigContext);
                // 验证参数失败
                if (!verifyResult)
                {
                    throw ResponseException.BuildText("ERROR", (int)HttpStatusCode.BadRequest);
                }
                _logger.LogInformation("{logPrefix} 验证支付通知数据及签名通过", logPrefix);
                //_logger.LogInformation($"{logPrefix} 验证支付通知数据及签名通过");

                //验签成功后判断上游订单状态
                JObject resJSON = new JObject();
                resJSON.Add("retCode", "success");
                var okResponse = JsonResp(resJSON);
                result.ResponseEntity = okResponse;

                string retCode = resJSON?.GetValue("ret_code").ToString(); //业务响应码
                string retMsg = resJSON?.GetValue("ret_msg").ToString(); //业务响应信息	
                string mchId = resJSON?.GetValue("mch_id")?.ToString();
                string orgCode = resJSON?.GetValue("org_code")?.ToString();
                result.ChannelMchNo = mchId;
                result.ChannelIsvNo = orgCode;
                if ("00".Equals(retCode))
                {
                    resJSON.TryGetString("transaction_id", out string transactionId);
                    resJSON.TryGetString("chn_transaction_id", out string chnTransactionId);//用户账单上的交易订单号	
                    resJSON.TryGetString("sub_openid", out string subOpenid);
                    string _status = resJSON.GetValue("status").ToString();
                    var status = JlPayEnum.ConvertStatus(_status);
                    switch (status)
                    {
                        case JlPayEnum.Status.Success:
                            result.ChannelMchNo = mchId;
                            result.ChannelOrderId = transactionId;
                            result.ChannelUserId = subOpenid;
                            result.PlatformOrderId = chnTransactionId;
                            result.PlatformMchOrderId = transactionId;
                            result.ChannelState = ChannelState.CONFIRM_SUCCESS;
                            break;
                        case JlPayEnum.Status.Failure:
                            result.ChannelState = ChannelState.CONFIRM_FAIL;
                            result.ChannelErrCode = retCode;
                            result.ChannelErrMsg = retMsg;
                            break;
                    }
                }
                else
                {
                    result.ChannelState = ChannelState.CONFIRM_FAIL;
                    result.ChannelErrCode = retCode;
                    result.ChannelErrMsg = retMsg;
                }
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR", (int)HttpStatusCode.BadRequest);
            }
        }

        public async Task<bool> VerifyParamsAsync(JObject jsonParams, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string payorderId = jsonParams.GetValue("out_trade_no").ToString(); // 商户订单号
            string amt = jsonParams.GetValue("total_fee").ToString();  // 支付金额
            if (string.IsNullOrWhiteSpace(payorderId))
            {
                _logger.LogInformation("订单ID为空 [payorderId]={payorderId}", payorderId);
                //_logger.LogInformation($"订单ID为空 [payorderId]={payorderId}");
                return false;
            }
            if (string.IsNullOrWhiteSpace(amt))
            {
                _logger.LogInformation("金额参数为空 [amt]={amt}", amt);
                //_logger.LogInformation($"金额参数为空 [amt]={amt}");
                return false;
            }

            //验签
            string publicKey;
            if (mchAppConfigContext.IsIsvSubMch())
            {
                JlPayIsvParams isvParams = (JlPayIsvParams)await _configContextQueryService.QueryIsvParamsAsync(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

                if (isvParams.OrgCode == null)
                {
                    _logger.LogError("服务商配置为空: isvParams={isvParams}", JsonConvert.SerializeObject(isvParams));
                    //_logger.LogError($"服务商配置为空: isvParams={JsonConvert.SerializeObject(isvParams)}");
                    throw new BizException("服务商配置为空。");
                }
                publicKey = isvParams.RsaPublicKey;
            }
            else
            {
                throw new BizException("指定通道不支持普通商户模式。");
            }

            //验签失败
            if (!JlPaySignUtil.Verify(jsonParams, publicKey))
            {
                _logger.LogInformation("【嘉联回调】 验签失败！ 回调参数: parameter={jsonParams}, publicKey={publicKey}", jsonParams, publicKey);
                //_logger.LogInformation($"【嘉联回调】 验签失败！ 回调参数: parameter={jsonParams}, publicKey={publicKey}");
                return false;
            }

            // 核对金额
            long dbPayAmt = payOrder.Amount;
            if (dbPayAmt != Convert.ToInt64(amt))
            {
                _logger.LogInformation("订单金额与参数金额不符。 dbPayAmt={dbPayAmt}, amt={amt}, payOrderId={payorderId}", dbPayAmt, amt, payorderId);
                //_logger.LogInformation($"订单金额与参数金额不符。 dbPayAmt={dbPayAmt}, amt={amt}, payOrderId={payorderId}");
                return false;
            }
            return true;
        }
    }
}
