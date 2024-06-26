﻿using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 支付接口配置参数表
    /// </summary>
    public class PayInterfaceConfigDto : BaseModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// 账号类型:ISV-服务商, ISV_OAUTH2-服务商oauth2, AGENT-代理商, MCH_APP-商户应用, MCH_APP_OAUTH2-商户应用oauth2
        /// </summary>
        public string InfoType { get; set; }

        /// <summary>
        /// 服务商或商户No
        /// </summary>
        public string InfoId { get; set; }

        /// <summary>
        /// 支付接口
        /// </summary>
        public string IfCode { get; set; }

        /// <summary>
        /// 接口配置参数,json字符串
        /// </summary>
        public string IfParams { get; set; }

        /// <summary>
        /// 结算周期（自然日）
        /// </summary>
        public byte SettHoldDay { get; set; }

        /// <summary>
        /// 支付接口费率
        /// </summary>
        public decimal? IfRate { get; set; }

        /// <summary>
        /// 是否开启进件: 0-关闭, 1-开启
        /// </summary>
        public byte IsOpenApplyment { get; set; }

        /// <summary>
        /// 是否开启提现: 0-关闭, 1-开启
        /// </summary>
        public byte IsOpenCashout { get; set; }

        /// <summary>
        /// 是否开启对账: 0-关闭, 1-开启
        /// </summary>
        public byte IsOpenCheckBill { get; set; }

        /// <summary>
        /// 对账过滤子商户
        /// </summary>
        public string IgnoreCheckBillMchNos { get; set; }

        /// <summary>
        /// oauth2配置Id
        /// </summary>
        public string Oauth2InfoId { get; set; }

        /// <summary>
        /// 状态: 0-停用, 1-启用
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 创建者用户ID
        /// </summary>
        public long? CreatedUid { get; set; }

        /// <summary>
        /// 创建者姓名
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// 更新者用户ID
        /// </summary>
        public long? UpdatedUid { get; set; }

        /// <summary>
        /// 更新者姓名
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 类型: 1-普通商户, 2-特约商户(服务商模式)
        /// </summary>
        public byte? MchType { get; set; }
    }
}
