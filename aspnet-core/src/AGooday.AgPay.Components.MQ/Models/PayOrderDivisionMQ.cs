using AGooday.AgPay.Components.MQ.Constant;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace AGooday.AgPay.Components.MQ.Models
{
    /// <summary>
    /// 定义MQ消息格式
    /// 业务场景： [ 支付订单的订单分账消息 ]
    /// </summary>
    public class PayOrderDivisionMQ : AbstractMQ
    {
        /// <summary>
        /// 【！重要配置项！】 定义MQ名称
        /// </summary>
        public static readonly string MQ_NAME = "QUEUE_PAY_ORDER_DIVISION";
        public static readonly MQSendTypeEnum MQ_TYPE = MQSendTypeEnum.QUEUE;

        /// <summary>
        /// 内置msg 消息体定义
        /// </summary>
        private MsgPayload Payload;

        public PayOrderDivisionMQ(MsgPayload payload) => Payload = payload;

        /// <summary>
        /// 【！重要配置项！】 定义Msg消息载体
        /// </summary>
        public class MsgPayload
        {
            /// <summary>
            /// 支付订单号
            /// </summary>
            public string PayOrderId { get; private set; }

            /// <summary>
            /// 是否使用默认分组
            /// </summary>
            public byte UseSysAutoDivisionReceivers { get; private set; }

            /// <summary>
            /// 分账接受者列表， 字段值为空表示系统默认配置项。
            /// 格式：{receiverId: '1001', receiverGroupId: '1001', divisionProfit: '0.1'}
            /// divisionProfit: 空表示使用系统默认比例。
            /// </summary>
            public List<CustomerDivisionReceiver> ReceiverList { get; private set; }

            public MsgPayload(string payOrderId, byte useSysAutoDivisionReceivers, List<CustomerDivisionReceiver> receiverList)
            {
                PayOrderId = payOrderId;
                UseSysAutoDivisionReceivers = useSysAutoDivisionReceivers;
                ReceiverList = receiverList;
            }
        }

        public override string GetMQName() => MQ_NAME;

        /// <summary>
        /// 【！重要配置项！】
        /// </summary>
        /// <returns></returns>
        public override MQSendTypeEnum GetMQType() => MQ_TYPE;  // QUEUE - 点对点 、 BROADCAST - 广播模式

        public override string ToMessage() => JsonConvert.SerializeObject(Payload);

        /// <summary>
        /// 【！重要配置项！】 构造MQModel , 一般用于发送MQ时
        /// </summary>
        /// <returns></returns>
        public static PayOrderDivisionMQ Build(string payOrderId, byte useSysAutoDivisionReceivers, List<CustomerDivisionReceiver> receiverList) 
            => new PayOrderDivisionMQ(new MsgPayload(payOrderId, useSysAutoDivisionReceivers, receiverList));

        /// <summary>
        /// 解析MQ消息， 一般用于接收MQ消息时
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static MsgPayload Parse(string msg) => JsonConvert.DeserializeObject<MsgPayload>(msg);

        /// <summary>
        /// 定义 IMQReceiver 接口： 项目实现该接口则可接收到对应的业务消息
        /// </summary>
        public interface IMQReceiver
        {
            void Receive(MsgPayload payload);
        }

        /// <summary>
        /// 自定义定义接收账号定义信息
        /// </summary>
        public class CustomerDivisionReceiver
        {
            /// <summary>
            /// 分账接收者ID (与receiverGroupId 二选一)
            /// </summary>
            public long ReceiverId { get; set; }

            /// <summary>
            /// 组ID（便于商户接口使用） (与 receiverId 二选一)
            /// </summary>
            public long ReceiverGroupId { get; set; }

            /// <summary>
            /// 分账比例 （可以为空， 为空表示使用系统默认值）
            /// </summary>
            public decimal? DivisionProfit { get; set; }

            public CustomerDivisionReceiver(long receiverId, long receiverGroupId, decimal divisionProfit)
            {
                ReceiverId = receiverId;
                ReceiverGroupId = receiverGroupId;
                DivisionProfit = divisionProfit;
            }
        }
    }
}
