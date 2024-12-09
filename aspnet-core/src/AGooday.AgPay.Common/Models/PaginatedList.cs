using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Common.Models
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }

        public bool HasNext => PageIndex < TotalPages;

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            TotalCount = count;
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            if (pageIndex > 0 && pageSize > 0)
            {
                source = source.Skip((pageIndex - 1) * pageSize)
                   .Take(pageSize);
            }
            var records = await source.ToListAsync();
            return new PaginatedList<T>(records, count, pageIndex, pageSize);
        }

        public static PaginatedList<T> Create(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            if (pageIndex > 0 && pageSize > 0)
            {
                source = source.Skip((pageIndex - 1) * pageSize)
                   .Take(pageSize);
            }
            var records = source.ToList();
            return new PaginatedList<T>(records, count, pageIndex, pageSize);
        }

        public static async Task<PaginatedList<TDestination>> CreateAsync<TDestination>(IQueryable<T> source, IMapper mapper, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            if (pageIndex > 0 && pageSize > 0)
            {
                source = source.Skip((pageIndex - 1) * pageSize)
                   .Take(pageSize);
            }
            var items = await source.ToListAsync();
            var records = mapper.Map<List<TDestination>>(items);
            return new PaginatedList<TDestination>(records, count, pageIndex, pageSize);
        }

        public static PaginatedList<TDestination> Create<TDestination>(IQueryable<T> source, IMapper mapper, int pageIndex, int pageSize)
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

        public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            if (pageIndex > 0 && pageSize > 0)
            {
                source = source.Skip((pageIndex - 1) * pageSize)
                   .Take(pageSize);
            }
            return new PaginatedList<T>(source.ToList(), count, pageIndex, pageSize);
        }

        public static async Task<PaginatedList<TDestination>> CreateAsync<TSource, TDestination>(IQueryable<TSource> source, Func<TSource, TDestination> selector, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            if (pageIndex > 0 && pageSize > 0)
            {
                source = source.Skip((pageIndex - 1) * pageSize)
                   .Take(pageSize);
            }
            var records = source.Select(selector).ToList();
            return new PaginatedList<TDestination>(records, count, pageIndex, pageSize);
        }

        public static PaginatedList<TDestination> Create<TSource, TDestination>(IQueryable<TSource> source, Func<TSource, TDestination> selector, int pageIndex, int pageSize)
        {
            var count = source.Count();
            if (pageIndex > 0 && pageSize > 0)
            {
                source = source.Skip((pageIndex - 1) * pageSize)
                   .Take(pageSize);
            }
            var records = source.Select(selector).ToList();
            return new PaginatedList<TDestination>(records, count, pageIndex, pageSize);
        }

        public static PaginatedList<TDestination> Create<TDestination>(IEnumerable<T> source, IMapper mapper, int pageIndex, int pageSize)
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
