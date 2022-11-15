using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.RQRS.Transfer;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay
{
    public class WxPayTransferService : ITransferService
    {
        public string GetIfCode()
        {
            return CS.IF_CODE.WXPAY;
        }

        public bool IsSupport(string entryType)
        {
            // 微信仅支持 零钱 和 银行卡入账方式
            if (TransferOrderEntry.WX_CASH.Equals(entryType) || TransferOrderEntry.BANK_CARD.Equals(entryType))
            {
                return true;
            }

            return false;
        }

        public string PreCheck(TransferOrderRQ bizRQ, TransferOrderDto transferOrder)
        {
            /**
             * 微信企业付款到零钱 产品：不支持服务商模式，参考如下链接：
             * https://developers.weixin.qq.com/community/develop/doc/0004888f8603b042a45c632355a400?highLine=%25E4%25BB%2598%25E6%25AC%25BE%25E5%2588%25B0%25E9%259B%25B6%25E9%2592%25B1%2520%2520%25E6%259C%258D%25E5%258A%25A1%25E5%2595%2586
             * 微信官方解答： 目前企业付款到零钱，是不支持服务商模式的哈，如果特约商户需要使用该功能，请自行登录商户平台申请使用。
             **/
            if (transferOrder.MchType == CS.MCH_TYPE_ISVSUB)
            {
                return "微信子商户暂不支持转账业务";
            }

            return null;
        }

        public ChannelRetMsg Transfer(TransferOrderRQ bizRQ, TransferOrderDto transferOrder, MchAppConfigContext mchAppConfigContext)
        {
            throw new NotImplementedException();
        }
    }
}
