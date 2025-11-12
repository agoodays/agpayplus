using System.Linq.Expressions;

namespace AGooday.AgPay.Infrastructure.Extensions
{
    /// <summary>
    /// 查询条件构建器
    /// <para>用于构建查询条件，支持链式调用</para>
    /// <para>使用示例：</para>
    /// <code>
    /// var query = new QueryConditionBuilder<MyEntity>(dbContext.MyEntities)
    ///     .WhereIf(condition1, x => x.Property1 == value1)
    ///     .WhereIfNotEmpty(value2, x => x.Property2 == value2)
    ///     .WhereIfNotNull(value3, x => x.Property3 == value3)
    ///     .Build();
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryConditionBuilder<T>
    {
        private readonly List<Expression<Func<T, bool>>> _conditions = new();
        private IQueryable<T> _query;

        public QueryConditionBuilder(IQueryable<T> query)
        {
            _query = query;
        }

        public QueryConditionBuilder<T> WhereIf(bool condition, Expression<Func<T, bool>> predicate)
        {
            if (condition)
            {
                _conditions.Add(predicate);
            }
            return this;
        }

        public QueryConditionBuilder<T> WhereIfNotEmpty(string value, Expression<Func<T, bool>> predicate)
        {
            return WhereIf(!string.IsNullOrWhiteSpace(value), predicate);
        }

        public QueryConditionBuilder<T> WhereIfNotNull<TValue>(TValue? value, Expression<Func<T, bool>> predicate) where TValue : struct
        {
            return WhereIf(value.HasValue, predicate);
        }

        public IQueryable<T> Build()
        {
            return _conditions.Aggregate(_query, (current, condition) => current.Where(condition));
        }
    }
}
