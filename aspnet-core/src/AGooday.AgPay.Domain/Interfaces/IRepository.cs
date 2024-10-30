using System.Linq.Expressions;

namespace AGooday.AgPay.Domain.Interfaces
{
    /// <summary>
    /// 定义泛型仓储接口，并继承IDisposable，显式释放资源
    /// </summary>
    public interface IRepository<TEntity, TPrimaryKey> : IDisposable
        where TEntity : class
        where TPrimaryKey : struct
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        void Add(TEntity entity);
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task AddAsync(TEntity entity);
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        TEntity GetById(TPrimaryKey id);
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        Task<TEntity> GetByIdAsync(TPrimaryKey id);
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> GetAll();
        /// <summary>
        /// 获取列表
        /// 不要追踪（跟踪）从数据库中检索的实体对象
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> GetAllAsNoTracking();
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IQueryable<T> GetAll<T>() where T : class;
        /// <summary>
        /// 获取列表
        /// 不要追踪（跟踪）从数据库中检索的实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IQueryable<T> GetAllAsNoTracking<T>() where T : class;
        /// <summary>
        /// 根据对象进行更新
        /// </summary>
        /// <param name="entity"></param>
        void Update(TEntity entity);
        /// <summary>
        /// 更新指定实体的指定列
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyExpression"></param>
        void Update(TEntity entity, Expression<Func<TEntity, object>> propertyExpression);
        /// <summary>
        /// 更新符合条件的多个实体的指定列
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="propertyExpression"></param>
        void Update(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, object>> propertyExpression);
        /// <summary>
        /// 根据id删除
        /// </summary>
        /// <param name="id"></param>
        void Remove(TPrimaryKey id);
        /// <summary>
        /// 保存或更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        void SaveOrUpdate(TEntity entity, TPrimaryKey? id);
        /// <summary>
        /// 保存或更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertiesToUpdate"></param>
        void SaveOrUpdate(TEntity entity, params Expression<Func<TEntity, object>>[] propertiesToUpdate);
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync();
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        bool SaveChanges(out int count);

        #region FromSql
        List<T> FromSql<T>(string sql, object parameters = null);
        IQueryable<TEntity> FromSql(FormattableString sql);
        IQueryable<TEntity> FromSqlRaw(string sql, object parameters = null);
        IQueryable<TEntity> FromSqlInterpolated(FormattableString sql);
        #endregion
    }
    public interface IRepository<TEntity> : IDisposable
        where TEntity : class
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        void Add(TEntity entity);
        Task AddAsync(TEntity entity);
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity GetById<TPrimaryKey>(TPrimaryKey id);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TEntity> GetByIdAsync<TPrimaryKey>(TPrimaryKey id);
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> GetAll();
        /// <summary>
        /// 获取列表
        /// 不要追踪（跟踪）从数据库中检索的实体对象
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> GetAllAsNoTracking();
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IQueryable<T> GetAll<T>() where T : class;
        /// <summary>
        /// 获取列表
        /// 不要追踪（跟踪）从数据库中检索的实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IQueryable<T> GetAllAsNoTracking<T>() where T : class;
        /// <summary>
        /// 根据对象进行更新
        /// </summary>
        /// <param name="entity"></param>
        void Update(TEntity entity);
        /// <summary>
        /// 更新指定实体的指定列
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyExpression"></param>
        void Update(TEntity entity, Expression<Func<TEntity, object>> propertyExpression);
        /// <summary>
        /// 更新符合条件的多个实体的指定列
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="propertyExpression"></param>
        void Update(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, object>> propertyExpression);
        /// <summary>
        /// 根据id删除
        /// </summary>
        /// <param name="id"></param>
        void Remove<TPrimaryKey>(TPrimaryKey id);
        /// <summary>
        /// 保存或更新
        /// </summary>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        void SaveOrUpdate<TPrimaryKey>(TEntity entity, TPrimaryKey id);
        /// <summary>
        /// 保存或更新
        /// </summary>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="entity"></param>
        /// <param name="propertiesToUpdate"></param>
        void SaveOrUpdate<TPrimaryKey>(TEntity entity, params Expression<Func<TEntity, object>>[] propertiesToUpdate);
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync();
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        bool SaveChanges(out int count);

        #region FromSql
        List<T> FromSql<T>(string sql, object parameters = null);
        IQueryable<TEntity> FromSql(FormattableString sql);
        IQueryable<TEntity> FromSqlRaw(string sql, object parameters = null);
        IQueryable<TEntity> FromSqlInterpolated(FormattableString sql);
        #endregion
    }
    public interface IRepository : IDisposable
    {
    }
}
