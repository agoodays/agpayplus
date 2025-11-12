using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Extensions;
using AutoMapper;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 转账订单表 服务实现类
    /// </summary>
    public class TransferOrderService : AgPayService<TransferOrderDto, TransferOrder>, ITransferOrderService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ITransferOrderRepository _transferOrderRepository;

        public TransferOrderService(IMapper mapper, IMediatorHandler bus,
            ITransferOrderRepository transferOrderRepository)
            : base(mapper, bus, transferOrderRepository)
        {
            _transferOrderRepository = transferOrderRepository;
        }
        public Task<bool> IsExistOrderByMchOrderNoAsync(string mchNo, string mchOrderNo)
        {
            return _transferOrderRepository.IsExistOrderByMchOrderNoAsync(mchNo, mchOrderNo);
        }

        public async Task<TransferOrderDto> QueryMchOrderAsync(string mchNo, string mchOrderNo, string transferId)
        {
            var entity = await _transferOrderRepository.FirstOrDefaultAsNoTrackingAsync(w => w.MchNo.Equals(mchNo)
                && ((!string.IsNullOrEmpty(transferId) && w.TransferId.Equals(transferId)) || (!string.IsNullOrEmpty(mchOrderNo) && w.MchOrderNo.Equals(mchOrderNo))));
            return _mapper.Map<TransferOrderDto>(entity);
        }

        public Task<PaginatedResult<TransferOrderDto>> GetPaginatedDataAsync(TransferOrderQueryDto dto)
        {
            var query = GetTransferOrders(dto).OrderByDescending(o => o.CreatedAt);
            return query.ToPaginatedResultAsync<TransferOrder, TransferOrderDto>(_mapper, dto.PageNumber, dto.PageSize);
        }
        private IQueryable<TransferOrder> GetTransferOrders(TransferOrderQueryDto dto)
        {
            var query = _transferOrderRepository.GetAllAsNoTracking()
                .WhereIfNotEmpty(dto.MchNo, w => w.MchNo.Equals(dto.MchNo))
                .WhereIfNotEmpty(dto.IsvNo, w => w.IsvNo.Equals(dto.IsvNo))
                .WhereIfNotNull(dto.MchType, w => w.MchType.Equals(dto.MchType))
                .WhereIfNotEmpty(dto.TransferId, w => w.TransferId.Equals(dto.TransferId))
                .WhereIfNotEmpty(dto.MchOrderNo, w => w.MchOrderNo.Equals(dto.MchOrderNo))
                .WhereIfNotEmpty(dto.ChannelOrderNo, w => w.ChannelOrderNo.Equals(dto.ChannelOrderNo))
                .WhereIfNotNull(dto.State, w => w.State.Equals(dto.State))
                .WhereIfNotEmpty(dto.AppId, w => w.AppId.Equals(dto.AppId))
                .WhereIfNotEmpty(dto.UnionOrderId, w => w.TransferId.Equals(dto.UnionOrderId) || w.MchOrderNo.Equals(dto.UnionOrderId) || w.ChannelOrderNo.Equals(dto.UnionOrderId))
                .WhereIfNotNull(dto.CreatedStart, w => w.CreatedAt >= dto.CreatedStart)
                .WhereIfNotNull(dto.CreatedEnd, w => w.CreatedAt <= dto.CreatedEnd);
            return query;
        }

        public async Task<JObject> StatisticsAsync(TransferOrderQueryDto dto)
        {
            var baseQuery = GetTransferOrders(dto);

            // 使用单个查询获取所有统计数据
            var statistics = await baseQuery
                .GroupBy(x => 1) // 按常量分组，实现整体聚合
                .Select(g => new
                {
                    AllTransferAmount = g.Sum(x => x.Amount),
                    AllTransferCount = g.Count(),
                    TransferAmount = g.Where(x => x.State == (byte)TransferOrderState.STATE_SUCCESS).Sum(x => x.Amount),
                    TransferCount = g.Count(x => x.State == (byte)TransferOrderState.STATE_SUCCESS)
                })
                .SafeFirstOrDefaultAsync();

            // 处理查询结果为 null 的情况
            statistics ??= new { AllTransferAmount = 0L, AllTransferCount = 0, TransferAmount = 0L, TransferCount = 0 };

            var allTransferAmount = statistics.AllTransferAmount;
            var allTransferCount = statistics.AllTransferCount;
            var transferAmount = statistics.TransferAmount;
            var transferCount = statistics.TransferCount;

            // 安全计算比率，避免除零
            var successRate = allTransferCount > 0
                ? Math.Round(transferCount / (decimal)allTransferCount, 2, MidpointRounding.AwayFromZero)
                : 0M;

            var result = new JObject
            {
                ["allTransferAmount"] = Decimal.Round(allTransferAmount / 100M, 2, MidpointRounding.AwayFromZero),
                ["allTransferCount"] = allTransferCount,
                ["transferAmount"] = Decimal.Round(transferAmount / 100M, 2, MidpointRounding.AwayFromZero),
                ["transferCount"] = transferCount,
                ["transferFeeAmount"] = 0,
                ["successRate"] = successRate  // 更合理的字段名
            };

            return result;
        }

        #region
        //private IQueryable<TransferOrder> GetTransferOrders(TransferOrderQueryDto dto)
        //{
        //    var result = _transferOrderRepository.GetAllAsNoTracking()
        //        .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
        //        && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
        //        && (dto.MchType.Equals(null) || w.MchType.Equals(dto.MchType))
        //        && (string.IsNullOrWhiteSpace(dto.TransferId) || w.TransferId.Equals(dto.TransferId))
        //        && (string.IsNullOrWhiteSpace(dto.MchOrderNo) || w.MchOrderNo.Equals(dto.MchOrderNo))
        //        && (string.IsNullOrWhiteSpace(dto.ChannelOrderNo) || w.ChannelOrderNo.Equals(dto.ChannelOrderNo))
        //        && (dto.State.Equals(null) || w.State.Equals(dto.State))
        //        && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
        //        && (string.IsNullOrWhiteSpace(dto.UnionOrderId) || w.TransferId.Equals(dto.UnionOrderId)
        //        || w.MchOrderNo.Equals(dto.UnionOrderId)|| w.ChannelOrderNo.Equals(dto.UnionOrderId))
        //        && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
        //        && (dto.CreatedEnd.Equals(null) || w.CreatedAt <= dto.CreatedEnd));
        //    return result;
        //}

        //public async Task<JObject> StatisticsAsync(TransferOrderQueryDto dto)
        //{
        //    var transferOrders = GetTransferOrders(dto);
        //    var allTransferAmount = await transferOrders.SumAsync(s => s.Amount);
        //    var allTransferCount = await transferOrders.CountAsync();
        //    var refund = transferOrders.Where(w => w.State.Equals((byte)TransferOrderState.STATE_SUCCESS));
        //    //var transferFeeAmount = await refund.SumAsync(s => s.FeeAmount);
        //    var transferAmount = await refund.SumAsync(s => s.Amount);
        //    var transferCount = await refund.CountAsync();
        //    JObject result = new JObject();
        //    result.Add("allTransferAmount", Decimal.Round(allTransferAmount / 100M, 2, MidpointRounding.AwayFromZero));
        //    result.Add("allTransferCount", allTransferCount);
        //    result.Add("transferAmount", Decimal.Round(transferAmount / 100M, 2, MidpointRounding.AwayFromZero));
        //    result.Add("transferCount", transferCount);
        //    result.Add("transferFeeAmount", 0);
        //    result.Add("round", Math.Round(allTransferAmount > 0 ? transferCount / Convert.ToDecimal(allTransferAmount) : 0M, 2, MidpointRounding.AwayFromZero));
        //    return result;
        //} 
        #endregion

        /// <summary>
        /// 更新转账订单状态 【转账订单生成】 --》 【转账中】
        /// </summary>
        /// <param name="transferId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateInit2IngAsync(string transferId)
        {
            var updateRecord = await _transferOrderRepository.GetByIdAsync(transferId);
            if (updateRecord.State != (byte)TransferOrderState.STATE_INIT)
            {
                return false;
            }
            updateRecord.State = (byte)TransferOrderState.STATE_ING;
            _transferOrderRepository.Update(updateRecord);
            var (result, _) = await _transferOrderRepository.SaveChangesWithResultAsync();
            return result;
        }
        /// <summary>
        /// 更新转账订单状态 【转账中】 --》 【转账成功】
        /// </summary>
        /// <param name="transferId"></param>
        /// <param name="channelOrderNo"></param>
        /// <returns></returns>
        public async Task<bool> UpdateIng2SuccessAsync(string transferId, string channelOrderNo)
        {
            var updateRecord = await _transferOrderRepository.GetByIdAsync(transferId);
            if (updateRecord.State != (byte)TransferOrderState.STATE_ING)
            {
                return false;
            }
            updateRecord.State = (byte)TransferOrderState.STATE_SUCCESS;
            updateRecord.ChannelOrderNo = channelOrderNo;
            updateRecord.SuccessTime = DateTime.Now;
            _transferOrderRepository.Update(updateRecord);
            var (result, _) = await _transferOrderRepository.SaveChangesWithResultAsync();
            return result;
        }
        /// <summary>
        /// 更新转账订单状态 【转账中】 --》 【转账失败】
        /// </summary>
        /// <param name="transferId"></param>
        /// <param name="channelOrderNo"></param>
        /// <param name="channelErrCode"></param>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        public async Task<bool> UpdateIng2FailAsync(string transferId, string channelOrderNo, string channelErrCode, string channelErrMsg)
        {
            var updateRecord = await _transferOrderRepository.GetByIdAsync(transferId);
            if (updateRecord.State != (byte)TransferOrderState.STATE_ING)
            {
                return false;
            }
            updateRecord.State = (byte)TransferOrderState.STATE_FAIL;
            updateRecord.ErrCode = channelErrCode;
            updateRecord.ErrMsg = channelErrMsg;
            updateRecord.ChannelOrderNo = channelOrderNo;
            _transferOrderRepository.Update(updateRecord);
            var (result, _) = await _transferOrderRepository.SaveChangesWithResultAsync();
            return result;
        }
        /// <summary>
        /// 更新转账订单状态 【转账中】 --》 【转账成功/转账失败】
        /// </summary>
        /// <param name="transferId"></param>
        /// <param name="state"></param>
        /// <param name="channelOrderId"></param>
        /// <param name="channelErrCode"></param>
        /// <param name="channelErrMsg"></param>
        /// <returns></returns>
        public Task<bool> UpdateIng2SuccessOrFailAsync(string transferId, byte updateState, string channelOrderNo, string channelErrCode, string channelErrMsg)
        {
            if (updateState == (byte)TransferOrderState.STATE_ING)
            {
                return Task.FromResult(true);
            }
            else if (updateState == (byte)TransferOrderState.STATE_SUCCESS)
            {
                return UpdateIng2SuccessAsync(transferId, channelOrderNo);
            }
            else if (updateState == (byte)TransferOrderState.STATE_FAIL)
            {
                return UpdateIng2FailAsync(transferId, channelOrderNo, channelErrCode, channelErrMsg);
            }
            return Task.FromResult(false);
        }
    }
}
