using AGooday.AgPay.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application
{
    public class DBApplicationConfig
    {
        /** 运营系统地址 **/
        public string MgrSiteUrl { get; set; }

        /** 商户系统地址 **/
        public string MchSiteUrl { get; set; }

        /** 支付网关地址 **/
        public string PaySiteUrl { get; set; }

        /** oss公共读文件地址 **/
        public string OssPublicSiteUrl { get; set; }

        /** 生成  【jsapi统一收银台跳转地址】 **/
        public string GenUniJsapiPayUrl(string payOrderId)
        {
            return $"{PaySiteUrl}/cashier/index.html#/hub/{AgPayUtil.AesEncode(payOrderId)}";
        }

        /** 生成  【jsapi统一收银台】oauth2获取用户ID回调地址 **/
        public string GenOauth2RedirectUrlEncode(string payOrderId)
        {
            return URLUtil.EncodeAll($"{PaySiteUrl}/cashier/index.html#/oauth2Callback/{AgPayUtil.AesEncode(payOrderId)}");
        }

        /** 生成  【商户获取渠道用户ID接口】oauth2获取用户ID回调地址 **/
        public string GenMchChannelUserIdApiOauth2RedirectUrlEncode(JsonObject param)
        {
            return URLUtil.EncodeAll($"{PaySiteUrl}/api/channelUserId/oauth2Callback/{AgPayUtil.AesEncode(param.ToJsonString())}");
        }

        /** 生成  【jsapi统一收银台二维码图片地址】 **/
        public string GenScanImgUrl(string url)
        {
            return $"{PaySiteUrl}/api/scan/imgs/{AgPayUtil.AesEncode(url)}.png";
        }

        /** 生成  【支付宝 isv子商户的授权链接地址】 **/
        public string GenAlipayIsvsubMchAuthUrl(string isvNo, string mchAppId)
        {
            return $"{PaySiteUrl}/api/channelbiz/alipay/redirectAppToAppAuth/{isvNo}_{mchAppId}";
        }
    }
}
