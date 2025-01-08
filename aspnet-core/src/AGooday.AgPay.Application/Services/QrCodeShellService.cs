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

        public override async Task<bool> AddAsync(QrCodeShellDto dto)
        {
            var entity = _mapper.Map<QrCodeShell>(dto);
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;
            await _qrCodeShellRepository.AddAsync(entity);
            var (result, _) = await _qrCodeShellRepository.SaveChangesWithResultAsync();
            dto.Id = entity.Id;
            return result;
        }

        public override async Task<bool> UpdateAsync(QrCodeShellDto dto)
        {
            var entity = _mapper.Map<QrCodeShell>(dto);
            entity.UpdatedAt = DateTime.Now;
            _qrCodeShellRepository.Update(entity);
            var (result, _) = await _qrCodeShellRepository.SaveChangesWithResultAsync();
            return result;
        }

        public async Task<PaginatedList<QrCodeShellDto>> GetPaginatedDataAsync(QrCodeShellQueryDto dto)
        {
            var query = _qrCodeShellRepository.GetAllAsNoTracking()
                .Where(w => string.IsNullOrWhiteSpace(dto.ShellAlias) || w.ShellAlias.Contains(dto.ShellAlias))
                .OrderByDescending(o => o.CreatedAt);
            var records = await PaginatedList<QrCodeShell>.CreateAsync<QrCodeShellDto>(query, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
