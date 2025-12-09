namespace AGooday.AgPay.Infrastructure.Extensions
{
    public static class NestedQueryBuilderExtensions
    {
        // 嵌套属性条件扩展
        public static DynamicQueryBuilder<T> WhereNestedEqual<T, TProperty>(this DynamicQueryBuilder<T> builder, string propertyPath, object value)
        {
            return builder.NestedEqual(propertyPath, value);
        }

        public static DynamicQueryBuilder<T> WhereNestedContains<T>(this DynamicQueryBuilder<T> builder, string propertyPath, string value)
        {
            return builder.NestedContains(propertyPath, value);
        }

        // 嵌套集合条件扩展
        public static DynamicQueryBuilder<T> WhereNestedAny<T, TProperty>(this DynamicQueryBuilder<T> builder, string propertyPath, Action<DynamicQueryBuilder<TProperty>> nestedBuilder)
        {
            return builder.NestedAny(propertyPath, nestedBuilder);
        }

        public static DynamicQueryBuilder<T> WhereNestedAll<T, TProperty>(this DynamicQueryBuilder<T> builder, string propertyPath, Action<DynamicQueryBuilder<TProperty>> nestedBuilder)
        {
            return builder.NestedAll(propertyPath, nestedBuilder);
        }

        // 嵌套数量条件扩展
        public static DynamicQueryBuilder<T> WhereNestedCountGreaterThan<T, TProperty>(this DynamicQueryBuilder<T> builder, string propertyPath, int count)
        {
            return builder.NestedCount<TProperty>(propertyPath, c => c > count);
        }

        public static DynamicQueryBuilder<T> WhereNestedCountEquals<T, TProperty>(this DynamicQueryBuilder<T> builder, string propertyPath, int count)
        {
            return builder.NestedCount<TProperty>(propertyPath, c => c == count);
        }
    }
}
