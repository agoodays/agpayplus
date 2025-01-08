namespace AGooday.AgPay.Domain.Interfaces
{
    /// <summary>
    /// 工作单元接口
    /// </summary>
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        void BeginTransaction();
        Task BeginTransactionAsync();
        void CommitTransaction();
        Task CommitTransactionAsync();
        void RollbackTransaction();
        Task RollbackTransactionAsync();
        //是否提交成功
        bool Commit();
        Task<bool> CommitAsync();
    }
}
