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
    /// 支付订单表
    /// </summary>
    public class PayOrder : Entity<long>
    {
        /**
         * 支付订单号
         */
        public string PayOrderId { get; set; }

        /**
         * 商户号
         */
        public string MchNo { get; set; }

        /**
         * 服务商号
         */
        public string IsvNo { get; set; }

        /**
         * 应用ID
         */
        public string AppId { get; set; }

        /**
         * 商户名称
         */
        public string MchName { get; set; }

        /**
         * 类型: 1-普通商户, 2-特约商户(服务商模式)
         */
        public byte MchType { get; set; }

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
         * 商户手续费费率快照
         */
        public decimal MchFeeRate { get; set; }

        /**
         * 商户手续费,单位分
         */
        public long MchFeeAmount { get; set; }

        /**
         * 三位货币代码,人民币:cny
         */
        public string Currency { get; set; }

        /**
         * 支付状态: 0-订单生成, 1-支付中, 2-支付成功, 3-支付失败, 4-已撤销, 5-已退款, 6-订单关闭
         */
        public byte State { get; set; }

        /**
         * 向下游回调状态, 0-未发送,  1-已发送
         */
        public byte NotifyState { get; set; }

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
         * 特定渠道发起额外参数
         */
        public string ChannelExtra { get; set; }

        /**
         * 渠道用户标识,如微信openId,支付宝账号
         */
        public string ChannelUser { get; set; }

        /**
         * 渠道订单号
         */
        public string ChannelOrderNo { get; set; }

        /**
         * 退款状态: 0-未发生实际退款, 1-部分退款, 2-全额退款
         */
        public byte RefundState { get; set; }

        /**
         * 退款次数
         */
        public int RefundTimes { get; set; }

        /**
         * 退款总金额,单位分
         */
        public long RefundAmount { get; set; }

        /**
         * 订单分账模式：0-该笔订单不允许分账, 1-支付成功按配置自动完成分账, 2-商户手动分账(解冻商户金额)
         */
        public byte DivisionMode { get; set; }

        /**
         * 0-未发生分账, 1-等待分账任务处理, 2-分账处理中, 3-分账任务已结束(不体现状态)
         */
        public byte DivisionState { get; set; }

        /**
         * 最新分账时间
         */
        public DateTime DivisionLastTime { get; set; }

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
         * 异步通知地址
         */
        public string NotifyUrl { get; set; }

        /**
         * 页面跳转地址
         */
        public string ReturnUrl { get; set; }

        /**
         * 订单失效时间
         */
        public DateTime ExpiredTime { get; set; }

        /**
         * 订单支付成功时间
         */
        public DateTime SuccessTime { get; set; }

        /**
         * 创建时间
         */
        public DateTime CreatedAt { get; set; }

        /**
         * 更新时间
         */
        public DateTime UpdatedAt { get; set; }
    }
}
