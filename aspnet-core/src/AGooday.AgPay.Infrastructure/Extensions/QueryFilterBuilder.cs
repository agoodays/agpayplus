using System.Linq.Expressions;

namespace AGooday.AgPay.Infrastructure.Extensions
{
    /// <summary>
    /// 查询过滤器构建器，用于动态添加过滤条件到IQueryable对象。
    /// 通过这种方式，可以在运行时动态地构建查询条件。
    /// 例如，可以基于用户输入的搜索条件动态地添加过滤表达式。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryFilterBuilder<T>
    {
        private readonly IQueryable<T> _query;
        private readonly List<Expression<Func<T, bool>>> _filters = new();

        public QueryFilterBuilder(IQueryable<T> query)
        {
            _query = query;
        }

        /// <summary>
        /// 添加过滤条件
        /// </summary>
        public QueryFilterBuilder<T> AddFilter(Expression<Func<T, bool>> filter, bool condition = true)
        {
            if (condition && filter != null)
            {
                _filters.Add(filter);
            }
            return this;
        }

        /// <summary>
        /// 添加字符串过滤条件
        /// </summary>
        public QueryFilterBuilder<T> AddStringFilter(Expression<Func<T, string>> propertySelector, string value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var parameter = propertySelector.Parameters[0];
                var property = propertySelector.Body;
                var constant = Expression.Constant(value);

                Expression condition;
                if (comparison == StringComparison.OrdinalIgnoreCase)
                {
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                    var propertyToLower = Expression.Call(property, toLowerMethod);
                    var valueToLower = Expression.Call(constant, toLowerMethod);
                    condition = Expression.Equal(propertyToLower, valueToLower);
                }
                else
                {
                    condition = Expression.Equal(property, constant);
                }

                var lambda = Expression.Lambda<Func<T, bool>>(condition, parameter);
                _filters.Add(lambda);
            }
            return this;
        }

        /// <summary>
        /// 构建最终查询
        /// </summary>
        public IQueryable<T> Build()
        {
            var result = _query;
            foreach (var filter in _filters)
            {
                result = result.Where(filter);
            }
            return result;
        }
    }
}
