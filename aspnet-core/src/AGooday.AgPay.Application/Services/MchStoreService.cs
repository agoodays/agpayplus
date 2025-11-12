using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Extensions;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    public class MchStoreService : AgPayService<MchStoreDto, MchStore>, IMchStoreService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IMchStoreRepository _mchStoreRepository;
        private readonly IMchInfoRepository _mchInfoRepository;

        public MchStoreService(IMapper mapper, IMediatorHandler bus,
            IMchStoreRepository mchStoreRepository,
            IMchInfoRepository mchInfoRepository)
            : base(mapper, bus, mchStoreRepository)
        {
            _mchStoreRepository = mchStoreRepository;
            _mchInfoRepository = mchInfoRepository;
        }

        public override async Task<bool> AddAsync(MchStoreDto dto)
        {
            var entity = _mapper.Map<MchStore>(dto);
            await _mchStoreRepository.AddAsync(entity);
            var (result, _) = await _mchStoreRepository.SaveChangesWithResultAsync();
            dto.StoreId = entity.StoreId;
            return result;
        }

        public MchStoreDto GetById(long recordId, string mchNo)
        {
            var entity = _mchStoreRepository.GetById(recordId, mchNo);
            return _mapper.Map<MchStoreDto>(entity);
        }

        public async Task<MchStoreDto> GetByIdAsync(long recordId, string mchNo)
        {
            var entity = await _mchStoreRepository.GetByIdAsync(recordId, mchNo);
            return _mapper.Map<MchStoreDto>(entity);
        }

        public async Task<MchStoreDto> GetByIdAsNoTrackingAsync(long recordId, string mchNo)
        {
            var entity = await _mchStoreRepository.GetByIdAsNoTrackingAsync(recordId, mchNo);
            return _mapper.Map<MchStoreDto>(entity);
        }

        public async Task<MchStoreDto> GetByIdAsNoTrackingAsync(long recordId)
        {
            var entity = await _mchStoreRepository.GetByIdAsNoTrackingAsync(recordId);
            return _mapper.Map<MchStoreDto>(entity);
        }

        public Task<List<MchStoreDto>> GetByMchNoAsNoTrackingAsync(string mchNo)
        {
            var query = _mchStoreRepository.GetAllAsNoTracking().Where(w => w.MchNo.Equals(mchNo));
            return query.ToListProjectToAsync<MchStore, MchStoreDto>(_mapper);
        }

        public Task<List<MchStoreDto>> GetByStoreIdsAsNoTrackingAsync(IEnumerable<long?> storeIds)
        {
            var query = _mchStoreRepository.GetAllAsNoTracking().Where(w => storeIds.Contains(w.StoreId));
            return query.ToListProjectToAsync<MchStore, MchStoreDto>(_mapper);
        }

        public Task<PaginatedResult<MchStoreListDto>> GetPaginatedDataAsync(MchStoreQueryDto dto, List<long> storeIds = null)
        {
            var query = _mchStoreRepository.GetAllAsNoTracking()
                .Join(_mchInfoRepository.GetAllAsNoTracking(), ms => ms.MchNo, mi => mi.MchNo, (ms, mi) => new { ms, mi })
                .WhereIfNotEmpty(dto.MchNo, w => w.ms.MchNo.Equals(dto.MchNo))
                .WhereIfNotNull(dto.StoreId, w => w.ms.StoreId.Equals(dto.StoreId))
                .WhereIfNotNull(storeIds, w => storeIds.Equals(w.ms.StoreId))
                .WhereIfNotEmpty(dto.StoreName, w => w.ms.StoreName.Contains(dto.StoreName))
                .WhereIfNotEmpty(dto.AgentNo, w => w.mi.AgentNo.Equals(dto.AgentNo))
                .Select(s => new MchStoreQueryResult { MchStore = s.ms, MchInfo = s.mi })
                .OrderByDescending(o => o.MchStore.CreatedAt);

            var records = query.ToPaginatedResultAsync<MchStoreQueryResult, MchStoreListDto>(s =>
            {
                var item = _mapper.Map<MchStoreListDto>(s.MchStore);
                item.MchName = s.MchInfo.MchName;
                return item;
            }, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
