namespace AGooday.AgPay.Application.Interfaces
{
    public interface IAgPayService<TDto, TPrimaryKey> : IDisposable
        where TDto : class
        where TPrimaryKey : struct
    {
        bool Add(TDto dto);
        Task<bool> AddAsync(TDto dto);
        bool AddRange(IEnumerable<TDto> dtos);
        Task<bool> AddRangeAsync(IEnumerable<TDto> dtos);
        bool Remove(TPrimaryKey id);
        Task<bool> RemoveAsync(TPrimaryKey id);
        Task<bool> RemoveRangeAsync(IEnumerable<TDto> dtos);
        bool Update(TDto dto);
        Task<bool> UpdateAsync(TDto dto);
        Task<bool> UpdateRangeAsync(IEnumerable<TDto> dtos);
        TDto GetById(TPrimaryKey id);
        Task<TDto> GetByIdAsync(TPrimaryKey recordId);
        TDto GetByIdAsNoTracking(TPrimaryKey recordId);
        Task<TDto> GetByIdAsNoTrackingAsync(TPrimaryKey recordId);
        IEnumerable<TDto> GetAll();
        IEnumerable<TDto> GetAllAsNoTracking();
    }
    public interface IAgPayService<TDto> : IDisposable
        where TDto : class
    {
        bool Add(TDto dto);
        Task<bool> AddAsync(TDto dto);
        bool AddRange(IEnumerable<TDto> dtos);
        Task<bool> AddRangeAsync(IEnumerable<TDto> dtos);
        bool Remove<TPrimaryKey>(TPrimaryKey id);
        Task<bool> RemoveAsync<TPrimaryKey>(TPrimaryKey id);
        Task<bool> RemoveRangeAsync(IEnumerable<TDto> dtos);
        bool Update(TDto dto);
        Task<bool> UpdateAsync(TDto dto);
        Task<bool> UpdateRangeAsync(IEnumerable<TDto> dtos);
        TDto GetById<TPrimaryKey>(TPrimaryKey id);
        Task<TDto> GetByIdAsync<TPrimaryKey>(TPrimaryKey recordId);
        TDto GetByIdAsNoTracking<TPrimaryKey>(TPrimaryKey recordId);
        Task<TDto> GetByIdAsNoTrackingAsync<TPrimaryKey>(TPrimaryKey recordId);
        IEnumerable<TDto> GetAll();
        IEnumerable<TDto> GetAllAsNoTracking();
    }
}
