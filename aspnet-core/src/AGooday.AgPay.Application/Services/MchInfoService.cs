using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Commands.MchInfos;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 商户信息表 服务实现类
    /// </summary>
    public class MchInfoService : AgPayService<MchInfoDto, MchInfo>, IMchInfoService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IMchInfoRepository _mchInfoRepository;

        public MchInfoService(IMapper mapper, IMediatorHandler bus,
        IMchInfoRepository mchInfoRepository)
            : base(mapper, bus, mchInfoRepository)
        {
            _mchInfoRepository = mchInfoRepository;
        }

        public bool IsExistMchNo(string mchNo)
        {
            return _mchInfoRepository.IsExistMchNo(mchNo);
        }

        public bool IsExistMchByIsvNo(string isvNo)
        {
            return _mchInfoRepository.IsExistMchByIsvNo(isvNo);
        }

        public bool IsExistMchByAgentNo(string agentNo)
        {
            return _mchInfoRepository.IsExistMchByAgentNo(agentNo);
        }

        public async Task CreateAsync(MchInfoCreateDto dto)
        {
            var command = _mapper.Map<CreateMchInfoCommand>(dto);
            await Bus.SendCommand(command);
        }

        public async Task RemoveAsync(string recordId)
        {
            //_mchInfoRepository.Remove(recordId);
            //_mchInfoRepository.SaveChanges();
            var command = new RemoveMchInfoCommand() { MchNo = recordId };
            await Bus.SendCommand(command);
        }

        public bool UpdateById(MchInfoDto dto)
        {
            var entity = _mchInfoRepository.GetById(dto.MchNo);
            entity.UpdatedAt = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(dto.MchLevel))
            {
                entity.MchLevel = dto.MchLevel;
                _mchInfoRepository.Update(entity, e => new { e.UpdatedAt });
            }
            if (!string.IsNullOrWhiteSpace(dto.Sipw))
            {
                entity.Sipw = dto.Sipw;
                _mchInfoRepository.Update(entity, e => new { e.UpdatedAt });
            }
            return _mchInfoRepository.SaveChanges(out int _);
        }

        public async Task ModifyAsync(MchInfoModifyDto dto)
        {
            var command = _mapper.Map<ModifyMchInfoCommand>(dto);
            await Bus.SendCommand(command);
        }

        public IEnumerable<MchInfoDto> GetByMchNos(List<string> mchNos)
        {
            var mchInfos = _mchInfoRepository.GetAllAsNoTracking().Where(w => mchNos.Contains(w.MchNo));
            return _mapper.Map<IEnumerable<MchInfoDto>>(mchInfos);
        }

        public IEnumerable<MchInfoDto> GetByIsvNo(string isvNo)
        {
            var mchInfos = _mchInfoRepository.GetAllAsNoTracking().Where(w => w.IsvNo.Equals(isvNo));
            return _mapper.Map<IEnumerable<MchInfoDto>>(mchInfos);
        }

        public PaginatedList<MchInfoDto> GetPaginatedData(MchInfoQueryDto dto)
        {
            var mchInfos = _mchInfoRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (string.IsNullOrWhiteSpace(dto.MchName) || w.MchName.Contains(dto.MchName) || w.MchShortName.Contains(dto.MchName))
                && (dto.Type.Equals(null) || w.Type.Equals(dto.Type))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                && (dto.CreatedUid.Equals(null) || w.CreatedUid.Equals(dto.CreatedUid))
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<MchInfo>.Create<MchInfoDto>(mchInfos, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
