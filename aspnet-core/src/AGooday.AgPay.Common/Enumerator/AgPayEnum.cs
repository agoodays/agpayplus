using System.ComponentModel;
using System.Reflection;

namespace AGooday.AgPay.Common.Enumerator
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            return value.GetDescriptionOrDefault();
        }

        public static string GetDescriptionOrDefault(this Enum value, string defaultValue = null)
        {
            if (value is null)
            {
                return defaultValue;
            }
            var field = value.GetType().GetField(value.ToString());
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute != null ? attribute.Description : value.ToString();
        }

        public static TEnum? ToEnum<TEnum>(this int value) where TEnum : struct, Enum
        {
            //return (TEnum)Enum.ToObject(typeof(TEnum), value);
            try
            {
                Type enumUnderlyingType = Enum.GetUnderlyingType(typeof(TEnum));
                if (enumUnderlyingType == typeof(byte))
                {
                    if (Enum.IsDefined(typeof(TEnum), value))
                    {
                        return (TEnum)(object)value;
                    }
                }
                else if (enumUnderlyingType == typeof(int))
                {
                    int intValue = value;
                    if (Enum.IsDefined(typeof(TEnum), intValue))
                    {
                        return (TEnum)(object)intValue;
                    }
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        public static TEnum? ToEnum<TEnum>(this byte value) where TEnum : struct, Enum
        {
            //return (TEnum)Enum.ToObject(typeof(TEnum), value);
            try
            {
                Type enumUnderlyingType = Enum.GetUnderlyingType(typeof(TEnum));
                if (enumUnderlyingType == typeof(byte))
                {
                    if (Enum.IsDefined(typeof(TEnum), value))
                    {
                        return (TEnum)(object)value;
                    }
                }
                else if (enumUnderlyingType == typeof(int))
                {
                    int intValue = value;
                    if (Enum.IsDefined(typeof(TEnum), intValue))
                    {
                        return (TEnum)(object)intValue;
                    }
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        public static TEnum? ToEnum<TEnum>(this string value) where TEnum : struct, Enum
        {
            //return (TEnum)Enum.Parse(typeof(TEnum), value);
            try
            {
                if (Enum.TryParse(typeof(TEnum), value, out object result))
                {
                    return (TEnum)result;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public static TEnum? ToEnum<TEnum>(this string value, bool ignoreCase) where TEnum : struct, Enum
        {
            //return (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
            try
            {
                if (Enum.TryParse(typeof(TEnum), value, ignoreCase, out object result))
                {
                    return (TEnum)result;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
    }

    /// <summary>
    /// 商户类型
    /// </summary>
    public enum MchInfoType
    {
        /// <summary>
        /// 商户类型： 1-普通商户
        /// </summary>
        [Description("普通商户")]
        TYPE_NORMAL = 1,
        /// <summary>
        /// 商户类型： 2-特约商户
        /// </summary>
        [Description("特约商户")]
        TYPE_ISVSUB = 2,
    }

    /// <summary>
    /// 支付类型: WECHAT-微信, ALIPAY-支付宝, YSFPAY-云闪付, UNIONPAY-银联, DCEPPAY-数字人民币, OTHER-其他
    /// </summary>
    public enum PayWayType
    {
        /// <summary>
        /// 支付类型： WECHAT-微信
        /// </summary>
        [Description("微信")]
        WECHAT,
        /// <summary>
        /// 支付类型： ALIPAY-支付宝
        /// </summary>
        [Description("支付宝")]
        ALIPAY,
        /// <summary>
        /// 支付类型： YSFPAY-支付宝
        /// </summary>
        [Description("云闪付")]
        YSFPAY,
        /// <summary>
        /// 支付类型： UNIONPAY-银联
        /// </summary>
        [Description("银联")]
        UNIONPAY,
        /// <summary>
        /// 支付类型： DCEPPAY-数字人民币
        /// </summary>
        [Description("数字人民币")]
        DCEPPAY,
        /// <summary>
        /// 支付类型：OTHER-其他
        /// </summary>
        [Description("其他")]
        OTHER,
    }

    /// <summary>
    /// 订单类型:1-支付,2-退款, 3-转账
    /// </summary>
    public enum MchNotifyRecordType
    {
        /// <summary>
        /// 支付
        /// </summary>
        [Description("支付")]
        TYPE_PAY_ORDER = 1,
        /// <summary>
        /// 退款
        /// </summary>
        [Description("退款")]
        TYPE_REFUND_ORDER = 2,
        /// <summary>
        /// 转账
        /// </summary>
        [Description("转账")]
        TYPE_TRANSFER_ORDER = 3,
    }

    /// <summary>
    /// 通知状态,1-通知中,2-通知成功,3-通知失败
    /// </summary>
    public enum MchNotifyRecordState
    {
        /// <summary>
        /// 通知中
        /// </summary>
        [Description("通知中")]
        STATE_ING = 1,
        /// <summary>
        /// 通知成功
        /// </summary>
        [Description("通知成功")]
        STATE_SUCCESS = 2,
        /// <summary>
        /// 通知失败
        /// </summary>
        [Description("通知失败")]
        STATE_FAIL = 3,
    }

    /// <summary>
    /// 支付状态: 0-订单生成, 1-支付中, 2-支付成功, 3-支付失败, 4-已撤销, 5-已退款, 6-订单关闭
    /// </summary>
    public enum PayOrderState
    {
        /// <summary>
        /// 订单生成
        /// </summary>
        [Description("订单生成")]
        STATE_INIT = 0,
        /// <summary>
        /// 支付中
        /// </summary>
        [Description("支付中")]
        STATE_ING = 1,
        /// <summary>
        /// 支付成功
        /// </summary>
        [Description("支付成功")]
        STATE_SUCCESS = 2,
        /// <summary>
        /// 支付失败
        /// </summary>
        [Description("支付失败")]
        STATE_FAIL = 3,
        /// <summary>
        /// 已撤销
        /// </summary>
        [Description("已撤销")]
        STATE_CANCEL = 4,
        /// <summary>
        /// 已退款
        /// </summary>
        [Description("已退款")]
        STATE_REFUND = 5,
        /// <summary>
        /// 订单关闭
        /// </summary>
        [Description("订单关闭")]
        STATE_CLOSED = 6,
    }

    public enum PayOrderRefund
    {
        /// <summary>
        /// 未发生实际退款
        /// </summary>
        [Description("未退款")]
        REFUND_STATE_NONE = 0,
        /// <summary>
        /// 部分退款
        /// </summary>
        [Description("部分退款")]
        REFUND_STATE_SUB = 1,
        /// <summary>
        /// 全额退款
        /// </summary>
        [Description("全额退款")]
        REFUND_STATE_ALL = 2,
    }

    public enum PayOrderDivisionMode
    {
        /// <summary>
        /// 该笔订单不允许分账
        /// </summary>
        [Description("禁止分账")]
        DIVISION_MODE_FORBID = 0,
        /// <summary>
        /// 支付成功按配置自动完成分账
        /// </summary>
        [Description("自动分账")]
        DIVISION_MODE_AUTO = 1,
        /// <summary>
        /// 商户手动分账(解冻商户金额)
        /// </summary>
        [Description("手动分账")]
        DIVISION_MODE_MANUAL = 2,
    }

    public enum PayOrderDivisionState
    {
        /// <summary>
        /// 未发生分账
        /// </summary>
        [Description("未分账")]
        DIVISION_STATE_UNHAPPEN = 0,
        /// <summary>
        /// 等待分账任务处理
        /// </summary>
        [Description("等待分账")]
        DIVISION_STATE_WAIT_TASK = 1,
        /// <summary>
        /// 分账处理中
        /// </summary>
        [Description("分账中")]
        DIVISION_STATE_ING = 2,
        /// <summary>
        /// 分账任务已结束(不体现状态)
        /// </summary>
        [Description("已分账")]
        DIVISION_STATE_FINISH = 3,
    }

    public enum PayOrderDivisionRecordState
    {
        /// <summary>
        /// 待分账
        /// </summary>
        [Description("待分账")]
        STATE_WAIT = 0,
        /// <summary>
        /// 分账成功（明确成功）
        /// </summary>
        [Description("分账成功")]
        STATE_SUCCESS = 1,
        /// <summary>
        /// 分账失败（明确失败）
        /// </summary>
        [Description("分账失败")]
        STATE_FAIL = 2,
        /// <summary>
        /// 分账已受理（上游受理）
        /// </summary>
        [Description("分账已受理")]
        STATE_ACCEPT = 3,
    }

    /// <summary>
    /// 退款状态:0-订单生成,1-退款中,2-退款成功,3-退款失败
    /// </summary>
    public enum RefundOrderState
    {
        /// <summary>
        /// 订单生成
        /// </summary>
        [Description("订单生成")]
        STATE_INIT = 0,
        /// <summary>
        /// 退款中
        /// </summary>
        [Description("退款中")]
        STATE_ING = 1,
        /// <summary>
        /// 退款成功
        /// </summary>
        [Description("退款成功")]
        STATE_SUCCESS = 2,
        /// <summary>
        /// 退款失败
        /// </summary>
        [Description("退款失败")]
        STATE_FAIL = 3,
        /// <summary>
        /// 退款任务关闭
        /// </summary>
        [Description("退款关闭")]
        STATE_CLOSED = 4,
    }

    /// <summary>
    /// 入账方式
    /// </summary>
    public enum TransferOrderEntry
    {
        WX_CASH,
        ALIPAY_CASH,
        BANK_CARD,
    }

    /// <summary>
    /// 支付状态: 0-订单生成, 1-转账中, 2-转账成功, 3-转账失败, 4-订单关闭
    /// </summary>
    public enum TransferOrderState
    {
        /// <summary>
        /// 订单生成
        /// </summary>
        [Description("订单生成")]
        STATE_INIT = 0,
        /// <summary>
        /// 转账中
        /// </summary>
        [Description("转账中")]
        STATE_ING = 1,
        /// <summary>
        /// 转账成功
        /// </summary>
        [Description("转账成功")]
        STATE_SUCCESS = 2,
        /// <summary>
        /// 转账失败
        /// </summary>
        [Description("转账失败")]
        STATE_FAIL = 3,
        /// <summary>
        /// 转账关闭
        /// </summary>
        [Description("转账关闭")]
        STATE_CLOSED = 4,
    }

    /// <summary>
    /// 文章类型
    /// </summary>
    public enum ArticleType
    {
        /// <summary>
        /// 文章类型： 1-公告
        /// </summary>
        [Description("公告")]
        NOTICE = 1,
    }
}
