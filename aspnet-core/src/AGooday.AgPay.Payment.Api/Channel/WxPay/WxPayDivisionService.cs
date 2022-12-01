using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Payment.Api.Channel.AliPay;
using AGooday.AgPay.Payment.Api.Channel.WxPay.Kits;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Models;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay
{
    /// <summary>
    /// 分账接口： 微信官方
    /// </summary>
    public class WxPayDivisionService : IDivisionService
    {
        private readonly ILogger<AliPayDivisionService> log;
        private readonly ConfigContextQueryService configContextQueryService;

        public WxPayDivisionService(ILogger<AliPayDivisionService> log, ConfigContextQueryService configContextQueryService)
        {
            this.log = log;
            this.configContextQueryService = configContextQueryService;
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.WXPAY;
        }

        public bool IsSupport()
        {
            return false;
        }
        public ChannelRetMsg Bind(MchDivisionReceiverDto mchDivisionReceiver, MchAppConfigContext mchAppConfigContext)
        {
            try
            {
                AddProfitSharingReceiverRequest request = new AddProfitSharingReceiverRequest();

                //不是特约商户， 无需放置此值
                if (mchAppConfigContext.IsIsvSubMch())
                {
                    WxPayIsvSubMchParams isvsubMchParams =
                            (WxPayIsvSubMchParams)configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, CS.IF_CODE.WXPAY);

                    request.SubMerchantId = isvsubMchParams.SubMchId;
                    request.SubAppId = isvsubMchParams.SubMchAppId;
                }

                // 0-个人， 1-商户  (目前仅支持服务商appI获取个人openId, 即： PERSONAL_OPENID， 不支持 PERSONAL_SUB_OPENID )
                request.Type = mchDivisionReceiver.AccType == 0 ? "PERSONAL_OPENID" : "MERCHANT_ID";
                request.Account = mchDivisionReceiver.AccNo;
                request.Name = mchDivisionReceiver.AccName;
                request.RelationType = mchDivisionReceiver.RelationType;
                request.CustomRelation = mchDivisionReceiver.RelationTypeName;

                WxServiceWrapper wxServiceWrapper = configContextQueryService.GetWxServiceWrapper(mchAppConfigContext);
                var client = (WechatTenpayClient)wxServiceWrapper.Client;
                var profitSharingReceiverResult = client.EncryptRequestSensitiveProperty(request);

                // 明确成功
                return ChannelRetMsg.ConfirmSuccess(null);
            }
            catch (WechatTenpayException wxPayException)
            {
                ChannelRetMsg channelRetMsg = ChannelRetMsg.ConfirmFail();
                WxPayKit.CommonSetErrInfo(channelRetMsg, wxPayException);
                return channelRetMsg;
            }
            catch (Exception e)
            {
                log.LogError(e, "请求微信绑定分账接口异常");
                ChannelRetMsg channelRetMsg = ChannelRetMsg.ConfirmFail();
                channelRetMsg.ChannelErrMsg = e.Message;
                return channelRetMsg;
            }
        }

        public ChannelRetMsg SingleDivision(PayOrderDto payOrder, List<PayOrderDivisionRecordDto> recordList, MchAppConfigContext mchAppConfigContext)
        {
            try
            {

                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                log.LogError(e, "微信分账失败");
                ChannelRetMsg channelRetMsg = ChannelRetMsg.ConfirmFail();
                channelRetMsg.ChannelErrMsg = e.Message;
                return channelRetMsg;
            }
        }
    }
}
