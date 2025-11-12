using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Extensions;
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

        public Task<PaginatedResult<QrCodeShellDto>> GetPaginatedDataAsync(QrCodeShellQueryDto dto)
        {
            var query = _qrCodeShellRepository.GetAllAsNoTracking()
                .WhereIfNotEmpty(dto.ShellAlias, w => w.ShellAlias.Contains(dto.ShellAlias))
                .OrderByDescending(o => o.CreatedAt);
            return query.ToPaginatedResultAsync<QrCodeShell, QrCodeShellDto>(_mapper, dto.PageNumber, dto.PageSize);
        }
    }
}
