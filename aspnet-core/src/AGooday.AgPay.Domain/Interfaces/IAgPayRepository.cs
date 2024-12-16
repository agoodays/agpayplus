namespace AGooday.AgPay.Domain.Interfaces
{
    /// <summary>
    /// 定义泛型仓储接口，并继承IDisposable，显式释放资源
    /// </summary>
    public interface IAgPayRepository<TEntity, TPrimaryKey> : IBaseRepository<TEntity, TPrimaryKey>
        where TEntity : class
        where TPrimaryKey : struct
    {
    }
    public interface IAgPayRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : class
    {
    }
    public interface IAgPayRepository : IBaseRepository
    {
    }
}
