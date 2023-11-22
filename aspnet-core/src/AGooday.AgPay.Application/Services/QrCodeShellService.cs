using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 码牌模板信息表 服务实现类
    /// </summary>
    public class QrCodeShellService : IQrCodeShellService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IQrCodeShellRepository _qrCodeShellRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public QrCodeShellService(IQrCodeShellRepository qrCodeShellRepository, IMapper mapper, IMediatorHandler bus)
        {
            _qrCodeShellRepository = qrCodeShellRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool Add(QrCodeShellDto dto)
        {
            var m = _mapper.Map<QrCodeShell>(dto);
            _qrCodeShellRepository.Add(m);
            return _qrCodeShellRepository.SaveChanges(out int _);
        }

        public bool Remove(long recordId)
        {
            _qrCodeShellRepository.Remove(recordId);
            return _qrCodeShellRepository.SaveChanges(out int _);
        }

        public bool Update(QrCodeShellDto dto)
        {
            var m = _mapper.Map<QrCodeShell>(dto);
            _qrCodeShellRepository.Update(m);
            return _qrCodeShellRepository.SaveChanges(out int _);
        }

        public QrCodeShellDto GetById(long recordId)
        {
            var entity = _qrCodeShellRepository.GetById(recordId);
            var dto = _mapper.Map<QrCodeShellDto>(entity);
            return dto;
        }

        public IEnumerable<QrCodeShellDto> GetAll()
        {
            var QrCodeShells = _qrCodeShellRepository.GetAll();
            return _mapper.Map<IEnumerable<QrCodeShellDto>>(QrCodeShells);
        }

        public PaginatedList<T> GetPaginatedData<T>(QrCodeShellQueryDto dto)
        {
            var QrCodeShells = _qrCodeShellRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.ShellAlias) || w.ShellAlias.Contains(dto.ShellAlias))
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<QrCodeShell>.Create<T>(QrCodeShells, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
