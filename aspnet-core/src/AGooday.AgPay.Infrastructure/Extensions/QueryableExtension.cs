using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using AGooday.AgPay.Common.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Infrastructure.Extensions
{
    /// <summary>
    /// IQueryable 扩展方法
    /// </summary>
    public static class QueryableExtension
    {
        #region 条件过滤扩展

        /// <summary>
        /// 条件满足时应用Where过滤
        /// </summary>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }

        /// <summary>
        /// 条件满足时应用Where过滤（字符串条件）
        /// </summary>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> truePredicate, Expression<Func<T, bool>> falsePredicate)
        {
            return condition ? query.Where(truePredicate) : query.Where(falsePredicate);
        }

        /// <summary>
        /// 字符串不为空时应用Where过滤
        /// </summary>
        public static IQueryable<T> WhereIfNotEmpty<T>(this IQueryable<T> query, string value, Expression<Func<T, bool>> predicate)
        {
            return !string.IsNullOrWhiteSpace(value) ? query.Where(predicate) : query;
        }

        /// <summary>
        /// 值不为空时应用Where过滤
        /// </summary>
        public static IQueryable<T> WhereIfNotNull<T, TValue>(this IQueryable<T> query, TValue value, Expression<Func<T, bool>> predicate)
        {
            return value != null ? query.Where(predicate) : query;
        }

        /// <summary>
        /// 条件Where - 仅在集合不为空时应用过滤
        /// </summary>
        public static IQueryable<TSource> WhereIfAny<TSource, TValue>(this IQueryable<TSource> query, IEnumerable<TValue> values, Expression<Func<TSource, bool>> predicate)
        {
            return values != null && values.Any() ? query.Where(predicate) : query;
        }

        #endregion

        #region JSON 过滤扩展

        /// <summary>
        /// JSON 字段是否包含指定的单个字符串值（自动跳过空值）
        /// </summary>
        public static IQueryable<T> WhereJsonContains<T>(
            this IQueryable<T> query,
            string value,
            Expression<Func<T, string>> jsonProperty)
        {
            if (string.IsNullOrWhiteSpace(value))
                return query;

            return query.WhereJsonContainsAny(new[] { value }, jsonProperty);
        }

        /// <summary>
        /// JSON 字段是否包含指定的多个字符串值中的任意一个（OR 逻辑，自动去重、跳过空值）
        /// </summary>
        public static IQueryable<T> WhereJsonContainsAny<T>(
            this IQueryable<T> query,
            IEnumerable<string> values,
            Expression<Func<T, string>> jsonProperty)
        {
            var cleanValues = values?.Where(v => !string.IsNullOrWhiteSpace(v)).Distinct().ToArray();
            if (cleanValues == null || cleanValues.Length == 0)
                return query;

            var parameter = jsonProperty.Parameters[0];
            var propertyAccess = jsonProperty.Body;

            // 构建：EF.Functions.JsonContains(property, new[] { "A", "B" })
            var efFunctions = Expression.Constant(EF.Functions);
            var valueArray = Expression.Constant(cleanValues); // 传 object[] 而非 JSON 字符串

            var jsonContainsMethod = typeof(MySqlJsonDbFunctionsExtensions)
                .GetMethod("JsonContains", new[] { typeof(DbFunctions), typeof(string), typeof(object) });

            if (jsonContainsMethod == null)
                throw new InvalidOperationException("EF.Functions.JsonContains not found.");

            var call = Expression.Call(null, jsonContainsMethod, efFunctions, propertyAccess, valueArray);
            var lambda = Expression.Lambda<Func<T, bool>>(call, parameter);

            return query.Where(lambda);
        }

        public static IQueryable<T> WhereJsonContainsAsString<T>(
            this IQueryable<T> query,
            string value,
            Expression<Func<T, string>> jsonProperty)
        {
            if (string.IsNullOrWhiteSpace(value)) return query;
            var jsonStr = JArray.FromObject(new[] { value }).ToString(Formatting.None);
            return WhereJsonContainsAsString(query, new[] { value }, jsonProperty);
        }

        private static IQueryable<T> WhereJsonContainsAsString<T>(
            this IQueryable<T> query,
            string[] values,
            Expression<Func<T, string>> jsonProperty)
        {
            var jsonStr = JArray.FromObject(values).ToString(Formatting.None);
            var param = jsonProperty.Parameters[0];
            var body = jsonProperty.Body;
            var ef = Expression.Constant(EF.Functions);
            var str = Expression.Constant(jsonStr);
            var method = typeof(MySqlJsonDbFunctionsExtensions)
                .GetMethod("JsonContains", new[] { typeof(DbFunctions), typeof(string), typeof(string) });
            var call = Expression.Call(null, method!, ef, body, str);
            var lambda = Expression.Lambda<Func<T, bool>>(call, param);
            return query.Where(lambda);
        }

        /// <summary>
        /// JSON 数组包含字符串值
        /// </summary>
        public static IQueryable<T> WhereJsonArrayContains<T>(
            this IQueryable<T> query,
            string value,
            Expression<Func<T, string>> jsonProperty)
        {
            if (string.IsNullOrWhiteSpace(value))
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Invoke(jsonProperty, parameter);

            // 创建 JSON 数组字符串
            var jsonArray = JArray.FromObject(new[] { value }).ToString(Formatting.None);
            var jsonArrayConstant = Expression.Constant(jsonArray);

            // 构建 EF.Functions.JsonContains 调用
            var efFunctions = Expression.Constant(EF.Functions);
            var jsonContainsMethod = typeof(MySqlJsonDbFunctionsExtensions)
                .GetMethod("JsonContains", new[] { typeof(DbFunctions), typeof(string), typeof(string) });

            var jsonContainsCall = Expression.Call(
                null,
                jsonContainsMethod,
                efFunctions,
                propertyAccess,
                jsonArrayConstant);

            var lambda = Expression.Lambda<Func<T, bool>>(jsonContainsCall, parameter);

            return query.Where(lambda);
        }

        /// <summary>
        /// JSON 数组包含多个字符串值
        /// </summary>
        public static IQueryable<T> WhereJsonArrayContainsAny<T>(
            this IQueryable<T> query,
            Expression<Func<T, string>> jsonProperty,
            IEnumerable<string> values)
        {
            var valueList = values?.Where(v => !string.IsNullOrWhiteSpace(v)).ToList();
            if (valueList == null || !valueList.Any())
                return query;

            // 为每个值创建 OR 条件
            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Invoke(jsonProperty, parameter);
            var efFunctions = Expression.Constant(EF.Functions);
            var jsonContainsMethod = typeof(MySqlJsonDbFunctionsExtensions)
                .GetMethod("JsonContains", new[] { typeof(DbFunctions), typeof(string), typeof(string) });

            Expression combinedCondition = null;

            foreach (var value in valueList.Distinct())
            {
                var jsonArray = JArray.FromObject(new[] { value }).ToString(Formatting.None);
                var jsonArrayConstant = Expression.Constant(jsonArray);

                var jsonContainsCall = Expression.Call(
                    null,
                    jsonContainsMethod,
                    efFunctions,
                    propertyAccess,
                    jsonArrayConstant);

                if (combinedCondition == null)
                {
                    combinedCondition = jsonContainsCall;
                }
                else
                {
                    combinedCondition = Expression.OrElse(combinedCondition, jsonContainsCall);
                }
            }

            var lambda = Expression.Lambda<Func<T, bool>>(combinedCondition, parameter);
            return query.Where(lambda);
        }

        /// <summary>
        /// JSON 对象包含指定键
        /// </summary>
        public static IQueryable<T> WhereJsonContainsKey<T>(
            this IQueryable<T> query,
            Expression<Func<T, string>> jsonProperty,
            string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Invoke(jsonProperty, parameter);

            // 使用 JSON_EXTRACT 或类似函数检查键是否存在
            var jsonExtractMethod = typeof(MySqlJsonDbFunctionsExtensions)
                .GetMethod("JsonExtract", new[] { typeof(DbFunctions), typeof(string), typeof(string) });

            var efFunctions = Expression.Constant(EF.Functions);
            var jsonPath = Expression.Constant($"$.{key}");

            var jsonExtractCall = Expression.Call(
                null,
                jsonExtractMethod,
                efFunctions,
                propertyAccess,
                jsonPath);

            var isNotNullCondition = Expression.NotEqual(jsonExtractCall, Expression.Constant(null));

            var lambda = Expression.Lambda<Func<T, bool>>(isNotNullCondition, parameter);
            return query.Where(lambda);
        }

        #endregion

        #region 分页扩展

        /// <summary>
        /// 分页扩展方法
        /// </summary>
        public static IQueryable<T> PageBy<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            // 参数验证
            pageIndex = Math.Max(1, pageIndex);
            pageSize = Math.Max(1, pageSize);

            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        public static PaginatedResult<T> ToPaginatedResult<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            var totalCount = query.Count();
            if (pageIndex > 0 && pageSize > 0)
            {
                query = query.PageBy(pageIndex, pageSize);
            }
            var items = query.ToList();
            return new PaginatedResult<T>(items, totalCount, pageIndex, pageSize);
        }

        /// <summary>
        /// 异步分页查询
        /// </summary>
        public static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(this IQueryable<T> query, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var totalCount = await query.CountAsync(cancellationToken);
            if (pageIndex > 0 && pageSize > 0)
            {
                query = query.PageBy(pageIndex, pageSize);
            }
            var items = await query.ToListAsync(cancellationToken);
            return new PaginatedResult<T>(items, totalCount, pageIndex, pageSize);
        }

        public static PaginatedResult<TDestination> ToPaginatedResult<TSource, TDestination>(this IQueryable<TSource> query, Func<TSource, TDestination> selector, int pageIndex, int pageSize)
        {
            var count = query.Count();
            if (pageIndex > 0 && pageSize > 0)
            {
                query = query.PageBy(pageIndex, pageSize);
            }
            var records = query.Select(selector).ToList();
            return new PaginatedResult<TDestination>(records, count, pageIndex, pageSize);
        }

        public static async Task<PaginatedResult<TDestination>> ToPaginatedResultAsync<TSource, TDestination>(this IQueryable<TSource> query, Func<TSource, TDestination> selector, int pageIndex, int pageSize)
        {
            var count = await query.CountAsync();
            if (pageIndex > 0 && pageSize > 0)
            {
                query = query.PageBy(pageIndex, pageSize);
            }
            var records = query.Select(selector).ToList();
            return new PaginatedResult<TDestination>(records, count, pageIndex, pageSize);
        }

        /// <summary>
        /// 同步分页 + 内存映射（安全，支持 JArray.Parse 等 .NET 方法）
        /// </summary>
        public static PaginatedResult<TDestination> ToPaginatedResult<TSource, TDestination>(this IQueryable<TSource> query, IMapper mapper, int pageIndex, int pageSize)
        {
            // 先查实体（在数据库分页）
            var entityResult = query.ToPaginatedResult(pageIndex, pageSize);

            // 再在内存中映射（安全执行 JArray.Parse）
            var dtos = mapper.Map<List<TDestination>>(entityResult.Items);

            return new PaginatedResult<TDestination>(dtos, entityResult.TotalCount, pageIndex, pageSize);
        }

        /// <summary>
        /// 异步分页 + 内存映射（安全，支持 JArray.Parse 等 .NET 方法）
        /// </summary>
        public static async Task<PaginatedResult<TDestination>> ToPaginatedResultAsync<TSource, TDestination>(this IQueryable<TSource> query, IMapper mapper, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            // 先查实体（在数据库分页）
            var entityResult = await query.ToPaginatedResultAsync(pageIndex, pageSize, cancellationToken);

            // 再在内存中映射（安全执行 JArray.Parse）
            var dtos = mapper.Map<List<TDestination>>(entityResult.Items);

            return new PaginatedResult<TDestination>(dtos, entityResult.TotalCount, pageIndex, pageSize);
        }

        /// <summary>
        /// 同步分页 + 数据库投影（仅用于简单映射，不能含 JArray.Parse 等）
        /// </summary>
        public static PaginatedResult<TDestination> ToProjectedPaginatedResult<TSource, TDestination>(this IQueryable<TSource> query, IMapper mapper, int pageIndex, int pageSize)
        {
            // 使用 ProjectTo 在数据库层面进行映射
            var projectedQuery = query.ProjectTo<TDestination>(mapper.ConfigurationProvider);
            return projectedQuery.ToPaginatedResult(pageIndex, pageSize);
        }

        /// <summary>
        /// 异步分页 + 数据库投影（仅用于简单映射，不能含 JArray.Parse 等）
        /// </summary>
        public static async Task<PaginatedResult<TDestination>> ToProjectedPaginatedResultAsync<TSource, TDestination>(this IQueryable<TSource> query, IMapper mapper, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            // 使用 ProjectTo 在数据库层面进行映射
            var projectedQuery = query.ProjectTo<TDestination>(mapper.ConfigurationProvider);
            return await projectedQuery.ToPaginatedResultAsync(pageIndex, pageSize, cancellationToken);
        }

        #endregion

        #region 排序扩展

        /// <summary>
        /// 动态排序（升序）
        /// </summary>
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName)
        {
            return ApplyOrder(query, propertyName, "OrderBy");
        }

        /// <summary>
        /// 动态排序（降序）
        /// </summary>
        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string propertyName)
        {
            return ApplyOrder(query, propertyName, "OrderByDescending");
        }

        /// <summary>
        /// 动态多字段排序
        /// </summary>
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, params (string PropertyName, bool IsDescending)[] sorts)
        {
            if (sorts == null || sorts.Length == 0)
                return query;

            var orderedQuery = ApplyOrder(query, sorts[0].PropertyName, sorts[0].IsDescending ? "OrderByDescending" : "OrderBy");

            for (int i = 1; i < sorts.Length; i++)
            {
                orderedQuery = ApplyOrder(orderedQuery, sorts[i].PropertyName, sorts[i].IsDescending ? "ThenByDescending" : "ThenBy");
            }

            return orderedQuery;
        }

        private static IQueryable<T> ApplyOrder<T>(IQueryable<T> query, string propertyName, string methodName)
        {
            var entityType = typeof(T);
            var propertyInfo = entityType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (propertyInfo == null)
                throw new ArgumentException($"Property '{propertyName}' not found on type '{entityType.Name}'");

            var parameter = Expression.Parameter(entityType, "x");
            var property = Expression.Property(parameter, propertyInfo);
            var lambda = Expression.Lambda(property, parameter);

            var result = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(entityType, propertyInfo.PropertyType)
                .Invoke(null, new object[] { query, lambda });

            return (IQueryable<T>)result;
        }

        #endregion

        #region 基础 ToListAsync 扩展

        /// <summary>
        /// 异步将查询结果转换为列表
        /// </summary>
        public static Task<List<TSource>> ToListAsync<TSource>(
            this IQueryable<TSource> query,
            CancellationToken cancellationToken = default)
        {
            return EntityFrameworkQueryableExtensions.ToListAsync(query, cancellationToken);
        }

        /// <summary>
        /// 异步将查询结果转换为列表（带最大数量限制）
        /// </summary>
        public static async Task<List<TSource>> ToListAsync<TSource>(
            this IQueryable<TSource> query,
            int maxCount,
            CancellationToken cancellationToken = default)
        {
            if (maxCount <= 0)
                return new List<TSource>();

            return await query.Take(maxCount).ToListAsync(cancellationToken);
        }

        #endregion

        #region 安全 ToListAsync 扩展

        /// <summary>
        /// 安全异步转换为列表（处理空查询）
        /// </summary>
        public static async Task<List<TSource>> SafeToListAsync<TSource>(
            this IQueryable<TSource> query,
            CancellationToken cancellationToken = default)
        {
            if (query == null)
                return new List<TSource>();

            try
            {
                return await query.ToListAsync(cancellationToken);
            }
            catch (Exception)
            {
                return new List<TSource>();
            }
        }

        /// <summary>
        /// 安全异步转换为列表（带默认值）
        /// </summary>
        public static async Task<List<TSource>> SafeToListAsync<TSource>(
            this IQueryable<TSource> query,
            List<TSource> defaultValue,
            CancellationToken cancellationToken = default)
        {
            if (query == null)
                return defaultValue;

            try
            {
                return await query.ToListAsync(cancellationToken);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 安全异步转换为列表（带最大数量限制）
        /// </summary>
        public static async Task<List<TSource>> SafeToListAsync<TSource>(
            this IQueryable<TSource> query,
            int maxCount,
            CancellationToken cancellationToken = default)
        {
            if (query == null || maxCount <= 0)
                return new List<TSource>();

            try
            {
                return await query.Take(maxCount).ToListAsync(cancellationToken);
            }
            catch (Exception)
            {
                return new List<TSource>();
            }
        }

        #endregion

        #region 条件 ToListAsync 扩展

        /// <summary>
        /// 条件异步转换为列表
        /// </summary>
        public static async Task<List<TSource>> ToListIfAsync<TSource>(
            this IQueryable<TSource> query,
            bool condition,
            CancellationToken cancellationToken = default)
        {
            if (!condition)
                return new List<TSource>();

            return await query.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 字符串不为空时异步转换为列表
        /// </summary>
        public static async Task<List<TSource>> ToListIfNotEmptyAsync<TSource>(
            this IQueryable<TSource> query,
            string value,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(value))
                return await query.ToListAsync(cancellationToken);

            return await query.Where(predicate).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 值不为空时异步转换为列表
        /// </summary>
        public static async Task<List<TSource>> ToListIfNotNullAsync<TSource, TValue>(
            this IQueryable<TSource> query,
            TValue value,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            if (value == null)
                return await query.ToListAsync(cancellationToken);

            return await query.Where(predicate).ToListAsync(cancellationToken);
        }

        #endregion

        #region 性能优化 ToListAsync 扩展

        /// <summary>
        /// 异步转换为列表（带性能监控）
        /// </summary>
        public static async Task<List<TSource>> ToListWithPerformanceAsync<TSource>(
            this IQueryable<TSource> query,
            Action<TimeSpan> performanceCallback = null,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var result = await query.ToListAsync(cancellationToken);
                stopwatch.Stop();
                performanceCallback?.Invoke(stopwatch.Elapsed);
                return result;
            }
            catch (Exception)
            {
                stopwatch.Stop();
                throw;
            }
        }

        /// <summary>
        /// 异步转换为列表（带内存监控）
        /// </summary>
        public static async Task<List<TSource>> ToListWithMemoryMonitoringAsync<TSource>(
            this IQueryable<TSource> query,
            Action<long> memoryUsageCallback = null,
            CancellationToken cancellationToken = default)
        {
            var initialMemory = GC.GetTotalMemory(true);

            try
            {
                var result = await query.ToListAsync(cancellationToken);

                var finalMemory = GC.GetTotalMemory(false);
                var memoryUsed = finalMemory - initialMemory;

                memoryUsageCallback?.Invoke(memoryUsed);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 分块异步转换为列表（适用于大数据量）
        /// </summary>
        public static async Task<List<TSource>> ToListInChunksAsync<TSource>(
            this IQueryable<TSource> query,
            int chunkSize = 1000,
            Action<int, int> progressCallback = null,
            CancellationToken cancellationToken = default)
        {
            if (chunkSize <= 0)
                throw new ArgumentException("Chunk size must be greater than 0", nameof(chunkSize));

            var result = new List<TSource>();
            var pageIndex = 1;
            List<TSource> chunk;

            do
            {
                chunk = await query
                    .Skip((pageIndex - 1) * chunkSize)
                    .Take(chunkSize)
                    .ToListAsync(cancellationToken);

                if (chunk.Any())
                {
                    result.AddRange(chunk);
                    progressCallback?.Invoke(pageIndex, chunk.Count);
                }

                pageIndex++;
            } while (chunk.Count == chunkSize);

            return result;
        }

        #endregion

        #region 投影 ToListAsync 扩展

        public static List<TDestination> ToList<TSource, TDestination>(this IQueryable<TSource> query, IMapper mapper)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            return mapper.Map<List<TDestination>>(query.ToList());
        }

        /// <summary>
        /// 异步投影到目标类型并转换为列表
        /// </summary>
        public static async Task<List<TDestination>> ToListAsync<TSource, TDestination>(
            this IQueryable<TSource> query,
            IMapper mapper,
            CancellationToken cancellationToken = default)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            return mapper.Map<List<TDestination>>(await query.ToListAsync(cancellationToken)); ;
        }

        public static List<TDestination> ToProjectedList<TSource, TDestination>(this IQueryable<TSource> query, IMapper mapper)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            return query.ProjectTo<TDestination>(mapper.ConfigurationProvider).ToList();
        }

        /// <summary>
        /// 异步投影到目标类型并转换为列表
        /// </summary>
        public static async Task<List<TDestination>> ToProjectedListAsync<TSource, TDestination>(
            this IQueryable<TSource> query,
            IMapper mapper,
            CancellationToken cancellationToken = default)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            return await query
                .ProjectTo<TDestination>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 安全异步投影到目标类型并转换为列表
        /// </summary>
        public static async Task<List<TDestination>> SafeToListProjectToAsync<TSource, TDestination>(
            this IQueryable<TSource> query,
            IMapper mapper,
            CancellationToken cancellationToken = default)
        {
            if (query == null || mapper == null)
                return new List<TDestination>();

            try
            {
                return await query
                    .ProjectTo<TDestination>(mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception)
            {
                return new List<TDestination>();
            }
        }

        /// <summary>
        /// 异步投影到目标类型并转换为列表（带最大数量限制）
        /// </summary>
        public static async Task<List<TDestination>> ToListProjectToAsync<TSource, TDestination>(
            this IQueryable<TSource> query,
            IMapper mapper,
            int maxCount,
            CancellationToken cancellationToken = default)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (maxCount <= 0)
                return new List<TDestination>();

            return await query
                .Take(maxCount)
                .ProjectTo<TDestination>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }

        #endregion

        #region 包含关联数据扩展

        /// <summary>
        /// 条件包含关联数据
        /// </summary>
        public static IQueryable<T> IncludeIf<T, TProperty>(
            this IQueryable<T> query,
            bool condition,
            Expression<Func<T, TProperty>> path) where T : class
        {
            return condition ? query.Include(path) : query;
        }

        /// <summary>
        /// 包含多个关联数据
        /// </summary>
        public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, params string[] includes) where T : class
        {
            if (includes == null || includes.Length == 0)
                return query;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        /// <summary>
        /// 包含多个关联数据（表达式）
        /// </summary>
        public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, params Expression<Func<T, object>>[] includes) where T : class
        {
            if (includes == null || includes.Length == 0)
                return query;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        #endregion

        #region 选择器扩展

        /// <summary>
        /// 条件选择字段
        /// </summary>
        public static IQueryable<TResult> SelectIf<TSource, TResult>(
            this IQueryable<TSource> source,
            bool condition,
            Expression<Func<TSource, TResult>> trueSelector,
            Expression<Func<TSource, TResult>> falseSelector)
        {
            return condition ? source.Select(trueSelector) : source.Select(falseSelector);
        }

        #endregion

        #region 聚合扩展

        /// <summary>
        /// 异步返回序列中的最小值
        /// </summary>
        public static Task<TSource> MinAsync<TSource>(
            this IQueryable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            return EntityFrameworkQueryableExtensions.MinAsync(source, cancellationToken);
        }

        /// <summary>
        /// 异步返回序列中选定器函数的最小值
        /// </summary>
        public static Task<TResult> MinAsync<TSource, TResult>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            CancellationToken cancellationToken = default)
        {
            return EntityFrameworkQueryableExtensions.MinAsync(source, selector, cancellationToken);
        }

        /// <summary>
        /// 异步返回序列中的最大值
        /// </summary>
        public static Task<TSource> MaxAsync<TSource>(
            this IQueryable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            return EntityFrameworkQueryableExtensions.MaxAsync(source, cancellationToken);
        }

        /// <summary>
        /// 异步返回序列中选定器函数的最大值
        /// </summary>
        public static Task<TResult> MaxAsync<TSource, TResult>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            CancellationToken cancellationToken = default)
        {
            return EntityFrameworkQueryableExtensions.MaxAsync(source, selector, cancellationToken);
        }

        /// <summary>
        /// 异步返回序列中选定器函数的总和
        /// </summary>
        public static Task<decimal> SumAsync<TSource>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, decimal>> selector,
            CancellationToken cancellationToken = default)
        {
            return EntityFrameworkQueryableExtensions.SumAsync(source, selector, cancellationToken);
        }

        /// <summary>
        /// 安全求和（避免空值异常）
        /// </summary>
        public static async Task<decimal> SafeSumAsync<TSource>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, decimal?>> selector,
            CancellationToken cancellationToken = default)
        {
            var result = await source.SumAsync(selector, cancellationToken);
            return result ?? 0;
        }

        /// <summary>
        /// 安全求和（整数）
        /// </summary>
        public static async Task<int> SafeSumAsync<TSource>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, int?>> selector)
        {
            var result = await source.SumAsync(selector);
            return result ?? 0;
        }

        /// <summary>
        /// 安全求和（长整数）
        /// </summary>
        public static async Task<long> SafeSumAsync<TSource>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, long?>> selector)
        {
            var result = await source.SumAsync(selector);
            return result ?? 0;
        }

        /// <summary>
        /// 异步返回序列中元素的平均值
        /// </summary>
        public static Task<decimal> AverageAsync(
            this IQueryable<decimal> source,
            CancellationToken cancellationToken = default)
        {
            return EntityFrameworkQueryableExtensions.AverageAsync(source, cancellationToken);
        }

        /// <summary>
        /// 安全平均值
        /// </summary>
        public static async Task<decimal> SafeAverageAsync<TSource>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, decimal?>> selector,
            CancellationToken cancellationToken = default)
        {
            var result = await source.AverageAsync(selector, cancellationToken);
            return result ?? 0;
        }

        #endregion

        #region 基础计数扩展

        /// <summary>
        /// 异步获取查询结果数量
        /// </summary>
        public static Task<int> CountAsync<TSource>(
            this IQueryable<TSource> query,
            CancellationToken cancellationToken = default)
        {
            return EntityFrameworkQueryableExtensions.CountAsync(query, cancellationToken);
        }

        /// <summary>
        /// 异步获取满足条件的查询结果数量
        /// </summary>
        public static Task<int> CountAsync<TSource>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return EntityFrameworkQueryableExtensions.CountAsync(query, predicate, cancellationToken);
        }

        #endregion

        #region 安全计数扩展

        /// <summary>
        /// 安全计数（处理空查询）
        /// </summary>
        public static async Task<int> SafeCountAsync<TSource>(
            this IQueryable<TSource> query,
            CancellationToken cancellationToken = default)
        {
            if (query == null)
                return 0;

            try
            {
                return await query.CountAsync(cancellationToken);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 安全计数（带条件）
        /// </summary>
        public static async Task<int> SafeCountAsync<TSource>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            if (query == null)
                return 0;

            try
            {
                return await query.CountAsync(predicate, cancellationToken);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 安全计数（带默认值）
        /// </summary>
        public static async Task<int> SafeCountAsync<TSource>(
            this IQueryable<TSource> query,
            int defaultValue,
            CancellationToken cancellationToken = default)
        {
            if (query == null)
                return defaultValue;

            try
            {
                return await query.CountAsync(cancellationToken);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        #endregion

        #region 条件计数扩展

        /// <summary>
        /// 条件计数
        /// </summary>
        public static async Task<int> CountIfAsync<TSource>(
            this IQueryable<TSource> query,
            bool condition,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            if (!condition)
                return await query.CountAsync(cancellationToken);

            return await query.CountAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// 多条件计数
        /// </summary>
        public static async Task<int> CountIfAsync<TSource>(
            this IQueryable<TSource> query,
            bool condition,
            CancellationToken cancellationToken = default)
        {
            if (!condition)
                return 0;

            return await query.CountAsync(cancellationToken);
        }

        /// <summary>
        /// 字符串不为空时计数
        /// </summary>
        public static async Task<int> CountIfNotEmptyAsync<TSource>(
            this IQueryable<TSource> query,
            string value,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(value))
                return await query.CountAsync(cancellationToken);

            return await query.CountAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// 值不为空时计数
        /// </summary>
        public static async Task<int> CountIfNotNullAsync<TSource, TValue>(
            this IQueryable<TSource> query,
            TValue value,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            if (value == null)
                return await query.CountAsync(cancellationToken);

            return await query.CountAsync(predicate, cancellationToken);
        }

        #endregion

        #region 性能优化计数扩展

        /// <summary>
        /// 带性能监控的计数
        /// </summary>
        public static async Task<int> CountWithPerformanceAsync<TSource>(
            this IQueryable<TSource> query,
            Action<TimeSpan> performanceCallback = null,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var count = await query.CountAsync(cancellationToken);
                stopwatch.Stop();
                performanceCallback?.Invoke(stopwatch.Elapsed);
                return count;
            }
            catch (Exception)
            {
                stopwatch.Stop();
                throw;
            }
        }

        /// <summary>
        /// 带条件性能监控的计数
        /// </summary>
        public static async Task<int> CountWithPerformanceAsync<TSource>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            Action<TimeSpan> performanceCallback = null,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var count = await query.CountAsync(predicate, cancellationToken);
                stopwatch.Stop();
                performanceCallback?.Invoke(stopwatch.Elapsed);
                return count;
            }
            catch (Exception)
            {
                stopwatch.Stop();
                throw;
            }
        }

        /// <summary>
        /// 快速计数估算（适用于大数据量）
        /// </summary>
        public static async Task<long> FastCountEstimateAsync<TSource>(
            this IQueryable<TSource> query,
            CancellationToken cancellationToken = default)
        {
            // 对于大数据量，可以使用数据库的统计信息进行估算
            // 这里是一个简单实现，实际应用中可能需要数据库特定的优化
            return await query.CountAsync(cancellationToken);
        }

        #endregion

        #region 批量计数扩展

        /// <summary>
        /// 批量计数（多个条件分别计数）
        /// </summary>
        public static async Task<Dictionary<string, int>> BatchCountAsync<TSource>(
            this IQueryable<TSource> query,
            params (string Key, Expression<Func<TSource, bool>> Predicate)[] predicates)
        {
            var results = new Dictionary<string, int>();

            foreach (var (key, predicate) in predicates)
            {
                var count = await query.CountAsync(predicate);
                results[key] = count;
            }

            return results;
        }

        /// <summary>
        /// 分组计数
        /// </summary>
        public static async Task<Dictionary<TKey, int>> CountByAsync<TSource, TKey>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, TKey>> keySelector,
            CancellationToken cancellationToken = default) where TKey : notnull
        {
            return await query
                .GroupBy(keySelector)
                .Select(g => new { Key = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken);
        }

        /// <summary>
        /// 分组计数（带条件）
        /// </summary>
        public static async Task<Dictionary<TKey, int>> CountByAsync<TSource, TKey>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default) where TKey : notnull
        {
            return await query
                .Where(predicate)
                .GroupBy(keySelector)
                .Select(g => new { Key = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken);
        }

        #endregion

        #region 存在性检查扩展

        /// <summary>
        /// 检查是否存在满足条件的记录
        /// </summary>
        public static Task<bool> AnyAsync<TSource>(
            this IQueryable<TSource> query,
            CancellationToken cancellationToken = default)
        {
            return EntityFrameworkQueryableExtensions.AnyAsync(query, cancellationToken);
        }

        /// <summary>
        /// 检查是否存在满足条件的记录
        /// </summary>
        public static Task<bool> AnyAsync<TSource>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return EntityFrameworkQueryableExtensions.AnyAsync(query, predicate, cancellationToken);
        }

        /// <summary>
        /// 安全的存在性检查
        /// </summary>
        public static async Task<bool> SafeAnyAsync<TSource>(
            this IQueryable<TSource> query,
            CancellationToken cancellationToken = default)
        {
            if (query == null)
                return false;

            try
            {
                return await query.AnyAsync(cancellationToken);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 安全的存在性检查（带条件）
        /// </summary>
        public static async Task<bool> SafeAnyAsync<TSource>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            if (query == null)
                return false;

            try
            {
                return await query.AnyAsync(predicate, cancellationToken);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 异步确定序列是否包含所有元素满足条件
        /// </summary>
        public static Task<bool> AllAsync<TSource>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return EntityFrameworkQueryableExtensions.AllAsync(source, predicate, cancellationToken);
        }

        #endregion

        #region 组合计数扩展

        /// <summary>
        /// 计数并检查是否存在
        /// </summary>
        public static async Task<(int Count, bool Exists)> CountAndAnyAsync<TSource>(
            this IQueryable<TSource> query,
            CancellationToken cancellationToken = default)
        {
            var count = await query.CountAsync(cancellationToken);
            return (count, count > 0);
        }

        /// <summary>
        /// 计数并检查是否存在（带条件）
        /// </summary>
        public static async Task<(int Count, bool Exists)> CountAndAnyAsync<TSource>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            var count = await query.CountAsync(predicate, cancellationToken);
            return (count, count > 0);
        }

        #endregion

        #region 批量操作扩展（EF Core 7.0+ 风格）

        /// <summary>
        /// 批量更新（EF Core 7.0+ 兼容）
        /// </summary>
        public static async Task<int> UpdateAsync<T>(
            this IQueryable<T> query,
            Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyCalls) where T : class
        {
            return await query.ExecuteUpdateAsync(setPropertyCalls);
        }

        /// <summary>
        /// 批量删除（EF Core 7.0+ 兼容）
        /// </summary>
        public static async Task<int> DeleteAsync<T>(this IQueryable<T> query) where T : class
        {
            return await query.ExecuteDeleteAsync();
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 构建动态查询条件
        /// </summary>
        public static IQueryable<T> BuildFilter<T>(this IQueryable<T> query, Action<QueryFilterBuilder<T>> buildAction)
        {
            var builder = new QueryFilterBuilder<T>(query);
            buildAction?.Invoke(builder);
            return builder.Build();
        }

        #endregion

        #region 性能相关扩展

        /// <summary>
        /// 启用异步跟踪（适用于只读场景）
        /// </summary>
        public static IQueryable<T> AsAsyncTracking<T>(this IQueryable<T> query) where T : class
        {
            return query.AsTracking(); // 或者根据需求返回 AsNoTracking()
        }

        /// <summary>
        /// 启用无跟踪查询（只读场景性能优化）
        /// </summary>
        public static IQueryable<T> AsNoTrackingIf<T>(this IQueryable<T> query, bool condition) where T : class
        {
            return condition ? query.AsNoTracking() : query;
        }

        /// <summary>
        /// 限制查询结果数量（防止过度查询）
        /// </summary>
        public static IQueryable<T> TakeIf<T>(this IQueryable<T> query, bool condition, int count)
        {
            return condition ? query.Take(count) : query;
        }

        ///// <summary>
        ///// 缓存查询结果（需要配合缓存服务）
        ///// </summary>
        //public static async Task<List<T>> ToCachedListAsync<T>(
        //    this IQueryable<T> query,
        //    string cacheKey,
        //    TimeSpan expiration,
        //    ICacheService cacheService,
        //    CancellationToken cancellationToken = default)
        //{
        //    var cachedResult = await cacheService.GetAsync<List<T>>(cacheKey);
        //    if (cachedResult != null)
        //        return cachedResult;

        //    var result = await query.ToListAsync(cancellationToken);
        //    await cacheService.SetAsync(cacheKey, result, expiration);
        //    return result;
        //} 

        #endregion

        #region 同步 FirstOrDefault 扩展

        /// <summary>
        /// 获取序列中的第一个元素，如果序列为空则返回默认值
        /// </summary>
        public static TSource FirstOrDefault<TSource>(
            this IQueryable<TSource> query)
        {
            return System.Linq.Queryable.FirstOrDefault(query);
        }

        /// <summary>
        /// 获取序列中满足条件的第一个元素，如果序列为空则返回默认值
        /// </summary>
        public static TSource FirstOrDefault<TSource>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate)
        {
            return System.Linq.Queryable.FirstOrDefault(query, predicate);
        }

        /// <summary>
        /// 获取序列中的第一个元素（带默认值）
        /// </summary>
        public static TSource FirstOrDefault<TSource>(
            this IQueryable<TSource> query,
            TSource defaultValue)
        {
            return query.FirstOrDefault() ?? defaultValue;
        }

        /// <summary>
        /// 获取序列中满足条件的第一个元素（带默认值）
        /// </summary>
        public static TSource FirstOrDefault<TSource>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            TSource defaultValue)
        {
            return query.FirstOrDefault(predicate) ?? defaultValue;
        }

        /// <summary>
        /// 条件获取第一个元素
        /// </summary>
        public static TSource FirstOrDefaultIf<TSource>(
            this IQueryable<TSource> query,
            bool condition,
            Expression<Func<TSource, bool>> predicate)
        {
            return condition ? query.FirstOrDefault(predicate) : query.FirstOrDefault();
        }

        /// <summary>
        /// 字符串不为空时获取第一个元素
        /// </summary>
        public static TSource FirstOrDefaultIfNotEmpty<TSource>(
            this IQueryable<TSource> query,
            string value,
            Expression<Func<TSource, bool>> predicate)
        {
            return !string.IsNullOrWhiteSpace(value) ? query.FirstOrDefault(predicate) : query.FirstOrDefault();
        }

        /// <summary>
        /// 值不为空时获取第一个元素
        /// </summary>
        public static TSource FirstOrDefaultIfNotNull<TSource, TValue>(
            this IQueryable<TSource> query,
            TValue value,
            Expression<Func<TSource, bool>> predicate)
        {
            return value != null ? query.FirstOrDefault(predicate) : query.FirstOrDefault();
        }

        #endregion

        #region 异步 FirstOrDefaultAsync 扩展

        /// <summary>
        /// 异步获取序列中的第一个元素，如果序列为空则返回默认值
        /// </summary>
        public static Task<TSource> FirstOrDefaultAsync<TSource>(
            this IQueryable<TSource> query,
            CancellationToken cancellationToken = default)
        {
            return EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(query, cancellationToken);
        }

        /// <summary>
        /// 异步获取序列中满足条件的第一个元素，如果序列为空则返回默认值
        /// </summary>
        public static Task<TSource> FirstOrDefaultAsync<TSource>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(query, predicate, cancellationToken);
        }

        /// <summary>
        /// 异步获取序列中的第一个元素（带默认值）
        /// </summary>
        public static async Task<TSource> FirstOrDefaultAsync<TSource>(
            this IQueryable<TSource> query,
            TSource defaultValue,
            CancellationToken cancellationToken = default)
        {
            var result = await query.FirstOrDefaultAsync(cancellationToken);
            return result ?? defaultValue;
        }

        /// <summary>
        /// 异步获取序列中满足条件的第一个元素（带默认值）
        /// </summary>
        public static async Task<TSource> FirstOrDefaultAsync<TSource>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            TSource defaultValue,
            CancellationToken cancellationToken = default)
        {
            var result = await query.FirstOrDefaultAsync(predicate, cancellationToken);
            return result ?? defaultValue;
        }

        /// <summary>
        /// 异步条件获取第一个元素
        /// </summary>
        public static async Task<TSource> FirstOrDefaultIfAsync<TSource>(
            this IQueryable<TSource> query,
            bool condition,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return condition
                ? await query.FirstOrDefaultAsync(predicate, cancellationToken)
                : await query.FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 异步字符串不为空时获取第一个元素
        /// </summary>
        public static async Task<TSource> FirstOrDefaultIfNotEmptyAsync<TSource>(
            this IQueryable<TSource> query,
            string value,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return !string.IsNullOrWhiteSpace(value)
                ? await query.FirstOrDefaultAsync(predicate, cancellationToken)
                : await query.FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 异步值不为空时获取第一个元素
        /// </summary>
        public static async Task<TSource> FirstOrDefaultIfNotNullAsync<TSource, TValue>(
            this IQueryable<TSource> query,
            TValue value,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return value != null
                ? await query.FirstOrDefaultAsync(predicate, cancellationToken)
                : await query.FirstOrDefaultAsync(cancellationToken);
        }

        #endregion

        #region 包含导航属性的 FirstOrDefault 扩展

        /// <summary>
        /// 异步获取第一个元素并包含指定导航属性
        /// </summary>
        public static async Task<TSource> FirstOrDefaultWithIncludeAsync<TSource, TProperty>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, TProperty>> include,
            CancellationToken cancellationToken = default) where TSource : class
        {
            return await query.Include(include).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 异步获取满足条件的第一个元素并包含指定导航属性
        /// </summary>
        public static async Task<TSource> FirstOrDefaultWithIncludeAsync<TSource, TProperty>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TProperty>> include,
            CancellationToken cancellationToken = default) where TSource : class
        {
            return await query.Include(include).FirstOrDefaultAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// 异步获取第一个元素并包含多个导航属性
        /// </summary>
        public static async Task<TSource> FirstOrDefaultWithIncludesAsync<TSource>(
            this IQueryable<TSource> query,
            params Expression<Func<TSource, object>>[] includes) where TSource : class
        {
            var queryWithIncludes = query;
            foreach (var include in includes)
            {
                queryWithIncludes = queryWithIncludes.Include(include);
            }
            return await queryWithIncludes.FirstOrDefaultAsync();
        }

        /// <summary>
        /// 异步获取满足条件的第一个元素并包含多个导航属性
        /// </summary>
        public static async Task<TSource> FirstOrDefaultWithIncludesAsync<TSource>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            params Expression<Func<TSource, object>>[] includes) where TSource : class
        {
            var queryWithIncludes = query;
            foreach (var include in includes)
            {
                queryWithIncludes = queryWithIncludes.Include(include);
            }
            return await queryWithIncludes.FirstOrDefaultAsync(predicate);
        }

        #endregion

        #region 投影 FirstOrDefault 扩展

        /// <summary>
        /// 异步获取第一个元素并投影到目标类型
        /// </summary>
        public static async Task<TDestination> FirstOrDefaultProjectToAsync<TSource, TDestination>(
            this IQueryable<TSource> query,
            IMapper mapper,
            CancellationToken cancellationToken = default)
        {
            return await query.ProjectTo<TDestination>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 异步获取满足条件的第一个元素并投影到目标类型
        /// </summary>
        public static async Task<TDestination> FirstOrDefaultProjectToAsync<TSource, TDestination>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            IMapper mapper,
            CancellationToken cancellationToken = default)
        {
            return await query.Where(predicate)
                .ProjectTo<TDestination>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 异步获取第一个元素并投影到目标类型（带默认值）
        /// </summary>
        public static async Task<TDestination> FirstOrDefaultProjectToAsync<TSource, TDestination>(
            this IQueryable<TSource> query,
            IMapper mapper,
            TDestination defaultValue,
            CancellationToken cancellationToken = default)
        {
            var result = await query.ProjectTo<TDestination>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
            return result ?? defaultValue;
        }

        #endregion

        #region 安全获取单个元素 扩展

        /// <summary>
        /// 安全获取单个元素 - 如果源为空或没有元素则返回默认值
        /// </summary>
        public static async Task<TSource> SafeSingleOrDefaultAsync<TSource>(
            this IQueryable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            if (source == null)
                return default;

            return await source.SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 安全获取单个元素 - 如果源为空或没有满足条件的元素则返回默认值
        /// </summary>
        public static async Task<TSource> SafeSingleOrDefaultAsync<TSource>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            if (source == null)
                return default;

            return await source.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// 安全异步获取第一个元素（处理空查询）
        /// </summary>
        public static async Task<TSource> SafeFirstOrDefaultAsync<TSource>(
            this IQueryable<TSource> query,
            CancellationToken cancellationToken = default)
        {
            if (query == null)
                return default;

            try
            {
                return await query.FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception)
            {
                return default;
            }
        }

        /// <summary>
        /// 安全异步获取满足条件的第一个元素（处理空查询）
        /// </summary>
        public static async Task<TSource> SafeFirstOrDefaultAsync<TSource>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            if (query == null)
                return default;

            try
            {
                return await query.FirstOrDefaultAsync(predicate, cancellationToken);
            }
            catch (Exception)
            {
                return default;
            }
        }

        /// <summary>
        /// 安全异步获取第一个元素（带默认值）
        /// </summary>
        public static async Task<TSource> SafeFirstOrDefaultAsync<TSource>(
            this IQueryable<TSource> query,
            TSource defaultValue,
            CancellationToken cancellationToken = default)
        {
            if (query == null)
                return defaultValue;

            try
            {
                var result = await query.FirstOrDefaultAsync(cancellationToken);
                return result ?? defaultValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        #endregion

        #region 性能监控 FirstOrDefault 扩展

        /// <summary>
        /// 异步获取第一个元素（带性能监控）
        /// </summary>
        public static async Task<TSource> FirstOrDefaultWithPerformanceAsync<TSource>(
            this IQueryable<TSource> query,
            Action<TimeSpan> performanceCallback = null,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var result = await query.FirstOrDefaultAsync(cancellationToken);
                stopwatch.Stop();
                performanceCallback?.Invoke(stopwatch.Elapsed);
                return result;
            }
            catch (Exception)
            {
                stopwatch.Stop();
                throw;
            }
        }

        /// <summary>
        /// 异步获取满足条件的第一个元素（带性能监控）
        /// </summary>
        public static async Task<TSource> FirstOrDefaultWithPerformanceAsync<TSource>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            Action<TimeSpan> performanceCallback = null,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var result = await query.FirstOrDefaultAsync(predicate, cancellationToken);
                stopwatch.Stop();
                performanceCallback?.Invoke(stopwatch.Elapsed);
                return result;
            }
            catch (Exception)
            {
                stopwatch.Stop();
                throw;
            }
        }

        #endregion

        #region 复合 FirstOrDefault 扩展

        /// <summary>
        /// 异步获取第一个元素或抛出异常
        /// </summary>
        public static async Task<TSource> FirstOrFailAsync<TSource>(
            this IQueryable<TSource> query,
            string errorMessage = "未找到指定记录",
            CancellationToken cancellationToken = default)
        {
            var result = await query.FirstOrDefaultAsync(cancellationToken);
            if (result == null)
                throw new InvalidOperationException(errorMessage);

            return result;
        }

        /// <summary>
        /// 异步获取满足条件的第一个元素或抛出异常
        /// </summary>
        public static async Task<TSource> FirstOrFailAsync<TSource>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            string errorMessage = "未找到指定记录",
            CancellationToken cancellationToken = default)
        {
            var result = await query.FirstOrDefaultAsync(predicate, cancellationToken);
            if (result == null)
                throw new InvalidOperationException(errorMessage);

            return result;
        }

        /// <summary>
        /// 异步获取第一个元素或添加新实体
        /// </summary>
        public static async Task<TSource> FirstOrAddAsync<TSource>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            Func<TSource> factory,
            DbContext context,
            CancellationToken cancellationToken = default) where TSource : class
        {
            var result = await query.FirstOrDefaultAsync(predicate, cancellationToken);
            if (result != null)
                return result;

            var newEntity = factory();
            await context.Set<TSource>().AddAsync(newEntity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return newEntity;
        }

        #endregion

        #region First 相关方法

        /// <summary>
        /// 异步返回序列中的第一个元素
        /// </summary>
        public static Task<TSource> FirstAsync<TSource>(
            this IQueryable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            return EntityFrameworkQueryableExtensions.FirstAsync(source, cancellationToken);
        }

        /// <summary>
        /// 异步返回序列中满足指定条件的第一个元素
        /// </summary>
        public static Task<TSource> FirstAsync<TSource>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return EntityFrameworkQueryableExtensions.FirstAsync(source, predicate, cancellationToken);
        }

        #endregion

        #region Single 相关方法

        /// <summary>
        /// 异步返回序列中的唯一元素，如果序列为空或包含多个元素则抛出异常
        /// </summary>
        public static Task<TSource> SingleAsync<TSource>(
            this IQueryable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SingleAsync(source, cancellationToken);
        }

        /// <summary>
        /// 异步返回序列中满足指定条件的唯一元素，如果不存在或存在多个则抛出异常
        /// </summary>
        public static Task<TSource> SingleAsync<TSource>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return EntityFrameworkQueryableExtensions.SingleAsync(source, predicate, cancellationToken);
        }

        /// <summary>
        /// 异步返回序列中的唯一元素，如果序列为空则返回默认值，如果存在多个元素则抛出异常
        /// </summary>
        public static Task<TSource> SingleOrDefaultAsync<TSource>(
            this IQueryable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            return EntityFrameworkQueryableExtensions.SingleOrDefaultAsync(source, cancellationToken);
        }

        /// <summary>
        /// 异步返回序列中满足指定条件的唯一元素，如果不存在则返回默认值，如果存在多个则抛出异常
        /// </summary>
        public static Task<TSource> SingleOrDefaultAsync<TSource>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return EntityFrameworkQueryableExtensions.SingleOrDefaultAsync(source, predicate, cancellationToken);
        }

        #endregion

        #region 其他常用异步方法

        /// <summary>
        /// 异步返回序列中的元素数组
        /// </summary>
        public static Task<TSource[]> ToArrayAsync<TSource>(
            this IQueryable<TSource> source,
            CancellationToken cancellationToken = default)
        {
            return EntityFrameworkQueryableExtensions.ToArrayAsync(source, cancellationToken);
        }
        #endregion
    }
}
