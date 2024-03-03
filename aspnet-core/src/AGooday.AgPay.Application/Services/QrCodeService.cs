using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
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
    public class QrCodeService : IQrCodeService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IQrCodeRepository _qrCodeShellRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public QrCodeService(IQrCodeRepository qrCodeShellRepository, IMapper mapper, IMediatorHandler bus)
        {
            _qrCodeShellRepository = qrCodeShellRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public string BatchIdDistinctCount()
        {
            var BatchIdPrefix = DateTime.Now.ToString("yyyyMMdd");
            var qrCodes = _qrCodeShellRepository.GetAll()
                .Where(w => (w.BatchId ?? "").StartsWith(BatchIdPrefix))
                .OrderByDescending(o => o.BatchId).FirstOrDefault();
            return $"{Convert.ToInt64(qrCodes?.BatchId ?? $"{BatchIdPrefix}00") + 1}";
        }

        public bool BatchAdd(QrCodeAddDto dto)
        {
            for (int i = 1; i <= dto.AddNum; i++)
            {
                var m = _mapper.Map<QrCode>(dto);
                m.QrcId = $"{dto.BatchId}{i:D4}";
                m.QrUrl = $"https://www.example.com/{m.QrcId}";
                _qrCodeShellRepository.Add(m);
            }
            return _qrCodeShellRepository.SaveChanges(out int _);
        }

        public bool Add(QrCodeDto dto)
        {
            var m = _mapper.Map<QrCode>(dto);
            m.State = CS.YES;
            m.CreatedAt = DateTime.Now;
            m.UpdatedAt = DateTime.Now;
            _qrCodeShellRepository.Add(m);
            return _qrCodeShellRepository.SaveChanges(out int _);
        }

        public bool Remove(string recordId)
        {
            _qrCodeShellRepository.Remove(recordId);
            return _qrCodeShellRepository.SaveChanges(out int _);
        }

        public bool Update(QrCodeDto dto)
        {
            var m = _mapper.Map<QrCode>(dto);
            m.UpdatedAt = DateTime.Now;
            _qrCodeShellRepository.Update(m);
            return _qrCodeShellRepository.SaveChanges(out int _);
        }

        public QrCodeDto GetById(string recordId)
        {
            var entity = _qrCodeShellRepository.GetById(recordId);
            var dto = _mapper.Map<QrCodeDto>(entity);
            return dto;
        }

        public IEnumerable<QrCodeDto> GetAll()
        {
            var qrCodes = _qrCodeShellRepository.GetAll();
            return _mapper.Map<IEnumerable<QrCodeDto>>(qrCodes);
        }

        public PaginatedList<T> GetPaginatedData<T>(QrCodeQueryDto dto)
        {
            var QrCodes = _qrCodeShellRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.QrcId) || w.QrcId.Equals(dto.QrcId))
                && (string.IsNullOrWhiteSpace(dto.BatchId) || w.MchNo.Equals(dto.BatchId))
                && (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
                && (dto.CreatedEnd.Equals(null) || w.CreatedAt <= dto.CreatedEnd)
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<QrCode>.Create<T>(QrCodes, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
