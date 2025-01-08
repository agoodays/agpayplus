using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace AGooday.AgPay.Infrastructure.UoW
{
    /// <summary>
    /// 工作单元类
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        //数据库上下文
        private readonly AgPayDbContext _dbcontext;
        private IDbContextTransaction _transaction;

        //构造函数注入
        public UnitOfWork(AgPayDbContext context)
        {
            _dbcontext = context;
        }

        public void BeginTransaction()
        {
            _transaction = _dbcontext.Database.BeginTransaction();
        }
        public async Task BeginTransactionAsync()
        {
            _transaction = await _dbcontext.Database.BeginTransactionAsync();
        }

        public void CommitTransaction()
        {
            _transaction.Commit();
        }

        public async Task CommitTransactionAsync()
        {
            await _transaction.CommitAsync();
        }

        public void RollbackTransaction()
        {
            _transaction.Rollback();
        }

        public async Task RollbackTransactionAsync()
        {
            await _transaction.RollbackAsync();
        }

        //上下文提交
        public bool Commit()
        {
            return _dbcontext.SaveChanges() > 0;
        }

        public async Task<bool> CommitAsync()
        {
            return await _dbcontext.SaveChangesAsync() > 0;
        }

        //手动回收
        public void Dispose()
        {
            _dbcontext.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await _dbcontext.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}
