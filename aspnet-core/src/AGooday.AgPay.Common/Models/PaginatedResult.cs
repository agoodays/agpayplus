using AutoMapper;

namespace AGooday.AgPay.Common.Models
{
    public class PaginatedResult<T>
    {
        public IList<T> Items { get; private set; }
        public int TotalCount { get; private set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public PaginatedResult(IList<T> items, int totalCount, int pageIndex, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public static PaginatedResult<T> Create(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            if (pageIndex > 0 && pageSize > 0)
            {
                source = source.Skip((pageIndex - 1) * pageSize)
                   .Take(pageSize);
            }
            var records = source.ToList();
            return new PaginatedResult<T>(records, count, pageIndex, pageSize);
        }

        public static PaginatedResult<TDestination> Create<TDestination>(IQueryable<T> source, IMapper mapper, int pageIndex, int pageSize)
        {
            var count = source.Count();
            if (pageIndex > 0 && pageSize > 0)
            {
                source = source.Skip((pageIndex - 1) * pageSize)
                   .Take(pageSize);
            }
            var records = mapper.Map<List<TDestination>>(source.ToList());
            return new PaginatedResult<TDestination>(records, count, pageIndex, pageSize);
        }

        public static PaginatedResult<TDestination> Create<TSource, TDestination>(IQueryable<TSource> source, Func<TSource, TDestination> selector, int pageIndex, int pageSize)
        {
            var count = source.Count();
            if (pageIndex > 0 && pageSize > 0)
            {
                source = source.Skip((pageIndex - 1) * pageSize)
                   .Take(pageSize);
            }
            var records = source.Select(selector).ToList();
            return new PaginatedResult<TDestination>(records, count, pageIndex, pageSize);
        }

        public static PaginatedResult<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            if (pageIndex > 0 && pageSize > 0)
            {
                source = source.Skip((pageIndex - 1) * pageSize)
                   .Take(pageSize);
            }
            return new PaginatedResult<T>(source.ToList(), count, pageIndex, pageSize);
        }

        public static PaginatedResult<TDestination> Create<TDestination>(IEnumerable<T> source, IMapper mapper, int pageIndex, int pageSize)
        {
            var count = source.Count();
            if (pageIndex > 0 && pageSize > 0)
            {
                source = source.Skip((pageIndex - 1) * pageSize)
                   .Take(pageSize);
            }
            var records = mapper.Map<List<TDestination>>(source.ToList());
            return new PaginatedResult<TDestination>(records, count, pageIndex, pageSize);
        }
    }
}
