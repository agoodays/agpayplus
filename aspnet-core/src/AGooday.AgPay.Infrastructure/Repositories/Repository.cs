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
    public class Repository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class
        where TPrimaryKey : struct
    {
        protected readonly DbContext Db;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(DbContext context)
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
        /// 更新指定实体的指定列
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyExpression"></param>
        public void Update(TEntity entity, Expression<Func<TEntity, object>> propertyExpression)
        {
            // 获取要更新的属性名称列表
            var propertyNames = RepositoryExtension<TEntity>.GetPropertyNames(propertyExpression);

            // 根据主键查找实体
            //var entry = Db.Entry(entity);
            //var keyValues = Db.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties
            //    .Select(x => entry.Property(x.Name).CurrentValue)
            //    .ToArray();
            var keyValues = RepositoryExtension<TEntity>.GetPrimaryKeyValues(Db, entity);
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
        public void Update(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, object>> propertyExpression)
        {
            var entitiesToUpdate = DbSet.Where(condition);
            var properties = RepositoryExtension<TEntity>.GetProperties(propertyExpression);

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
        public void SaveOrUpdate(TEntity entity, params Expression<Func<TEntity, object>>[] propertiesToUpdate)
        {
            var primaryKeyValue = RepositoryExtension<TEntity>.GetPrimaryKeyValues(Db, entity);
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
                            var propertyName = RepositoryExtension<TEntity>.GetPropertyName(property);
                            Db.Entry(existingEntity).Property(propertyName).IsModified = true;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public int SaveChanges()
        {
            return Db.SaveChanges();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveChangesAsync()
        {
            return await Db.SaveChangesAsync();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="count">影响行数</param>
        /// <returns></returns>
        public bool SaveChanges(out int count)
        {
            count = Db.SaveChanges();
            return count > 0;
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
        public virtual IQueryable<TEntity> FromSqlRaw(string sql, object parameters = null)
        {
            return DbSet.FromSqlRaw(sql, parameters);
        }
        public virtual IQueryable<TEntity> FromSqlInterpolated(FormattableString sql)
        {
            return DbSet.FromSqlInterpolated(sql);
        }
        #endregion

        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected readonly DbContext Db;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(DbContext context)
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
        /// <param name="obj"></param>
        public virtual void Update(TEntity obj)
        {
            DbSet.Update(obj);
        }
        /// <summary>
        /// 更新指定实体的指定列
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyExpression"></param>
        public void Update(TEntity entity, Expression<Func<TEntity, object>> propertyExpression)
        {
            // 获取要更新的属性名称列表
            var propertyNames = RepositoryExtension<TEntity>.GetPropertyNames(propertyExpression);

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
        public void Update(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, object>> propertyExpression)
        {
            var entitiesToUpdate = DbSet.Where(condition);
            var properties = RepositoryExtension<TEntity>.GetProperties(propertyExpression);

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
        /// 保存或更新
        /// </summary>
        /// <returns></returns>
        public virtual void SaveOrUpdate<TPrimaryKey>(TEntity obj, TPrimaryKey id)
        {
            var entity = DbSet.Find(id);
            if (id != null && entity != null)
                Update(obj);
            else
                Add(obj);
        }
        /// <summary>
        /// 保存或更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertiesToUpdate"></param>
        public void SaveOrUpdate<TPrimaryKey>(TEntity entity, params Expression<Func<TEntity, object>>[] propertiesToUpdate)
        {
            var primaryKeyValue = RepositoryExtension<TEntity>.GetPrimaryKeyValues(Db, entity);
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
                            var propertyName = RepositoryExtension<TEntity>.GetPropertyName(property);
                            Db.Entry(existingEntity).Property(propertyName).IsModified = true;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public int SaveChanges()
        {
            return Db.SaveChanges();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveChangesAsync()
        {
            return await Db.SaveChangesAsync();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="count">影响行数</param>
        /// <returns></returns>
        public bool SaveChanges(out int count)
        {
            count = Db.SaveChanges();
            return count > 0;
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
        public virtual IQueryable<TEntity> FromSqlRaw(string sql, object parameters = null)
        {
            return DbSet.FromSqlRaw(sql, parameters);
        }
        public virtual IQueryable<TEntity> FromSqlInterpolated(FormattableString sql)
        {
            return DbSet.FromSqlInterpolated(sql);
        }
        #endregion

        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
    public class Repository : IRepository
    {
        protected readonly DbContext Db;
        public Repository(DbContext context)
        {
            Db = context;
        }
        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
    public static class RepositoryExtension<TEntity>
        where TEntity : class
    {
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
