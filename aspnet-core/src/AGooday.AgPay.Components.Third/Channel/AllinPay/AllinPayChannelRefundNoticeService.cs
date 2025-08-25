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
using static AGooday.AgPay.Components.Third.Channel.IChannelRefundNoticeService;

namespace AGooday.AgPay.Components.Third.Channel.AllinPay
{
    /// <summary>
    /// 通联 退款回调接口实现类
    /// </summary>
    public class AllinPayChannelRefundNoticeService : AbstractChannelRefundNoticeService
    {
        public AllinPayChannelRefundNoticeService(ILogger<AllinPayChannelRefundNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public AllinPayChannelRefundNoticeService()
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
                JObject @params = await GetReqParamJsonAsync();
                string refundOrderId = @params.GetValue("cusorderid").ToString();
                return new Dictionary<string, object>() { { refundOrderId, @params } };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public override async Task<ChannelRetMsg> DoNoticeAsync(HttpRequest request, object @params, RefundOrderDto payOrder, MchAppConfigContext mchAppConfigContext, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                ChannelRetMsg result = ChannelRetMsg.ConfirmSuccess(null);

                string logPrefix = "【处理通联退款回调】";
                // 获取请求参数
                JObject jsonParams = JObject.FromObject(@params);
                _logger.LogInformation("{logPrefix} 回调参数, jsonParams={jsonParams}", logPrefix, jsonParams);
                //_logger.LogInformation($"{logPrefix} 回调参数, jsonParams={jsonParams}");

                // 校验退款回调
                bool verifyResult = await VerifyParamsAsync(jsonParams, mchAppConfigContext);
                // 验证参数失败
                if (!verifyResult)
                {
                    throw ResponseException.BuildText("ERROR");
                }
                _logger.LogInformation("{logPrefix} 验证退款通知数据及签名通过", logPrefix);
                //_logger.LogInformation($"{logPrefix}验证退款通知数据及签名通过");

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

        public async Task<bool> VerifyParamsAsync(JObject jsonParams, MchAppConfigContext mchAppConfigContext)
        {
            string publicKey;
            if (mchAppConfigContext.IsIsvSubMch())
            {
                // 获取支付参数
                AllinPayIsvParams isvParams = (AllinPayIsvParams)await _configContextQueryService.QueryIsvParamsAsync(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

                if (isvParams.Orgid == null)
                {
                    _logger.LogError("服务商配置为空: isvParams={isvParams}", JsonConvert.SerializeObject(isvParams));
                    //_logger.LogError($"服务商配置为空: isvParams=JsonConvert.SerializeObject(isvParams)}");
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

            //验签失败
            if (!AllinPaySignUtil.Verify(jsonParams, publicKey))
            {
                _logger.LogInformation("【通联回调】 验签失败, 回调参数: parameter={jsonParams}, publicKey={publicKey}", jsonParams, publicKey);
                //_logger.LogInformation($"【通联回调】 验签失败, 回调参数：parameter={jsonParams}, publicKey={publicKey}");
                return false;
            }
            return true;
        }
    }
}
