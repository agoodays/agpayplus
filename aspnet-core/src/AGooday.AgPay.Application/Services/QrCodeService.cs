using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;

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

        public override bool Add(QrCodeDto dto)
        {
            var entity = _mapper.Map<QrCode>(dto);
            entity.State = CS.YES;
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;
            _qrCodeRepository.Add(entity);
            return _qrCodeRepository.SaveChanges(out int _);
        }

        public override bool Update(QrCodeDto dto)
        {
            var entity = _mapper.Map<QrCode>(dto);
            entity.UpdatedAt = DateTime.Now;
            _qrCodeRepository.Update(entity);
            return _qrCodeRepository.SaveChanges(out int _);
        }

        public QrCodeDto GetByIdAsNoTracking(string recordId)
        {
            var entity = _qrCodeRepository.GetByIdAsNoTracking(recordId);
            var dto = _mapper.Map<QrCodeDto>(entity);
            return dto;
        }

        public string BatchIdDistinctCount()
        {
            var batchIdPrefix = DateTime.Now.ToString("yyyyMMdd");
            var qrCodes = _qrCodeRepository.GetAllAsNoTracking()
                .Where(w => (w.BatchId ?? string.Empty).StartsWith(batchIdPrefix))
                .OrderByDescending(o => o.BatchId).FirstOrDefault();
            return $"{Convert.ToInt64(qrCodes?.BatchId ?? $"{batchIdPrefix}00") + 1}";
        }

        public bool BatchAdd(QrCodeAddDto dto)
        {
            if (_qrCodeRepository.IsExistBatchId(dto.BatchId))
            {
                throw new BizException("批次号已存在，请重新填写");
            }
            for (int i = 1; i <= dto.AddNum; i++)
            {
                var entity = _mapper.Map<QrCode>(dto);
                entity.QrcId = $"{dto.BatchId}{i:D4}";
                _qrCodeRepository.Add(entity);
            }
            return _qrCodeRepository.SaveChanges(out int _);
        }

        public bool UnBind(string recordId)
        {
            var entity = _qrCodeRepository.GetById(recordId);
            entity.BindState = CS.NO;
            entity.MchNo = null;
            entity.AppId = null;
            entity.StoreId = null;
            _qrCodeRepository.Update(entity);
            return _qrCodeRepository.SaveChanges(out int _);
        }

        public PaginatedList<QrCodeDto> GetPaginatedData(QrCodeQueryDto dto)
        {
            var QrCodes = _qrCodeRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.QrcId) || w.QrcId.Equals(dto.QrcId))
                && (string.IsNullOrWhiteSpace(dto.BatchId) || w.MchNo.Equals(dto.BatchId))
                && (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
                && (dto.CreatedEnd.Equals(null) || w.CreatedAt <= dto.CreatedEnd)
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<QrCode>.Create<QrCodeDto>(QrCodes, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
