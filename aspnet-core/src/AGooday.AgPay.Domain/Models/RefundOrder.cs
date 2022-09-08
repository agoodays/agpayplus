using AGooday.AgPay.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 退款订单表
    /// </summary>
    public class RefundOrder : Entity<long>
    {
        /**
         * 退款订单号（支付系统生成订单号）
         */
        public string RefundOrderId{ get; set; }

        /**
         * 支付订单号（与t_pay_order对应）
         */
        public string PayOrderId{ get; set; }

        /**
         * 渠道支付单号（与t_pay_order channel_order_no对应）
         */
        public string ChannelPayOrderNo{ get; set; }

        /**
         * 商户号
         */
        public string MchNo{ get; set; }

        /**
         * 服务商号
         */
        public string IsvNo{ get; set; }

        /**
         * 应用Id
         */
        public string AppId{ get; set; }

        /**
         * 商户名称
         */
        public string MchName{ get; set; }

        /**
         * 类型: 1-普通商户, 2-特约商户(服务商模式)
         */
        public byte MchType{ get; set; }

        /**
         * 商户退款单号（商户系统的订单号）
         */
        public string MchRefundNo{ get; set; }

        /**
         * 支付方式代码
         */
        public string WayCode{ get; set; }

        /**
         * 支付接口代码
         */
        public string IfCode{ get; set; }

        /**
         * 支付金额,单位分
         */
        public long PayAmount{ get; set; }

        /**
         * 退款金额,单位分
         */
        public long RefundAmount{ get; set; }

        /**
         * 三位货币代码,人民币:cny
         */
        public string Currency{ get; set; }

        /**
         * 退款状态:0-订单生成,1-退款中,2-退款成功,3-退款失败,4-退款任务关闭
         */
        public byte State{ get; set; }

        /**
         * 客户端IP
         */
        public string ClientIp{ get; set; }

        /**
         * 退款原因
         */
        public string RefundReason{ get; set; }

        /**
         * 渠道订单号
         */
        public string ChannelOrderNo{ get; set; }

        /**
         * 渠道错误码
         */
        public string ErrCode{ get; set; }

        /**
         * 渠道错误描述
         */
        public string ErrMsg{ get; set; }

        /**
         * 特定渠道发起时额外参数
         */
        public string ChannelExtra{ get; set; }

        /**
         * 通知地址
         */
        public string NotifyUrl{ get; set; }

        /**
         * 扩展参数
         */
        public string ExtParam{ get; set; }

        /**
         * 订单退款成功时间
         */
        public DateTime SuccessTime{ get; set; }

        /**
         * 退款失效时间（失效后系统更改为退款任务关闭状态）
         */
        public DateTime ExpiredTime{ get; set; }

        /**
         * 创建时间
         */
        public DateTime CreatedAt{ get; set; }

        /**
         * 更新时间
         */
        public DateTime UpdatedAt{ get; set; }
    }
}
