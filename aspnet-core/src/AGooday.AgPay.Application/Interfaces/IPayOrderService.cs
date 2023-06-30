using AGooday.AgPay.Application.DataTransfer;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayOrderService : IDisposable
    {
        bool Add(PayOrderDto dto);
        bool Remove(string recordId);
        bool Update(PayOrderDto dto);
        PayOrderDto GetById(string recordId);
        IEnumerable<PayOrderDto> GetAll();
        PayOrderDto QueryMchOrder(string mchNo, string payOrderId, string mchOrderNo);
        PaginatedList<PayOrderDto> GetPaginatedData(PayOrderQueryDto dto);
        JObject Statistics(PayOrderQueryDto dto);
        bool IsExistOrderUseIfCode(string ifCode);
        bool IsExistOrderUseWayCode(string wayCode);
        bool IsExistOrderByMchOrderNo(string mchNo, string mchOrderNo);
        /// <summary>
        /// 更新订单状态 【订单生成】 --》 【支付中】
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <param name="payOrder"></param>
        /// <returns></returns>
        bool UpdateInit2Ing(string payOrderId, PayOrderDto payOrder);
        /// <summary>
        /// 更新订单状态 【支付中】 --》 【支付成功】
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <param name="channelOrderNo"></param>
        /// <param name="channelUserId"></param>
        /// <returns></returns>
        bool UpdateIng2Success(string payOrderId, string channelOrderNo, string channelUserId);
        /// <summary>
        /// 更新订单状态  【支付中】 --》 【订单关闭】
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <returns></returns>
        bool UpdateIng2Close(string payOrderId);
        /// <summary>
        /// 更新订单状态 【支付中】 --》 【支付失败】
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <param name="channelOrderNo"></param>
        /// <param name="channelUserId"></param>
        /// <param name="channelErrCode"></param>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        bool UpdateIng2Fail(string payOrderId, string channelOrderNo, string channelUserId, string channelErrCode, string channelErrMsg);
        /// <summary>
        /// 更新订单状态 【支付中】 --》 【支付成功/支付失败】
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <param name="updateState"></param>
        /// <param name="channelOrderNo"></param>
        /// <param name="channelUserId"></param>
        /// <param name="channelErrCode"></param>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        bool UpdateIng2SuccessOrFail(string payOrderId, byte updateState, string channelOrderNo, string channelUserId, string channelErrCode, string channelErrMsg);
        /// <summary>
        /// 更新订单为 超时状态
        /// </summary>
        /// <returns></returns>
        int UpdateOrderExpired();
        /// <summary>
        /// 更新订单 通知状态 --> 已发送
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        bool UpdateNotifySent(string orderId);
        /// <summary>
        /// 更新订单表分账状态为： 等待分账任务处理
        /// </summary>
        /// <param name="payOrder"></param>
        /// <returns></returns>
        bool UpdateDivisionState(PayOrderDto payOrder);
        /// <summary>
        /// 计算支付订单商家入账金额
        /// 商家订单入账金额 （支付金额 - 手续费 - 退款金额 - 总分账金额）</summary>
        /// <param name="dbPayOrder"></param>
        /// <returns></returns>
        long CalMchIncomeAmount(PayOrderDto payOrder);
        /// <summary>
        /// 首页支付周统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <returns></returns>
        JObject MainPageWeekCount(string mchNo, string agentNo);
        /// <summary>
        /// 首页统计总数量
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <returns></returns>
        JObject MainPageNumCount(string mchNo, string agentNo);
        /// <summary>
        /// 服务商/代理商/商户统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <returns></returns>
        JObject MainPageIsvAndMchCount(string mchNo, string agentNo);
        /// <summary>
        /// 今日/昨日交易统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <returns></returns>
        JObject MainPagePayDayCount(string mchNo, string agentNo, DateTime? day);
        /// <summary>
        /// 首页支付统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <param name="createdStart"></param>
        /// <param name="createdEnd"></param>
        /// <returns></returns>
        List<Dictionary<string, object>> MainPagePayCount(string mchNo, string agentNo, string createdStart, string createdEnd);
        /// <summary>
        /// 首页支付类型统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <param name="createdStart"></param>
        /// <param name="createdEnd"></param>
        /// <returns></returns>
        List<PayTypeCountDto> MainPagePayTypeCount(string mchNo, string agentNo, string createdStart, string createdEnd);
    }
}
