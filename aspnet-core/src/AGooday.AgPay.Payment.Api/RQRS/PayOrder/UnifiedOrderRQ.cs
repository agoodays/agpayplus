using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder
{
    /// <summary>
    /// 创建订单请求参数对象
    /// 聚合支付接口（统一下单）
    /// </summary>
    public class UnifiedOrderRQ : AbstractMchAppRQ
    {
        /// <summary>
        /// 商户订单号
        /// </summary>
        [Required(ErrorMessage = "商户订单号不能为空")]
        public string MchOrderNo { get; set; }

        /// <summary>
        /// 支付方式  如： wxpay_jsapi,alipay_wap等
        /// </summary>
        [Required(ErrorMessage = "支付方式不能为空")]
        public string WayCode { get; set; }

        /// <summary>
        /// 支付金额， 单位：分
        /// </summary>
        [Required(ErrorMessage = "支付金额不能为空")]
        [Range(minimum: 1, long.MaxValue, ErrorMessage = "支付金额不能为空")]
        public long Amount { get; set; }

        /// <summary>
        /// 货币代码
        /// </summary>
        [Required(ErrorMessage = "货币代码不能为空")]
        [AllowedValues("CNY", ErrorMessage = "货币代码，目前只支持人民币：CNY")]
        public string Currency { get; set; }

        /// <summary>
        /// 客户端IP地址
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// 商品标题
        /// </summary>
        [Required(ErrorMessage = "商品标题不能为空")]
        public string Subject { get; set; }

        /// <summary>
        /// 商品描述信息
        /// </summary>
        [Required(ErrorMessage = "商品描述信息不能为空")]
        public string Body { get; set; }

        /// <summary>
        /// 卖家备注
        /// </summary>
        public string SellerRemark { get; set; }

        /// <summary>
        /// 买家备注
        /// </summary>
        public string BuyerRemark { get; set; }

        /// <summary>
        /// 商品门店ID	
        /// </summary>
        public long? StoreId { get; set; }

        /// <summary>
        /// 商品码牌ID	
        /// </summary>
        public string QrcId { get; set; }

        /// <summary>
        /// 异步通知地址
        /// </summary>
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 跳转通知地址
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 订单失效时间, 单位：秒
        /// </summary>
        public int? ExpiredTime { get; set; }

        /// <summary>
        /// 特定渠道发起额外参数
        /// </summary>
        public string ChannelExtra { get; set; }

        /// <summary>
        /// 商户扩展参数
        /// </summary>
        public string ExtParam { get; set; }

        /// <summary>
        /// 分账模式： 0-该笔订单不允许分账, 1-支付成功按配置自动完成分账, 2-商户手动分账(解冻商户金额) 
        /// </summary>
        [Range(0, 2, ErrorMessage = "分账模式设置值有误")]
        public byte? DivisionMode { get; set; }

        /// <summary>
        /// 返回真实的bizRQ
        /// </summary>
        /// <returns></returns>
        public UnifiedOrderRQ BuildBizRQ()
        {
            ValidateData(this);

            if (CS.PAY_WAY_CODE.ALI_BAR.Equals(WayCode))
            {
                AliBarOrderRQ bizRQ = JsonConvert.DeserializeObject<AliBarOrderRQ>(!string.IsNullOrWhiteSpace(this.ChannelExtra) ? this.ChannelExtra : "{}");
                CopyUtil.CopyProperties(this, bizRQ);
                ValidateData(bizRQ);
                return bizRQ;
            }
            else if (CS.PAY_WAY_CODE.ALI_JSAPI.Equals(WayCode))
            {
                AliJsapiOrderRQ bizRQ = JsonConvert.DeserializeObject<AliJsapiOrderRQ>(!string.IsNullOrWhiteSpace(this.ChannelExtra) ? this.ChannelExtra : "{}");
                CopyUtil.CopyProperties(this, bizRQ);
                ValidateData(bizRQ);
                return bizRQ;
            }
            else if (CS.PAY_WAY_CODE.ALI_LITE.Equals(WayCode))
            {
                AliLiteOrderRQ bizRQ = JsonConvert.DeserializeObject<AliLiteOrderRQ>(!string.IsNullOrWhiteSpace(this.ChannelExtra) ? this.ChannelExtra : "{}");
                CopyUtil.CopyProperties(this, bizRQ);
                ValidateData(bizRQ);
                return bizRQ;
            }
            else if (CS.PAY_WAY_CODE.QR_CASHIER.Equals(WayCode))
            {
                QrCashierOrderRQ bizRQ = JsonConvert.DeserializeObject<QrCashierOrderRQ>(!string.IsNullOrWhiteSpace(this.ChannelExtra) ? this.ChannelExtra : "{}");
                CopyUtil.CopyProperties(this, bizRQ);
                ValidateData(bizRQ);
                return bizRQ;
            }
            else if (CS.PAY_WAY_CODE.WX_JSAPI.Equals(WayCode))
            {
                WxJsapiOrderRQ bizRQ = JsonConvert.DeserializeObject<WxJsapiOrderRQ>(!string.IsNullOrWhiteSpace(this.ChannelExtra) ? this.ChannelExtra : "{}");
                CopyUtil.CopyProperties(this, bizRQ);
                ValidateData(bizRQ);
                return bizRQ;
            }
            else if (CS.PAY_WAY_CODE.WX_LITE.Equals(WayCode))
            {
                WxLiteOrderRQ bizRQ = JsonConvert.DeserializeObject<WxLiteOrderRQ>(!string.IsNullOrWhiteSpace(this.ChannelExtra) ? this.ChannelExtra : "{}");
                CopyUtil.CopyProperties(this, bizRQ);
                ValidateData(bizRQ);
                return bizRQ;
            }
            else if (CS.PAY_WAY_CODE.WX_BAR.Equals(WayCode))
            {
                WxBarOrderRQ bizRQ = JsonConvert.DeserializeObject<WxBarOrderRQ>(!string.IsNullOrWhiteSpace(this.ChannelExtra) ? this.ChannelExtra : "{}");
                CopyUtil.CopyProperties(this, bizRQ);
                ValidateData(bizRQ);
                return bizRQ;
            }
            else if (CS.PAY_WAY_CODE.WX_NATIVE.Equals(WayCode))
            {
                WxNativeOrderRQ bizRQ = JsonConvert.DeserializeObject<WxNativeOrderRQ>(!string.IsNullOrWhiteSpace(this.ChannelExtra) ? this.ChannelExtra : "{}");
                CopyUtil.CopyProperties(this, bizRQ);
                ValidateData(bizRQ);
                return bizRQ;
            }
            else if (CS.PAY_WAY_CODE.WX_H5.Equals(WayCode))
            {
                WxH5OrderRQ bizRQ = JsonConvert.DeserializeObject<WxH5OrderRQ>(!string.IsNullOrWhiteSpace(this.ChannelExtra) ? this.ChannelExtra : "{}");
                CopyUtil.CopyProperties(this, bizRQ);
                ValidateData(bizRQ);
                return bizRQ;
            }
            else if (CS.PAY_WAY_CODE.YSF_BAR.Equals(WayCode))
            {
                YsfBarOrderRQ bizRQ = JsonConvert.DeserializeObject<YsfBarOrderRQ>(!string.IsNullOrWhiteSpace(this.ChannelExtra) ? this.ChannelExtra : "{}");
                CopyUtil.CopyProperties(this, bizRQ);
                ValidateData(bizRQ);
                return bizRQ;
            }
            else if (CS.PAY_WAY_CODE.YSF_JSAPI.Equals(WayCode))
            {
                YsfJsapiOrderRQ bizRQ = JsonConvert.DeserializeObject<YsfJsapiOrderRQ>(!string.IsNullOrWhiteSpace(this.ChannelExtra) ? this.ChannelExtra : "{}");
                CopyUtil.CopyProperties(this, bizRQ);
                ValidateData(bizRQ);
                return bizRQ;
            }
            else if (CS.PAY_WAY_CODE.AUTO_BAR.Equals(WayCode))
            {
                AutoBarOrderRQ bizRQ = JsonConvert.DeserializeObject<AutoBarOrderRQ>(!string.IsNullOrWhiteSpace(this.ChannelExtra) ? this.ChannelExtra : "{}");
                CopyUtil.CopyProperties(this, bizRQ);
                ValidateData(bizRQ);
                return bizRQ;
            }
            else if (CS.PAY_WAY_CODE.ALI_APP.Equals(WayCode))
            {
                AliAppOrderRQ bizRQ = JsonConvert.DeserializeObject<AliAppOrderRQ>(!string.IsNullOrWhiteSpace(this.ChannelExtra) ? this.ChannelExtra : "{}");
                CopyUtil.CopyProperties(this, bizRQ);
                ValidateData(bizRQ);
                return bizRQ;
            }
            else if (CS.PAY_WAY_CODE.ALI_WAP.Equals(WayCode))
            {
                AliWapOrderRQ bizRQ = JsonConvert.DeserializeObject<AliWapOrderRQ>(!string.IsNullOrWhiteSpace(this.ChannelExtra) ? this.ChannelExtra : "{}");
                CopyUtil.CopyProperties(this, bizRQ);
                ValidateData(bizRQ);
                return bizRQ;
            }
            else if (CS.PAY_WAY_CODE.ALI_PC.Equals(WayCode))
            {
                AliPcOrderRQ bizRQ = JsonConvert.DeserializeObject<AliPcOrderRQ>(!string.IsNullOrWhiteSpace(this.ChannelExtra) ? this.ChannelExtra : "{}");
                CopyUtil.CopyProperties(this, bizRQ);
                ValidateData(bizRQ);
                return bizRQ;
            }
            else if (CS.PAY_WAY_CODE.ALI_QR.Equals(WayCode))
            {
                AliQrOrderRQ bizRQ = JsonConvert.DeserializeObject<AliQrOrderRQ>(!string.IsNullOrWhiteSpace(this.ChannelExtra) ? this.ChannelExtra : "{}");
                CopyUtil.CopyProperties(this, bizRQ);
                ValidateData(bizRQ);
                return bizRQ;
            }
            else if (CS.PAY_WAY_CODE.PP_PC.Equals(WayCode))
            {
                PpPcOrderRQ bizRQ = JsonConvert.DeserializeObject<PpPcOrderRQ>(!string.IsNullOrWhiteSpace(this.ChannelExtra) ? this.ChannelExtra : "{}");
                CopyUtil.CopyProperties(this, bizRQ);
                ValidateData(bizRQ);
                return bizRQ;
            }

            return this;
        }

        private void ValidateData(object data)
        {
            if (!TryValidateModel(data, out List<ValidationResult> validationResults))
            {
                throw new BizException(string.Join(Environment.NewLine, validationResults.Select(s => s.ErrorMessage)));
            }
        }

        private bool TryValidateModel(object model, out List<ValidationResult> validationResults)
        {
            ValidationContext validationContext = new ValidationContext(model);
            validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);
            return isValid;
        }

        /// <summary>
        /// 获取渠道用户ID
        /// </summary>
        /// <returns></returns>
        public virtual string GetChannelUserId() => null;
    }
}
