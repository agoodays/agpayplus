using AGooday.AgPay.Application.DataTransfer;
using AutoMapper;
using System.Runtime.InteropServices;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder
{
    public class QueryPayOrderRS : AbstractRS
    {
        /**
         * 支付订单号
         */
        public string PAyOrderId { get; set; }

        /**
         * 商户号
         */
        public string MchNo { get; set; }

        /**
         * 商户应用ID
         */
        public string AppId { get; set; }

        /**
         * 商户订单号
         */
        public string MchOrderNo { get; set; }

        /**
         * 支付接口代码
         */
        public string IfCode { get; set; }

        /**
         * 支付方式代码
         */
        public string WayCode { get; set; }

        /**
         * 支付金额,单位分
         */
        public long Amount { get; set; }

        /**
         * 三位货币代码,人民币:cny
         */
        public string Currency { get; set; }

        /**
         * 支付状态: 0-订单生成, 1-支付中, 2-支付成功, 3-支付失败, 4-已撤销, 5-已退款, 6-订单关闭
         */
        public byte State { get; set; }

        /**
         * 客户端IP
         */
        public string ClientIp { get; set; }

        /**
         * 商品标题
         */
        public string Subject { get; set; }

        /**
         * 商品描述信息
         */
        public string Body { get; set; }

        /**
         * 渠道订单号
         */
        public string ChannelOrderNo { get; set; }

        /**
         * 渠道支付错误码
         */
        public string ErrCode { get; set; }

        /**
         * 渠道支付错误描述
         */
        public string ErrMsg { get; set; }

        /**
         * 商户扩展参数
         */
        public string ExtParam { get; set; }

        /**
         * 订单支付成功时间
         */
        public long? SuccessTime { get; set; }

        /**
         * 创建时间
         */
        public long? CreatedAt { get; set; }


        public static QueryPayOrderRS BuildByPayOrder(PayOrderDto payOrder)
        {

            if (payOrder == null)
            {
                return null;
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<PayOrderDto, QueryPayOrderRS>());
            var mapper = config.CreateMapper();
            var result = mapper.Map<PayOrderDto, QueryPayOrderRS>(payOrder);
            result.SuccessTime = payOrder.SuccessTime == null ? null :new DateTimeOffset(payOrder.SuccessTime.Value).ToUnixTimeSeconds();
            result.CreatedAt= payOrder.CreatedAt == null ? null : new DateTimeOffset(payOrder.CreatedAt.Value).ToUnixTimeSeconds();

            return result;
        }
    }
}
