using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Common.Enumerator
{
    public enum MchInfoType
    {
        TYPE_NORMAL = 1, //商户类型： 1-普通商户
        TYPE_ISVSUB = 2, //商户类型： 2-特约商户
    }

    /// <summary>
    /// 订单类型:1-支付,2-退款, 3-转账
    /// </summary>
    public enum MchNotifyRecordType
    {
        TYPE_PAY_ORDER = 1,
        TYPE_REFUND_ORDER = 2,
        TYPE_TRANSFER_ORDER = 3,
    }

    /// <summary>
    /// 通知状态
    /// </summary>
    public enum MchNotifyRecordState
    {
        STATE_ING = 1,
        STATE_SUCCESS = 2,
        STATE_FAIL = 3,
    }

    public enum PayOrderState
    {
        STATE_INIT = 0, //订单生成
        STATE_ING = 1, //支付中
        STATE_SUCCESS = 2, //支付成功
        STATE_FAIL = 3, //支付失败
        STATE_CANCEL = 4, //已撤销
        STATE_REFUND = 5, //已退款
        STATE_CLOSED = 6, //订单关闭
    }

    public enum PayOrderRefund
    {
        REFUND_STATE_NONE = 0, //未发生实际退款
        REFUND_STATE_SUB = 1, //部分退款
        REFUND_STATE_ALL = 2, //全额退款

        DIVISION_MODE_FORBID = 0, //该笔订单不允许分账
        DIVISION_MODE_AUTO = 1, //支付成功按配置自动完成分账
        DIVISION_MODE_MANUAL = 2, //商户手动分账(解冻商户金额)

        DIVISION_STATE_UNHAPPEN = 0, //未发生分账
        DIVISION_STATE_WAIT_TASK = 1, //等待分账任务处理
        DIVISION_STATE_ING = 2, //分账处理中
        DIVISION_STATE_FINISH = 3, //分账任务已结束(不体现状态)
    }

    public enum PayOrderDivision
    {
        DIVISION_MODE_FORBID = 0, //该笔订单不允许分账
        DIVISION_MODE_AUTO = 1, //支付成功按配置自动完成分账
        DIVISION_MODE_MANUAL = 2, //商户手动分账(解冻商户金额)

        DIVISION_STATE_UNHAPPEN = 0, //未发生分账
        DIVISION_STATE_WAIT_TASK = 1, //等待分账任务处理
        DIVISION_STATE_ING = 2, //分账处理中
        DIVISION_STATE_FINISH = 3, //分账任务已结束(不体现状态)
    }

    public enum PayOrderDivisionState
    {
        STATE_WAIT = 0, // 待分账
        STATE_SUCCESS = 1, // 分账成功
        STATE_FAIL = 2, // 分账失败
    }

    public enum RefundOrderState
    {
        STATE_INIT = 0, //订单生成
        STATE_ING = 1, //退款中
        STATE_SUCCESS = 2, //退款成功
        STATE_FAIL = 3, //退款失败
        STATE_CLOSED = 4, //退款任务关闭
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

    public enum TransferOrderState
    {
        STATE_INIT = 0, //订单生成
        STATE_ING = 1, //转账中
        STATE_SUCCESS = 2, //转账成功
        STATE_FAIL = 3, //转账失败
        STATE_CLOSED = 4, //转账关闭
    }
}
