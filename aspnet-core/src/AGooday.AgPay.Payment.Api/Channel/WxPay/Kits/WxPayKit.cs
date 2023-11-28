using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using System.Security.Cryptography;
using System.Text;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay.Kits
{
    /// <summary>
    /// 【微信支付】支付通道工具包
    /// </summary>
    public class WxPayKit
    {
        public static IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// 放置 isv特殊信息
        /// </summary>
        /// <param name="mchAppConfigContext"></param>
        /// <param name="req"></param>
        public static void PutApiIsvInfo(MchAppConfigContext mchAppConfigContext, WechatTenpayRequest req)
        {
            //不是特约商户， 无需放置此值
            if (!mchAppConfigContext.IsIsvSubMch())
            {
                return;
            }

            ConfigContextQueryService configContextQueryService = ServiceProvider.GetService<ConfigContextQueryService>();

            WxPayIsvSubMchParams isvsubMchParams =
                (WxPayIsvSubMchParams)configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, CS.IF_CODE.WXPAY);

            //req.SubMchId = isvsubMchParams.SubMchId;
            //req.SubAppId = isvsubMchParams.SubMchAppId;
        }
        public static string Sign(Dictionary<string, string> dictionary, string key)
        {
            var json = $"{string.Join("&", dictionary.OrderBy(o => o.Key)
                .Select(s => $"{s.Key}={s.Value}"))}&key={key}";
            var bytes = Encoding.UTF8.GetBytes(json);
            byte[] temp = MD5.HashData(bytes);
            string sign = "";
            foreach (byte b in temp)
            {
                sign += b.ToString("X").PadLeft(2, '0');
            }
            return sign.ToUpper();
        }

        public static string AppendErrCode(string code, string subCode)
        {
            return StringUtil.DefaultIfEmpty(subCode, code); //优先： subCode
        }

        public static string AppendErrMsg(string msg, string subMsg)
        {
            if (StringUtil.IsAllNotNullOrWhiteSpace(msg, subMsg))
            {
                return $"{msg}【{subMsg}】";
            }
            return StringUtil.DefaultIfEmpty(subMsg, msg);
        }
        public static void CommonSetErrInfo(ChannelRetMsg channelRetMsg, WechatTenpayException wxPayException)
        {
            //channelRetMsg.ChannelErrCode= AppendErrCode(wxPayException.ReturnCode, wxPayException.ErrCode);
            //channelRetMsg.ChannelErrMsg= AppendErrMsg("OK".Equals(wxPayException.ReturnMsg, StringComparison.OrdinalIgnoreCase) ? null : wxPayException.ReturnMsg, wxPayException.ErrCodeDes);
        }

        public static string FailResp(string msg)
        {
            return GenerateXml("FAIL", msg);
        }

        public static string SuccessResp(string msg)
        {
            return GenerateXml("SUCCESS", msg);
        }

        public static string GenerateXml(string code, string msg)
        {
            return $"<xml><return_code><![CDATA[{code}]]></return_code><return_msg><![CDATA[{msg}]]></return_msg></xml>";
        }
    }
}
