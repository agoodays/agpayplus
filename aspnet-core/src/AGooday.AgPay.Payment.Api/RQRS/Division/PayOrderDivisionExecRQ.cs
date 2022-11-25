using System.ComponentModel.DataAnnotations;

namespace AGooday.AgPay.Payment.Api.RQRS.Division
{
    /// <summary>
    /// 发起订单分账 请求参数
    /// </summary>
    public class PayOrderDivisionExecRQ : AbstractMchAppRQ
    {
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string MchOrderNo { get; set; }

        /// <summary>
        /// 支付系统订单号
        /// </summary>
        public string PayOrderId { get; set; }

        /// <summary>
        /// 是否使用系统配置的自动分账组： 0-否 1-是
        /// </summary>
        [Required(ErrorMessage = "是否使用系统配置的自动分账组不能为空")]
        public byte UseSysAutoDivisionReceivers { get; set; }

        /// <summary>
        ///  接收者账号列表（JSONArray 转换为字符串类型）
        /// 仅当useSysAutoDivisionReceivers=0 时有效。
        /// 
        /// 参考：
        /// 
        /// 方式1： 按账号纬度
        /// [{
        ///     receiverId: 800001,
        ///     divisionProfit: 0.1 (若不填入则使用系统默认配置值)
        /// }]
        /// 
        /// 方式2： 按组纬度
        /// [{
        ///     receiverGroupId: 100001, (该组所有 当前订单的渠道账号并且可用状态的全部参与分账)
        ///     divisionProfit: 0.1 (每个账号的分账比例， 若不填入则使用系统默认配置值， 建议不填写)
        /// }]
        /// </summary>
        public string Receivers { get; set; }
    }
}
