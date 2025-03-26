using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.AllinPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.AllinPay.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static AGooday.AgPay.Components.Third.Channel.IChannelNoticeService;

namespace AGooday.AgPay.Components.Third.Channel.AllinPay
{
    /// <summary>
    /// 通联回调
    /// </summary>
    public class AllinPayChannelNoticeService : AbstractChannelNoticeService
    {
        public AllinPayChannelNoticeService(ILogger<AllinPayChannelNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public AllinPayChannelNoticeService()
            : base()
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.ALLINPAY;
        }

        public override async Task<Dictionary<string, object>> ParseParamsAsync(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                JObject @params = await GetReqParamJSONAsync();
                string payOrderId = @params.GetValue("cusorderid").ToString();
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

                string logPrefix = "【处理通联支付回调】";
                // 获取请求参数
                JObject jsonParams = JObject.FromObject(@params);
                _logger.LogInformation("{logPrefix} 回调参数, jsonParams：{jsonParams}", logPrefix, jsonParams);
                //_logger.LogInformation($"{logPrefix} 回调参数, jsonParams：{jsonParams}");

                // 校验支付回调
                bool verifyResult = await VerifyParamsAsync(jsonParams, payOrder, mchAppConfigContext);
                // 验证参数失败
                if (!verifyResult)
                {
                    throw ResponseException.BuildText("ERROR");
                }
                _logger.LogInformation("{logPrefix}验证支付通知数据及签名通过", logPrefix);
                //_logger.LogInformation($"{logPrefix}验证支付通知数据及签名通过");

                //验签成功后判断上游订单状态
                var okResponse = TextResp("success");
                result.ResponseEntity = okResponse;

                jsonParams.TryGetString("cusid", out string cusid);//商户编号
                string trxid = jsonParams.GetValue("trxid").ToString();
                jsonParams.TryGetString("cusorderid", out string cusorderid);
                jsonParams.TryGetString("chnltrxid", out string chnltrxid);//微信/支付宝流水号
                /*买家用户号
                支付宝渠道：买家支付宝用户号buyer_user_id
                微信渠道：微信平台的sub_openid*/
                jsonParams.TryGetString("acct", out string acct);
                result.ChannelMchNo = cusid;
                //result.ChannelIsvNo = orgid;
                result.ChannelOrderId = trxid;
                result.ChannelUserId = acct;
                result.PlatformOrderId = chnltrxid;
                result.PlatformMchOrderId = cusorderid;
                result.ChannelState = ChannelState.CONFIRM_SUCCESS;
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public async Task<bool> VerifyParamsAsync(JObject jsonParams, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string ordNo = jsonParams.GetValue("cusorderid").ToString();       // 商户订单号
            string amt = jsonParams.GetValue("trxamt").ToString();         // 支付金额
            if (string.IsNullOrWhiteSpace(ordNo))
            {
                _logger.LogInformation("订单ID为空 [orderNo]={ordNo}", ordNo);
                //_logger.LogInformation($"订单ID为空 [orderNo]={ordNo}");
                return false;
            }
            if (string.IsNullOrWhiteSpace(amt))
            {
                _logger.LogInformation("金额参数为空 [amt] :{amt}", amt);
                //_logger.LogInformation($"金额参数为空 [amt] :{amt}");
                return false;
            }

            string publicKey;
            if (mchAppConfigContext.IsIsvSubMch())
            {
                // 获取支付参数
                AllinPayIsvParams isvParams = (AllinPayIsvParams)await _configContextQueryService.QueryIsvParamsAsync(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

                if (isvParams.Orgid == null)
                {
                    _logger.LogError("服务商配置为空：isvParams：{isvParams}", JsonConvert.SerializeObject(isvParams));
                    //_logger.LogError($"服务商配置为空：isvParams：{JsonConvert.SerializeObject(isvParams)}");
                    throw new BizException("服务商配置为空。");
                }
                publicKey = isvParams.PublicKey;
            }
            else
            {
                // 获取支付参数
                AllinPayNormalMchParams normalMchParams = (AllinPayNormalMchParams)await _configContextQueryService.QueryNormalMchParamsAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());
                publicKey = normalMchParams.PublicKey;
            }

            //验签
            if (!AllinPaySignUtil.Verify(jsonParams, publicKey))
            {
                _logger.LogInformation("【通联回调】 验签失败！ 回调参数：parameter = {jsonParams}, publicKey={publicKey}", jsonParams, publicKey);
                //_logger.LogInformation($"【通联回调】 验签失败！ 回调参数：parameter = {jsonParams}, publicKey={publicKey}");
                return false;
            }

            // 核对金额
            long dbPayAmt = payOrder.Amount;
            if (dbPayAmt != Convert.ToInt64(amt))
            {
                _logger.LogInformation("订单金额与参数金额不符。 dbPayAmt={dbPayAmt}, amt={amt}, payOrderId={ordNo}", dbPayAmt, amt, ordNo);
                //_logger.LogInformation($"订单金额与参数金额不符。 dbPayAmt={dbPayAmt}, amt={amt}, payOrderId={ordNo}");
                return false;
            }
            return true;
        }
    }
}
