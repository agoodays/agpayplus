using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Exceptions;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using log4net;
using Org.BouncyCastle.Utilities;

namespace AGooday.AgPay.Payment.Api.Channel.AliPay
{
    /// <summary>
    /// 分账接口： 支付宝官方
    /// </summary>
    public class AliPayDivisionService : IDivisionService
    {
        private readonly ILogger<AliPayDivisionService> log;
        private readonly ConfigContextQueryService configContextQueryService;

        public AliPayDivisionService(ILogger<AliPayDivisionService> logger, ConfigContextQueryService configContextQueryService)
        {
            this.log = logger;
            this.configContextQueryService = configContextQueryService;
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.ALIPAY;
        }

        public bool IsSupport()
        {
            return false;
        }

        public ChannelRetMsg Bind(MchDivisionReceiverDto mchDivisionReceiver, MchAppConfigContext mchAppConfigContext)
        {
            try
            {
                AlipayTradeRoyaltyRelationBindRequest request = new AlipayTradeRoyaltyRelationBindRequest();
                AlipayTradeRoyaltyRelationBindModel model = new AlipayTradeRoyaltyRelationBindModel();
                request.SetBizModel(model);
                model.OutRequestNo = SeqUtil.GenDivisionBatchId();

                //统一放置 isv接口必传信息
                AliPayKit.PutApiIsvInfo(mchAppConfigContext, request, model);

                RoyaltyEntity royaltyEntity = new RoyaltyEntity();

                royaltyEntity.Type = "loginName";
                if (RegUtil.IsAliPayUserId(mchDivisionReceiver.AccNo))
                {
                    royaltyEntity.Type = "userId";
                }
                royaltyEntity.Account = mchDivisionReceiver.AccNo;
                royaltyEntity.Name = mchDivisionReceiver.AccName;
                royaltyEntity.Memo = mchDivisionReceiver.RelationTypeName; //分账关系描述
                model.ReceiverList = new List<RoyaltyEntity>() { royaltyEntity };

                AlipayTradeRoyaltyRelationBindResponse alipayResp = configContextQueryService.GetAlipayClientWrapper(mchAppConfigContext).Execute(request);

                if (!alipayResp.IsError)
                {
                    return ChannelRetMsg.ConfirmSuccess(null);
                }

                //异常：
                ChannelRetMsg channelRetMsg = ChannelRetMsg.ConfirmFail();
                channelRetMsg.ChannelErrCode = AliPayKit.AppendErrCode(alipayResp.Code, alipayResp.SubCode);
                channelRetMsg.ChannelErrMsg = AliPayKit.AppendErrMsg(alipayResp.Msg, alipayResp.SubMsg);
                return channelRetMsg;

            }
            catch (ChannelException e)
            {

                ChannelRetMsg channelRetMsg = ChannelRetMsg.ConfirmFail();
                channelRetMsg.ChannelErrCode = e.ChannelRetMsg.ChannelErrCode;
                channelRetMsg.ChannelErrMsg = e.ChannelRetMsg.ChannelErrMsg;
                return channelRetMsg;

            }
            catch (Exception e)
            {
                log.LogError(e, "绑定支付宝账号异常");
                ChannelRetMsg channelRetMsg = ChannelRetMsg.ConfirmFail();
                channelRetMsg.ChannelErrMsg = e.Message;
                return channelRetMsg;
            }
        }

        public ChannelRetMsg SingleDivision(PayOrderDto payOrder, List<PayOrderDivisionRecordDto> recordList, MchAppConfigContext mchAppConfigContext)
        {
            throw new NotImplementedException();
        }
    }
}
