using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace AGooday.AgPay.Application.Services
{
    public class SysArticleService : ISysArticleService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysArticleRepository _sysArticleRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public SysArticleService(ISysArticleRepository sysArticleRepository, IMapper mapper, IMediatorHandler bus)
        {
            _sysArticleRepository = sysArticleRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool Add(SysArticleDto dto)
        {
            var m = _mapper.Map<SysArticle>(dto);
            _sysArticleRepository.Add(m);
            return _sysArticleRepository.SaveChanges(out int _);
        }

        public bool Remove(long recordId)
        {
            _sysArticleRepository.Remove(recordId);
            return _sysArticleRepository.SaveChanges(out _);
        }

        public bool Update(SysArticleDto dto)
        {
            var renew = _mapper.Map<SysArticle>(dto);
            //var old = _sysArticleRepository.GetById(dto.StoreId);
            renew.UpdatedAt = DateTime.Now;
            _sysArticleRepository.Update(renew);
            return _sysArticleRepository.SaveChanges(out int _);
        }

        public SysArticleDto GetById(long recordId)
        {
            var entity = _sysArticleRepository.GetById(recordId);
            var dto = _mapper.Map<SysArticleDto>(entity);
            return dto;
        }

        public IEnumerable<SysArticleDto> GetAll()
        {
            var sysArticles = _sysArticleRepository.GetAll();
            return _mapper.Map<IEnumerable<SysArticleDto>>(sysArticles);
        }

        public PaginatedList<SysArticleDto> GetPaginatedData(SysArticleQueryDto dto, string agentNo = null)
        {
            var sysLogs = _sysArticleRepository.GetAll()
                .Where(w => (dto.ArticleId.Equals(0) || w.ArticleId.Equals(dto.ArticleId))
                && (string.IsNullOrWhiteSpace(dto.Title) || w.Title.Contains(dto.Title) || w.Subtitle.Contains(dto.Title))
                && (dto.ArticleType.Equals(0) || w.ArticleType.Equals(dto.ArticleType))
                //&& (string.IsNullOrWhiteSpace(dto.ArticleRange) || EF.Functions.JsonContains(w.ArticleRange, new string[] { dto.ArticleRange }))// w.ArticleRange.Contains(dto.ArticleRange))
                && (dto.CreatedStart == null || w.CreatedAt >= dto.CreatedStart)
                && (dto.CreatedEnd == null || w.CreatedAt < dto.CreatedEnd))
                .OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<SysArticle>.Create<SysArticleDto>(sysLogs.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
