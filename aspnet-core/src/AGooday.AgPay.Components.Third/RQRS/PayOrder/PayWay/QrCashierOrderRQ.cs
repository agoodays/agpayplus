﻿using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay
{
    public class QrCashierOrderRQ : CommonPayDataRQ
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public QrCashierOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.QR_CASHIER;
        }
    }
}
