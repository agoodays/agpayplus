using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 账户帐单表 服务实现类
    /// </summary>
    public abstract class AgPayService<TDto, TEntity, TPrimaryKey> : IAgPayService<TDto, TPrimaryKey>
        where TDto : class
        where TEntity : class
        where TPrimaryKey : struct
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        protected readonly IAgPayRepository<TEntity, TPrimaryKey> _agPayRepository;
        // 用来进行DTO
        protected readonly IMapper _mapper;
        // 中介者 总线
        protected readonly IMediatorHandler Bus;

        public AgPayService(IMapper mapper, IMediatorHandler bus,
            IAgPayRepository<TEntity, TPrimaryKey> agPayRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _agPayRepository = agPayRepository;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public virtual bool Add(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            _agPayRepository.Add(entity);
            var result = _agPayRepository.SaveChanges(out int _);
            return result;
        }

        public virtual async Task<bool> AddAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            await _agPayRepository.AddAsync(entity);
            var result = _agPayRepository.SaveChanges(out int _);
            return result;
        }

        public virtual bool Remove(TPrimaryKey id)
        {
            _agPayRepository.Remove(id);
            return _agPayRepository.SaveChanges(out int _);
        }

        public virtual bool Update(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            _agPayRepository.Update(entity);
            return _agPayRepository.SaveChanges(out int _);
        }

        public virtual TDto GetById(TPrimaryKey id)
        {
            var entity = _agPayRepository.GetById(id);
            var dto = _mapper.Map<TDto>(entity);
            return dto;
        }

        public virtual async Task<TDto> GetByIdAsync(TPrimaryKey id)
        {
            var entity = await _agPayRepository.GetByIdAsync(id);
            var dto = _mapper.Map<TDto>(entity);
            return dto;
        }

        public virtual IEnumerable<TDto> GetAll()
        {
            var entitys = _agPayRepository.GetAll();
            return _mapper.Map<IEnumerable<TDto>>(entitys);
        }

        public virtual IEnumerable<TDto> GetAllAsNoTracking()
        {
            var entitys = _agPayRepository.GetAllAsNoTracking();
            return _mapper.Map<IEnumerable<TDto>>(entitys);
        }
    }
    public abstract class AgPayService<TDto, TEntity> : IAgPayService<TDto>
        where TDto : class
        where TEntity : class
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        protected readonly IAgPayRepository<TEntity> _agPayRepository;
        // 用来进行DTO
        protected readonly IMapper _mapper;
        // 中介者 总线
        protected readonly IMediatorHandler Bus;

        public AgPayService(IMapper mapper, IMediatorHandler bus,
            IAgPayRepository<TEntity> agPayRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _agPayRepository = agPayRepository;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public virtual bool Add(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            _agPayRepository.Add(entity);
            var result = _agPayRepository.SaveChanges(out int _);
            return result;
        }

        public virtual async Task<bool> AddAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            await _agPayRepository.AddAsync(entity);
            var result = _agPayRepository.SaveChanges(out int _);
            return result;
        }

        public virtual bool Remove<TPrimaryKey>(TPrimaryKey id)
        {
            _agPayRepository.Remove(id);
            return _agPayRepository.SaveChanges(out int _);
        }

        public virtual bool Update(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            _agPayRepository.Update(entity);
            return _agPayRepository.SaveChanges(out int _);
        }

        public virtual TDto GetById<TPrimaryKey>(TPrimaryKey id)
        {
            var entity = _agPayRepository.GetById(id);
            var dto = _mapper.Map<TDto>(entity);
            return dto;
        }

        public virtual async Task<TDto> GetByIdAsync<TPrimaryKey>(TPrimaryKey id)
        {
            var entity = await _agPayRepository.GetByIdAsync(id);
            var dto = _mapper.Map<TDto>(entity);
            return dto;
        }

        public virtual IEnumerable<TDto> GetAll()
        {
            //第一种写法 Map
            var sysUsers = _agPayRepository.GetAll();
            return _mapper.Map<IEnumerable<TDto>>(sysUsers);

            //第二种写法 ProjectTo
            //return (_agPayRepository.GetAll()).ProjectTo<TDto>(_mapper.ConfigurationProvider);
        }

        public virtual IEnumerable<TDto> GetAllAsNoTracking()
        {
            //第一种写法 Map
            var sysUsers = _agPayRepository.GetAllAsNoTracking();
            return _mapper.Map<IEnumerable<TDto>>(sysUsers);

            //第二种写法 ProjectTo
            //return (_agPayRepository.GetAllAsNoTracking()).ProjectTo<TDto>(_mapper.ConfigurationProvider);
        }
    }
}
