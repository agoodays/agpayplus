using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Extensions;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 服务商信息表 服务实现类
    /// </summary>
    public class IsvInfoService : AgPayService<IsvInfoDto, IsvInfo>, IIsvInfoService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IIsvInfoRepository _isvInfoRepository;

        public IsvInfoService(IMapper mapper, IMediatorHandler bus,
            IIsvInfoRepository isvInfoRepository)
            : base(mapper, bus, isvInfoRepository)
        {
            _isvInfoRepository = isvInfoRepository;
        }

        public override async Task<bool> AddAsync(IsvInfoDto dto)
        {
            do
            {
                dto.IsvNo = SeqUtil.GenIsvNo();
            } while (await IsExistIsvNoAsync(dto.IsvNo));
            var entity = _mapper.Map<IsvInfo>(dto);
            await _isvInfoRepository.AddAsync(entity);
            var (result, _) = await _isvInfoRepository.SaveChangesWithResultAsync();
            return result;
        }

        public override async Task<bool> UpdateAsync(IsvInfoDto dto)
        {
            var entity = _mapper.Map<IsvInfo>(dto);
            entity.UpdatedAt = DateTime.Now;
            _isvInfoRepository.Update(entity);
            var (result, _) = await _isvInfoRepository.SaveChangesWithResultAsync();
            return result;
        }

        public Task<bool> IsExistIsvNoAsync(string isvNo)
        {
            return _isvInfoRepository.IsExistIsvNoAsync(isvNo);
        }

        public Task<PaginatedResult<IsvInfoDto>> GetPaginatedDataAsync(IsvInfoQueryDto dto)
        {
            var query = _isvInfoRepository.GetAllAsNoTracking()
                .WhereIfNotEmpty(dto.IsvNo, w => w.IsvNo.Equals(dto.IsvNo))
                .WhereIfNotEmpty(dto.IsvName, w => w.IsvName.Contains(dto.IsvName) || w.IsvShortName.Contains(dto.IsvName))
                .WhereIfNotNull(dto.State, w => w.State.Equals(dto.State))
                .OrderByDescending(o => o.CreatedAt);
            return query.ToPaginatedResultAsync<IsvInfo, IsvInfoDto>(_mapper, dto.PageNumber, dto.PageSize);
        }
    }
}
