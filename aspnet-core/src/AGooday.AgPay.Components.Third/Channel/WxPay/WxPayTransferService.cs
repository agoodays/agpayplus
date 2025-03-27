using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.RQRS.Transfer;
using AGooday.AgPay.Components.Third.Services;
using SKIT.FlurlHttpClient.Wechat.TenpayV2;
using SKIT.FlurlHttpClient.Wechat.TenpayV2.Models;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Models;
using WechatTenpayClientV2 = SKIT.FlurlHttpClient.Wechat.TenpayV2.WechatTenpayClient;
using WechatTenpayClientV3 = SKIT.FlurlHttpClient.Wechat.TenpayV3.WechatTenpayClient;

namespace AGooday.AgPay.Components.Third.Channel.WxPay
{
    /// <summary>
    /// 转账接口： 微信官方
    /// </summary>
    public class WxPayTransferService : ITransferService
    {
        private readonly ILogger<WxPayTransferService> _logger;
        private readonly ConfigContextQueryService _configContextQueryService;

        public WxPayTransferService(ILogger<WxPayTransferService> logger, ConfigContextQueryService configContextQueryService)
        {
            _logger = logger;
            _configContextQueryService = configContextQueryService;
        }

        public WxPayTransferService()
        {
        }

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

        public async Task<ChannelRetMsg> TransferAsync(TransferOrderRQ bizRQ, TransferOrderDto transferOrder, MchAppConfigContext mchAppConfigContext)
        {
            try
            {
                WxServiceWrapper wxServiceWrapper = await _configContextQueryService.GetWxServiceWrapperAsync(mchAppConfigContext);

                if (CS.PAY_IF_VERSION.WX_V2.Equals(wxServiceWrapper.Config.ApiVersion))  // V2
                {
                    CreatePayMarketingTransfersPromotionTransferRequest request = new CreatePayMarketingTransfersPromotionTransferRequest();

                    request.AppId = wxServiceWrapper.Config.AppId;  // 商户账号appid
                    request.MerchantId = wxServiceWrapper.Config.MchId;  // 商户号

                    request.PartnerTradeNumber = transferOrder.TransferId;  // 商户订单号
                    request.OpenId = transferOrder.AccountNo;  // openid
                    request.Amount = (int)transferOrder.Amount;  // 付款金额，单位为分
                    request.ClientIp = transferOrder.ClientIp;
                    request.Description = transferOrder.TransferDesc;  // 付款备注
                    if (!string.IsNullOrEmpty(transferOrder.AccountName))
                    {
                        request.UserName = transferOrder.AccountName;
                        request.CheckNameType = "FORCE_CHECK";
                    }
                    else
                    {
                        request.CheckNameType = "NO_CHECK";
                    }

                    var client = (WechatTenpayClientV2)wxServiceWrapper.Client;
                    var result = await client.ExecuteCreatePayMarketingTransfersPromotionTransferAsync(request);
                    return ChannelRetMsg.Waiting();
                }
                else if (CS.PAY_IF_VERSION.WX_V3.Equals(wxServiceWrapper.Config.ApiVersion))  // V3
                {
                    var client = (WechatTenpayClientV3)wxServiceWrapper.Client;
                    if (mchAppConfigContext.IsIsvSubMch()) // 特约商户
                    {
                        WxPayIsvSubMchParams isvsubMchParams = (WxPayIsvSubMchParams)await _configContextQueryService.QueryIsvSubMchParamsAsync(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, GetIfCode());

                        CreatePartnerTransferBatchRequest request = new CreatePartnerTransferBatchRequest();
                        request.AppId = wxServiceWrapper.Config.AppId;
                        request.SubAppId = isvsubMchParams.SubMchAppId;
                        request.SubMerchantId = isvsubMchParams.SubMchId;
                        request.OutBatchNumber = transferOrder.TransferId;
                        if (!string.IsNullOrWhiteSpace(transferOrder.AccountName))
                        {
                            request.BatchName = transferOrder.AccountName;
                        }
                        else
                        {
                            request.BatchName = transferOrder.TransferDesc;
                        }
                        request.BatchRemark = transferOrder.TransferDesc;
                        request.TotalAmount = (int)transferOrder.Amount;
                        request.TotalNumber = 1;
                        CreatePartnerTransferBatchRequest.Types.TransferDetail transferDetail = new CreatePartnerTransferBatchRequest.Types.TransferDetail();
                        transferDetail.OutDetailNumber = transferOrder.TransferId;
                        transferDetail.OpenId = transferOrder.AccountNo;
                        transferDetail.TransferAmount = (int)transferOrder.Amount;  // 付款金额，单位为分
                        transferDetail.UserName = transferOrder.AccountName;
                        transferDetail.TransferRemark = transferOrder.TransferDesc;
                        request.TransferDetailList.Add(transferDetail);
                        var result = await client.ExecuteCreatePartnerTransferBatchAsync(request);
                    }
                    else
                    {
                        CreateTransferBatchRequest request = new CreateTransferBatchRequest();
                        request.AppId = wxServiceWrapper.Config.AppId;
                        request.OutBatchNumber = transferOrder.TransferId;
                        request.BatchName = transferOrder.AccountName;
                        request.BatchRemark = transferOrder.TransferDesc;
                        request.TotalAmount = (int)transferOrder.Amount;
                        request.TotalNumber = 1;
                        CreateTransferBatchRequest.Types.TransferDetail transferDetail = new CreateTransferBatchRequest.Types.TransferDetail();
                        transferDetail.OutDetailNumber = transferOrder.TransferId;
                        transferDetail.OpenId = transferOrder.AccountNo;
                        transferDetail.TransferAmount = (int)transferOrder.Amount;  // 付款金额，单位为分
                        transferDetail.UserName = transferOrder.AccountName;
                        transferDetail.TransferRemark = transferOrder.TransferDesc;
                        request.TransferDetailList.Add(transferDetail);

                        var result = await client.ExecuteCreateTransferBatchAsync(request);
                    }
                    return ChannelRetMsg.Waiting();
                }
                else
                {
                    return ChannelRetMsg.SysError("请选择微信V2或V3模式");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "转账异常");
                return ChannelRetMsg.Waiting();
            }
        }

        public async Task<ChannelRetMsg> QueryAsync(TransferOrderDto transferOrder, MchAppConfigContext mchAppConfigContext)
        {
            try
            {
                WxServiceWrapper wxServiceWrapper = await _configContextQueryService.GetWxServiceWrapperAsync(mchAppConfigContext);

                if (CS.PAY_IF_VERSION.WX_V2.Equals(wxServiceWrapper.Config.ApiVersion))  // V2
                {
                    GetPayMarketingTransfersTransferInfoRequest request = new GetPayMarketingTransfersTransferInfoRequest();
                    request.AppId = wxServiceWrapper.Config.AppId;  // 商户账号appid
                    request.MerchantId = wxServiceWrapper.Config.MchId;  // 商户号
                    request.PartnerTradeNumber = transferOrder.TransferId;

                    var client = (WechatTenpayClientV2)wxServiceWrapper.Client;
                    var result = await client.ExecuteGetPayMarketingTransfersTransferInfoAsync(request);
                    // SUCCESS,明确成功
                    if ("SUCCESS".Equals(result.Status, StringComparison.OrdinalIgnoreCase))
                    {
                        return ChannelRetMsg.ConfirmSuccess(result.PaymentNumber);
                    }
                    else if ("FAILED".Equals(result.Status, StringComparison.OrdinalIgnoreCase))  // FAILED,明确失败
                    {
                        return ChannelRetMsg.ConfirmFail(result.Status, result.FailReason);
                    }
                    else
                    {
                        return ChannelRetMsg.Waiting();
                    }
                }
                else if (CS.PAY_IF_VERSION.WX_V3.Equals(wxServiceWrapper.Config.ApiVersion))  // V3
                {
                    GetTransferBatchDetailByOutDetailNumberRequest request = new GetTransferBatchDetailByOutDetailNumberRequest();
                    request.OutBatchNumber = transferOrder.TransferId;
                    request.OutDetailNumber = transferOrder.TransferId;

                    var client = (WechatTenpayClientV3)wxServiceWrapper.Client;
                    GetTransferBatchDetailByOutDetailNumberResponse result;
                    if (mchAppConfigContext.IsIsvSubMch()) // 特约商户
                    {
                        result = await client.ExecuteGetPartnerTransferBatchDetailByOutDetailNumberAsync((GetPartnerTransferBatchDetailByOutDetailNumberRequest)request);
                    }
                    else
                    {
                        result = await client.ExecuteGetTransferBatchDetailByOutDetailNumberAsync(request);
                    }

                    // SUCCESS,明确成功
                    if ("SUCCESS".Equals(result.DetailStatus, StringComparison.OrdinalIgnoreCase))
                    {
                        return ChannelRetMsg.ConfirmSuccess(result.DetailId);
                    }
                    else if ("FAIL".Equals(result.DetailStatus, StringComparison.OrdinalIgnoreCase))  // FAIL,明确失败
                    {
                        return ChannelRetMsg.ConfirmFail(result.DetailStatus, result.FailReason);
                    }
                    else
                    {
                        return ChannelRetMsg.Waiting();
                    }
                }
                else
                {
                    return ChannelRetMsg.SysError("请选择微信V2或V3模式");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "转账状态查询异常");
                return ChannelRetMsg.Waiting();
            }
        }
    }
}
