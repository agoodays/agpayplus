using AGooday.AgPay.Common.Utils;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Application
{
    public class DBApplicationConfig
    {
        /// <summary>
        /// 运营系统地址(不包含结尾/)
        /// </summary>
        public string MgrSiteUrl { get; set; }

        /// <summary>
        /// 生成码牌模板图片显示URL
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        public string GenShellImgViewUrl(string recordId)
        {
            return $"{MgrSiteUrl}/api/qrc/shell/imgview/{AgPayUtil.AesEncode(recordId)}.png";
        }

        /// <summary>
        /// 商户系统地址(不包含结尾/)
        /// </summary>
        public string MchSiteUrl { get; set; }

        /// <summary>
        /// 支付网关地址(不包含结尾/)
        /// </summary>
        public string PaySiteUrl { get; set; }

        /// <summary>
        /// 生成  【jsapi统一收银台跳转地址】
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string GenUniJsapiPayUrl(string data)
        {
            return $"{PaySiteUrl}/hub/{AgPayUtil.AesEncode(data)}";
        }

        /// <summary>
        /// 生成  【jsapi统一收银台】oauth2获取用户ID回调地址
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string GenOauth2RedirectUrlEncode(string data)
        {
            return URLUtil.EncodeAll($"{PaySiteUrl}/oauth2Callback/{AgPayUtil.AesEncode(data)}");
        }

        /// <summary>
        /// 生成  【商户获取渠道用户ID接口】oauth2获取用户ID回调地址
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public string GenMchChannelUserIdApiOauth2RedirectUrlEncode(JObject param)
        {
            return URLUtil.EncodeAll($"{PaySiteUrl}/api/channelUserId/oauth2Callback/{AgPayUtil.AesEncode(param.ToString())}");
        }

        /// <summary>
        /// 生成  【jsapi统一收银台二维码图片地址】
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GenScanImgUrl(string url)
        {
            return $"{PaySiteUrl}/api/scan/imgs/{AgPayUtil.AesEncode(url)}.png";
        }

        /// <summary>
        /// 生成  【支付宝 isv子商户的授权链接地址】
        /// </summary>
        /// <param name="isvNo"></param>
        /// <param name="mchAppId"></param>
        /// <returns></returns>
        public string GenAlipayIsvsubMchAuthUrl(string isvNo, string mchAppId)
        {
            return $"{PaySiteUrl}/api/channelbiz/alipay/redirectAppToAppAuth/{isvNo}_{mchAppId}";
        }
    }
}
