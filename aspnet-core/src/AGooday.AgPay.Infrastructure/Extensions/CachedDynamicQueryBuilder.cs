using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace AGooday.AgPay.Infrastructure.Extensions
{
    public class CachedDynamicQueryBuilder<T> : DynamicQueryBuilder<T>
    {
        private static readonly ConcurrentDictionary<string, Expression<Func<T, bool>>> _expressionCache = new();

        public override Expression<Func<T, bool>> Build()
        {
            var cacheKey = GenerateCacheKey();
            if (_expressionCache.TryGetValue(cacheKey, out var cachedExpression))
            {
                return cachedExpression;
            }

            var expression = base.Build();
            _expressionCache.TryAdd(cacheKey, expression);
            return expression;
        }

        private string GenerateCacheKey()
        {
            // 基于构建器状态生成缓存键
            // 实际实现需要根据构建器的内部状态来生成
            return $"{typeof(T).Name}_{_expression.ToString()}";
        }

        public static void ClearCache()
        {
            _expressionCache.Clear();
        }
    }
}
