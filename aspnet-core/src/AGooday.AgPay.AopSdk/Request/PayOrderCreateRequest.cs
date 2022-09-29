using AGooday.AgPay.AopSdk.Models;
using AGooday.AgPay.AopSdk.Nets;
using AGooday.AgPay.AopSdk.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.AopSdk.Request
{
    /// <summary>
    /// 支付下单请求实现
    /// </summary>
    public class PayOrderCreateRequest : IAgPayRequest<PayOrderCreateResponse>
    {
        private string ApiVersion = AgPay.VERSION;
        private string ApiUri  = "api/pay/unifiedOrder";
        private RequestOptions Options;
        private AgPayObject BizModel  = null;

        public string GetApiUri()
        {
            return this.ApiUri;
        }

        public string GetApiVersion()
        {
            return this.ApiVersion;
        }

        public void SetApiVersion(string apiVersion)
        {
            this.ApiVersion = apiVersion;
        }

        public RequestOptions GetRequestOptions()
        {
            return this.Options;
        }

        public void SetRequestOptions(RequestOptions options)
        {
            this.Options = options;
        }

        public AgPayObject GetBizModel()
        {
            return this.BizModel;
        }

        public void SetBizModel(AgPayObject bizModel)
        {
            this.BizModel = bizModel;
        }
    }
}
