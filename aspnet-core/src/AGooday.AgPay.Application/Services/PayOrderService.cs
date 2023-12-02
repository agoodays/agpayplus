using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 支付订单表 服务实现类
    /// </summary>
    public class PayOrderService : IPayOrderService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayOrderRepository _payOrderRepository;
        private readonly IRefundOrderRepository _refundOrderRepository;
        private readonly IMchInfoRepository _mchInfoRepository;
        private readonly IAgentInfoRepository _agentInfoRepository;
        private readonly IIsvInfoRepository _isvInfoRepository;
        private readonly IPayWayRepository _payWayRepository;
        private readonly IPayOrderDivisionRecordRepository _payOrderDivisionRecordRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public PayOrderService(IPayOrderRepository payOrderRepository,
            IRefundOrderRepository refundOrderRepository,
            IMchInfoRepository mchInfoRepository,
            IAgentInfoRepository agentInfoRepository,
            IIsvInfoRepository isvInfoRepository,
            IPayWayRepository payWayRepository,
            IPayOrderDivisionRecordRepository payOrderDivisionRecordRepository,
            IMapper mapper, IMediatorHandler bus)
        {
            _payOrderRepository = payOrderRepository;
            _refundOrderRepository = refundOrderRepository;
            _mchInfoRepository = mchInfoRepository;
            _agentInfoRepository = agentInfoRepository;
            _isvInfoRepository = isvInfoRepository;
            _payWayRepository = payWayRepository;
            _payOrderDivisionRecordRepository = payOrderDivisionRecordRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool Add(PayOrderDto dto)
        {
            var m = _mapper.Map<PayOrder>(dto);
            _payOrderRepository.Add(m);
            return _payOrderRepository.SaveChanges(out int _);
        }

        public bool Remove(string recordId)
        {
            _payOrderRepository.Remove(recordId);
            return _payOrderRepository.SaveChanges(out int _);
        }

        public bool Update(PayOrderDto dto)
        {
            var m = _mapper.Map<PayOrder>(dto);
            _payOrderRepository.Update(m);
            return _payOrderRepository.SaveChanges(out int _);
        }

        public PayOrderDto GetById(string recordId)
        {
            var entity = _payOrderRepository.GetById(recordId);
            var dto = _mapper.Map<PayOrderDto>(entity);
            return dto;
        }

        public IEnumerable<PayOrderDto> GetAll()
        {
            var payOrders = _payOrderRepository.GetAll();
            return _mapper.Map<IEnumerable<PayOrderDto>>(payOrders);
        }

        public PayOrderDto QueryMchOrder(string mchNo, string payOrderId, string mchOrderNo)
        {
            var entity = _payOrderRepository.GetAll().Where(w => w.MchNo.Equals(mchNo)
            && (w.PayOrderId.Equals(payOrderId) || w.MchOrderNo.Equals(mchOrderNo))).FirstOrDefault();
            return _mapper.Map<PayOrderDto>(entity);
        }

        /// <summary>
        /// 通用列表查询条件
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public PaginatedList<PayOrderDto> GetPaginatedData(PayOrderQueryDto dto)
        {
            var payOrders = _payOrderRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (dto.MchType.Equals(null) || w.MchType.Equals(dto.MchType))
                && (string.IsNullOrWhiteSpace(dto.IfCode) || w.IfCode.Equals(dto.IfCode))
                && (string.IsNullOrWhiteSpace(dto.WayCode) || w.WayCode.Equals(dto.WayCode))
                && (string.IsNullOrWhiteSpace(dto.MchOrderNo) || w.MchOrderNo.Equals(dto.MchOrderNo))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (string.IsNullOrWhiteSpace(dto.StoreId) || w.StoreId.Equals(dto.StoreId))
                && (string.IsNullOrWhiteSpace(dto.StoreName) || w.StoreName.Equals(dto.StoreName))
                && (dto.DivisionState.Equals(null) || w.DivisionState.Equals(dto.DivisionState))
                && (string.IsNullOrWhiteSpace(dto.PayOrderId) || w.PayOrderId.Equals(dto.PayOrderId))
                && (string.IsNullOrWhiteSpace(dto.MchOrderNo) || w.MchOrderNo.Equals(dto.MchOrderNo))
                && (string.IsNullOrWhiteSpace(dto.ChannelOrderNo) || w.ChannelOrderNo.Equals(dto.ChannelOrderNo))
                && (string.IsNullOrWhiteSpace(dto.PlatformOrderNo) || w.PlatformOrderNo.Equals(dto.PlatformOrderNo))
                && (string.IsNullOrWhiteSpace(dto.PlatformMchOrderNo) || w.PlatformMchOrderNo.Equals(dto.PlatformMchOrderNo))
                && (string.IsNullOrWhiteSpace(dto.UnionOrderId) || w.PayOrderId.Equals(dto.UnionOrderId) || w.MchOrderNo.Equals(dto.UnionOrderId) || w.ChannelOrderNo.Equals(dto.UnionOrderId))
                && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
                && (dto.CreatedEnd.Equals(null) || w.CreatedAt <= dto.CreatedEnd)
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<PayOrder>.Create<PayOrderDto>(payOrders, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }

        public JObject Statistics(PayOrderQueryDto dto)
        {
            var payOrders = _payOrderRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (dto.MchType.Equals(null) || w.MchType.Equals(dto.MchType))
                && (string.IsNullOrWhiteSpace(dto.WayCode) || w.WayCode.Equals(dto.WayCode))
                && (string.IsNullOrWhiteSpace(dto.MchOrderNo) || w.MchOrderNo.Equals(dto.MchOrderNo))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (string.IsNullOrWhiteSpace(dto.StoreId) || w.StoreId.Equals(dto.StoreId))
                && (string.IsNullOrWhiteSpace(dto.StoreName) || w.StoreName.Equals(dto.StoreName))
                && (dto.DivisionState.Equals(null) || w.DivisionState.Equals(dto.DivisionState))
                && (string.IsNullOrWhiteSpace(dto.UnionOrderId) || w.PayOrderId.Equals(dto.UnionOrderId) || w.MchOrderNo.Equals(dto.UnionOrderId) || w.ChannelOrderNo.Equals(dto.UnionOrderId))
                && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
                && (dto.CreatedEnd.Equals(null) || w.CreatedAt <= dto.CreatedEnd));
            var allPayAmount = payOrders.Sum(s => s.Amount);
            var allPayCount = payOrders.Count();
            var failPayAmount = payOrders.Where(w => !w.State.Equals((byte)PayOrderState.STATE_SUCCESS)).Sum(s => s.Amount);
            var failPayCount = payOrders.Where(w => !w.State.Equals((byte)PayOrderState.STATE_SUCCESS)).Count();
            var mchFeeAmount = payOrders.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS)).Sum(s => s.MchFeeAmount);
            var payAmount = payOrders.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS)).Sum(s => s.Amount);
            var payCount = payOrders.Where(w => w.State.Equals((byte)PayOrderState.STATE_SUCCESS)).Count();
            var refundAmount = payOrders.Where(w => w.State.Equals((byte)PayOrderState.STATE_REFUND)).Sum(s => s.Amount);
            var refundCount = payOrders.Where(w => w.State.Equals((byte)PayOrderState.STATE_REFUND)).Count();
            JObject json = new JObject();
            json.Add("allPayAmount", Decimal.Round(allPayAmount / 100M, 2, MidpointRounding.AwayFromZero));
            json.Add("allPayCount", allPayCount);
            json.Add("failPayAmount", Decimal.Round(failPayAmount / 100M, 2, MidpointRounding.AwayFromZero));
            json.Add("failPayCount", failPayCount);
            json.Add("mchFeeAmount", Decimal.Round(mchFeeAmount / 100M, 2, MidpointRounding.AwayFromZero));
            json.Add("payAmount", Decimal.Round(payAmount / 100M, 2, MidpointRounding.AwayFromZero));
            json.Add("payCount", payCount);
            json.Add("refundAmount", Decimal.Round(refundAmount / 100M, 2, MidpointRounding.AwayFromZero));
            json.Add("refundCount", refundCount);
            return json;
        }

        public bool IsExistOrderUseIfCode(string ifCode)
        {
            return _payOrderRepository.IsExistOrderUseIfCode(ifCode);
        }
        public bool IsExistOrderUseWayCode(string wayCode)
        {
            return _payOrderRepository.IsExistOrderUseWayCode(wayCode);
        }
        public bool IsExistOrderByMchOrderNo(string mchNo, string mchOrderNo)
        {
            return _payOrderRepository.IsExistOrderByMchOrderNo(mchNo, mchOrderNo);
        }
        /// <summary>
        /// 更新订单状态 【订单生成】 --》 【支付中】
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <param name="payOrder"></param>
        /// <returns></returns>
        public bool UpdateInit2Ing(string payOrderId, PayOrderDto payOrder)
        {
            var updateRecord = _payOrderRepository.GetById(payOrderId);
            if (updateRecord.State != (byte)PayOrderState.STATE_INIT)
            {
                return false;
            }

            updateRecord.State = (byte)PayOrderState.STATE_ING;

            //同时更新， 未确定 --》 已确定的其他信息。  如支付接口的确认、 费率的计算。
            updateRecord.IfCode = payOrder.IfCode;
            updateRecord.WayCode = payOrder.WayCode;
            updateRecord.WayType = payOrder.WayType;
            updateRecord.MchFeeRate = payOrder.MchFeeRate;
            updateRecord.MchFeeAmount = payOrder.MchFeeAmount;
            updateRecord.ChannelUser = payOrder.ChannelUser;
            _payOrderRepository.Update(updateRecord);
            return _payOrderRepository.SaveChanges(out int _);
        }
        /// <summary>
        /// 更新订单状态 【支付中】 --》 【支付成功】
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <param name="channelMchNo"></param>
        /// <param name="channelIsvNo"></param>
        /// <param name="channelOrderNo"></param>
        /// <param name="channelUserId"></param>
        /// <param name="platformOrderNo"></param>
        /// <param name="platformMchOrderNo"></param>
        /// <returns></returns>
        public bool UpdateIng2Success(string payOrderId, string channelMchNo, string channelIsvNo, string channelOrderNo, string channelUserId, string platformOrderNo, string platformMchOrderNo)
        {
            var updateRecord = _payOrderRepository.GetById(payOrderId);
            if (updateRecord.State != (byte)PayOrderState.STATE_ING)
            {
                return false;
            }
            updateRecord.State = (byte)PayOrderState.STATE_SUCCESS;
            updateRecord.ChannelMchNo = channelMchNo;
            updateRecord.ChannelIsvNo = channelIsvNo;
            updateRecord.ChannelOrderNo = channelOrderNo;
            updateRecord.ChannelUser = channelUserId;
            updateRecord.PlatformOrderNo = platformOrderNo;
            updateRecord.PlatformMchOrderNo = platformMchOrderNo;
            updateRecord.SuccessTime = DateTime.Now;
            _payOrderRepository.Update(updateRecord);
            return _payOrderRepository.SaveChanges(out int _);
        }
        /// <summary>
        /// 更新订单状态  【支付中】 --》 【订单关闭】
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <returns></returns>
        public bool UpdateIng2Close(string payOrderId)
        {
            var updateRecord = _payOrderRepository.GetById(payOrderId);
            if (updateRecord.State != (byte)PayOrderState.STATE_ING)
            {
                return false;
            }
            updateRecord.State = (byte)PayOrderState.STATE_CLOSED;
            updateRecord.SuccessTime = DateTime.Now;
            _payOrderRepository.Update(updateRecord);
            return _payOrderRepository.SaveChanges(out int _);
        }
        /// <summary>
        /// 更新订单状态 【支付中】 --》 【支付失败】
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <param name="channelMchNo"></param>
        /// <param name="channelIsvNo"></param>
        /// <param name="channelOrderNo"></param>
        /// <param name="channelUserId"></param>
        /// <param name="platformOrderNo"></param>
        /// <param name="platformMchOrderNo"></param>
        /// <param name="channelErrCode"></param>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        public bool UpdateIng2Fail(string payOrderId, string channelMchNo, string channelIsvNo, string channelOrderNo, string channelUserId, string platformOrderNo, string platformMchOrderNo, string channelErrCode, string channelErrMsg)
        {
            var updateRecord = _payOrderRepository.GetById(payOrderId);
            if (updateRecord.State != (byte)PayOrderState.STATE_ING)
            {
                return false;
            }
            updateRecord.State = (byte)PayOrderState.STATE_FAIL;
            updateRecord.ErrCode = channelErrCode;
            updateRecord.ErrMsg = channelErrMsg;
            updateRecord.ChannelMchNo = channelMchNo;
            updateRecord.ChannelIsvNo = channelIsvNo;
            updateRecord.ChannelOrderNo = channelOrderNo;
            updateRecord.ChannelUser = channelUserId;
            updateRecord.PlatformOrderNo = platformOrderNo;
            updateRecord.PlatformMchOrderNo = platformMchOrderNo;
            _payOrderRepository.Update(updateRecord);
            return _payOrderRepository.SaveChanges(out int _);
        }
        /// <summary>
        /// 更新订单状态 【支付中】 --》 【支付成功/支付失败】
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <param name="updateState"></param>
        /// <param name="channelMchNo"></param>
        /// <param name="channelIsvNo"></param>
        /// <param name="channelOrderNo"></param>
        /// <param name="channelUserId"></param>
        /// <param name="platformOrderNo"></param>
        /// <param name="platformMchOrderNo"></param>
        /// <param name="channelErrCode"></param>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        public bool UpdateIng2SuccessOrFail(string payOrderId, byte updateState, string channelMchNo, string channelIsvNo, string channelOrderNo, string channelUserId, string platformOrderNo, string platformMchOrderNo, string channelErrCode, string channelErrMsg)
        {
            if (updateState == (byte)PayOrderState.STATE_ING)
            {
                return true;
            }
            else if (updateState == (byte)PayOrderState.STATE_SUCCESS)
            {
                return UpdateIng2Success(payOrderId, channelMchNo, channelIsvNo, channelOrderNo, channelUserId, platformOrderNo, platformMchOrderNo);
            }
            else if (updateState == (byte)PayOrderState.STATE_FAIL)
            {
                return UpdateIng2Fail(payOrderId, channelMchNo, channelIsvNo, channelOrderNo, channelUserId, platformOrderNo, platformMchOrderNo, channelErrCode, channelErrMsg);
            }
            return false;
        }
        /// <summary>
        /// 更新订单为 超时状态
        /// </summary>
        /// <returns></returns>
        public int UpdateOrderExpired()
        {
            var updateRecords = _payOrderRepository.GetAll().Where(
                w => (new List<byte>() { (byte)PayOrderState.STATE_INIT, (byte)PayOrderState.STATE_ING }).Contains(w.State)
                && w.ExpiredTime < DateTime.Now);
            foreach (var payOrder in updateRecords)
            {
                payOrder.State = (byte)PayOrderState.STATE_CLOSED;
                _payOrderRepository.Update(payOrder);
            }
            return _payOrderRepository.SaveChanges();
        }
        /// <summary>
        /// 更新订单 通知状态 --> 已发送
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool UpdateNotifySent(string orderId)
        {
            var updateRecord = _payOrderRepository.GetById(orderId);
            updateRecord.NotifyState = CS.YES;
            _payOrderRepository.Update(updateRecord);
            return _payOrderRepository.SaveChanges(out int _);
        }
        /// <summary>
        /// 更新订单表分账状态为： 等待分账任务处理
        /// </summary>
        /// <param name="payOrder"></param>
        /// <returns></returns>
        public bool UpdateDivisionState(PayOrderDto payOrder)
        {
            var updateRecord = _payOrderRepository.GetById(payOrder.PayOrderId);
            if (updateRecord.DivisionState != (byte)PayOrderDivision.DIVISION_STATE_UNHAPPEN)
            {
                return false;
            }
            updateRecord.DivisionState = (byte)PayOrderDivision.DIVISION_STATE_WAIT_TASK;
            _payOrderRepository.Update(updateRecord);
            return _payOrderRepository.SaveChanges(out int _);
        }

        /// <summary>
        /// 计算支付订单商家入账金额
        /// 商家订单入账金额 （支付金额 - 手续费 - 退款金额 - 总分账金额）</summary>
        /// <param name="dbPayOrder"></param>
        /// <returns></returns>
        public long CalMchIncomeAmount(PayOrderDto dbPayOrder)
        {
            //商家订单入账金额 （支付金额 - 手续费 - 退款金额 - 总分账金额）
            long mchIncomeAmount = dbPayOrder.Amount - dbPayOrder.MchFeeAmount - dbPayOrder.RefundAmount;

            //减去已分账金额
            mchIncomeAmount -= _payOrderDivisionRecordRepository.SumSuccessDivisionAmount(dbPayOrder.PayOrderId);

            return mchIncomeAmount <= 0 ? 0 : mchIncomeAmount;
        }

        /// <summary>
        /// 支付方式统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <param name="state"></param>
        /// <param name="refundState"></param>
        /// <param name="dayStart"></param>
        /// <param name="dayEnd"></param>
        /// <returns></returns>
        private List<PayTypeCountDto> PayTypeCount(string mchNo, string agentNo, byte? state, byte? refundState, DateTime? dayStart, DateTime? dayEnd)
        {
            var result = _payOrderRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(mchNo) || w.MchNo.Equals(mchNo))
                && (string.IsNullOrWhiteSpace(agentNo) || w.AgentNo.Equals(agentNo))
                && (state.Equals(null) || w.State.Equals(state))
                && (refundState.Equals(null) || w.RefundState.Equals(refundState))
                && (dayStart.Equals(null) || w.CreatedAt >= dayStart)
                && (dayEnd.Equals(null) || w.CreatedAt <= dayEnd)).AsEnumerable()
                .GroupBy(g => g.WayType, (key, group) => new { WayType = key, Items = group.AsEnumerable() })
                .Select(s => new PayTypeCountDto
                {
                    WayType = s.WayType,
                    TypeCount = s.Items.Count(),
                    TypeAmount = Decimal.Round((s.Items.Sum(s => s.Amount) - s.Items.Sum(s => s.RefundAmount)) / 100M, 2, MidpointRounding.AwayFromZero)
                }).ToList();
            return result;
        }

        /// <summary>
        /// 成功、退款订单统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <param name="dayStart"></param>
        /// <param name="dayEnd"></param>
        /// <returns></returns>
        private List<(string GroupDate, decimal PayAmount, int PayCount, decimal RefundAmount)> SelectOrderCount(string mchNo, string agentNo, DateTime? dayStart, DateTime? dayEnd)
        {
            var ordercounts = _payOrderRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(mchNo) || w.MchNo.Equals(mchNo))
                && (string.IsNullOrWhiteSpace(agentNo) || w.AgentNo.Equals(agentNo))
                && (new List<byte> { (byte)PayOrderState.STATE_SUCCESS, (byte)PayOrderState.STATE_REFUND }).Contains(w.State)
                && (dayStart.Equals(null) || w.CreatedAt >= dayStart)
                && (dayEnd.Equals(null) || w.CreatedAt <= dayEnd)).AsEnumerable()
                .GroupBy(g => g.CreatedAt.ToString("yyyy-MM-dd"), (key, group) => new { GroupDate = key, Items = group.AsEnumerable() })
                .Select(s => new
                {
                    GroupDate = s.GroupDate,
                    PayAmount = (s.Items.Sum(s => s.Amount) - s.Items.Sum(s => s.RefundAmount)),
                    PayCount = s.Items.Count(),
                    RefundAmount = s.Items.Sum(s => s.RefundAmount)
                }).ToList();
            var result = ordercounts.Select(s =>
            (s.GroupDate, Decimal.Round(s.PayAmount / 100M, 2, MidpointRounding.AwayFromZero), s.PayCount, Decimal.Round(s.RefundAmount / 100M, 2, MidpointRounding.AwayFromZero))
            ).ToList();
            return result;
        }

        /// <summary>
        /// 服务商/代理商/商户统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <returns></returns>
        public JObject MainPageIsvAndMchCount(string mchNo, string agentNo)
        {
            JObject json = new JObject();
            // 商户总数
            var mchInfos = _mchInfoRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(mchNo) || w.MchNo.Equals(mchNo))
                && (string.IsNullOrWhiteSpace(agentNo) || w.AgentNo.Equals(agentNo)));
            int isvSubMchCount = mchInfos.Where(w => w.Type.Equals(CS.MCH_TYPE_ISVSUB)).Count();
            int normalMchCount = mchInfos.Where(w => w.Type.Equals(CS.MCH_TYPE_NORMAL)).Count();
            int mchCount = mchInfos.Count();

            int agentCount = 0;

            if (string.IsNullOrWhiteSpace(agentNo))
            {
                // 代理商总数
                var agentInfos = _agentInfoRepository.GetAll()
                    .Where(w => (string.IsNullOrWhiteSpace(agentNo) || w.AgentNo.Equals(agentNo)));
                agentCount = agentInfos.Count();
            }
            else
            {
                var subAgentInfos = GetSons(_agentInfoRepository.GetAll(), agentNo);
                agentCount = subAgentInfos.Count();
            }

            // 服务商总数
            var isvInfos = _isvInfoRepository.GetAll();
            int isvCount = isvInfos.Count();
            if (string.IsNullOrWhiteSpace(mchNo))
            {
#if DEBUG
                // 生成虚拟数据
                isvSubMchCount = isvSubMchCount < 10 ? Random.Shared.Next(0, 500) : isvSubMchCount;
                normalMchCount = normalMchCount < 10 ? Random.Shared.Next(0, 500) : normalMchCount;
                agentCount = agentCount < 10 ? Random.Shared.Next(0, 500) : agentCount;
                agentCount = isvCount < 10 ? Random.Shared.Next(0, 500) : isvCount;
#endif
                mchCount = isvSubMchCount + normalMchCount;
                json.Add("isvSubMchCount", isvSubMchCount);
                json.Add("normalMchCount", normalMchCount);
                json.Add("totalMch", mchCount);
                json.Add("totalAgent", agentCount);
                if (string.IsNullOrWhiteSpace(agentNo))
                {
                    json.Add("totalIsv", isvCount);
                }
            }
            return json;
        }

        /// <summary>
        /// 今日/昨日交易统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <returns></returns>
        public JObject MainPagePayDayCount(string mchNo, string agentNo, DateTime? day)
        {
            DateTime? dayStart = day;
            DateTime? dayEnd = day?.AddDays(1).AddSeconds(-1);
            JObject json = new JObject();
            int allCount = 0;
            var payorders = _payOrderRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(mchNo) || w.MchNo.Equals(mchNo))
                && (string.IsNullOrWhiteSpace(agentNo) || w.AgentNo.Equals(agentNo))
                //&& w.State.Equals((byte)PayOrderState.STATE_SUCCESS)
                && (dayStart.Equals(null) || w.CreatedAt >= dayStart)
                && (dayEnd.Equals(null) || w.CreatedAt <= dayEnd)).AsEnumerable();
            allCount += payorders.Count();
            payorders = payorders.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS));
            var payAmount = payorders.Sum(s => s.Amount);
            var payCount = payorders.Count();

            var refundOrder = _refundOrderRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(mchNo) || w.MchNo.Equals(mchNo))
                && (string.IsNullOrWhiteSpace(agentNo) || w.AgentNo.Equals(agentNo))
                //&& w.State.Equals((byte)RefundOrderState.STATE_SUCCESS)
                && (dayStart.Equals(null) || w.CreatedAt >= dayStart)
                && (dayEnd.Equals(null) || w.CreatedAt <= dayEnd)).AsEnumerable();
            allCount += refundOrder.Count();
            refundOrder = refundOrder.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS)).AsEnumerable();
            var refundAmount = refundOrder.Sum(s => s.RefundAmount);
            var refundCount = refundOrder.Count();

#if DEBUG
            // 生成虚拟数据
            payAmount = payAmount <= 0 ? Random.Shared.Next(0, 1000000) : payAmount;
            payCount = payCount <= 0 ? Random.Shared.Next(0, 1000) : payCount;
            refundAmount = refundAmount <= 0 ? Random.Shared.Next(0, 500000) : refundAmount;
            refundCount = refundCount <= 0 ? Random.Shared.Next(0, 500) : refundCount;
#endif
            allCount = payCount + refundCount;
            json.Add("dayCount", JObject.FromObject(new
            {
                allCount = allCount,
                payAmount = Decimal.Round(payAmount / 100M, 2, MidpointRounding.AwayFromZero),
                payCount = payCount,
                refundAmount = Decimal.Round(refundAmount / 100M, 2, MidpointRounding.AwayFromZero),
                refundCount = refundCount,
            }));
            return json;
        }

        #region 获取所有下级代理商
        private IEnumerable<AgentInfo> GetSons(IQueryable<AgentInfo> list, string pid)
        {
            var query = list.Where(p => p.AgentNo == pid).ToList();
            var list2 = query.Concat(GetSonList(list, pid));
            return list2;
        }

        private IEnumerable<AgentInfo> GetSonList(IQueryable<AgentInfo> list, string pid)
        {
            var query = list.Where(p => p.Pid == pid).ToList();
            return query.ToList().Concat(query.ToList().SelectMany(t => GetSonList(list, t.AgentNo)));
        }
        #endregion

        /// <summary>
        /// 近期交易统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <param name="recentDay"></param>
        /// <returns></returns>
        public JObject MainPagePayTrendCount(string mchNo, string agentNo, int recentDay)
        {
            // 查询支付的记录
            var dayStart = DateTime.Today.AddDays(-(recentDay - 1));
            var dayEnd = DateTime.Today.AddDays(1).AddSeconds(-1);
            var payOrderList = SelectOrderCount(mchNo, agentNo, dayStart, dayEnd);
            // 生成前端返回参数类型
            List<string> dateList = new List<string>();
            List<string> payAmountList = new List<string>();
            for (DateTime dt = Convert.ToDateTime(dayStart); dt <= Convert.ToDateTime(dayEnd); dt = dt.AddDays(1))
            {
                var item = payOrderList.FirstOrDefault(x => x.GroupDate.Equals(dt.ToString("yyyy-MM-dd")));
                dateList.Add(dt.ToString("MM-dd"));
#if DEBUG
                // 生成虚拟数据
                item.PayAmount = item.PayAmount <= 0 ? Random.Shared.Next(0, 1000000) : item.PayAmount;
#endif
                payAmountList.Add((item.PayAmount / 100M).ToString("0.00"));
            }
            JObject result = new JObject();
            result.Add("dateList", JToken.FromObject(dateList));
            result.Add("payAmountList", JToken.FromObject(payAmountList));
            return result;
        }
        /// <summary>
        /// 首页支付统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <param name="createdStart"></param>
        /// <param name="createdEnd"></param>
        /// <returns></returns>
        public JObject MainPagePayCount(string mchNo, string agentNo, string createdStart, string createdEnd)
        {
            int daySpace = 6; // 默认最近七天（含当天）
            if (!DateTime.TryParse(createdStart, out DateTime dayStart) || !DateTime.TryParse(createdEnd, out DateTime dayEnd))
            {
                DateTime today = DateTime.Today;
                dayStart = today.AddDays(-daySpace);
                dayEnd = today.AddDays(1).AddSeconds(-1);
            }
            else
            {
                // 计算两时间间隔天数
                daySpace = dayEnd.Subtract(dayStart).Days;
            }

            // 查询支付的记录
            var payOrderList = SelectOrderCount(mchNo, agentNo, dayStart, dayEnd);
            // 生成前端返回参数类型
            List<string> resDateArr = new List<string>();
            List<string> resPayAmountArr = new List<string>();
            List<string> resPayCountArr = new List<string>();
            List<string> resRefAmountArr = new List<string>();
            for (DateTime dt = Convert.ToDateTime(dayStart); dt <= Convert.ToDateTime(dayEnd); dt = dt.AddDays(1))
            {
                var item = payOrderList.FirstOrDefault(x => x.GroupDate.Equals(dt.ToString("yyyy-MM-dd")));
#if DEBUG
                // 生成虚拟数据
                item.PayAmount = item.PayAmount <= 0 ? Random.Shared.Next(0, 1000000) : item.PayAmount;
                item.PayCount = item.PayCount <= 0 ? Random.Shared.Next(0, 1000) : item.PayCount;
                item.RefundAmount = item.RefundAmount <= 0 ? Random.Shared.Next(0, 500000) : item.RefundAmount;
#endif
                resDateArr.Add(dt.ToString("yyyy-MM-dd"));
                resPayAmountArr.Add((item.PayAmount / 100M).ToString("0.00"));
                resPayCountArr.Add(item.PayCount.ToString());
                resRefAmountArr.Add((item.RefundAmount / 100M).ToString("0.00"));
            }

            JObject result = new JObject();
            result.Add("resDateArr", JToken.FromObject(resDateArr));
            result.Add("resPayAmountArr", JToken.FromObject(resPayAmountArr));
            result.Add("resPayCountArr", JToken.FromObject(resPayCountArr));
            result.Add("resRefAmountArr", JToken.FromObject(resRefAmountArr));
            return result;
        }

        /// <summary>
        /// 首页支付类型统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <param name="createdStart"></param>
        /// <param name="createdEnd"></param>
        /// <returns></returns>
        public List<PayTypeCountDto> MainPagePayTypeCount(string mchNo, string agentNo, string createdStart, string createdEnd)
        {
            if (!DateTime.TryParse(createdStart, out DateTime dayStart) || !DateTime.TryParse(createdEnd, out DateTime dayEnd))
            {
                DateTime today = DateTime.Today; // 当前日期
                dayStart = today.AddDays(-6); // 一周前日期
                dayEnd = today.AddDays(1).AddSeconds(-1);
            }
            // 统计列表
            var payCountMap = PayTypeCount(mchNo, agentNo, (byte)PayOrderState.STATE_SUCCESS, null, dayStart, dayEnd);

            // 支付方式名称标注
            foreach (var payCount in payCountMap)
            {
                payCount.TypeName = payCount.WayType.ToEnum<PayWayType>().GetDescriptionOrDefault("未知");
            }

#if DEBUG
            // 生成虚拟数据
            if (payCountMap?.Count <= 0)
            {
                // 得到所有支付方式
                var payWayList = _payWayRepository.GetAll();
                payCountMap = new List<PayTypeCountDto>();
                foreach (var wayType in payWayList.Select(s => s.WayType).Distinct())
                {
                    payCountMap.Add(new PayTypeCountDto()
                    {
                        WayType = wayType,
                        TypeName = wayType.ToEnum<PayWayType>().GetDescriptionOrDefault("未知"),
                        TypeCount = Random.Shared.Next(0, 100),
                        TypeAmount = Decimal.Round(Random.Shared.Next(10000, 100000) / 100M, 2, MidpointRounding.AwayFromZero),
                    });
                }
            }
#endif

            // 返回数据列
            return payCountMap.OrderBy(o => (int)Enum.Parse(typeof(PayWayType), o.WayType)).ToList();
        }
    }
}
