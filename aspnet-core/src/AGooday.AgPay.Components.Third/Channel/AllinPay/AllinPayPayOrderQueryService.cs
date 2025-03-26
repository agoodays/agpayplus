using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.Channel.AllinPay.Enumerator;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Components.Third.Channel.AllinPay
{
    /// <summary>
    /// 通联查单
    /// </summary>
    public class AllinPayPayOrderQueryService : IPayOrderQueryService
    {
        private readonly ILogger<AllinPayPayOrderQueryService> _logger;
        private readonly AllinPayPaymentService _paymentService;

        public AllinPayPayOrderQueryService(ILogger<AllinPayPayOrderQueryService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            //_paymentService = (AllinPayPaymentService)serviceProvider.GetRequiredKeyedService<IPaymentService>(GetIfCode());
            _paymentService = ActivatorUtilities.CreateInstance<AllinPayPaymentService>(serviceProvider);
        }

        public AllinPayPayOrderQueryService()
        {
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.ALLINPAY;
        }

        public async Task<ChannelRetMsg> QueryAsync(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            JObject reqParams = new JObject();
            string payType = AllinPayEnum.GetPayType(payOrder.WayCode);
            string logPrefix = $"【通联({payType})查单】";

            try
            {
                reqParams.Add("reqsn", payOrder.PayOrderId); //订单号
                reqParams.Add("trxid", payOrder.ChannelOrderNo);

                //封装公共参数 & 签名 & 调起http请求 & 返回响应数据并包装为json格式。
                JObject resJSON = await _paymentService.PackageParamAndReqAsync("/apiweb/tranx/query", reqParams, logPrefix, mchAppConfigContext);
                _logger.LogInformation("查询订单 payorderId:{PayOrderId}, 返回结果:{resJSON}", payOrder.PayOrderId, resJSON);
                //_logger.LogInformation($"查询订单 payorderId:{payOrder.PayOrderId}, 返回结果:{resJSON}");
                if (resJSON == null)
                {
                    return ChannelRetMsg.Waiting(); //支付中
                }
                var channelRetMsg = ChannelRetMsg.Waiting();
                //请求 & 响应成功， 判断业务逻辑
                string code = resJSON.GetValue("retcode").ToString(); //请求响应码
                string msg = resJSON.GetValue("retmsg").ToString(); //响应信息
                reqParams.TryGetString("cusid", out string cusid); // 商户号
                channelRetMsg.ChannelMchNo = cusid;
                //channelRetMsg.ChannelIsvNo = orgid;
                if ("SUCCESS".Equals(code))
                {
                    string trxstatus = resJSON.GetValue("trxstatus").ToString();
                    string reqsn = resJSON.GetValue("reqsn").ToString();
                    string trxid = resJSON.GetValue("trxid").ToString();//天阙平台订单号
                    resJSON.TryGetString("chnltrxid", out string chnltrxid);//微信/支付宝流水号
                    /*买家用户号
                    支付宝渠道：买家支付宝用户号buyer_user_id
                    微信渠道：微信平台的sub_openid*/
                    resJSON.TryGetString("acct", out string acct);
                    switch (trxstatus)
                    {
                        case "0000":
                            channelRetMsg = ChannelRetMsg.ConfirmSuccess(trxid);  //支付成功
                            channelRetMsg.ChannelMchNo = cusid;
                            //channelRetMsg.ChannelIsvNo = orgid;
                            channelRetMsg.ChannelOrderId = trxid;
                            channelRetMsg.ChannelUserId = acct;
                            channelRetMsg.PlatformOrderId = chnltrxid;
                            channelRetMsg.PlatformMchOrderId = reqsn;
                            break;
                        case "2008":
                        case "2000":
                            //case "3088":
                            break;
                        default:
                            channelRetMsg = ChannelRetMsg.ConfirmFail(code, msg);
                            break;
                    }
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = code;
                    channelRetMsg.ChannelErrMsg = msg;
                }
                return channelRetMsg; //支付中
            }
            catch (Exception e)
            {
                _logger.LogError(e, "查询订单 payorderId:{PayOrderId}, 异常:{Message}", payOrder.PayOrderId, e.Message);
                //_logger.LogError(e, $"查询订单 payorderId:{payOrder.PayOrderId}, 异常:{e.Message}");
                return ChannelRetMsg.Waiting(); //支付中
            }
        }
    }
}
