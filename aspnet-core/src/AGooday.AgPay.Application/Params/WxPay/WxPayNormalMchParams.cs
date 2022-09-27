using AGooday.AgPay.Common.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Params.WxPay
{
    public class WxPayNormalMchParams : NormalMchParams
    {
        /**
         * 应用App ID
         */
        public string appId{get;set;}

        /**
         * 应用AppSecret
         */
        public string appSecret{get;set;}

        /**
         * 微信支付商户号
         */
        public string mchId{get;set;}

        /**
         * oauth2地址
         */
        public string oauth2Url{get;set;}

        /**
         * API密钥
         */
        public string key{get;set;}

        /**
         * 微信支付API版本
         **/
        public string apiVersion{get;set;}

        /**
         * API V3秘钥
         **/
        public string apiV3Key{get;set;}

        /**
         * 序列号
         **/
        public string serialNo{get;set;}

        /**
         * API证书(.p12格式)
         **/
        public string cert{get;set;}

        /**
         * 私钥文件(.pem格式)
         **/
        public string apiClientKey{get;set;}

        public override string DeSenData()
        {
            if (!string.IsNullOrWhiteSpace(appSecret))
            {
                appSecret = StringUtil.Str2Star(appSecret, 4, 4, 6);
            }
            if (!string.IsNullOrWhiteSpace(key))
            {
                appSecret = StringUtil.Str2Star(appSecret, 4, 4, 6);
            }
            if (!string.IsNullOrWhiteSpace(appSecret))
            {
                appSecret = StringUtil.Str2Star(appSecret, 4, 4, 6);
            }
            if (!string.IsNullOrWhiteSpace(appSecret))
            {
                appSecret = StringUtil.Str2Star(appSecret, 4, 4, 6);
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}
