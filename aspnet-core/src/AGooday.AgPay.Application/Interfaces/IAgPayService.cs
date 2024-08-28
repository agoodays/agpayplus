namespace AGooday.AgPay.Application.Interfaces
{
    public interface IAgPayService<TDto, TPrimaryKey> : IDisposable
        where TDto : class
        where TPrimaryKey : struct
    {
        bool Add(TDto dto);
        Task<bool> AddAsync(TDto dto);
        bool Remove(TPrimaryKey id);
        bool Update(TDto dto);
        TDto GetById(TPrimaryKey id);
        Task<TDto> GetByIdAsync(TPrimaryKey recordId);
        IEnumerable<TDto> GetAll();
        IEnumerable<TDto> GetAllAsNoTracking();
    }
    public interface IAgPayService<TDto> : IDisposable
        where TDto : class
    {
        bool Add(TDto dto);
        Task<bool> AddAsync(TDto dto);
        bool Remove<TPrimaryKey>(TPrimaryKey id);
        bool Update(TDto dto);
        TDto GetById<TPrimaryKey>(TPrimaryKey id);
        Task<TDto> GetByIdAsync<TPrimaryKey>(TPrimaryKey recordId);
        IEnumerable<TDto> GetAll();
        IEnumerable<TDto> GetAllAsNoTracking();
    }
}
