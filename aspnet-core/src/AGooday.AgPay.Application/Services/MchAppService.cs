using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 商户应用表 服务实现类
    /// </summary>
    public class MchAppService : AgPayService<MchAppDto, MchApp>, IMchAppService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IMchAppRepository _mchAppRepository;
        private readonly IMchInfoRepository _mchInfoRepository;

        public MchAppService(IMapper mapper, IMediatorHandler bus,
            IMchAppRepository mchAppRepository, IMchInfoRepository mchInfoRepository)
            : base(mapper, bus, mchAppRepository)
        {
            _mchAppRepository = mchAppRepository;
            _mchInfoRepository = mchInfoRepository;
        }

        public override async Task<bool> UpdateAsync(MchAppDto dto)
        {
            var origin = await _mchAppRepository.GetByIdAsNoTrackingAsync(dto.AppId);
            var entity = _mapper.Map<MchApp>(dto);
            if (string.IsNullOrWhiteSpace(entity.AppSecret))
            {
                entity.AppSecret = origin.AppSecret;
            }
            if (string.IsNullOrWhiteSpace(entity.AppRsa2PublicKey))
            {
                entity.AppRsa2PublicKey = origin.AppRsa2PublicKey;
            }
            entity.UpdatedAt = DateTime.Now;
            _mchAppRepository.Update(entity);
            var (result, _) = await _mchAppRepository.SaveChangesWithResultAsync();
            return result;
        }

        public async Task<MchAppDto> GetByIdAsync(string recordId, string mchNo)
        {
            var entity = await _mchAppRepository.GetByIdAsync(recordId, mchNo);
            return _mapper.Map<MchAppDto>(entity);
        }

        public async Task<MchAppDto> GetByIdAsNoTrackingAsync(string recordId, string mchNo)
        {
            var entity = await _mchAppRepository.GetByIdAsNoTrackingAsync(recordId, mchNo);
            return _mapper.Map<MchAppDto>(entity);
        }

        public IEnumerable<MchAppDto> GetByMchNoAsNoTracking(string mchNo)
        {
            var mchApps = _mchAppRepository.GetAllAsNoTracking().Where(w => w.MchNo.Equals(mchNo));
            return _mapper.Map<IEnumerable<MchAppDto>>(mchApps);
        }

        public IEnumerable<MchAppDto> GetByMchNos(IEnumerable<string> mchNos)
        {
            var mchApps = _mchAppRepository.GetAllAsNoTracking().Where(w => mchNos.Contains(w.MchNo));
            return _mapper.Map<IEnumerable<MchAppDto>>(mchApps);
        }

        public IEnumerable<MchAppDto> GetByAppIds(IEnumerable<string> appIds)
        {
            var mchApps = _mchAppRepository.GetAllAsNoTracking().Where(w => appIds.Contains(w.AppId));
            return _mapper.Map<IEnumerable<MchAppDto>>(mchApps);
        }

        public Task<PaginatedList<MchAppDto>> GetPaginatedDataAsync(MchAppQueryDto dto, string agentNo = null)
        {
            var query = _mchAppRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (string.IsNullOrWhiteSpace(dto.AppName) || w.AppName.Contains(dto.AppName))
                && (dto.State.Equals(null) || w.State.Equals(dto.State)))
                .OrderByDescending(o => o.CreatedAt);

            if (!string.IsNullOrWhiteSpace(agentNo))
            {
                var mchNos = _mchInfoRepository.GetAllAsNoTracking()
                    .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                    && w.AgentNo.Equals(agentNo)).Select(s => s.MchNo);
                query = query.Where(w => mchNos.Contains(w.MchNo)).OrderByDescending(o => o.CreatedAt);
            }
            var records = PaginatedList<MchApp>.CreateAsync<MchAppDto>(query, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
