using System.Linq.Expressions;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Infrastructure.Extensions.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    /// <summary>
    /// 泛型仓储，实现泛型仓储接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class BaseRepository<TEntity, TPrimaryKey> : IBaseRepository<TEntity, TPrimaryKey>
        where TEntity : class
        where TPrimaryKey : struct
    {
        protected readonly DbContext Db;
        protected readonly DbSet<TEntity> DbSet;

        public BaseRepository(DbContext context)
        {
            Db = context;
            DbSet = Db.Set<TEntity>();
        }

        public virtual EntityEntry<TEntity> DbEntry(TEntity entity)
        {
            return Db.Entry(entity);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Add(TEntity entity)
        {
            DbSet.Add(entity);
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task AddAsync(TEntity entity)
        {
            await DbSet.AddAsync(entity);
        }
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
        }
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await DbSet.AddRangeAsync(entities);
        }
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public virtual TEntity GetById(TPrimaryKey id)
        {
            return DbSet.Find(id);
        }
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public virtual async Task<TEntity> GetByIdAsync(TPrimaryKey id)
        {
            return await DbSet.FindAsync(id);
        }
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public virtual TEntity GetByIdAsNoTracking(TPrimaryKey id)
        {
            // 创建完整的 lambda 表达式 e => e.Id == id
            Expression<Func<TEntity, bool>> lambda = BaseRepositoryExtension<TEntity>.GetPrimaryKeyExpression(Db, id);

            // 使用 AsNoTracking 禁用更改追踪，并使用编译后的 lambda 表达式进行查找
            return DbSet.AsNoTracking().FirstOrDefault(lambda);
        }
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public virtual Task<TEntity> GetByIdAsNoTrackingAsync(TPrimaryKey id)
        {
            // 创建完整的 lambda 表达式 e => e.Id == id
            Expression<Func<TEntity, bool>> lambda = BaseRepositoryExtension<TEntity>.GetPrimaryKeyExpression(Db, id);

            // 使用 AsNoTracking 禁用更改追踪，并使用编译后的 lambda 表达式进行查找
            return DbSet.AsNoTracking().FirstOrDefaultAsync(lambda);
        }
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <param name="propertyName">主键名称</param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> GetByIdAsNoTrackingAsync(string propertyName, TPrimaryKey id)
        {
            return await DbSet.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<TPrimaryKey>(e, propertyName).Equals(id));
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetAll()
        {
            return DbSet;
        }
        /// <summary>
        /// 获取列表
        /// 不要追踪（跟踪）从数据库中检索的实体对象
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetAllAsNoTracking()
        {
            // 跟踪与非跟踪查询：https://learn.microsoft.com/zh-cn/ef/core/querying/tracking
            return DbSet.AsNoTracking();
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual IQueryable<T> GetAll<T>() where T : class
        {
            return Db.Set<T>();
        }
        /// <summary>
        /// 获取列表
        /// 不要追踪（跟踪）从数据库中检索的实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual IQueryable<T> GetAllAsNoTracking<T>() where T : class
        {
            // 跟踪与非跟踪查询：https://learn.microsoft.com/zh-cn/ef/core/querying/tracking
            return Db.Set<T>().AsNoTracking();
        }
        /// <summary>
        /// 根据对象进行更新
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(TEntity entity)
        {
            DbSet.Update(entity);
        }
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="entities"></param>
        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            DbSet.UpdateRange(entities);
        }
        /// <summary>
        /// 更新指定实体的指定列
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyExpression"></param>
        public virtual void Update(TEntity entity, Expression<Func<TEntity, object>> propertyExpression)
        {
            // 获取要更新的属性名称列表
            var propertyNames = BaseRepositoryExtension<TEntity>.GetPropertyNames(propertyExpression);

            // 根据主键查找实体
            //var entry = Db.Entry(entity);
            //var keyValues = Db.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties
            //    .Select(x => entry.Property(x.Name).CurrentValue)
            //    .ToArray();
            var keyValues = BaseRepositoryExtension<TEntity>.GetPrimaryKeyValues(Db, entity);
            var existingEntity = DbSet.Find(keyValues);

            // 更新实体的指定属性
            foreach (var propertyName in propertyNames)
            {
                var property = typeof(TEntity).GetProperty(propertyName);
                if (property != null)
                {
                    var newValue = property.GetValue(entity);
                    property.SetValue(existingEntity, newValue);

                    Db.Entry(existingEntity).Property(propertyName).IsModified = true;
                }
            }
        }
        /// <summary>
        /// 更新符合条件的多个实体的指定列
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="propertyExpression"></param>
        public virtual void Update(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, object>> propertyExpression)
        {
            var entitiesToUpdate = DbSet.Where(condition);
            var properties = BaseRepositoryExtension<TEntity>.GetProperties(propertyExpression);

            foreach (var entity in entitiesToUpdate)
            {
                DbSet.Attach(entity);

                foreach (var property in properties)
                {
                    var propertyName = property.Key;
                    var newValue = property.Value;

                    var propertyInfo = typeof(TEntity).GetProperty(propertyName);
                    propertyInfo.SetValue(entity, newValue);

                    Db.Entry(entity).Property(propertyName).IsModified = true;
                }
            }
        }
        /// <summary>
        /// 根据id删除
        /// </summary>
        /// <param name="id">主键ID</param>
        public virtual void Remove(TPrimaryKey id)
        {
            DbSet.Remove(DbSet.Find(id));
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体</param>
        public virtual void Remove(TEntity entity)
        {
            DbSet.Remove(DbSet.Find(entity));
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="entities"></param>
        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
        }
        /// <summary>
        /// 批量删除指定条件实体
        /// </summary>
        /// <param name="condition"></param>
        public virtual void RemoveRange(Expression<Func<TEntity, bool>> condition)
        {
            var entities = DbSet.Where(condition);
            DbSet.RemoveRange(entities);
        }
        /// <summary>
        /// 保存或更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id">主键ID</param>
        public virtual void SaveOrUpdate(TEntity entity, TPrimaryKey? id)
        {
            if (id != null)
                Update(entity);
            else
                Add(entity);
        }
        /// <summary>
        /// 保存或更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertiesToUpdate"></param>
        public virtual void SaveOrUpdate(TEntity entity, params Expression<Func<TEntity, object>>[] propertiesToUpdate)
        {
            var primaryKeyValue = BaseRepositoryExtension<TEntity>.GetPrimaryKeyValues(Db, entity);
            if (primaryKeyValue.Equals(default(TPrimaryKey)))
            {
                DbSet.Add(entity);
            }
            else
            {
                var existingEntity = DbSet.Find(primaryKeyValue);
                if (existingEntity != null)
                {
                    Db.Entry(existingEntity).CurrentValues.SetValues(entity);
                    if (propertiesToUpdate == null || propertiesToUpdate.Length == 0)
                    {
                        Db.Entry(existingEntity).State = EntityState.Modified;
                    }
                    else
                    {
                        foreach (var property in propertiesToUpdate)
                        {
                            var propertyName = BaseRepositoryExtension<TEntity>.GetPropertyName(property);
                            Db.Entry(existingEntity).Property(propertyName).IsModified = true;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entity">实体</param>
        public virtual void AddOrUpdate(TEntity entity)
        {
            var keyValues = BaseRepositoryExtension<TEntity>.GetPrimaryKeyValues(Db, entity);
            var existingEntity = DbSet.Find(keyValues);

            if (existingEntity != null)
            {
                Db.Entry(existingEntity).CurrentValues.SetValues(entity);
                DbSet.Update(existingEntity);
            }
            else
            {
                DbSet.Add(entity);
            }
        }
        /// <summary>
        /// 异步添加或更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public virtual async Task AddOrUpdateAsync(TEntity entity)
        {
            var keyValues = BaseRepositoryExtension<TEntity>.GetPrimaryKeyValues(Db, entity);
            var existingEntity = await DbSet.FindAsync(keyValues);

            if (existingEntity != null)
            {
                Db.Entry(existingEntity).CurrentValues.SetValues(entity);
                DbSet.Update(existingEntity);
            }
            else
            {
                await DbSet.AddAsync(entity);
            }
        }
        /// <summary>
        /// 批量添加或更新
        /// </summary>
        /// <param name="entities">实体集合</param>
        public virtual void AddOrUpdateRange(IEnumerable<TEntity> entities)
        {
            var entitiesToAdd = new List<TEntity>();
            var entitiesToUpdate = new List<TEntity>();

            foreach (var entity in entities)
            {
                var keyValues = BaseRepositoryExtension<TEntity>.GetPrimaryKeyValues(Db, entity);
                var existingEntity = DbSet.Find(keyValues);

                if (existingEntity != null)
                {
                    Db.Entry(existingEntity).CurrentValues.SetValues(entity);
                    entitiesToUpdate.Add(existingEntity);
                }
                else
                {
                    entitiesToAdd.Add(entity);
                }
            }

            if (entitiesToAdd.Any())
            {
                DbSet.AddRange(entitiesToAdd);
            }

            if (entitiesToUpdate.Any())
            {
                DbSet.UpdateRange(entitiesToUpdate);
            }
        }
        /// <summary>
        /// 异步批量添加或更新
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        public virtual async Task AddOrUpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            var entitiesToAdd = new List<TEntity>();
            var entitiesToUpdate = new List<TEntity>();

            foreach (var entity in entities)
            {
                var keyValues = BaseRepositoryExtension<TEntity>.GetPrimaryKeyValues(Db, entity);
                var existingEntity = await DbSet.FindAsync(keyValues);

                if (existingEntity != null)
                {
                    Db.Entry(existingEntity).CurrentValues.SetValues(entity);
                    entitiesToUpdate.Add(existingEntity);
                }
                else
                {
                    entitiesToAdd.Add(entity);
                }
            }

            if (entitiesToAdd.Any())
            {
                await DbSet.AddRangeAsync(entitiesToAdd);
            }

            if (entitiesToUpdate.Any())
            {
                DbSet.UpdateRange(entitiesToUpdate);
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public virtual int SaveChanges()
        {
            return Db.SaveChanges();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public virtual Task<int> SaveChangesAsync()
        {
            return Db.SaveChangesAsync();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="count">影响行数</param>
        /// <returns></returns>
        public virtual bool SaveChanges(out int count)
        {
            count = Db.SaveChanges();
            return count > 0;
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public virtual async Task<(bool result, int count)> SaveChangesWithResultAsync()
        {
            var count = await Db.SaveChangesAsync();
            var result = count > 0;
            return (result, count);
        }

        #region FromSql
        public virtual List<T> FromSql<T>(string sql, object parameters = null)
        {
            return Db.Database.FromSql<T>(sql, parameters);
        }
        public virtual IQueryable<TEntity> FromSql(FormattableString sql)
        {
            return DbSet.FromSql(sql);
        }
        public virtual IQueryable<TEntity> FromSqlAsNoTracking(FormattableString sql)
        {
            return DbSet.FromSql(sql).AsNoTracking();
        }
        public virtual IQueryable<TEntity> FromSqlRaw(string sql, params object[] parameters)
        {
            return DbSet.FromSqlRaw(sql, parameters);
        }
        public virtual IQueryable<TEntity> FromSqlRawAsNoTracking(string sql, params object[] parameters)
        {
            return DbSet.FromSqlRaw(sql, parameters).AsNoTracking();
        }
        public virtual IQueryable<TEntity> FromSqlInterpolated(FormattableString sql)
        {
            return DbSet.FromSqlInterpolated(sql);
        }
        public virtual IQueryable<TEntity> FromSqlInterpolatedAsNoTracking(FormattableString sql)
        {
            return DbSet.FromSqlInterpolated(sql).AsNoTracking();
        }
        #endregion

        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await Db.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
    public class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : class
    {
        protected readonly DbContext Db;
        protected readonly DbSet<TEntity> DbSet;

        public BaseRepository(DbContext context)
        {
            Db = context;
            DbSet = Db.Set<TEntity>();
        }

        public virtual EntityEntry<TEntity> DbEntry(TEntity entity)
        {
            return Db.Entry(entity);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Add(TEntity entity)
        {
            DbSet.Add(entity);
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task AddAsync(TEntity entity)
        {
            await DbSet.AddAsync(entity);
        }
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
        }
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await DbSet.AddRangeAsync(entities);
        }
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public virtual TEntity GetById<TPrimaryKey>(TPrimaryKey id)
        {
            return DbSet.Find(id);
        }
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public virtual async Task<TEntity> GetByIdAsync<TPrimaryKey>(TPrimaryKey id)
        {
            return await DbSet.FindAsync(id);
        }
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public virtual TEntity GetByIdAsNoTracking<TPrimaryKey>(TPrimaryKey id)
        {
            // 创建完整的 lambda 表达式 e => e.Id == id
            Expression<Func<TEntity, bool>> lambda = BaseRepositoryExtension<TEntity>.GetPrimaryKeyExpression(Db, id);

            // 使用 AsNoTracking 禁用更改追踪，并使用编译后的 lambda 表达式进行查找
            return DbSet.AsNoTracking().FirstOrDefault(lambda);
        }
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public virtual Task<TEntity> GetByIdAsNoTrackingAsync<TPrimaryKey>(TPrimaryKey id)
        {
            // 创建完整的 lambda 表达式 e => e.Id == id
            Expression<Func<TEntity, bool>> lambda = BaseRepositoryExtension<TEntity>.GetPrimaryKeyExpression(Db, id);

            // 使用 AsNoTracking 禁用更改追踪，并使用编译后的 lambda 表达式进行查找
            return DbSet.AsNoTracking().FirstOrDefaultAsync(lambda);
        }
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <param name="propertyName">主键名称</param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> GetByIdAsNoTrackingAsync<TPrimaryKey>(string propertyName, TPrimaryKey id)
        {
            return await DbSet.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<TPrimaryKey>(e, propertyName).Equals(id));
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetAll()
        {
            return DbSet;
        }
        /// <summary>
        /// 获取列表
        /// 不要追踪（跟踪）从数据库中检索的实体对象
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetAllAsNoTracking()
        {
            // 跟踪与非跟踪查询：https://learn.microsoft.com/zh-cn/ef/core/querying/tracking
            return DbSet.AsNoTracking();
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual IQueryable<T> GetAll<T>() where T : class
        {
            return Db.Set<T>();
        }
        /// <summary>
        /// 获取列表
        /// 不要追踪（跟踪）从数据库中检索的实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual IQueryable<T> GetAllAsNoTracking<T>() where T : class
        {
            // 跟踪与非跟踪查询：https://learn.microsoft.com/zh-cn/ef/core/querying/tracking
            return Db.Set<T>().AsNoTracking();
        }
        /// <summary>
        /// 根据对象进行更新
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(TEntity entity)
        {
            DbSet.Update(entity);
        }
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="entities"></param>
        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            DbSet.UpdateRange(entities);
        }
        /// <summary>
        /// 更新指定实体的指定列
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyExpression"></param>
        public virtual void Update(TEntity entity, Expression<Func<TEntity, object>> propertyExpression)
        {
            // 获取要更新的属性名称列表
            var propertyNames = BaseRepositoryExtension<TEntity>.GetPropertyNames(propertyExpression);

            // 根据主键查找实体
            var entry = Db.Entry(entity);
            var keyValues = Db.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties
                .Select(x => entry.Property(x.Name).CurrentValue)
                .ToArray();
            var existingEntity = DbSet.Find(keyValues);

            // 更新实体的指定属性
            foreach (var propertyName in propertyNames)
            {
                var property = typeof(TEntity).GetProperty(propertyName);
                if (property != null)
                {
                    var newValue = property.GetValue(entity);
                    property.SetValue(existingEntity, newValue);
                }
            }
        }
        /// <summary>
        /// 更新符合条件的多个实体的指定列
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="propertyExpression"></param>
        public virtual void Update(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, object>> propertyExpression)
        {
            var entitiesToUpdate = DbSet.Where(condition);
            var properties = BaseRepositoryExtension<TEntity>.GetProperties(propertyExpression);

            foreach (var entity in entitiesToUpdate)
            {
                DbSet.Attach(entity);

                foreach (var property in properties)
                {
                    var propertyName = property.Key;
                    var newValue = property.Value;

                    var propertyInfo = typeof(TEntity).GetProperty(propertyName);
                    propertyInfo.SetValue(entity, newValue);

                    Db.Entry(entity).Property(propertyName).IsModified = true;
                }
            }
        }
        /// <summary>
        /// 根据id删除
        /// </summary>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="id">主键ID</param>
        public virtual void Remove<TPrimaryKey>(TPrimaryKey id)
        {
            DbSet.Remove(DbSet.Find(id));
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体</param>
        public virtual void Remove(TEntity entity)
        {
            DbSet.Remove(DbSet.Find(entity));
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="entities"></param>
        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
        }
        /// <summary>
        /// 批量删除指定条件实体
        /// </summary>
        /// <param name="condition"></param>
        public virtual void RemoveRange(Expression<Func<TEntity, bool>> condition)
        {
            var entities = DbSet.Where(condition);
            DbSet.RemoveRange(entities);
        }
        /// <summary>
        /// 保存或更新
        /// </summary>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        public virtual void SaveOrUpdate<TPrimaryKey>(TEntity entity, TPrimaryKey id)
        {
            if (id != null)
                Update(entity);
            else
                Add(entity);
        }
        /// <summary>
        /// 保存或更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertiesToUpdate"></param>
        public virtual void SaveOrUpdate<TPrimaryKey>(TEntity entity, params Expression<Func<TEntity, object>>[] propertiesToUpdate)
        {
            var primaryKeyValue = BaseRepositoryExtension<TEntity>.GetPrimaryKeyValues(Db, entity);
            if (primaryKeyValue.Equals(default(TPrimaryKey)))
            {
                DbSet.Add(entity);
            }
            else
            {
                var existingEntity = DbSet.Find(primaryKeyValue);
                if (existingEntity != null)
                {
                    Db.Entry(existingEntity).CurrentValues.SetValues(entity);
                    if (propertiesToUpdate == null || propertiesToUpdate.Length == 0)
                    {
                        Db.Entry(existingEntity).State = EntityState.Modified;
                    }
                    else
                    {
                        foreach (var property in propertiesToUpdate)
                        {
                            var propertyName = BaseRepositoryExtension<TEntity>.GetPropertyName(property);
                            Db.Entry(existingEntity).Property(propertyName).IsModified = true;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entity">实体</param>
        public virtual void AddOrUpdate(TEntity entity)
        {
            var keyValues = BaseRepositoryExtension<TEntity>.GetPrimaryKeyValues(Db, entity);
            var existingEntity = DbSet.Find(keyValues);

            if (existingEntity != null)
            {
                Db.Entry(existingEntity).CurrentValues.SetValues(entity);
                DbSet.Update(existingEntity);
            }
            else
            {
                DbSet.Add(entity);
            }
        }
        /// <summary>
        /// 异步添加或更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public virtual async Task AddOrUpdateAsync(TEntity entity)
        {
            var keyValues = BaseRepositoryExtension<TEntity>.GetPrimaryKeyValues(Db, entity);
            var existingEntity = await DbSet.FindAsync(keyValues);

            if (existingEntity != null)
            {
                Db.Entry(existingEntity).CurrentValues.SetValues(entity);
                DbSet.Update(existingEntity);
            }
            else
            {
                await DbSet.AddAsync(entity);
            }
        }
        /// <summary>
        /// 批量添加或更新
        /// </summary>
        /// <param name="entities">实体集合</param>
        public virtual void AddOrUpdateRange(IEnumerable<TEntity> entities)
        {
            var entitiesToAdd = new List<TEntity>();
            var entitiesToUpdate = new List<TEntity>();

            foreach (var entity in entities)
            {
                var keyValues = BaseRepositoryExtension<TEntity>.GetPrimaryKeyValues(Db, entity);
                var existingEntity = DbSet.Find(keyValues);

                if (existingEntity != null)
                {
                    Db.Entry(existingEntity).CurrentValues.SetValues(entity);
                    entitiesToUpdate.Add(existingEntity);
                }
                else
                {
                    entitiesToAdd.Add(entity);
                }
            }

            if (entitiesToAdd.Any())
            {
                DbSet.AddRange(entitiesToAdd);
            }

            if (entitiesToUpdate.Any())
            {
                DbSet.UpdateRange(entitiesToUpdate);
            }
        }
        /// <summary>
        /// 异步批量添加或更新
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        public virtual async Task AddOrUpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            var entitiesToAdd = new List<TEntity>();
            var entitiesToUpdate = new List<TEntity>();

            foreach (var entity in entities)
            {
                var keyValues = BaseRepositoryExtension<TEntity>.GetPrimaryKeyValues(Db, entity);
                var existingEntity = await DbSet.FindAsync(keyValues);

                if (existingEntity != null)
                {
                    Db.Entry(existingEntity).CurrentValues.SetValues(entity);
                    entitiesToUpdate.Add(existingEntity);
                }
                else
                {
                    entitiesToAdd.Add(entity);
                }
            }

            if (entitiesToAdd.Any())
            {
                await DbSet.AddRangeAsync(entitiesToAdd);
            }

            if (entitiesToUpdate.Any())
            {
                DbSet.UpdateRange(entitiesToUpdate);
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public virtual int SaveChanges()
        {
            return Db.SaveChanges();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public virtual Task<int> SaveChangesAsync()
        {
            return Db.SaveChangesAsync();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="count">影响行数</param>
        /// <returns></returns>
        public virtual bool SaveChanges(out int count)
        {
            count = Db.SaveChanges();
            return count > 0;
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public virtual async Task<(bool result, int count)> SaveChangesWithResultAsync()
        {
            var count = await Db.SaveChangesAsync();
            var result = count > 0;
            return (result, count);
        }

        #region FromSql
        public virtual List<T> FromSql<T>(string sql, object parameters = null)
        {
            return Db.Database.FromSql<T>(sql, parameters);
        }
        public virtual IQueryable<TEntity> FromSql(FormattableString sql)
        {
            return DbSet.FromSql(sql);
        }
        public virtual IQueryable<TEntity> FromSqlAsNoTracking(FormattableString sql)
        {
            return DbSet.FromSql(sql).AsNoTracking();
        }
        public virtual IQueryable<TEntity> FromSqlRaw(string sql, params object[] parameters)
        {
            return DbSet.FromSqlRaw(sql, parameters);
        }
        public virtual IQueryable<TEntity> FromSqlRawAsNoTracking(string sql, params object[] parameters)
        {
            return DbSet.FromSqlRaw(sql, parameters).AsNoTracking();
        }
        public virtual IQueryable<TEntity> FromSqlInterpolated(FormattableString sql)
        {
            return DbSet.FromSqlInterpolated(sql);
        }
        public virtual IQueryable<TEntity> FromSqlInterpolatedAsNoTracking(FormattableString sql)
        {
            return DbSet.FromSqlInterpolated(sql).AsNoTracking();
        }
        #endregion

        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await Db.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
    public class BaseRepository : IBaseRepository
    {
        protected readonly DbContext Db;
        public BaseRepository(DbContext context)
        {
            Db = context;
        }
        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await Db.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
    public static class BaseRepositoryExtension<TEntity>
        where TEntity : class
    {
        public static Expression<Func<TEntity, bool>> GetPrimaryKeyExpression<TPrimaryKey>(DbContext Db, TPrimaryKey id)
        {
            // 获取实体类型信息
            var entityType = Db.Model.FindEntityType(typeof(TEntity));

            // 获取主键属性
            var keyProperty = entityType.FindPrimaryKey().Properties.First();

            // 构建参数表达式
            var parameter = Expression.Parameter(typeof(TEntity), "e");

            // 构建成员访问表达式以访问主键属性
            var propertyAccess = Expression.MakeMemberAccess(parameter, keyProperty.PropertyInfo);

            // 将主键属性值转换为正确的类型进行比较
            var constantValue = Expression.Constant(id, typeof(TPrimaryKey));

            // 创建相等表达式 e.Id == id
            var equalityExpression = Expression.Equal(propertyAccess, Expression.Convert(constantValue, propertyAccess.Type));

            // 创建完整的 lambda 表达式 e => e.Id == id
            var lambda = Expression.Lambda<Func<TEntity, bool>>(equalityExpression, parameter);
            return lambda;
        }

        public static object[] GetPrimaryKeyValues(DbContext Db, TEntity entity)
        {
            var entityType = Db.Model.FindEntityType(typeof(TEntity));
            var primaryKey = entityType.FindPrimaryKey();
            if (primaryKey != null)
            {
                var primaryKeyProperties = primaryKey.Properties;
                var primaryKeyValues = primaryKeyProperties.Select(p => p.GetGetter().GetClrValue(entity)).ToArray();
                return primaryKeyValues;
            }
            throw new InvalidOperationException("Entity does not have a primary key defined.");
        }

        public static string GetPropertyName(Expression<Func<TEntity, object>> propertyExpression)
        {
            if (propertyExpression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            throw new InvalidOperationException("Invalid property expression.");
        }

        public static string[] GetPropertyNames(Expression<Func<TEntity, object>> expression)
        {
            if (expression.Body is not NewExpression newExpression)
            {
                throw new ArgumentException("Invalid expression. Only new expressions are supported.");
            }

            var propertyNames = newExpression?.Members
                .Select(member => member.Name)
                .ToArray();

            return propertyNames;
        }
        public static Dictionary<string, object> GetProperties(Expression<Func<TEntity, object>> expression)
        {
            var properties = new Dictionary<string, object>();
            var body = expression.Body;

            if (body is NewExpression newExpression)
            {
                var memberNames = newExpression.Members.Select(m => m.Name).ToList();
                var memberValues = newExpression.Arguments.Select(arg => Expression.Lambda(arg).Compile().DynamicInvoke()).ToList();

                for (var i = 0; i < memberNames.Count; i++)
                {
                    properties.Add(memberNames[i], memberValues[i]);
                }
            }
            else
            {
                throw new ArgumentException("Invalid property expression");
            }

            return properties;
        }
    }
}
