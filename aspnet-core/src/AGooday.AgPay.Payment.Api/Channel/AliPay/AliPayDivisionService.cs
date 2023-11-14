using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.AliPay.Kits;
using AGooday.AgPay.Payment.Api.Exceptions;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using Newtonsoft.Json;

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
            try
            {
                if (recordList.Count == 0)
                {
                    // 当无分账用户时，支付宝不允许发起分账请求，支付宝没有完结接口，直接响应成功即可。
                    return ChannelRetMsg.ConfirmSuccess(null);
                }

                var request = new AlipayTradeOrderSettleRequest();
                var model = new AlipayTradeOrderSettleModel();
                request.SetBizModel(model);

                model.OutRequestNo = recordList[0].BatchOrderId; // 结算请求流水号，由商家自定义。32个字符以内，仅可包含字母、数字、下划线。需保证在商户端不重复。
                model.TradeNo = recordList[0].PayOrderChannelOrderNo; // 支付宝订单号

                // royalty_mode 参数说明:  同步执行 sync  ; 分账异步执行 async ( 使用异步：  需要 1、请配置 支付宝应用的网关地址 （ xxx.com/api/channelbiz/alipay/appGatewayMsgReceive ）， 2、 订阅消息。   )
                // 2023-03-30 咨询支付宝客服：  如果没有传royalty_mode分账模式,这个默认会是同步分账,同步分账不需要关注异步通知,接口调用成功就分账成功了  2,同步分账默认不会给您发送异步通知。
                // 3. 服务商代商户调用商家分账，当异步分账时服务商必须调用alipay.open.app.message.topic.subscribe(订阅消息主题)对消息api做关联绑定，服务商才会收到alipay.trade.order.settle.notify通知，否则服务商无法收到通知。
                // https://opendocs.alipay.com/open/20190308105425129272/quickstart#%E8%9A%82%E8%9A%81%E6%B6%88%E6%81%AF%EF%BC%9A%E4%BA%A4%E6%98%93%E5%88%86%E8%B4%A6%E7%BB%93%E6%9E%9C%E9%80%9A%E7%9F%A5

                model.RoyaltyMode = "sync"; // 综上所述， 目前使用同步调用。

                // 统一放置 isv 接口必传信息
                AliPayKit.PutApiIsvInfo(mchAppConfigContext, request, model);

                var reqReceiverList = new List<OpenApiRoyaltyDetailInfoPojo>();

                foreach (var record in recordList)
                {
                    if (record.CalDivisionAmount <= 0)
                    {
                        // 金额为 0 不参与分账处理
                        continue;
                    }

                    var reqReceiver = new OpenApiRoyaltyDetailInfoPojo();
                    reqReceiver.RoyaltyType = "transfer"; // 分账类型：普通分账

                    // 出款信息
                    // reqReceiver.TransOutType = "loginName";
                    // reqReceiver.TransOut = "xqxemt4735@sandbox.com";

                    // 入款信息
                    reqReceiver.TransIn = record.AccNo; // 收入方账号
                    reqReceiver.TransInType = "loginName";
                    if (RegUtil.IsAliPayUserId(record.AccNo))
                    {
                        reqReceiver.TransInType = "userId";
                    }
                    // 分账金额
                    reqReceiver.Amount = AmountUtil.ConvertCent2Dollar(record.CalDivisionAmount);
                    reqReceiver.Desc = "[" + payOrder.PayOrderId + "]订单分账";
                    reqReceiverList.Add(reqReceiver);
                }

                if (reqReceiverList.Count == 0)
                {
                    // 当无分账用户时，支付宝不允许发起分账请求，支付宝没有完结接口，直接响应成功即可。
                    return ChannelRetMsg.ConfirmSuccess(null);
                }

                model.RoyaltyParameters = reqReceiverList; // 分账明细信息

                // 完结
                var settleExtendParams = new SettleExtendParams();
                settleExtendParams.RoyaltyFinish = "true";
                model.ExtendParams = settleExtendParams;

                // 调起支付宝分账接口
                if (log.IsEnabled(LogLevel.Information))
                {
                    log.LogInformation($"订单：[{payOrder.PayOrderId}], 支付宝分账请求：{JsonConvert.SerializeObject(model)}");
                }
                var alipayResp = configContextQueryService.GetAlipayClientWrapper(mchAppConfigContext).Execute(request);
                log.LogInformation($"订单：[{payOrder.PayOrderId}], 支付宝分账响应：{alipayResp.Body}");
                if (!alipayResp.IsError)
                {
                    return ChannelRetMsg.ConfirmSuccess(alipayResp.TradeNo);
                }

                // 异常
                var channelRetMsg = ChannelRetMsg.ConfirmFail();
                channelRetMsg.ChannelErrCode = AliPayKit.AppendErrCode(alipayResp.Code, alipayResp.SubCode);
                channelRetMsg.ChannelErrMsg = AliPayKit.AppendErrMsg(alipayResp.Msg, alipayResp.SubMsg);
                return channelRetMsg;
            }
            catch (ChannelException e)
            {
                var channelRetMsg = ChannelRetMsg.ConfirmFail();
                channelRetMsg.ChannelErrCode = e.ChannelRetMsg.ChannelErrCode;
                channelRetMsg.ChannelErrMsg = e.ChannelRetMsg.ChannelErrMsg;
                return channelRetMsg;
            }
            catch (Exception e)
            {
                log.LogError(e, "绑定支付宝账号异常");
                var channelRetMsg = ChannelRetMsg.ConfirmFail();
                channelRetMsg.ChannelErrMsg = e.Message;
                return channelRetMsg;
            }
        }

        public Dictionary<long, ChannelRetMsg> QueryDivision(PayOrderDto payOrder, List<PayOrderDivisionRecordDto> recordList, MchAppConfigContext mchAppConfigContext)
        {
            // 创建返回结果
            Dictionary<long, ChannelRetMsg> resultMap = new Dictionary<long, ChannelRetMsg>();

            try
            {
                // 得到所有的 accNo 与 recordId map
                Dictionary<string, long?> accnoAndRecordIdSet = new Dictionary<string, long?>();
                foreach (PayOrderDivisionRecordDto record in recordList)
                {
                    accnoAndRecordIdSet[record.AccNo] = record.RecordId;
                }

                var request = new AlipayTradeOrderSettleQueryRequest();
                var model = new AlipayTradeOrderSettleQueryModel();
                request.SetBizModel(model);

                // 统一放置 isv 接口必传信息
                AliPayKit.PutApiIsvInfo(mchAppConfigContext, request, model);

                //结算请求流水号，由商家自定义。32个字符以内，仅可包含字母、数字、下划线。需保证在商户端不重复。
                model.OutRequestNo = recordList[0].BatchOrderId;
                model.TradeNo = payOrder.ChannelOrderNo; //支付宝订单号

                // 调起支付宝分账接口
                log.LogInformation($"订单：[{recordList[0].BatchOrderId}], 支付宝查询分账请求：{JsonConvert.SerializeObject(model)}");
                var alipayResp = configContextQueryService.GetAlipayClientWrapper(mchAppConfigContext).Execute(request);
                log.LogInformation($"订单：[{payOrder.PayOrderId}], 支付宝查询分账响应：{alipayResp.Body}");

                if (!alipayResp.IsError)
                {
                    var detailList = alipayResp.RoyaltyDetailList;
                    if (detailList != null && detailList.Count > 0)
                    {
                        // 遍历匹配与当前账户相同的分账单
                        foreach (var item in detailList)
                        {
                            // 我方系统的分账接收记录ID
                            long? recordId = accnoAndRecordIdSet[item.TransIn];

                            // 分账操作类型为转账类型
                            if ("transfer".Equals(item.OperationType) && recordId != null)
                            {
                                // 仅返回分账记录为最终态的结果 处理中的分账单不做返回处理
                                if ("SUCCESS".Equals(item.State))
                                {

                                    resultMap[recordId.Value] = ChannelRetMsg.ConfirmSuccess(null);

                                }
                                else if ("FAIL".Equals(item.State))
                                {
                                    resultMap[recordId.Value] = ChannelRetMsg.ConfirmFail(null, item.ErrorCode, item.ErrorDesc);
                                }
                            }
                        }
                    }
                }
                else
                {
                    log.LogError($"支付宝分账查询响应异常, alipayResp:{0}", JsonConvert.SerializeObject(alipayResp));
                    throw new BizException("支付宝分账查询响应异常：" + alipayResp.SubMsg);
                }
            }
            catch (Exception e)
            {
                log.LogError(e, "查询分账信息异常");
                throw new BizException(e.Message);
            }

            return resultMap;
        }
    }
}
