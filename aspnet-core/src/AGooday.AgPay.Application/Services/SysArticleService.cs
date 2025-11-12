using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Extensions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 公告信息表 服务实现类
    /// </summary>
    public class SysArticleService : AgPayService<SysArticleDto, SysArticle, long>, ISysArticleService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysArticleRepository _sysArticleRepository;

        public SysArticleService(IMapper mapper, IMediatorHandler bus,
            ISysArticleRepository sysArticleRepository)
            : base(mapper, bus, sysArticleRepository)
        {
            _sysArticleRepository = sysArticleRepository;
        }

        public override async Task<bool> AddAsync(SysArticleDto dto)
        {
            var entity = _mapper.Map<SysArticle>(dto);
            await _sysArticleRepository.AddAsync(entity);
            var (result, _) = await _sysArticleRepository.SaveChangesWithResultAsync();
            dto.ArticleId = entity.ArticleId;
            return result;
        }

        public override async Task<bool> UpdateAsync(SysArticleDto dto)
        {
            var entity = _mapper.Map<SysArticle>(dto);
            entity.UpdatedAt = DateTime.Now;
            _sysArticleRepository.Update(entity);
            var (result, _) = await _sysArticleRepository.SaveChangesWithResultAsync();
            dto.ArticleId = entity.ArticleId;
            return result;
        }

        public Task<PaginatedResult<SysArticleDto>> GetPaginatedDataAsync(SysArticleQueryDto dto, string agentNo = null)
        {
            var query = _sysArticleRepository.GetAllAsNoTracking()
                .WhereIfNotNull(dto.ArticleId, w => w.ArticleId.Equals(dto.ArticleId))
                .WhereIfNotEmpty(dto.Title, w => w.Title.Contains(dto.Title) || w.Subtitle.Contains(dto.Title))
                .WhereIfNotNull(dto.ArticleType.Equals(null), w => w.ArticleType.Equals(dto.ArticleType))
                .WhereIfNotEmpty(dto.ArticleRange, w => EF.Functions.JsonContains(w.ArticleRange, JArray.FromObject(new string[] { dto.ArticleRange }).ToString(Newtonsoft.Json.Formatting.None)))// w.ArticleRange.Contains(dto.ArticleRange))
                .WhereIfNotNull(dto.CreatedStart, w => w.CreatedAt >= dto.CreatedStart)
                .WhereIfNotNull(dto.CreatedEnd, w => w.CreatedAt <= dto.CreatedEnd)
                .OrderByDescending(o => o.CreatedAt);
            return query.ToPaginatedResultAsync<SysArticle, SysArticleDto>(_mapper, dto.PageNumber, dto.PageSize);
        }
    }
}
