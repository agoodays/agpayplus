using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
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
            var result = await _sysArticleRepository.SaveChangesAsync() > 0;
            dto.ArticleId = entity.ArticleId;
            return result;
        }

        public override async Task<bool> UpdateAsync(SysArticleDto dto)
        {
            var entity = _mapper.Map<SysArticle>(dto);
            entity.UpdatedAt = DateTime.Now;
            _sysArticleRepository.Update(entity);
            return await _sysArticleRepository.SaveChangesAsync() > 0;
        }

        public async Task<PaginatedList<SysArticleDto>> GetPaginatedDataAsync(SysArticleQueryDto dto, string agentNo = null)
        {
            var query = _sysArticleRepository.GetAllAsNoTracking()
                .Where(w => (dto.ArticleId.Equals(null) || w.ArticleId.Equals(dto.ArticleId))
                && (string.IsNullOrWhiteSpace(dto.Title) || w.Title.Contains(dto.Title) || w.Subtitle.Contains(dto.Title))
                && (dto.ArticleType.Equals(null) || w.ArticleType.Equals(dto.ArticleType))
                && (string.IsNullOrWhiteSpace(dto.ArticleRange) || EF.Functions.JsonContains(w.ArticleRange,
                JArray.FromObject(new string[] { dto.ArticleRange }).ToString(Newtonsoft.Json.Formatting.None)))// w.ArticleRange.Contains(dto.ArticleRange))
                && (dto.CreatedStart.Equals(null) || w.CreatedAt >= dto.CreatedStart)
                && (dto.CreatedEnd.Equals(null) || w.CreatedAt <= dto.CreatedEnd))
                .OrderByDescending(o => o.CreatedAt);
            var records = await PaginatedList<SysArticle>.CreateAsync<SysArticleDto>(query, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
