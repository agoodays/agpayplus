using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
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

        public override bool Add(MchStoreDto dto)
        {
            var entity = _mapper.Map<MchStore>(dto);
            _mchStoreRepository.Add(entity);
            var result = _mchStoreRepository.SaveChanges(out int _);
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
            var dto = _mapper.Map<MchStoreDto>(entity);
            return dto;
        }

        public IEnumerable<MchStoreDto> GetByMchNoAsNoTracking(string mchNo)
        {
            var mchStores = _mchStoreRepository.GetAllAsNoTracking().Where(w => w.MchNo.Equals(mchNo));
            return _mapper.Map<IEnumerable<MchStoreDto>>(mchStores);
        }

        public IEnumerable<MchStoreDto> GetByStoreIdsAsNoTracking(IEnumerable<long?> storeIds)
        {
            var mchStores = _mchStoreRepository.GetAllAsNoTracking().Where(w => storeIds.Contains(w.StoreId));
            return _mapper.Map<IEnumerable<MchStoreDto>>(mchStores);
        }

        public async Task<PaginatedList<MchStoreListDto>> GetPaginatedDataAsync(MchStoreQueryDto dto, List<long> storeIds = null)
        {
            var query = _mchStoreRepository.GetAllAsNoTracking()
                .Join(_mchInfoRepository.GetAllAsNoTracking(),
                ms => ms.MchNo, mi => mi.MchNo,
                (ms, mi) => new { ms, mi })
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.ms.MchNo.Equals(dto.MchNo))
                && (dto.StoreId.Equals(null) || w.ms.StoreId.Equals(dto.StoreId))
                && (storeIds == null || storeIds.Equals(w.ms.StoreId))
                && (string.IsNullOrWhiteSpace(dto.StoreName) || w.ms.StoreName.Contains(dto.StoreName))
                && (string.IsNullOrWhiteSpace(dto.AgentNo) || w.mi.AgentNo.Equals(dto.AgentNo)))
                .OrderByDescending(o => o.ms.CreatedAt);

            var records = await PaginatedList<MchStoreListDto>.CreateAsync(query, s =>
            {
                var item = _mapper.Map<MchStoreListDto>(s.ms);
                item.MchName = s.mi.MchName;
                return item;
            }, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
