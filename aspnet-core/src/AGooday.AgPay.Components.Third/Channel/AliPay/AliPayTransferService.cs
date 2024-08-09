using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.AliPay.Kits;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.Transfer;
using AGooday.AgPay.Components.Third.Services;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;

namespace AGooday.AgPay.Components.Third.Channel.AliPay
{
    /// <summary>
    /// 转账接口： 支付宝官方
    /// </summary>
    public class AliPayTransferService : ITransferService
    {
        private readonly ConfigContextQueryService configContextQueryService;

        public AliPayTransferService(ConfigContextQueryService configContextQueryService)
        {
            this.configContextQueryService = configContextQueryService;
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.ALIPAY;
        }

        public bool IsSupport(string entryType)
        {
            // 支付宝账户
            if (TransferOrderEntry.ALIPAY_CASH.Equals(entryType))
            {
                return true;
            }

            return false;
        }

        public string PreCheck(TransferOrderRQ bizRQ, TransferOrderDto transferOrder)
        {
            return null;
        }

        public ChannelRetMsg Transfer(TransferOrderRQ bizRQ, TransferOrderDto transferOrder, MchAppConfigContext mchAppConfigContext)
        {
            AlipayFundTransUniTransferRequest request = new AlipayFundTransUniTransferRequest();
            AlipayFundTransUniTransferModel model = new AlipayFundTransUniTransferModel();
            model.TransAmount = AmountUtil.ConvertCent2Dollar(transferOrder.Amount); // 转账金额，单位：元。
            model.OutBizNo = transferOrder.TransferId; // 商户转账唯一订单号
            model.Remark = transferOrder.TransferDesc; // 转账备注
            model.ProductCode = "TRANS_ACCOUNT_NO_PWD";  // 销售产品码。单笔无密转账固定为 TRANS_ACCOUNT_NO_PWD
            model.BizScene = "DIRECT_TRANSFER";           // 业务场景 单笔无密转账固定为 DIRECT_TRANSFER。
            model.OrderTitle = "转账";                     // 转账业务的标题，用于在支付宝用户的账单里显示。
            model.BusinessParams = transferOrder.ChannelExtra;   // 转账业务请求的扩展参数 {\"payer_show_name_use_alias\":\"xx公司\"}

            Participant accPayeeInfo = new Participant();
            accPayeeInfo.Name = StringUtil.DefaultString(transferOrder.AccountName, null); //收款方真实姓名
            accPayeeInfo.IdentityType = "ALIPAY_LOGON_ID";    //ALIPAY_USERID： 支付宝用户ID      ALIPAY_LOGONID:支付宝登录账号
            accPayeeInfo.Identity = transferOrder.AccountNo; //收款方账户
            model.PayeeInfo = accPayeeInfo;

            request.SetBizModel(model);

            //统一放置 isv接口必传信息
            AliPayKit.PutApiIsvInfo(mchAppConfigContext, request, model);

            // 调起支付宝接口
            AlipayFundTransUniTransferResponse response = configContextQueryService.GetAlipayClientWrapper(mchAppConfigContext).Execute(request);

            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            channelRetMsg.ChannelAttach = response.Body;

            // 调用成功
            if (!response.IsError)
            {
                if ("SUCCESS".Equals(response.Status))
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                    channelRetMsg.ChannelOrderId = response.OrderId;
                    return channelRetMsg;
                }
                else if ("FAIL".Equals(response.Status))
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = AliPayKit.AppendErrCode(response.Code, response.SubCode);
                    channelRetMsg.ChannelErrMsg = AliPayKit.AppendErrMsg(response.Msg, response.SubMsg);
                    return channelRetMsg;
                }
                else
                {
                    return ChannelRetMsg.Waiting();
                }
            }
            else
            {
                //若 系统繁忙， 无法确认结果
                if ("SYSTEM_ERROR".Equals(response.SubCode, StringComparison.OrdinalIgnoreCase))
                {
                    return ChannelRetMsg.Waiting();
                }

                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                channelRetMsg.ChannelErrCode = response.SubCode;
                channelRetMsg.ChannelErrMsg = response.SubMsg;

                return channelRetMsg;
            }
        }

        public ChannelRetMsg Query(TransferOrderDto transferOrder, MchAppConfigContext mchAppConfigContext)
        {
            AlipayFundTransCommonQueryRequest request = new AlipayFundTransCommonQueryRequest();
            AlipayFundTransCommonQueryModel model = new AlipayFundTransCommonQueryModel();
            model.ProductCode = TransferOrderEntry.BANK_CARD.Equals(transferOrder.EntryType) ? "TRANS_BANKCARD_NO_PWD" : "TRANS_ACCOUNT_NO_PWD";
            model.BizScene = "DIRECT_TRANSFER";
            model.OutBizNo = transferOrder.TransferId; // 商户转账唯一订单号
            request.SetBizModel(model);

            //统一放置 isv接口必传信息
            AliPayKit.PutApiIsvInfo(mchAppConfigContext, request, model);

            // 调起支付宝接口
            AlipayFundTransCommonQueryResponse response = configContextQueryService.GetAlipayClientWrapper(mchAppConfigContext).Execute(request);

            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            channelRetMsg.ChannelAttach = response.Body;

            // 调用成功
            if (!response.IsError)
            {
                if (response.Status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                {
                    return ChannelRetMsg.ConfirmSuccess(response.OrderId);
                }
                else if (response.Status.Equals("REFUND", StringComparison.OrdinalIgnoreCase))
                {
                    return ChannelRetMsg.ConfirmFail(response.ErrorCode, response.FailReason);
                }
                else
                {
                    return ChannelRetMsg.Waiting();
                }
            }
            else
            {
                //若 系统繁忙， 无法确认结果
                if ("SYSTEM_ERROR".Equals(response.SubCode, StringComparison.OrdinalIgnoreCase))
                {
                    return ChannelRetMsg.Waiting();
                }

                channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                channelRetMsg.ChannelErrCode = response.SubCode;
                channelRetMsg.ChannelErrMsg = response.SubMsg;
            }

            return channelRetMsg;
        }
    }
}
