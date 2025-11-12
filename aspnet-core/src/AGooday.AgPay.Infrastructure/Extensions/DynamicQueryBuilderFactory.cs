using System.Reflection;
using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Infrastructure.Extensions
{
    public static class DynamicQueryBuilderFactory
    {
        public static DynamicQueryBuilder<T> Create<T>()
        {
            return new DynamicQueryBuilder<T>();
        }

        public static DynamicQueryBuilder<T> Create<T>(Action<DynamicQueryBuilder<T>> configure)
        {
            var builder = new DynamicQueryBuilder<T>();
            configure(builder);
            return builder;
        }

        // 从 DTO 自动创建查询构建器
        public static DynamicQueryBuilder<T> FromDto<T, TDto>(TDto dto)
        {
            var builder = new DynamicQueryBuilder<T>();
            var dtoType = typeof(TDto);
            var entityType = typeof(T);

            foreach (var dtoProperty in dtoType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = dtoProperty.GetValue(dto);
                if (value == null || IsDefaultValue(value)) continue;

                // 查找对应的实体属性
                var entityProperty = entityType.GetProperty(dtoProperty.Name, BindingFlags.Public | BindingFlags.Instance);
                if (entityProperty != null)
                {
                    builder.Equal(dtoProperty.Name, value);
                }
            }

            return builder;
        }

        private static bool IsDefaultValue(object value)
        {
            if (value == null) return true;

            var type = value.GetType();
            if (type.IsValueType)
            {
                return value.Equals(Activator.CreateInstance(type));
            }

            if (type == typeof(string))
            {
                return string.IsNullOrWhiteSpace((string)value);
            }

            return false;
        }
    }

    //public class DynamicQueryBuilderTsst
    //{
    //    // 支付订单复杂查询
    //    public IQueryable<PayOrder> GetComplexPayOrders(PayOrderComplexQueryDto dto)
    //    {
    //        return DynamicQueryBuilderFactory.Create<PayOrder>(builder =>
    //        {
    //            // 基础条件
    //            builder
    //                .WhereIfNotEmpty(nameof(PayOrderComplexQueryDto.MchNo), dto.MchNo)
    //                .WhereIfNotEmpty("AgentNo", dto.AgentNo)
    //                .WhereIfDateRange("CreatedAt", dto.StartDate, dto.EndDate);

    //            // 金额范围
    //            builder.WhereIfRange("Amount", dto.MinAmount, dto.MaxAmount);

    //            // 状态条件 - 使用 OR 逻辑
    //            if (dto.IncludeStates?.Any() == true)
    //            {
    //                builder.Or(innerBuilder =>
    //                {
    //                    foreach (var state in dto.IncludeStates)
    //                    {
    //                        innerBuilder.Equal("State", state);
    //                    }
    //                });
    //            }

    //            // 排除特定订单
    //            if (dto.ExcludeOrderNos?.Any() == true)
    //            {
    //                builder.NotIn("OrderNo", dto.ExcludeOrderNos);
    //            }

    //            // 复杂字符串搜索
    //            if (!string.IsNullOrWhiteSpace(dto.Keyword))
    //            {
    //                builder.Or(innerBuilder =>
    //                {
    //                    innerBuilder
    //                        .Contains("OrderNo", dto.Keyword)
    //                        .Or(nestedBuilder => nestedBuilder.Contains("MchName", dto.Keyword))
    //                        .Or(nestedBuilder => nestedBuilder.Contains("ProductName", dto.Keyword));
    //                });
    //            }

    //            // 排序
    //            builder
    //                .OrderBy("CreatedAt", true) // 按创建时间降序
    //                .ThenBy("Amount", true);    // 然后按金额降序
    //        }).BuildQuery(_payOrderRepository.GetAllAsNoTracking());
    //    }

    //    // 商户统计查询
    //    public IQueryable<MchInfo> GetMchStatisticsQuery(MchStatisticsQueryDto dto)
    //    {
    //        var builder = DynamicQueryBuilderFactory.Create<MchInfo>();

    //        // 使用扩展方法简化条件构建
    //        builder
    //            .WhereIfNotEmpty("MchNo", dto.MchNo)
    //            .WhereIfContains("MchName", dto.MchName)
    //            .WhereIfNotNull("Type", dto.MchType)
    //            .WhereIfDateRange("CreatedAt", dto.RegStartDate, dto.RegEndDate)
    //            .WhereIfAny("AgentNo", dto.AgentNos)
    //            .WhereIfRange("CreditScore", dto.MinCreditScore, dto.MaxCreditScore);

    //        // 活跃商户条件
    //        if (dto.OnlyActive)
    //        {
    //            builder.And(innerBuilder =>
    //            {
    //                innerBuilder
    //                    .Equal("State", 1) // 启用状态
    //                    .IsNotNull("LastLoginAt"); // 最近有登录
    //            });
    //        }

    //        // 排序配置
    //        builder
    //            .OrderBy("CreatedAt", true)
    //            .ThenBy("CreditScore", true);

    //        return builder.BuildQuery(_mchInfoRepository.GetAllAsNoTracking());
    //    }
    //}
}
