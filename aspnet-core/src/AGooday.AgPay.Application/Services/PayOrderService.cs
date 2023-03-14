using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Data;

namespace AGooday.AgPay.Application.Services
{
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
            var entity = _payOrderRepository.GetAll().Where(w => w.MchNo.Equals(mchNo) && (
            w.PayOrderId.Equals(payOrderId) || w.MchOrderNo.Equals(mchOrderNo)
            )).FirstOrDefault();
            return _mapper.Map<PayOrderDto>(entity);
        }

        /// <summary>
        /// 通用列表查询条件
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public PaginatedList<PayOrderDto> GetPaginatedData(PayOrderQueryDto dto)
        {
            var mchInfos = _payOrderRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (dto.MchType.Equals(0) || w.MchType.Equals(dto.MchType))
                && (string.IsNullOrWhiteSpace(dto.WayCode) || w.WayCode.Equals(dto.WayCode))
                && (string.IsNullOrWhiteSpace(dto.MchOrderNo) || w.MchOrderNo.Equals(dto.MchOrderNo))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (dto.DivisionState.Equals(null) || w.DivisionState.Equals(dto.DivisionState))
                && (string.IsNullOrWhiteSpace(dto.UnionOrderId) || w.PayOrderId.Equals(dto.UnionOrderId) || w.MchOrderNo.Equals(dto.UnionOrderId) || w.ChannelOrderNo.Equals(dto.UnionOrderId))
                && (dto.CreatedEnd == null || w.CreatedAt < dto.CreatedEnd)
                && (dto.CreatedStart == null || w.CreatedAt >= dto.CreatedStart)
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<PayOrder>.Create<PayOrderDto>(mchInfos.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
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
            updateRecord.MchFeeRate = payOrder.MchFeeRate;
            updateRecord.MchFeeAmount = payOrder.MchFeeAmount;
            updateRecord.ChannelUser = payOrder.ChannelUser;
            _payOrderRepository.Update(updateRecord);
            return _payOrderRepository.SaveChanges(out int _);
        }
        /// <summary>
        /// 更新订单状态  【支付中】 --》 【支付成功】
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <param name="channelOrderNo"></param>
        /// <param name="channelUserId"></param>
        /// <returns></returns>
        public bool UpdateIng2Success(string payOrderId, string channelOrderNo, string channelUserId)
        {
            var updateRecord = _payOrderRepository.GetById(payOrderId);
            if (updateRecord.State != (byte)PayOrderState.STATE_ING)
            {
                return false;
            }
            updateRecord.State = (byte)PayOrderState.STATE_SUCCESS;
            updateRecord.ChannelOrderNo = channelOrderNo;
            updateRecord.ChannelUser = channelUserId;
            updateRecord.SuccessTime = DateTime.Now;
            _payOrderRepository.Update(updateRecord);
            return _payOrderRepository.SaveChanges(out int _);
        }
        /// <summary>
        /// 更新订单状态  【支付中】 --》 【支付成功】
        /// </summary>
        /// <param name="payOrderId"></param>
        /// <param name="channelOrderNo"></param>
        /// <param name="channelUserId"></param>
        /// <param name="channelErrCode"></param>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        public bool UpdateIng2Fail(string payOrderId, string channelOrderNo, string channelUserId, string channelErrCode, string channelErrMsg)
        {
            var updateRecord = _payOrderRepository.GetById(payOrderId);
            if (updateRecord.State != (byte)PayOrderState.STATE_ING)
            {
                return false;
            }
            updateRecord.State = (byte)PayOrderState.STATE_FAIL;
            updateRecord.ErrCode = channelErrCode;
            updateRecord.ErrMsg = channelErrMsg;
            updateRecord.ChannelOrderNo = channelOrderNo;
            updateRecord.ChannelUser = channelUserId;
            _payOrderRepository.Update(updateRecord);
            return _payOrderRepository.SaveChanges(out int _);
        }
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
        public bool UpdateIng2SuccessOrFail(string payOrderId, byte updateState, string channelOrderNo, string channelUserId, string channelErrCode, string channelErrMsg)
        {
            if (updateState == (byte)PayOrderState.STATE_ING)
            {
                return true;
            }
            else if (updateState == (byte)PayOrderState.STATE_SUCCESS)
            {
                return UpdateIng2Success(payOrderId, channelOrderNo, channelUserId);
            }
            else if (updateState == (byte)PayOrderState.STATE_FAIL)
            {
                return UpdateIng2Fail(payOrderId, channelOrderNo, channelUserId, channelErrCode, channelErrMsg);
            }
            return false;
        }
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
        public bool UpdateNotifySent(string orderId)
        {
            var updateRecord = _payOrderRepository.GetById(orderId);
            updateRecord.NotifyState = CS.YES;
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
        /// 交易统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <param name="state"></param>
        /// <param name="refundState"></param>
        /// <param name="dayStart"></param>
        /// <param name="dayEnd"></param>
        /// <returns></returns>
        public (decimal PayAmount, int PayCount) PayCount(string mchNo, string agentNo, byte? state, byte? refundState, DateTime? dayStart, DateTime? dayEnd)
        {
            var payorders = _payOrderRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(mchNo) || w.MchNo.Equals(mchNo))
                && (string.IsNullOrWhiteSpace(agentNo) || w.AgentNo.Equals(agentNo))
                && (state.Equals(null) || w.State.Equals(state))
                && (refundState.Equals(null) || w.RefundState.Equals(refundState))
                && (dayEnd.Equals(null) || w.CreatedAt < dayEnd)
                && (dayStart.Equals(null) || w.CreatedAt >= dayStart)).AsEnumerable();
            var amount = payorders.Sum(s => s.Amount);
            var refundAmount = payorders.Sum(s => s.RefundAmount);
            var payCount = payorders.Count();
            return (Decimal.Round((amount - refundAmount) / 100M, 2, MidpointRounding.AwayFromZero), payCount);
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
        public List<PayTypeCountDto> PayTypeCount(string mchNo, string agentNo, byte? state, byte? refundState, DateTime? dayStart, DateTime? dayEnd)
        {
            var result = _payOrderRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(mchNo) || w.MchNo.Equals(mchNo))
                && (string.IsNullOrWhiteSpace(agentNo) || w.AgentNo.Equals(agentNo))
                && (state.Equals(null) || w.State.Equals(state))
                && (refundState.Equals(null) || w.RefundState.Equals(refundState))
                && (dayEnd.Equals(null) || w.CreatedAt < dayEnd)
                && (dayStart.Equals(null) || w.CreatedAt >= dayStart)).AsEnumerable()
                .GroupBy(g => g.WayCode, (key, group) => new { WayCode = key, Items = group.AsEnumerable() })
                .Select(s => new PayTypeCountDto
                {
                    WayCode = s.WayCode,
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
        public List<(string GroupDate, decimal PayAmount, decimal RefundAmount)> SelectOrderCount(string mchNo, string agentNo, DateTime? dayStart, DateTime? dayEnd)
        {
            var ordercounts = _payOrderRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(mchNo) || w.MchNo.Equals(mchNo))
                && (string.IsNullOrWhiteSpace(agentNo) || w.AgentNo.Equals(agentNo))
                && (new List<byte> { 2, 5 }).Contains(w.State)
                && (dayEnd.Equals(null) || w.CreatedAt < dayEnd)
                && (dayStart.Equals(null) || w.CreatedAt >= dayStart)).AsEnumerable()
                .GroupBy(g => g.CreatedAt.ToString("MM-dd"), (key, group) => new { GroupDate = key, Items = group.AsEnumerable() })
                .Select(s => new
                {
                    GroupDate = s.GroupDate,
                    PayAmount = (s.Items.Sum(s => s.Amount) - s.Items.Sum(s => s.RefundAmount)),
                    RefundAmount = s.Items.Sum(s => s.RefundAmount)
                }).ToList();
            var result = ordercounts.Select(s => (s.GroupDate, Decimal.Round(s.PayAmount / 100M, 2, MidpointRounding.AwayFromZero), Decimal.Round(s.RefundAmount / 100M, 2, MidpointRounding.AwayFromZero)))
                .ToList();
            return result;
        }

        /// <summary>
        /// 首页支付周统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <returns></returns>
        public JObject MainPageWeekCount(string mchNo, string agentNo)
        {
            JObject json = new JObject();
            List<decimal> array = new List<decimal>();
            decimal payAmount = 0M; // 当日金额
            decimal payWeek = payAmount; // 周总收益
            decimal todayAmount = 0M; // 今日金额
            int todayPayCount = 0; // 今日交易笔数
            decimal yesterdayAmount = 0M;    // 昨日金额
            DateTime today = DateTime.Today;
            for (int i = 0; i < 7; i++)
            {
                DateTime date = today.AddDays(-i);
                DateTime dayStart = date;
                DateTime dayEnd = date.AddDays(1);
                // 每日交易金额查询
                var dayAmount = PayCount(mchNo, agentNo, (byte)PayOrderState.STATE_SUCCESS, null, dayStart, dayEnd);
                payAmount = dayAmount.PayAmount;
                // 今天
                if (i == 0)
                {
                    todayAmount = dayAmount.PayAmount;
                    todayPayCount = dayAmount.PayCount;
                }
                // 昨天
                if (i == 1)
                {
                    yesterdayAmount = dayAmount.PayAmount;
                }
                payWeek += payAmount;
                array.Add(payAmount);
            }

            json.Add("dataArray", JArray.FromObject(array.OrderByDescending(o => o)));// 倒序排列
            json.Add("todayAmount", todayAmount);
            json.Add("todayPayCount", todayPayCount);
            json.Add("payWeek", payWeek);
            json.Add("yesterdayAmount", yesterdayAmount);
            return json;
        }

        /// <summary>
        /// 首页统计总数量
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <returns></returns>
        public JObject MainPageNumCount(string mchNo, string agentNo)
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
            // 总交易金额
            var payCountMap = PayCount(mchNo, agentNo, (byte)PayOrderState.STATE_SUCCESS, null, null, null);
            if (string.IsNullOrWhiteSpace(mchNo))
            {
                json.Add("isvSubMchCount", isvSubMchCount);
                json.Add("normalMchCount", normalMchCount);
                json.Add("totalMch", mchCount);
                json.Add("totalAgent", agentCount);
                if (string.IsNullOrWhiteSpace(agentNo))
                {
                    json.Add("totalIsv", isvCount);
                }
            }
            json.Add("totalAmount", payCountMap.PayAmount);
            json.Add("totalCount", payCountMap.PayCount);
            return json;
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
                isvSubMchCount = isvSubMchCount < 10 ? Random.Shared.Next(0, 500) : isvSubMchCount;
                normalMchCount = normalMchCount < 10 ? Random.Shared.Next(0, 500) : normalMchCount;
                mchCount = isvSubMchCount + normalMchCount;
                json.Add("isvSubMchCount", isvSubMchCount);
                json.Add("normalMchCount", normalMchCount);
                json.Add("totalMch", mchCount);
                json.Add("totalAgent", agentCount < 10 ? Random.Shared.Next(0, 500) : isvSubMchCount);
                if (string.IsNullOrWhiteSpace(agentNo))
                {
                    json.Add("totalIsv", isvCount < 10 ? Random.Shared.Next(0, 500) : isvCount);
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
            DateTime? dayEnd = day?.AddDays(1);
            JObject json = new JObject();
            int allCount = 0;
            var payorders = _payOrderRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(mchNo) || w.MchNo.Equals(mchNo))
                && (string.IsNullOrWhiteSpace(agentNo) || w.AgentNo.Equals(agentNo))
                //&& w.State.Equals((byte)PayOrderState.STATE_SUCCESS)
                && (dayEnd.Equals(null) || w.CreatedAt < dayEnd)
                && (dayStart.Equals(null) || w.CreatedAt >= dayStart)).AsEnumerable();
            allCount += payorders.Count();
            payorders = payorders.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS)).AsEnumerable();
            var payAmount = payorders.Sum(s => s.Amount);
            var payCount = payorders.Count();

            var refundOrder = _refundOrderRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(mchNo) || w.MchNo.Equals(mchNo))
                && (string.IsNullOrWhiteSpace(agentNo) || w.AgentNo.Equals(agentNo))
                //&& w.State.Equals((byte)RefundOrderState.STATE_SUCCESS)
                && (dayEnd.Equals(null) || w.CreatedAt < dayEnd)
                && (dayStart.Equals(null) || w.CreatedAt >= dayStart)).AsEnumerable();
            allCount += refundOrder.Count();
            refundOrder = refundOrder.Where(w => w.State.Equals((byte)RefundOrderState.STATE_SUCCESS)).AsEnumerable();
            var refundAmount = refundOrder.Sum(s => s.RefundAmount);
            var refundCount = refundOrder.Count();

            // 生成虚拟数据
            payAmount = payAmount <= 0 ? Random.Shared.Next(0, 1000000) : payAmount;
            payCount = payCount <= 0 ? Random.Shared.Next(0, 1000) : payCount;
            refundAmount = refundAmount <= 0 ? Random.Shared.Next(0, 500000) : refundAmount;
            refundCount = refundCount <= 0 ? Random.Shared.Next(0, 500) : refundCount;
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
        /// 首页支付统计
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="agentNo"></param>
        /// <param name="createdStart"></param>
        /// <param name="createdEnd"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> MainPagePayCount(string mchNo, string agentNo, string createdStart, string createdEnd)
        {
            int daySpace = 6; // 默认最近七天（含当天）
            if (DateTime.TryParse(createdStart, out DateTime dayStart) && DateTime.TryParse(createdEnd, out DateTime dayEnd))
            {
                dayStart = dayStart.Date;
                dayEnd = dayEnd.Date.AddDays(1);
                // 计算两时间间隔天数
                daySpace = dayEnd.AddSeconds(-1).Subtract(dayStart).Days;
            }
            else
            {
                DateTime today = DateTime.Today;
                dayStart = today.AddDays(-daySpace);
                dayEnd = today.AddDays(1);
            }

            // 查询收款的记录
            var payOrderList = SelectOrderCount(mchNo, agentNo, dayStart, dayEnd);
            // 查询退款的记录
            var refundOrderList = SelectOrderCount(mchNo, agentNo, dayStart, dayEnd);
            // 生成前端返回参数类型
            var returnList = GetReturnList(daySpace, dayEnd.AddDays(-1), payOrderList, refundOrderList);
            return returnList;
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
            if (DateTime.TryParse(createdStart, out DateTime dayStart) && DateTime.TryParse(createdEnd, out DateTime dayEnd))
            {
                dayStart = dayStart.Date;
                dayEnd = dayEnd.Date.AddDays(1);
            }
            else
            {
                DateTime today = DateTime.Today; // 当前日期
                dayStart = today.AddDays(-6); // 一周前日期
                dayEnd = today.AddDays(1);
            }
            // 统计列表
            var payCountMap = PayTypeCount(mchNo, agentNo, (byte)PayOrderState.STATE_SUCCESS, null, dayStart, dayEnd);

            // 得到所有支付方式
            var payWayList = _payWayRepository.GetAll();

            // 支付方式名称标注
            foreach (var payCount in payCountMap)
            {
                var payWay = payWayList.FirstOrDefault(f => f.WayCode.Equals(payCount.WayCode));
                if (payWay != null)
                {
                    payCount.TypeName = payWay.WayName;
                }
                else
                {
                    payCount.TypeName = payCount.WayCode;
                }
            }

            // 生成虚拟数据
            if (payCountMap?.Count <= 0)
            {
                payCountMap = new List<PayTypeCountDto>();
                foreach (var payWay in payWayList)
                {
                    payCountMap.Add(new PayTypeCountDto()
                    {
                        WayCode = payWay.WayCode,
                        TypeName = payWay.WayName,
                        TypeCount = Random.Shared.Next(0, 100),
                        TypeAmount = Decimal.Round(Random.Shared.Next(10000, 100000) / 100M, 2, MidpointRounding.AwayFromZero),
                    });
                }
            }

            // 返回数据列
            return payCountMap;
        }

        /// <summary>
        /// 生成首页交易统计数据类型
        /// </summary>
        /// <param name="daySpace"></param>
        /// <param name="createdStart"></param>
        /// <param name="payOrderList"></param>
        /// <param name="refundOrderList"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetReturnList(int daySpace, DateTime endDay, List<(string GroupDate, decimal PayAmount, decimal RefundAmount)> payOrderList, List<(string GroupDate, decimal PayAmount, decimal RefundAmount)> refundOrderList)
        {
            List<KeyValuePair<string, string>> dayList = new List<KeyValuePair<string, string>>();
            // 先判断间隔天数 根据天数设置空的list
            for (int i = 0; i <= daySpace; i++)
            {
                KeyValuePair<string, string> map = new KeyValuePair<string, string>("date", endDay.AddDays(-i).ToString("MM-dd"));
                dayList.Add(map);
            }
            // 日期倒序排列
            dayList.OrderBy(s => s.Value);

            List<Dictionary<string, object>> payListMap = new List<Dictionary<string, object>>(); // 收款的列
            List<Dictionary<string, object>> refundListMap = new List<Dictionary<string, object>>(); // 退款的列
            foreach (var dayMap in dayList.OrderBy(s => s.Value))// 日期升序排列
            {
                var date = dayMap.Value;

                // 为收款列和退款列赋值默认参数【payAmount字段切记不可为string，否则前端图表解析不出来】
                Dictionary<string, object> payMap = new Dictionary<string, object>();
                payMap.Add("date", date);
                payMap.Add("type", "收款");
                payMap.Add("payAmount", 0);

                Dictionary<string, object> refundMap = new Dictionary<string, object>();
                refundMap.Add("date", date);
                refundMap.Add("type", "退款");
                refundMap.Add("payAmount", 0);
                foreach (var payOrderMap in payOrderList)
                {
                    if (date.Equals(payOrderMap.GroupDate))
                    {
                        payMap.Remove("payAmount");
                        payMap.TryAdd("payAmount", payOrderMap.PayAmount);
                    }
                }
                payListMap.Add(payMap);
                foreach (var refundOrderMap in refundOrderList)
                {
                    if (date.Equals(refundOrderMap.GroupDate))
                    {
                        refundMap.Remove("payAmount");
                        refundMap.TryAdd("payAmount", refundOrderMap.RefundAmount);
                    }
                }
                refundListMap.Add(refundMap);
            }
            payListMap.AddRange(refundListMap);
            return payListMap;
        }
    }
}
