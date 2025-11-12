namespace AGooday.AgPay.Infrastructure.Extensions
{
    /// <summary>
    /// 动态查询构建器扩展方法。
    /// </summary>
    public static class DynamicQueryBuilderExtensions
    {
        // 字符串扩展
        public static DynamicQueryBuilder<T> WhereIfNotEmpty<T>(this DynamicQueryBuilder<T> builder, string propertyName, string value)
        {
            return string.IsNullOrWhiteSpace(value) ? builder : builder.Equal(propertyName, value);
        }

        public static DynamicQueryBuilder<T> WhereIfContains<T>(this DynamicQueryBuilder<T> builder, string propertyName, string value)
        {
            return string.IsNullOrWhiteSpace(value) ? builder : builder.Contains(propertyName, value);
        }

        // 可空值类型扩展
        public static DynamicQueryBuilder<T> WhereIfNotNull<T, TValue>(this DynamicQueryBuilder<T> builder, string propertyName, TValue? value) where TValue : struct
        {
            return value.HasValue ? builder.Equal(propertyName, value.Value) : builder;
        }

        // 条件扩展
        public static DynamicQueryBuilder<T> WhereIf<T>(this DynamicQueryBuilder<T> builder, bool condition, string propertyName, object value)
        {
            return condition ? builder.Equal(propertyName, value) : builder;
        }

        // 集合扩展
        public static DynamicQueryBuilder<T> WhereIfAny<T, TValue>(this DynamicQueryBuilder<T> builder, string propertyName, IEnumerable<TValue> values)
        {
            return values != null && values.Any() ? builder.In(propertyName, values) : builder;
        }

        // 日期范围扩展
        public static DynamicQueryBuilder<T> WhereIfDateRange<T>(this DynamicQueryBuilder<T> builder, string propertyName, DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue || endDate.HasValue)
            {
                return builder.DateRange(propertyName, startDate, endDate);
            }
            return builder;
        }

        // 数值范围扩展
        public static DynamicQueryBuilder<T> WhereIfRange<T, TValue>(this DynamicQueryBuilder<T> builder, string propertyName, TValue? minValue, TValue? maxValue) where TValue : struct
        {
            if (minValue.HasValue || maxValue.HasValue)
            {
                return builder.Range(propertyName, minValue, maxValue);
            }
            return builder;
        }
    }
}
