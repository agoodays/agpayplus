using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 分账记录表
    /// </summary>
    public class PayOrderDivisionRecordDto
    {
        /// <summary>
        /// 分账记录ID
        /// </summary>
        public long RecordId { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }

        /// <summary>
        /// 服务商号
        /// </summary>
        public string IsvNo { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        public string MchName { get; set; }

        /// <summary>
        /// 类型: 1-普通商户, 2-特约商户(服务商模式)
        /// </summary>
        public byte MchType { get; set; }

        /// <summary>
        /// 支付接口代码
        /// </summary>
        public string IfCode { get; set; }

        /// <summary>
        /// 支付订单号
        /// </summary>
        public string PayOrderId { get; set; }

        /// <summary>
        /// 支付订单渠道支付订单号
        /// </summary>
        public string PayOrderChannelOrderNo { get; set; }

        /// <summary>
        /// 订单金额,单位分
        /// </summary>
        public long PayOrderAmount { get; set; }

        /// <summary>
        /// 订单实际分账金额, 单位：分（订单金额 - 商户手续费 - 已退款金额）
        /// </summary>
        public long PayOrderDivisionAmount { get; set; }

        /// <summary>
        /// 系统分账批次号
        /// </summary>
        public string BatchOrderId { get; set; }

        /// <summary>
        /// 上游分账批次号
        /// </summary>
        public string ChannelBatchOrderId { get; set; }

        /// <summary>
        /// 状态: 0-待分账 1-分账成功, 2-分账失败
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// 上游返回数据包
        /// </summary>
        public string ChannelRespResult { get; set; }

        /// <summary>
        /// 账号快照》 分账接收者ID
        /// </summary>
        public long ReceiverId { get; set; }

        /// <summary>
        /// 账号快照》 组ID（便于商户接口使用）
        /// </summary>
        public long ReceiverGroupId { get; set; }

        /// <summary>
        /// 账号快照》 分账接收者别名
        /// </summary>
        public string ReceiverAlias { get; set; }

        /// <summary>
        /// 账号快照》 分账接收账号类型: 0-个人 1-商户
        /// </summary>
        public byte AccType { get; set; }

        /// <summary>
        /// 账号快照》 分账接收账号
        /// </summary>
        public string AccNo { get; set; }

        /// <summary>
        /// 账号快照》 分账接收账号名称
        /// </summary>
        public string AccName { get; set; }

        /// <summary>
        /// 账号快照》 分账关系类型（参考微信）， 如： SERVICE_PROVIDER 服务商等
        /// </summary>
        public string RelationType { get; set; }

        /// <summary>
        /// 账号快照》 当选择自定义时，需要录入该字段。 否则为对应的名称
        /// </summary>
        public string RelationTypeName { get; set; }

        /// <summary>
        /// 账号快照》 配置的实际分账比例
        /// </summary>
        public decimal DivisionProfit { get; set; }

        /// <summary>
        /// 计算该接收方的分账金额,单位分
        /// </summary>
        public long CalDivisionAmount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
