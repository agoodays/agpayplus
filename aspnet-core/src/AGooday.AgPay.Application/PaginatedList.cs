using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Application
{
    public class PaginatedList<TSource> : List<TSource>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }

        public bool HasNext => PageIndex < TotalPages;

        public PaginatedList(List<TSource> items, int count, int pageIndex, int pageSize)
        {
            TotalCount = count;
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<TSource>> CreateAsync(IQueryable<TSource> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var records = await source.Skip(
                (pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();
            return new PaginatedList<TSource>(records, count, pageIndex, pageSize);
        }

        public static PaginatedList<TSource> Create(IQueryable<TSource> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            if (pageIndex > 0)
            {
                source = source.Skip((pageIndex - 1) * pageSize)
                   .Take(pageSize);
            }
            var records = source.ToList();
            return new PaginatedList<TSource>(records, count, pageIndex, pageSize);
        }

        public static async Task<PaginatedList<TDestination>> CreateAsync<TDestination>(IQueryable<TSource> source, IMapper mapper, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip(
                (pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();
            var records = mapper.Map<List<TDestination>>(items);
            return new PaginatedList<TDestination>(records, count, pageIndex, pageSize);
        }

        public static PaginatedList<TDestination> Create<TDestination>(IQueryable<TSource> source, IMapper mapper, int pageIndex, int pageSize)
        {
            var count = source.Count();
            if (pageIndex > 0 && pageSize > 0)
            {
                source = source.Skip((pageIndex - 1) * pageSize)
                   .Take(pageSize);
            }
            var records = mapper.Map<List<TDestination>>(source.ToList());
            return new PaginatedList<TDestination>(records, count, pageIndex, pageSize);
        }
    }
}
