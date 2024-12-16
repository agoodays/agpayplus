using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    /// <summary>
    /// 泛型仓储，实现泛型仓储接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class AgPayRepository<TEntity, TPrimaryKey> : BaseRepository<TEntity, TPrimaryKey>, IAgPayRepository<TEntity, TPrimaryKey>
        where TEntity : class
        where TPrimaryKey : struct
    {
        public AgPayRepository(AgPayDbContext context)
            : base(context)
        {
        }
    }
    public class AgPayRepository<TEntity> : BaseRepository<TEntity>, IAgPayRepository<TEntity>
        where TEntity : class
    {
        public AgPayRepository(AgPayDbContext context)
            : base(context)
        {
        }
    }
    public class AgPayRepository : BaseRepository, IAgPayRepository
    {
        public AgPayRepository(AgPayDbContext context)
            : base(context)
        {
        }
    }
}
