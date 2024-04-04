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
    public class QrCodeShellService : AgPayService<QrCodeShellDto, QrCodeShell>, IQrCodeShellService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IQrCodeShellRepository _qrCodeShellRepository;

        public QrCodeShellService(IMapper mapper, IMediatorHandler bus,
            IQrCodeShellRepository qrCodeShellRepository)
            : base(mapper, bus, qrCodeShellRepository)
        {
            _qrCodeShellRepository = qrCodeShellRepository;
        }

        public override bool Add(QrCodeShellDto dto)
        {
            var m = _mapper.Map<QrCodeShell>(dto);
            m.CreatedAt = DateTime.Now;
            m.UpdatedAt = DateTime.Now;
            _qrCodeShellRepository.Add(m);
            var result = _qrCodeShellRepository.SaveChanges(out int _);
            dto.Id = m.Id;
            return result;
        }

        public override bool Update(QrCodeShellDto dto)
        {
            var m = _mapper.Map<QrCodeShell>(dto);
            m.UpdatedAt = DateTime.Now;
            _qrCodeShellRepository.Update(m);
            return _qrCodeShellRepository.SaveChanges(out int _);
        }

        public PaginatedList<QrCodeShellDto> GetPaginatedData(QrCodeShellQueryDto dto)
        {
            var QrCodeShells = _qrCodeShellRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.ShellAlias) || w.ShellAlias.Contains(dto.ShellAlias))
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<QrCodeShell>.Create<QrCodeShellDto>(QrCodeShells, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
