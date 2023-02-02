namespace AGooday.AgPay.Common.Enumerator
{
    /// <summary>
    /// 商户类型
    /// </summary>
    public enum MchInfoType
    {
        /// <summary>
        /// 商户类型： 1-普通商户
        /// </summary>
        TYPE_NORMAL = 1,
        /// <summary>
        /// 商户类型： 2-特约商户
        /// </summary>
        TYPE_ISVSUB = 2,
    }

    /// <summary>
    /// 订单类型:1-支付,2-退款, 3-转账
    /// </summary>
    public enum MchNotifyRecordType
    {
        /// <summary>
        /// 支付
        /// </summary>
        TYPE_PAY_ORDER = 1,
        /// <summary>
        /// 退款
        /// </summary>
        TYPE_REFUND_ORDER = 2,
        /// <summary>
        /// 转账
        /// </summary>
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
        STATE_ING = 1,
        /// <summary>
        /// 通知成功
        /// </summary>
        STATE_SUCCESS = 2,
        /// <summary>
        /// 通知失败
        /// </summary>
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
        STATE_INIT = 0,
        /// <summary>
        /// 支付中
        /// </summary>
        STATE_ING = 1,
        /// <summary>
        /// 支付成功
        /// </summary>
        STATE_SUCCESS = 2,
        /// <summary>
        /// 支付失败
        /// </summary>
        STATE_FAIL = 3,
        /// <summary>
        /// 已撤销
        /// </summary>
        STATE_CANCEL = 4,
        /// <summary>
        /// 已退款
        /// </summary>
        STATE_REFUND = 5,
        /// <summary>
        /// 订单关闭
        /// </summary>
        STATE_CLOSED = 6,
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

    /// <summary>
    /// 退款状态:0-订单生成,1-退款中,2-退款成功,3-退款失败
    /// </summary>
    public enum RefundOrderState
    {
        /// <summary>
        /// 订单生成
        /// </summary>
        STATE_INIT = 0,
        /// <summary>
        /// 退款中
        /// </summary>
        STATE_ING = 1,
        /// <summary>
        /// 退款成功
        /// </summary>
        STATE_SUCCESS = 2,
        /// <summary>
        /// 退款失败
        /// </summary>
        STATE_FAIL = 3,
        /// <summary>
        /// 退款任务关闭
        /// </summary>
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
        STATE_INIT = 0,
        /// <summary>
        /// 转账中
        /// </summary>
        STATE_ING = 1,
        /// <summary>
        /// 转账成功
        /// </summary>
        STATE_SUCCESS = 2,
        /// <summary>
        /// 转账失败
        /// </summary>
        STATE_FAIL = 3,
        /// <summary>
        /// 转账关闭
        /// </summary>
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
        NOTICE = 1,
    }
}
