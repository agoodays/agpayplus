using System.ComponentModel.DataAnnotations;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder
{
    public class UnifiedOrderRQ : AbstractMchAppRQ
    {
        /// <summary>
        /// 商户订单号
        /// </summary>
        [Required(ErrorMessage = "商户订单号不能为空")]
        public string mchOrderNo { get; set; }

        /// <summary>
        /// 支付方式  如： wxpay_jsapi,alipay_wap等
        /// </summary>
        [Required(ErrorMessage = "支付方式不能为空")]
        public string wayCode { get; set; }

        /// <summary>
        /// 支付金额， 单位：分
        /// </summary>
        [Required(ErrorMessage = "支付金额不能为空")]
        [Range(minimum: 1, long.MaxValue, ErrorMessage = "支付金额不能为空")]
        public long amount { get; set; }

        /// <summary>
        /// 货币代码
        /// </summary>
        [Required(ErrorMessage = "货币代码不能为空")]
        public string currency { get; set; }

        /// <summary>
        /// 客户端IP地址
        /// </summary>
        public string clientIp { get; set; }

        /// <summary>
        /// 商品标题
        /// </summary>
        [Required(ErrorMessage = "商品标题不能为空")]
        public string subject { get; set; }

        /// <summary>
        /// 商品描述信息
        /// </summary>
        [Required(ErrorMessage = "商品描述信息不能为空")]
        public string body { get; set; }

        /// <summary>
        /// 异步通知地址
        /// </summary>
        public string notifyUrl { get; set; }

        /// <summary>
        /// 跳转通知地址
        /// </summary>
        public string returnUrl { get; set; }

        /// <summary>
        /// 订单失效时间, 单位：秒
        /// </summary>
        public int expiredTime { get; set; }

        /// <summary>
        /// 特定渠道发起额外参数
        /// </summary>
        public string channelExtra { get; set; }

        /// <summary>
        /// 商户扩展参数
        /// </summary>
        public string extParam { get; set; }

        /// <summary>
        /// 分账模式： 0-该笔订单不允许分账, 1-支付成功按配置自动完成分账, 2-商户手动分账(解冻商户金额) 
        /// </summary>
        [Range(0, 2, ErrorMessage = "分账模式设置值有误")]
        public byte divisionMode { get; set; }

        /// <summary>
        /// 返回真实的bizRQ
        /// </summary>
        /// <returns></returns>
        public UnifiedOrderRQ BuildBizRQ()
        {
            return null;
        }

        /// <summary>
        /// 获取渠道用户ID
        /// </summary>
        /// <returns></returns>
        public virtual string GetChannelUserId() => null;
    }
}
