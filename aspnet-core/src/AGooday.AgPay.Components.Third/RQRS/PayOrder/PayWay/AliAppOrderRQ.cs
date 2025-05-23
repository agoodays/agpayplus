﻿using System.ComponentModel.DataAnnotations;
using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Components.Third.RQRS.PayOrder.PayWay
{
    /// <summary>
    /// 支付方式： ALI_APP
    /// </summary>
    public class AliAppOrderRQ : UnifiedOrderRQ
    {
        /// <summary>
        /// 支付宝用户ID
        /// </summary>
        /// <param name=""></param>
        [Required(ErrorMessage = "用户ID不能为空")]
        public string BuyerUserId { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AliAppOrderRQ()
        {
            this.WayCode = CS.PAY_WAY_CODE.ALI_APP; //默认 wayCode, 避免validate出现问题
        }

        public override string GetChannelUserId()
        {
            return this.BuyerUserId;
        }
    }
}
