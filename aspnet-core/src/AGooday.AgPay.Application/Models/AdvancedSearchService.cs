using AGooday.AgPay.Infrastructure.Extensions;

namespace AGooday.AgPay.Application.Models
{
    /// <summary>
    /// 高级搜索服务类，用于构建动态查询。
    /// 此类提供了一种灵活的方式来构建动态查询，支持多种过滤条件。
    /// 使用此服务，可以轻松地根据用户输入的搜索条件动态构建查询。
    /// 例如，可以根据字段名称、操作符和值动态构建查询条件。
    /// 此类依赖于DynamicQueryBuilderFactory来创建动态查询构建器，
    /// 并根据请求动态构建查询条件。
    /// 此类还支持排序功能，可以根据指定的字段和方向对结果进行排序。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AdvancedSearchService<T>
    {
        public IQueryable<T> Search(AdvancedSearchRequest request)
        {
            var builder = DynamicQueryBuilderFactory.Create<T>();

            foreach (var filter in request.Filters)
            {
                ApplyFilter(builder, filter);
            }

            // 应用排序
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                builder.OrderBy(request.SortBy, request.SortDescending);
            }

            return builder.BuildQuery(GetBaseQuery());
        }

        private void ApplyFilter<T>(DynamicQueryBuilder<T> builder, SearchFilter filter)
        {
            switch (filter.Operator.ToLower())
            {
                case "equals":
                    builder.Equal(filter.Field, filter.Value);
                    break;
                case "contains":
                    builder.Contains(filter.Field, filter.Value?.ToString());
                    break;
                case "startswith":
                    builder.StartsWith(filter.Field, filter.Value?.ToString());
                    break;
                case "endswith":
                    builder.EndsWith(filter.Field, filter.Value?.ToString());
                    break;
                case "greaterthan":
                    builder.GreaterThan(filter.Field, filter.Value);
                    break;
                case "lessthan":
                    builder.LessThan(filter.Field, filter.Value);
                    break;
                case "in":
                    if (filter.Values != null)
                        builder.In(filter.Field, filter.Values);
                    break;
                case "isnull":
                    builder.IsNull(filter.Field);
                    break;
                case "isnotnull":
                    builder.IsNotNull(filter.Field);
                    break;
            }
        }

        private IQueryable<T> GetBaseQuery()
        {
            // 返回基础查询，例如：_repository.GetAllAsNoTracking()
            throw new NotImplementedException();
        }
    }

    public class AdvancedSearchRequest
    {
        public List<SearchFilter> Filters { get; set; } = new();
        public string SortBy { get; set; }
        public bool SortDescending { get; set; }
    }

    public class SearchFilter
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
        public IEnumerable<object> Values { get; set; }
    }
}
