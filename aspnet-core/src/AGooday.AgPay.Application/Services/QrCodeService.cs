using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 码牌信息表 服务实现类
    /// </summary>
    public class QrCodeService : AgPayService<QrCodeDto, QrCode>, IQrCodeService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IQrCodeRepository _qrCodeRepository;

        public QrCodeService(IMapper mapper, IMediatorHandler bus,
            IQrCodeRepository qrCodeRepository)
            : base(mapper, bus, qrCodeRepository)
        {
            _qrCodeRepository = qrCodeRepository;
        }

        public override async Task<bool> AddAsync(QrCodeDto dto)
        {
            var entity = _mapper.Map<QrCode>(dto);
            entity.State = CS.YES;
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;
            await _qrCodeRepository.AddAsync(entity);
            var (result, _) = await _qrCodeRepository.SaveChangesWithResultAsync();
            return result;
        }

        public override async Task<bool> UpdateAsync(QrCodeDto dto)
        {
            var entity = _mapper.Map<QrCode>(dto);
            entity.UpdatedAt = DateTime.Now;
            _qrCodeRepository.Update(entity);
            var (result, _) = await _qrCodeRepository.SaveChangesWithResultAsync();
            return result;
        }

        public async Task<QrCodeDto> GetByIdAsNoTrackingAsync(string recordId)
        {
            var entity = await _qrCodeRepository.GetByIdAsNoTrackingAsync(recordId);
            var dto = _mapper.Map<QrCodeDto>(entity);
            return dto;
        }

        public async Task<string> BatchIdDistinctCountAsync()
        {
            var batchIdPrefix = DateTime.Now.ToString("yyyyMMdd");
            var qrCodes = await _qrCodeRepository.GetAllAsNoTracking()
                .Where(w => (w.BatchId ?? string.Empty).StartsWith(batchIdPrefix))
                .OrderByDescending(o => o.BatchId).FirstOrDefaultAsync();
            return $"{Convert.ToInt64(qrCodes?.BatchId ?? $"{batchIdPrefix}00") + 1}";
        }

        public async Task<bool> BatchAddAsync(QrCodeAddDto dto)
        {
            if (await _qrCodeRepository.IsExistBatchIdAsync(dto.BatchId))
            {
                throw new BizException("批次号已存在，请重新填写");
            }

            // 使用 Enumerable.Range 和 Select 创建所有实体
            var entities = Enumerable.Range(1, dto.AddNum)
                .Select(i =>
                {
                    var entity = _mapper.Map<QrCode>(dto);
                    entity.QrcId = $"{dto.BatchId}{i:D4}";
                    return entity;
                }).AsQueryable();

            await _qrCodeRepository.AddRangeAsync(entities);
            var (result, _) = await _qrCodeRepository.SaveChangesWithResultAsync();
            return result;
        }

        public async Task<bool> UnBindAsync(string recordId)
        {
            var entity = await _qrCodeRepository.GetByIdAsync(recordId);
            entity.BindState = CS.NO;
            entity.MchNo = null;
            entity.AppId = null;
            entity.StoreId = null;
            _qrCodeRepository.Update(entity);
            var (result, _) = await _qrCodeRepository.SaveChangesWithResultAsync();
            return result;
        }

        public Task<PaginatedList<QrCodeDto>> GetPaginatedDataAsync(QrCodeQueryDto dto)
        {
            var query = _qrCodeRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.QrcId) || w.QrcId.Equals(dto.QrcId))
                && (string.IsNullOrWhiteSpace(dto.BatchId) || w.MchNo.Equals(dto.BatchId))
                && (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
                && (dto.CreatedEnd.Equals(null) || w.CreatedAt <= dto.CreatedEnd))
                .OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<QrCode>.CreateAsync<QrCodeDto>(query, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
