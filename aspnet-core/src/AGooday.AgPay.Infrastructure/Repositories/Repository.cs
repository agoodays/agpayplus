using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        protected readonly AgPayDbContext Db;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(AgPayDbContext context)
        {
            Db = context;
            DbSet = Db.Set<TEntity>();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="obj"></param>
        public virtual void Add(TEntity obj)
        {
            DbSet.Add(obj);
        }
        public virtual async Task AddAsync(TEntity entity)
        {
            await DbSet.AddAsync(entity);
        }
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity GetById(TPrimaryKey id)
        {
            return DbSet.Find(id);
        }
        public virtual async Task<TEntity> FindByIdAsync(TPrimaryKey id)
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
        public virtual IQueryable<T> GetAll<T>() where T : class
        {
            return Db.Set<T>();
        }
        public virtual async Task<IEnumerable<TEntity>> ListAsync()
        {
            return await DbSet.AsNoTracking().ToListAsync();
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
        /// 根据id删除
        /// </summary>
        /// <param name="id"></param>
        public virtual void Remove(TPrimaryKey id)
        {
            DbSet.Remove(DbSet.Find(id));
        }
        /// <summary>
        /// 保存或更新
        /// </summary>
        /// <returns></returns>
        public virtual void SaveOrUpdate(TEntity obj, TPrimaryKey? id)
        {
            var entity = DbSet.Find(id);
            if (id != null && entity != null)
                Update(obj);
            else
                Add(obj);
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
        /// <param name="count">影响行数</param>
        /// <returns></returns>
        public bool SaveChanges(out int count)
        {
            count = Db.SaveChanges();
            return count > 0;
        }

        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected readonly AgPayDbContext Db;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(AgPayDbContext context)
        {
            Db = context;
            DbSet = Db.Set<TEntity>();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="obj"></param>
        public virtual void Add(TEntity obj)
        {
            DbSet.Add(obj);
        }
        public virtual async Task AddAsync(TEntity entity)
        {
            await DbSet.AddAsync(entity);
        }
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity GetById<TPrimaryKey>(TPrimaryKey id)
        {
            return DbSet.Find(id);
        }
        public virtual async Task<TEntity> FindByIdAsync<TPrimaryKey>(TPrimaryKey id)
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
        public virtual IQueryable<T> GetAll<T>() where T : class
        {
            return Db.Set<T>();
        }
        public virtual async Task<IEnumerable<TEntity>> ListAsync()
        {
            return await DbSet.AsNoTracking().ToListAsync();
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
        /// 根据id删除
        /// </summary>
        /// <param name="id"></param>
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
        /// <param name="count">影响行数</param>
        /// <returns></returns>
        public bool SaveChanges(out int count)
        {
            count = Db.SaveChanges();
            return count > 0;
        }

        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
    public class Repository : IRepository
    {
        protected readonly AgPayDbContext Db;
        public Repository(AgPayDbContext context)
        {
            Db = context;
        }
        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
