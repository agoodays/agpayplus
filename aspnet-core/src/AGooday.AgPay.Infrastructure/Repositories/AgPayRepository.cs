using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    /// <summary>
    /// 泛型仓储，实现泛型仓储接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class AgPayRepository<TEntity, TPrimaryKey> : Repository<TEntity, TPrimaryKey>
        where TEntity : class
        where TPrimaryKey : struct
    {
        public AgPayRepository(AgPayDbContext context)
            : base(context)
        {
        }
    }
    public class AgPayRepository<TEntity> : Repository<TEntity>
        where TEntity : class
    {
        public AgPayRepository(AgPayDbContext context)
            : base(context)
        {
        }
    }
    public class AgPayRepository : Repository
    {
        public AgPayRepository(AgPayDbContext context)
            : base(context)
        {
        }
    }
}
