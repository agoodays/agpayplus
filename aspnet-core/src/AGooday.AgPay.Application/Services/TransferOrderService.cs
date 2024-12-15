using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
            var entity = await _transferOrderRepository.GetAllAsNoTracking()
                .Where(w => w.MchNo.Equals(mchNo)
                && ((!string.IsNullOrEmpty(transferId) && w.TransferId.Equals(transferId))
                || (!string.IsNullOrEmpty(mchOrderNo) && w.MchOrderNo.Equals(mchOrderNo))))
                .FirstOrDefaultAsync();
            return _mapper.Map<TransferOrderDto>(entity);
        }

        public Task<PaginatedList<TransferOrderDto>> GetPaginatedDataAsync(TransferOrderQueryDto dto)
        {
            var query = GetTransferOrders(dto).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<TransferOrder>.CreateAsync<TransferOrderDto>(query, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }

        private IQueryable<TransferOrder> GetTransferOrders(TransferOrderQueryDto dto)
        {
            var result = _transferOrderRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (dto.MchType.Equals(null) || w.MchType.Equals(dto.MchType))
                && (string.IsNullOrWhiteSpace(dto.TransferId) || w.TransferId.Equals(dto.TransferId))
                && (string.IsNullOrWhiteSpace(dto.MchOrderNo) || w.MchOrderNo.Equals(dto.MchOrderNo))
                && (string.IsNullOrWhiteSpace(dto.ChannelOrderNo) || w.ChannelOrderNo.Equals(dto.ChannelOrderNo))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (string.IsNullOrWhiteSpace(dto.UnionOrderId) || w.TransferId.Equals(dto.UnionOrderId)
                || w.MchOrderNo.Equals(dto.UnionOrderId) || w.MchOrderNo.Equals(dto.UnionOrderId) || w.ChannelOrderNo.Equals(dto.UnionOrderId))
                && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
                && (dto.CreatedEnd.Equals(null) || w.CreatedAt <= dto.CreatedEnd));
            return result;
        }

        public async Task<JObject> StatisticsAsync(TransferOrderQueryDto dto)
        {
            var transferOrders = GetTransferOrders(dto);
            var allTransferAmount = await transferOrders.SumAsync(s => s.Amount);
            var allTransferCount = await transferOrders.CountAsync();
            var refund = transferOrders.Where(w => w.State.Equals((byte)TransferOrderState.STATE_SUCCESS));
            //var transferFeeAmount = await refund.SumAsync(s => s.FeeAmount);
            var transferAmount = await refund.SumAsync(s => s.Amount);
            var transferCount = await refund.CountAsync();
            JObject result = new JObject();
            result.Add("allTransferAmount", Decimal.Round(allTransferAmount / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("allTransferCount", allTransferCount);
            result.Add("transferAmount", Decimal.Round(transferAmount / 100M, 2, MidpointRounding.AwayFromZero));
            result.Add("transferCount", transferCount);
            result.Add("transferFeeAmount", 0);
            result.Add("round", Math.Round(allTransferAmount > 0 ? transferCount / Convert.ToDecimal(allTransferAmount) : 0M, 2, MidpointRounding.AwayFromZero));
            return result;
        }

        /// <summary>
        /// 更新转账订单状态 【转账订单生成】 --》 【转账中】
        /// </summary>
        /// <param name="transferId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateInit2IngAsync(string transferId)
        {
            var updateRecord = _transferOrderRepository.GetById(transferId);
            if (updateRecord.State != (byte)TransferOrderState.STATE_INIT)
            {
                return false;
            }
            updateRecord.State = (byte)TransferOrderState.STATE_ING;
            _transferOrderRepository.Update(updateRecord);
            return await _transferOrderRepository.SaveChangesAsync() > 0;
        }
        /// <summary>
        /// 更新转账订单状态 【转账中】 --》 【转账成功】
        /// </summary>
        /// <param name="transferId"></param>
        /// <param name="channelOrderNo"></param>
        /// <returns></returns>
        public async Task<bool> UpdateIng2SuccessAsync(string transferId, string channelOrderNo)
        {
            var updateRecord = _transferOrderRepository.GetById(transferId);
            if (updateRecord.State != (byte)TransferOrderState.STATE_ING)
            {
                return false;
            }
            updateRecord.State = (byte)TransferOrderState.STATE_SUCCESS;
            updateRecord.ChannelOrderNo = channelOrderNo;
            updateRecord.SuccessTime = DateTime.Now;
            _transferOrderRepository.Update(updateRecord);
            return await _transferOrderRepository.SaveChangesAsync() > 0;
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
            var updateRecord = _transferOrderRepository.GetById(transferId);
            if (updateRecord.State != (byte)TransferOrderState.STATE_ING)
            {
                return false;
            }
            updateRecord.State = (byte)TransferOrderState.STATE_FAIL;
            updateRecord.ErrCode = channelErrCode;
            updateRecord.ErrMsg = channelErrMsg;
            updateRecord.ChannelOrderNo = channelOrderNo;
            _transferOrderRepository.Update(updateRecord);
            return await _transferOrderRepository.SaveChangesAsync() > 0;
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
