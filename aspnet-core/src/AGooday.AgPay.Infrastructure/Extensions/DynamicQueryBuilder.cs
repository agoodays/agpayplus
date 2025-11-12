using System.Linq.Expressions;

namespace AGooday.AgPay.Infrastructure.Extensions
{
    /// <summary>
    /// 动态查询构建器，用于构建复杂的LINQ表达式。支持相等、包含和范围比较等操作。
    /// 此类提供了一系列方法来构建动态查询表达式，例如相等、包含和范围比较等。
    /// 使用此类可以方便地构建复杂的查询表达式，例如：
    /// var query = new DynamicQueryBuilder<MyEntity>()
    ///     .Equal("Name", "John")
    ///     .NotEqual("Status", "Inactive")
    ///     .Contains("Description", "example")
    ///     .GreaterThanOrEqual("Age", 18)
    ///     .LessThanOrEqual("Age", 60)
    ///     .Range("Age", 18, 60)
    ///     .GreaterThan("Age", 25)
    ///     .LessThan("Age", 60)
    ///     .Build();
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DynamicQueryBuilder<T>
    {
        private ParameterExpression _parameter;
        protected Expression _expression;
        private readonly List<SortExpression> _sortExpressions = new();

        public DynamicQueryBuilder()
        {
            _parameter = Expression.Parameter(typeof(T), "x");
            _expression = Expression.Constant(true); // 初始化为 true
        }

        // 基础比较操作
        public DynamicQueryBuilder<T> Equal(string propertyName, object value)
        {
            if (value != null && !IsDefaultValue(value))
            {
                //var property = Expression.Property(_parameter, propertyName);
                //var constant = Expression.Constant(value);
                //var condition = Expression.Equal(property, constant);
                var condition = BuildComparisonExpression(propertyName, value, ExpressionType.Equal);
                _expression = Expression.AndAlso(_expression, condition);
            }
            return this;
        }

        public DynamicQueryBuilder<T> NotEqual(string propertyName, object value)
        {
            if (value != null && !IsDefaultValue(value))
            {
                var condition = BuildComparisonExpression(propertyName, value, ExpressionType.NotEqual);
                _expression = Expression.AndAlso(_expression, condition);
            }
            return this;
        }

        public DynamicQueryBuilder<T> GreaterThan(string propertyName, object value)
        {
            if (value != null && !IsDefaultValue(value))
            {
                var condition = BuildComparisonExpression(propertyName, value, ExpressionType.GreaterThan);
                _expression = Expression.AndAlso(_expression, condition);
            }
            return this;
        }

        public DynamicQueryBuilder<T> GreaterThanOrEqual(string propertyName, object value)
        {
            if (value != null && !IsDefaultValue(value))
            {
                //var property = Expression.Property(_parameter, propertyName);
                //var constant = Expression.Constant(value, typeof(TValue));
                //var condition = Expression.GreaterThanOrEqual(property, constant);
                var condition = BuildComparisonExpression(propertyName, value, ExpressionType.GreaterThanOrEqual);
                _expression = Expression.AndAlso(_expression, condition);
            }
            return this;
        }

        public DynamicQueryBuilder<T> LessThan(string propertyName, object value)
        {
            if (value != null && !IsDefaultValue(value))
            {
                var condition = BuildComparisonExpression(propertyName, value, ExpressionType.LessThan);
                _expression = Expression.AndAlso(_expression, condition);
            }
            return this;
        }

        public DynamicQueryBuilder<T> LessThanOrEqual(string propertyName, object value)
        {
            if (value != null && !IsDefaultValue(value))
            {
                //var property = Expression.Property(_parameter, propertyName);
                //var constant = Expression.Constant(value, typeof(TValue));
                //var condition = Expression.LessThanOrEqual(property, constant);
                var condition = BuildComparisonExpression(propertyName, value, ExpressionType.LessThanOrEqual);
                _expression = Expression.AndAlso(_expression, condition);
            }
            return this;
        }

        // 字符串操作
        public DynamicQueryBuilder<T> Contains(string propertyName, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var property = Expression.Property(_parameter, propertyName);
                var constant = Expression.Constant(value);
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var containsExpression = Expression.Call(property, containsMethod, constant);
                _expression = Expression.AndAlso(_expression, containsExpression);
            }
            return this;
        }

        public DynamicQueryBuilder<T> StartsWith(string propertyName, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var property = Expression.Property(_parameter, propertyName);
                var constant = Expression.Constant(value);
                var startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                var startsWithExpression = Expression.Call(property, startsWithMethod, constant);
                _expression = Expression.AndAlso(_expression, startsWithExpression);
            }
            return this;
        }

        public DynamicQueryBuilder<T> EndsWith(string propertyName, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var property = Expression.Property(_parameter, propertyName);
                var constant = Expression.Constant(value);
                var endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                var endsWithExpression = Expression.Call(property, endsWithMethod, constant);
                _expression = Expression.AndAlso(_expression, endsWithExpression);
            }
            return this;
        }

        // 集合操作
        public DynamicQueryBuilder<T> In<TValue>(string propertyName, IEnumerable<TValue> values)
        {
            if (values != null && values.Any())
            {
                var property = Expression.Property(_parameter, propertyName);
                var constant = Expression.Constant(values);
                var containsMethod = typeof(Enumerable).GetMethods()
                    .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(TValue));
                var containsExpression = Expression.Call(containsMethod, constant, property);
                _expression = Expression.AndAlso(_expression, containsExpression);
            }
            return this;
        }

        public DynamicQueryBuilder<T> NotIn<TValue>(string propertyName, IEnumerable<TValue> values)
        {
            if (values != null && values.Any())
            {
                var property = Expression.Property(_parameter, propertyName);
                var constant = Expression.Constant(values);
                var containsMethod = typeof(Enumerable).GetMethods()
                    .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(TValue));
                var containsExpression = Expression.Call(containsMethod, constant, property);
                var notContainsExpression = Expression.Not(containsExpression);
                _expression = Expression.AndAlso(_expression, notContainsExpression);
            }
            return this;
        }

        // 空值检查
        public DynamicQueryBuilder<T> IsNull(string propertyName)
        {
            var property = Expression.Property(_parameter, propertyName);
            var nullExpression = Expression.Equal(property, Expression.Constant(null));
            _expression = Expression.AndAlso(_expression, nullExpression);
            return this;
        }

        public DynamicQueryBuilder<T> IsNotNull(string propertyName)
        {
            var property = Expression.Property(_parameter, propertyName);
            var notNullExpression = Expression.NotEqual(property, Expression.Constant(null));
            _expression = Expression.AndAlso(_expression, notNullExpression);
            return this;
        }

        public DynamicQueryBuilder<T> IsNullOrEmpty(string propertyName)
        {
            var property = Expression.Property(_parameter, propertyName);
            var nullExpression = Expression.Equal(property, Expression.Constant(null));
            var emptyExpression = Expression.Equal(property, Expression.Constant(string.Empty));
            var orExpression = Expression.OrElse(nullExpression, emptyExpression);
            _expression = Expression.AndAlso(_expression, orExpression);
            return this;
        }

        public DynamicQueryBuilder<T> IsNotNullOrEmpty(string propertyName)
        {
            var property = Expression.Property(_parameter, propertyName);
            var nullExpression = Expression.Equal(property, Expression.Constant(null));
            var emptyExpression = Expression.Equal(property, Expression.Constant(string.Empty));
            var orExpression = Expression.OrElse(nullExpression, emptyExpression);
            var notNullOrEmptyExpression = Expression.Not(orExpression);
            _expression = Expression.AndAlso(_expression, notNullOrEmptyExpression);
            return this;
        }

        // 日期范围操作
        public DynamicQueryBuilder<T> DateRange(string propertyName, DateTime? startDate, DateTime? endDate)
        {
            var property = Expression.Property(_parameter, propertyName);

            if (startDate.HasValue)
            {
                var startConstant = Expression.Constant(startDate.Value);
                var greaterThanOrEqualExpression = Expression.GreaterThanOrEqual(property, startConstant);
                _expression = Expression.AndAlso(_expression, greaterThanOrEqualExpression);
            }

            if (endDate.HasValue)
            {
                var endConstant = Expression.Constant(endDate.Value);
                var lessThanOrEqualExpression = Expression.LessThanOrEqual(property, endConstant);
                _expression = Expression.AndAlso(_expression, lessThanOrEqualExpression);
            }

            return this;
        }

        // 数值范围操作
        public DynamicQueryBuilder<T> Range<TValue>(string propertyName, TValue? minValue, TValue? maxValue) where TValue : struct
        {
            var property = Expression.Property(_parameter, propertyName);

            if (minValue.HasValue)
            {
                var minConstant = Expression.Constant(minValue.Value, typeof(TValue));
                var greaterThanOrEqualExpression = Expression.GreaterThanOrEqual(property, minConstant);
                _expression = Expression.AndAlso(_expression, greaterThanOrEqualExpression);
            }

            if (maxValue.HasValue)
            {
                var maxConstant = Expression.Constant(maxValue.Value, typeof(TValue));
                var lessThanOrEqualExpression = Expression.LessThanOrEqual(property, maxConstant);
                _expression = Expression.AndAlso(_expression, lessThanOrEqualExpression);
            }

            return this;
        }

        // 逻辑操作
        public DynamicQueryBuilder<T> And(Action<DynamicQueryBuilder<T>> innerBuilder)
        {
            var innerQueryBuilder = new DynamicQueryBuilder<T>();
            innerBuilder(innerQueryBuilder);
            var innerExpression = innerQueryBuilder.Build().Body;
            _expression = Expression.AndAlso(_expression, innerExpression);
            return this;
        }

        public DynamicQueryBuilder<T> Or(Action<DynamicQueryBuilder<T>> innerBuilder)
        {
            var innerQueryBuilder = new DynamicQueryBuilder<T>();
            innerBuilder(innerQueryBuilder);
            var innerExpression = innerQueryBuilder.Build().Body;
            _expression = Expression.OrElse(_expression, innerExpression);
            return this;
        }

        // 排序操作
        public DynamicQueryBuilder<T> OrderBy(string propertyName, bool descending = false)
        {
            _sortExpressions.Add(new SortExpression
            {
                PropertyName = propertyName,
                Descending = descending,
                ThenBy = false
            });
            return this;
        }

        public DynamicQueryBuilder<T> ThenBy(string propertyName, bool descending = false)
        {
            _sortExpressions.Add(new SortExpression
            {
                PropertyName = propertyName,
                Descending = descending,
                ThenBy = true
            });
            return this;
        }

        // 添加自定义条件的方法
        public DynamicQueryBuilder<T> Custom(Expression<Func<T, bool>> expression)
        {
            _expression = Expression.AndAlso(_expression, expression.Body);
            return this;
        }

        // 嵌套属性查询
        public DynamicQueryBuilder<T> NestedEqual<TProperty>(string propertyPath, object value)
        {
            if (value != null && !IsDefaultValue(value))
            {
                var condition = BuildNestedComparisonExpression(propertyPath, value, ExpressionType.Equal);
                _expression = Expression.AndAlso(_expression, condition);
            }
            return this;
        }

        public DynamicQueryBuilder<T> NestedContains<TProperty>(string propertyPath, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var property = BuildNestedPropertyExpression(propertyPath);
                var constant = Expression.Constant(value);
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var containsExpression = Expression.Call(property, containsMethod, constant);
                _expression = Expression.AndAlso(_expression, containsExpression);
            }
            return this;
        }

        // 集合嵌套查询
        public DynamicQueryBuilder<T> NestedAny<TProperty>(string propertyPath, Action<DynamicQueryBuilder<TProperty>> nestedBuilder)
        {
            var nestedProperty = BuildNestedPropertyExpression(propertyPath);

            var nestedQueryBuilder = new DynamicQueryBuilder<TProperty>();
            nestedBuilder(nestedQueryBuilder);

            var nestedExpression = nestedQueryBuilder.Build();

            var anyMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TProperty));

            var anyExpression = Expression.Call(
                anyMethod,
                nestedProperty,
                nestedExpression
            );

            _expression = Expression.AndAlso(_expression, anyExpression);
            return this;
        }

        public DynamicQueryBuilder<T> NestedAll<TProperty>(string propertyPath, Action<DynamicQueryBuilder<TProperty>> nestedBuilder)
        {
            var nestedProperty = BuildNestedPropertyExpression(propertyPath);

            var nestedQueryBuilder = new DynamicQueryBuilder<TProperty>();
            nestedBuilder(nestedQueryBuilder);

            var nestedExpression = nestedQueryBuilder.Build();

            var allMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "All" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TProperty));

            var allExpression = Expression.Call(
                allMethod,
                nestedProperty,
                nestedExpression
            );

            _expression = Expression.AndAlso(_expression, allExpression);
            return this;
        }

        // 嵌套属性数量查询
        public DynamicQueryBuilder<T> NestedCount<TProperty>(string propertyPath, Expression<Func<int, bool>> countPredicate)
        {
            var nestedProperty = BuildNestedPropertyExpression(propertyPath);

            var countMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Count" && m.GetParameters().Length == 1)
                .MakeGenericMethod(typeof(TProperty));

            var countExpression = Expression.Call(countMethod, nestedProperty);
            var condition = Expression.Invoke(countPredicate, countExpression);

            _expression = Expression.AndAlso(_expression, condition);
            return this;
        }

        private Expression BuildNestedPropertyExpression(string propertyPath)
        {
            Expression property = _parameter;
            foreach (var propertyName in propertyPath.Split('.'))
            {
                property = Expression.PropertyOrField(property, propertyName);
            }
            return property;
        }

        private BinaryExpression BuildNestedComparisonExpression(string propertyPath, object value, ExpressionType expressionType)
        {
            var property = BuildNestedPropertyExpression(propertyPath);
            var constant = Expression.Constant(value);
            return Expression.MakeBinary(expressionType, property, constant);
        }

        // 构建表达式
        public virtual Expression<Func<T, bool>> Build()
        {
            return Expression.Lambda<Func<T, bool>>(_expression, _parameter);
        }

        // 应用排序到查询
        public IQueryable<T> ApplySorting(IQueryable<T> query)
        {
            if (!_sortExpressions.Any())
                return query;

            var firstSort = _sortExpressions.First();
            IOrderedQueryable<T> orderedQuery;

            if (firstSort.Descending)
            {
                orderedQuery = query.OrderByDescending(CreatePropertyExpression(firstSort.PropertyName));
            }
            else
            {
                orderedQuery = query.OrderBy(CreatePropertyExpression(firstSort.PropertyName));
            }

            // 应用后续排序
            foreach (var sort in _sortExpressions.Skip(1))
            {
                if (sort.Descending)
                {
                    orderedQuery = orderedQuery.ThenByDescending(CreatePropertyExpression(sort.PropertyName));
                }
                else
                {
                    orderedQuery = orderedQuery.ThenBy(CreatePropertyExpression(sort.PropertyName));
                }
            }

            return orderedQuery;
        }

        // 完整构建查询（条件 + 排序）
        public IQueryable<T> BuildQuery(IQueryable<T> source)
        {
            var filteredQuery = source.Where(Build());
            return ApplySorting(filteredQuery);
        }

        // 辅助方法
        private Expression<Func<T, object>> CreatePropertyExpression(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);
            var convert = Expression.Convert(property, typeof(object));
            return Expression.Lambda<Func<T, object>>(convert, parameter);
        }

        private BinaryExpression BuildComparisonExpression(string propertyName, object value, ExpressionType expressionType)
        {
            var property = Expression.Property(_parameter, propertyName);
            var constant = Expression.Constant(value);
            return Expression.MakeBinary(expressionType, property, constant);
        }

        private bool IsDefaultValue(object value)
        {
            if (value == null) return true;

            var type = value.GetType();
            if (type.IsValueType)
            {
                return value.Equals(Activator.CreateInstance(type));
            }

            return false;
        }

        private class SortExpression
        {
            public string PropertyName { get; set; }
            public bool Descending { get; set; }
            public bool ThenBy { get; set; }
        }
    }
}
