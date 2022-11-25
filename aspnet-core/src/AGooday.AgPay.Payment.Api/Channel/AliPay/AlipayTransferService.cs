using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.Transfer;
using AGooday.AgPay.Payment.Api.Services;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;

namespace AGooday.AgPay.Payment.Api.Channel.AliPay
{
    /// <summary>
    /// 转账接口： 支付宝官方
    /// </summary>
    public class AlipayTransferService : ITransferService
    {
        private readonly ConfigContextQueryService configContextQueryService;

        public AlipayTransferService(ConfigContextQueryService configContextQueryService)
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
            AlipayFundTransToaccountTransferRequest request = new AlipayFundTransToaccountTransferRequest();
            AlipayFundTransToaccountTransferModel model = new AlipayFundTransToaccountTransferModel();
            model.Amount = AmountUtil.ConvertCent2Dollar(transferOrder.Amount); //转账金额，单位：元。
            model.OutBizNo = transferOrder.TransferId; //商户转账唯一订单号
            model.PayeeType = "ALIPAY_LOGONID";  //ALIPAY_USERID： 支付宝用户ID      ALIPAY_LOGONID:支付宝登录账号
            model.PayeeAccount = transferOrder.AccountNo; //收款方账户
            model.PayeeRealName = StringUtil.DefaultString(transferOrder.AccountName, null); //收款方真实姓名
            model.Remark = transferOrder.TransferDesc; //转账备注
            request.SetBizModel(model);

            //统一放置 isv接口必传信息
            AliPayKit.PutApiIsvInfo(mchAppConfigContext, request, model);

            // 调起支付宝接口
            AlipayFundTransToaccountTransferResponse response = configContextQueryService.GetAlipayClientWrapper(mchAppConfigContext).Execute(request);

            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            channelRetMsg.ChannelAttach = response.Body;

            // 调用成功
            if (!response.IsError)
            {
                channelRetMsg.ChannelState = ChannelState.CONFIRM_SUCCESS;
                channelRetMsg.ChannelOrderId = response.OrderId;
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
