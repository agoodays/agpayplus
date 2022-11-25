using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Reflection;

namespace AGooday.AgPay.Infrastructure.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class QueryableExtension
    {
        /// <summary>
        /// IQueryable分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IQueryable<T> Paged<T>(this IQueryable<T> query, int currentPage = 1, int pageSize = 20)
        {
            if (currentPage < 1)
            {
                currentPage = 1;
            }
            query = query.Skip((currentPage - 1) * pageSize).Take(pageSize);
            return query;
        }


        private static readonly TypeInfo QueryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();

        private static readonly FieldInfo QueryCompilerField = typeof(EntityQueryProvider).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryCompiler");
        private static readonly FieldInfo QueryModelGeneratorField = typeof(QueryCompiler).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryModelGenerator");
        private static readonly FieldInfo DataBaseField = QueryCompilerTypeInfo.DeclaredFields.Single(x => x.Name == "_database");
        private static readonly PropertyInfo DatabaseDependenciesField = typeof(Microsoft.EntityFrameworkCore.DbLoggerCategory.Database).GetTypeInfo().DeclaredProperties.Single(x => x.Name == "Dependencies");
    }
}
