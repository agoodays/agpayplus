﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.SxfPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.SxfPay.Utils;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using Newtonsoft.Json.Linq;
using static AGooday.AgPay.Components.Third.Channel.IChannelNoticeService;

namespace AGooday.AgPay.Components.Third.Channel.SxfPay
{
    /// <summary>
    /// 随行付回调
    /// </summary>
    public class SxfPayChannelNoticeService : AbstractChannelNoticeService
    {
        public SxfPayChannelNoticeService(ILogger<SxfPayChannelNoticeService> logger,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(logger, requestKit, configContextQueryService)
        {
        }

        public override string GetIfCode()
        {
            return CS.IF_CODE.SXFPAY;
        }

        public override Dictionary<string, object> ParseParams(HttpRequest request, string urlOrderId, NoticeTypeEnum noticeTypeEnum)
        {
            try
            {
                JObject @params = GetReqParamJSON();
                string payOrderId = @params.GetValue("ordNo").ToString();
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

                string logPrefix = "【处理随行付支付回调】";
                // 获取请求参数
                JObject jsonParams = JObject.FromObject(@params);
                _logger.LogInformation($"{logPrefix} 回调参数, jsonParams：{jsonParams}");

                // 校验支付回调
                bool verifyResult = VerifyParams(jsonParams, payOrder, mchAppConfigContext);
                // 验证参数失败
                if (!verifyResult)
                {
                    throw ResponseException.BuildText("ERROR");
                }
                _logger.LogInformation($"{logPrefix}验证支付通知数据及签名通过");

                //验签成功后判断上游订单状态
                JObject resJSON = new JObject();
                resJSON.Add("code", "success");
                resJSON.Add("msg", "成功");
                var okResponse = JsonResp(resJSON);
                result.ResponseEntity = okResponse;

                string bizCode = jsonParams.GetValue("bizCode").ToString(); //业务响应码
                string bizMsg = jsonParams.GetValue("bizMsg").ToString(); //业务响应信息
                if ("0000".Equals(bizCode))
                {
                    jsonParams.TryGetString("mno", out string mno);//商户编号
                    jsonParams.TryGetString("orgId", out string orgId);//机构号
                    string uuid = jsonParams.GetValue("uuid").ToString();//天阙平台订单号
                    /*落单号
                    仅供退款使用
                    消费者账单中的条形码订单号*/
                    jsonParams.TryGetString("sxfUuid", out string sxfUuid);
                    jsonParams.TryGetString("channelId", out string channelId);//渠道商商户号
                    jsonParams.TryGetString("transactionId", out string transactionId);//微信/支付宝流水号
                    /*买家用户号
                    支付宝渠道：买家支付宝用户号buyer_user_id
                    微信渠道：微信平台的sub_openid*/
                    jsonParams.TryGetString("buyerId", out string buyerId);
                    result.ChannelMchNo = mno;
                    result.ChannelIsvNo = orgId;
                    result.ChannelOrderId = uuid;
                    result.ChannelUserId = buyerId;
                    result.PlatformOrderId = transactionId;
                    result.PlatformMchOrderId = sxfUuid;
                    result.ChannelState = ChannelState.CONFIRM_SUCCESS;
                }
                else
                {
                    result.ChannelState = ChannelState.CONFIRM_FAIL;
                    result.ChannelErrCode = bizCode;
                    result.ChannelErrMsg = bizMsg;
                }
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                throw ResponseException.BuildText("ERROR");
            }
        }

        public bool VerifyParams(JObject jsonParams, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string ordNo = jsonParams.GetValue("ordNo").ToString();       // 商户订单号
            string amt = jsonParams.GetValue("amt").ToString();         // 支付金额
            if (string.IsNullOrWhiteSpace(ordNo))
            {
                _logger.LogInformation($"订单ID为空 [orderNo]={ordNo}");
                return false;
            }
            if (string.IsNullOrWhiteSpace(amt))
            {
                _logger.LogInformation($"金额参数为空 [amt] :{amt}");
                return false;
            }

            SxfPayIsvParams isvParams = (SxfPayIsvParams)configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, GetIfCode());

            //验签
            string publicKey = isvParams.PublicKey;

            //验签失败
            if (!SxfSignUtil.Verify(jsonParams, publicKey))
            {
                _logger.LogInformation($"【随行付回调】 验签失败！ 回调参数：parameter = {jsonParams}, publicKey={publicKey} ");
                return false;
            }

            // 核对金额
            long dbPayAmt = payOrder.Amount;
            if (dbPayAmt != Convert.ToInt64(amt))
            {
                _logger.LogInformation($"订单金额与参数金额不符。 dbPayAmt={dbPayAmt}, amt={amt}, payOrderId={ordNo}");
                return false;
            }
            return true;
        }
    }
}
