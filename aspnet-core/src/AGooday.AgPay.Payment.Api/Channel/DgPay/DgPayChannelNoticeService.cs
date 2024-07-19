using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.DgPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.DgPay.Enumerator;
using AGooday.AgPay.Payment.Api.Channel.DgPay.Utils;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using static AGooday.AgPay.Payment.Api.Channel.IChannelNoticeService;

namespace AGooday.AgPay.Payment.Api.Channel.DgPay
{
    /// <summary>
    /// 斗拱回调
    /// </summary>
    public class DgPayChannelNoticeService : AbstractChannelNoticeService
    {
        public DgPayChannelNoticeService(ILogger<DgPayChannelNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.DGPAY;
        }

        public override Dictionary<string, object> ParseParams(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                JObject @params = GetReqParamJSON();
                var data = @params.GetValue("resp_data")?.ToObject<JObject>();
                string payOrderId = data?.GetValue("req_seq_id")?.ToString();
                return new Dictionary<string, object>() { { payOrderId, @params } };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public override ChannelRetMsg DoNotice(HttpRequest request, object @params, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                ChannelRetMsg result = ChannelRetMsg.ConfirmSuccess(null);

                string logPrefix = "【处理斗拱支付回调】";
                // 获取请求参数
                JObject jsonParams = JObject.FromObject(@params);
                _logger.LogInformation($"{logPrefix} 回调参数, jsonParams：{jsonParams}");

                // 校验支付回调
                bool verifyResult = VerifyParams(jsonParams, payOrder, mchAppConfigContext);
                // 验证参数失败
                if (!verifyResult)
                {
                    throw ResponseException.BuildText("ERROR", (int)HttpStatusCode.BadRequest);
                }
                _logger.LogInformation($"{logPrefix}验证支付通知数据及签名通过");

                //验签成功后判断上游订单状态
                JObject resJSON = new JObject();
                resJSON.Add("resp_code", "00000000");
                resJSON.Add("resp_desc", "成功");
                var okResponse = JsonResp(resJSON);
                result.ResponseEntity = okResponse;

                var data = jsonParams.GetValue("resp_data")?.ToObject<JObject>();
                string respCode = data?.GetValue("resp_code").ToString(); //业务响应码
                string respDesc = data?.GetValue("resp_desc").ToString(); //业务响应信息
                string bankCode = null, bankDesc = null, bankMessage = null;
                data?.TryGetString("bank_code", out bankCode); //外部通道返回码
                data?.TryGetString("bank_desc", out bankDesc); //外部通道返回描述
                data?.TryGetString("bank_message", out bankMessage); //外部通道返回描述
                string code = bankCode ?? respCode;
                string msg = (bankMessage ?? bankDesc) ?? respDesc;
                if ("00000000".Equals(respCode))
                {
                    data.TryGetString("huifu_id", out string huifuId);//商户编号
                    data.TryGetString("hf_seq_id", out string hfSeqId);//全局流水号
                    data.TryGetString("req_seq_id", out string reqSeqId);//请求流水号
                    data.TryGetString("out_trans_id", out string outTransId);//用户账单上的交易订单号	
                    data.TryGetString("party_order_id", out string partyOrderId);//用户账单上的商户订单号	
                    var wxResponse = data.GetValue("wx_response")?.ToObject<JObject>(); ;
                    var alipayResponse = data.GetValue("alipay_response")?.ToObject<JObject>();
                    var unionpayResponse = data.GetValue("unionpay_response")?.ToObject<JObject>();
                    var subOpenid = wxResponse?.GetValue("sub_openid").ToString();
                    var buyerId = alipayResponse?.GetValue("buyer_id").ToString();
                    string _transStat = data.GetValue("trans_stat").ToString();
                    var transStat = DgPayEnum.ConvertTransStat(_transStat);

                    switch (transStat)
                    {
                        case DgPayEnum.TransStat.S:
                            result.ChannelMchNo = huifuId;
                            result.ChannelOrderId = hfSeqId;
                            result.ChannelUserId = subOpenid ?? buyerId;
                            result.PlatformOrderId = outTransId;
                            result.PlatformMchOrderId = partyOrderId;
                            result.ChannelState = ChannelState.CONFIRM_SUCCESS;
                            break;
                        case DgPayEnum.TransStat.F:
                            result.ChannelState = ChannelState.CONFIRM_FAIL;
                            result.ChannelErrCode = code;
                            result.ChannelErrMsg = msg;
                            break;
                    }
                }
                else if ("90000000".Equals(respCode))
                {
                    result.ChannelState = ChannelState.CONFIRM_FAIL;
                    result.ChannelErrCode = code;
                    result.ChannelErrMsg = msg;
                }
                else
                {
                    result.ChannelState = ChannelState.CONFIRM_FAIL;
                    result.ChannelErrCode = code;
                    result.ChannelErrMsg = msg;
                }
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR", (int)HttpStatusCode.BadRequest);
            }
        }

        public bool VerifyParams(JObject jsonParams, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string reqSeqId = jsonParams.GetValue("req_seq_id").ToString(); // 商户订单号
            string transAmt = jsonParams.GetValue("trans_amt").ToString();  // 支付金额
            if (string.IsNullOrWhiteSpace(reqSeqId))
            {
                _logger.LogInformation($"订单ID为空 [reqSeqId]={reqSeqId}");
                return false;
            }
            if (string.IsNullOrWhiteSpace(transAmt))
            {
                _logger.LogInformation($"金额参数为空 [amt] :{transAmt}");
                return false;
            }

            //验签
            string publicKey;
            if (mchAppConfigContext.IsIsvSubMch())
            {
                DgPayIsvParams isvParams = (DgPayIsvParams)configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

                if (isvParams.SysId == null)
                {
                    _logger.LogError($"服务商配置为空：isvParams：{JsonConvert.SerializeObject(isvParams)}");
                    throw new BizException("服务商配置为空。");
                }
                publicKey = isvParams.RsaPublicKey;
            }
            else
            {
                var normalMchParams = (DgPayNormalMchParams)configContextQueryService.QueryNormalMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());

                if (normalMchParams.HuifuId == null)
                {
                    _logger.LogError($"商户配置为空：normalMchParams：{JsonConvert.SerializeObject(normalMchParams)}");
                    throw new BizException("商户配置为空。");
                }

                publicKey = normalMchParams.RsaPublicKey;
            }

            //验签失败
            if (!DgSignUtil.Verify(jsonParams, publicKey))
            {
                _logger.LogInformation($"【斗拱回调】 验签失败！ 回调参数：parameter = {jsonParams}, publicKey={publicKey} ");
                return false;
            }

            // 核对金额
            long dbPayAmt = payOrder.Amount;
            if (dbPayAmt != Convert.ToInt64(transAmt))
            {
                _logger.LogInformation($"订单金额与参数金额不符。 dbPayAmt={dbPayAmt}, amt={transAmt}, payOrderId={reqSeqId}");
                return false;
            }
            return true;
        }
    }
}
