using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace AGooday.AgPay.Payment.Api.RQRS.Division
{
    /// <summary>
    /// 分账账号的绑定 请求参数
    /// </summary>
    public class DivisionReceiverBindRQ : AbstractMchAppRQ
    {
        /// <summary>
        /// 支付接口代码
        /// </summary>
        [Required(ErrorMessage = "支付接口代码不能为空")]
        public string IfCode { get; set; }

        /// <summary>
        /// 接收者账号别名
        /// </summary>
        public string ReceiverAlias { get; set; }

        /// <summary>
        /// 组ID
        /// </summary>
        [Required(ErrorMessage = "组ID不能为空， 若不存在请先登录商户平台进行创建操作")]
        public long ReceiverGroupId { get; set; }

        /// <summary>
        /// 分账接收账号类型: 0-个人(对私) 1-商户(对公) 
        /// </summary>
        [Required(ErrorMessage = "分账接收账号类型不能为空")]
        [Range(0, 1, ErrorMessage = "支付金额不能为空")]
        public byte AccType { get; set; }

        /// <summary>
        /// 分账接收账号
        /// </summary>
        [Required(ErrorMessage = "分账接收账号不能为空")]
        public string AccNo { get; set; }

        /// <summary>
        /// 分账接收账号名称
        /// </summary>
        public string AccName { get; set; }

        /// <summary>
        /// 分账关系类型（参考微信）， 如： SERVICE_PROVIDER 服务商等
        /// </summary>
        [Required(ErrorMessage = "分账关系类型不能为空")]
        public string RelationType { get; set; }

        /// <summary>
        /// 当选择自定义时，需要录入该字段。 否则为对应的名称
        /// </summary>
        public string RelationTypeName { get; set; }

        /// <summary>
        /// 渠道特殊信息
        /// </summary>
        public string ChannelExtInfo { get; set; }

        /// <summary>
        /// 分账比例
        /// </summary>
        [Required(ErrorMessage = "分账比例不能为空")]
        public string DivisionProfit { get; set; }
    }
}
