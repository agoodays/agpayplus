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

        public MchAppDto GetById(string recordId, string mchNo)
        {
            var entity = _mchAppRepository.GetAll().Where(w => w.MchNo.Equals(mchNo) && w.AppId.Equals(recordId)).FirstOrDefault();
            return _mapper.Map<MchAppDto>(entity);
        }

        public IEnumerable<MchAppDto> GetByMchNo(string mchNo)
        {
            var mchApps = _mchAppRepository.GetAll().Where(w => w.MchNo.Equals(mchNo));
            return _mapper.Map<IEnumerable<MchAppDto>>(mchApps);
        }

        public IEnumerable<MchAppDto> GetByMchNos(IEnumerable<string> mchNos)
        {
            var mchApps = _mchAppRepository.GetAll().Where(w => mchNos.Contains(w.MchNo));
            return _mapper.Map<IEnumerable<MchAppDto>>(mchApps);
        }

        public PaginatedList<MchAppDto> GetPaginatedData(MchAppQueryDto dto, string agentNo = null)
        {
            var mchApps = _mchAppRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (string.IsNullOrWhiteSpace(dto.AppName) || w.AppName.Contains(dto.AppName))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                ).OrderByDescending(o => o.CreatedAt);

            if (!string.IsNullOrWhiteSpace(agentNo))
            {
                var mchNos = _mchInfoRepository.GetAllAsNoTracking()
                    .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                    && w.AgentNo.Equals(agentNo)).Select(s => s.MchNo);
                mchApps = mchApps.Where(w => mchNos.Contains(w.MchNo)).OrderByDescending(o => o.CreatedAt);
            }
            var records = PaginatedList<MchApp>.Create<MchAppDto>(mchApps, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
