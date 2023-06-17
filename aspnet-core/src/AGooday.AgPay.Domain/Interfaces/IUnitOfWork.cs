namespace AGooday.AgPay.Domain.Interfaces
{
    /// <summary>
    /// 工作单元接口
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        //是否提交成功
        bool Commit();
        Task<bool> CommitAsync();
    }
}
