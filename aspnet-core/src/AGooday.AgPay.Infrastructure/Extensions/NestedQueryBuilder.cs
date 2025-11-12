using System.Linq.Expressions;

namespace AGooday.AgPay.Infrastructure.Extensions
{
    public class NestedQueryBuilder<T>
    {
        private readonly DynamicQueryBuilder<T> _innerBuilder;

        public NestedQueryBuilder()
        {
            _innerBuilder = new DynamicQueryBuilder<T>();
        }

        // 单层嵌套属性查询
        public NestedQueryBuilder<T> WhereNested<TProperty>(string propertyPath, Action<DynamicQueryBuilder<TProperty>> nestedBuilder)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            // 构建嵌套属性访问表达式
            Expression nestedProperty = parameter;
            foreach (var propertyName in propertyPath.Split('.'))
            {
                nestedProperty = Expression.PropertyOrField(nestedProperty, propertyName);
            }

            // 创建嵌套查询构建器
            var nestedQueryBuilder = new DynamicQueryBuilder<TProperty>();
            nestedBuilder(nestedQueryBuilder);

            var nestedExpression = nestedQueryBuilder.Build();

            // 使用 Any 或 All 方法，这里使用 Any 作为默认
            var anyMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TProperty));

            var anyExpression = Expression.Call(
                anyMethod,
                nestedProperty,
                nestedExpression
            );

            // 将嵌套条件添加到主查询
            var lambda = Expression.Lambda<Func<T, bool>>(anyExpression, parameter);
            _innerBuilder.Custom(lambda);

            return this;
        }

        // 单值嵌套属性查询（非集合）
        public NestedQueryBuilder<T> WhereNestedProperty<TProperty>(string propertyPath, Action<DynamicQueryBuilder<TProperty>> nestedBuilder)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            // 构建嵌套属性访问表达式
            Expression nestedProperty = parameter;
            foreach (var propertyName in propertyPath.Split('.'))
            {
                nestedProperty = Expression.PropertyOrField(nestedProperty, propertyName);
            }

            // 创建嵌套查询构建器
            var nestedQueryBuilder = new DynamicQueryBuilder<TProperty>();
            nestedBuilder(nestedQueryBuilder);

            var nestedExpression = nestedQueryBuilder.Build();

            // 替换嵌套表达式中的参数
            var visitor = new ParameterReplaceVisitor(
                nestedExpression.Parameters[0],
                nestedProperty);

            var replacedBody = visitor.Visit(nestedExpression.Body);
            var finalExpression = Expression.Lambda<Func<T, bool>>(replacedBody, parameter);

            _innerBuilder.Custom(finalExpression);

            return this;
        }

        // 集合数量查询
        public NestedQueryBuilder<T> WhereNestedCount<TProperty>(string propertyPath, Func<int, bool> countPredicate)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            // 构建嵌套属性访问表达式
            Expression nestedProperty = parameter;
            foreach (var propertyName in propertyPath.Split('.'))
            {
                nestedProperty = Expression.PropertyOrField(nestedProperty, propertyName);
            }

            // 获取 Count 方法
            var countMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Count" && m.GetParameters().Length == 1)
                .MakeGenericMethod(typeof(TProperty));

            var countExpression = Expression.Call(countMethod, nestedProperty);

            // 构建数量条件
            var constant = Expression.Constant(countPredicate);
            var condition = Expression.Invoke(constant, countExpression);

            var lambda = Expression.Lambda<Func<T, bool>>(condition, parameter);
            _innerBuilder.Custom(lambda);

            return this;
        }

        public IQueryable<T> BuildQuery(IQueryable<T> source)
        {
            return _innerBuilder.BuildQuery(source);
        }

        public Expression<Func<T, bool>> BuildPredicate()
        {
            return _innerBuilder.Build();
        }
    }

    // 参数替换访问器
    public class ParameterReplaceVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly Expression _replacement;

        public ParameterReplaceVisitor(ParameterExpression oldParameter, Expression replacement)
        {
            _oldParameter = oldParameter;
            _replacement = replacement;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _replacement : base.VisitParameter(node);
        }
    }
}
