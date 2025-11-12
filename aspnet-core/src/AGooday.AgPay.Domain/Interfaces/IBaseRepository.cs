using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AGooday.AgPay.Domain.Interfaces
{
    /// <summary>
    /// 定义泛型仓储接口，并继承IDisposable，显式释放资源
    /// </summary>
    public interface IBaseRepository<TEntity, TPrimaryKey> : IDisposable, IAsyncDisposable
        where TEntity : class
        where TPrimaryKey : struct
    {
        EntityEntry<TEntity> DbEntry(TEntity entity);
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
        /// 批量添加
        /// </summary>
        /// <param name="entities"></param>
        void AddRange(IEnumerable<TEntity> entities);
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task AddRangeAsync(IEnumerable<TEntity> entities);
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
        /// 根据id获取对象
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        TEntity GetByIdAsNoTracking(TPrimaryKey id);
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        Task<TEntity> GetByIdAsNoTrackingAsync(TPrimaryKey id);
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
        /// 根据条件获取第一个实体
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 根据条件获取第一个实体
        /// 不要追踪（跟踪）从数据库中检索的实体对象
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        Task<TEntity> FirstOrDefaultAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 根据对象进行更新
        /// </summary>
        /// <param name="entity"></param>
        void Update(TEntity entity);
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="entities"></param>
        void UpdateRange(IEnumerable<TEntity> entities);
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
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体</param>
        void Remove(TEntity entity);
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="entities"></param>
        void RemoveRange(IEnumerable<TEntity> entities);
        /// <summary>
        /// 批量删除指定条件实体
        /// </summary>
        /// <param name="condition"></param>
        void RemoveRange(Expression<Func<TEntity, bool>> condition);
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
        /// 添加或更新
        /// </summary>
        /// <param name="entity">实体</param>
        void AddOrUpdate(TEntity entity);
        /// <summary>
        /// 异步添加或更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task AddOrUpdateAsync(TEntity entity);
        /// <summary>
        /// 批量添加或更新
        /// </summary>
        /// <param name="entities">实体集合</param>
        void AddOrUpdateRange(IEnumerable<TEntity> entities);
        /// <summary>
        /// 异步批量添加或更新
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        Task AddOrUpdateRangeAsync(IEnumerable<TEntity> entities);
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
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        Task<(bool result, int count)> SaveChangesWithResultAsync();

        #region FromSql
        List<T> FromSql<T>(string sql, object parameters = null);
        IQueryable<TEntity> FromSql(FormattableString sql);
        IQueryable<TEntity> FromSqlAsNoTracking(FormattableString sql);
        IQueryable<TEntity> FromSqlRaw(string sql, params object[] parameters);
        IQueryable<TEntity> FromSqlRawAsNoTracking(string sql, params object[] parameters);
        IQueryable<TEntity> FromSqlInterpolated(FormattableString sql);
        IQueryable<TEntity> FromSqlInterpolatedAsNoTracking(FormattableString sql);
        #endregion
    }
    public interface IBaseRepository<TEntity> : IDisposable, IAsyncDisposable
        where TEntity : class
    {
        EntityEntry<TEntity> DbEntry(TEntity entity);
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        void Add(TEntity entity);
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        Task AddAsync(TEntity entity);
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities"></param>
        void AddRange(IEnumerable<TEntity> entities);
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        TEntity GetById<TPrimaryKey>(TPrimaryKey id);
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        Task<TEntity> GetByIdAsync<TPrimaryKey>(TPrimaryKey id);
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        TEntity GetByIdAsNoTracking<TPrimaryKey>(TPrimaryKey id);
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TEntity> GetByIdAsNoTrackingAsync<TPrimaryKey>(TPrimaryKey id);
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
        /// 根据条件获取第一个实体
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 根据条件获取第一个实体
        /// 不要追踪（跟踪）从数据库中检索的实体对象
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        Task<TEntity> FirstOrDefaultAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 根据对象进行更新
        /// </summary>
        /// <param name="entity"></param>
        void Update(TEntity entity);
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="entities"></param>
        void UpdateRange(IEnumerable<TEntity> entities);
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
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体</param>
        void Remove(TEntity entity);
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="entities"></param>
        void RemoveRange(IEnumerable<TEntity> entities);
        /// <summary>
        /// 批量删除指定条件实体
        /// </summary>
        /// <param name="condition"></param>
        void RemoveRange(Expression<Func<TEntity, bool>> condition);
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
        /// 添加或更新
        /// </summary>
        /// <param name="entity">实体</param>
        void AddOrUpdate(TEntity entity);
        /// <summary>
        /// 异步添加或更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task AddOrUpdateAsync(TEntity entity);
        /// <summary>
        /// 批量添加或更新
        /// </summary>
        /// <param name="entities">实体集合</param>
        void AddOrUpdateRange(IEnumerable<TEntity> entities);
        /// <summary>
        /// 异步批量添加或更新
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        Task AddOrUpdateRangeAsync(IEnumerable<TEntity> entities);
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
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        Task<(bool result, int count)> SaveChangesWithResultAsync();

        #region FromSql
        List<T> FromSql<T>(string sql, object parameters = null);
        IQueryable<TEntity> FromSql(FormattableString sql);
        IQueryable<TEntity> FromSqlAsNoTracking(FormattableString sql);
        IQueryable<TEntity> FromSqlRaw(string sql, params object[] parameters);
        IQueryable<TEntity> FromSqlRawAsNoTracking(string sql, params object[] parameters);
        IQueryable<TEntity> FromSqlInterpolated(FormattableString sql);
        IQueryable<TEntity> FromSqlInterpolatedAsNoTracking(FormattableString sql);
        #endregion
    }
    public interface IBaseRepository : IDisposable, IAsyncDisposable
    {
    }
}
