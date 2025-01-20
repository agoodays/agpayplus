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

        public Task<bool> IsExistMchNoAsync(string mchNo)
        {
            return _mchInfoRepository.IsExistMchNoAsync(mchNo);
        }

        public Task<bool> IsExistMchByIsvNoAsync(string isvNo)
        {
            return _mchInfoRepository.IsExistMchByIsvNoAsync(isvNo);
        }

        public Task<bool> IsExistMchByAgentNoAsync(string agentNo)
        {
            return _mchInfoRepository.IsExistMchByAgentNoAsync(agentNo);
        }

        public Task CreateAsync(MchInfoCreateDto dto)
        {
            var command = _mapper.Map<CreateMchInfoCommand>(dto);
            return Bus.SendCommand(command);
        }

        public Task RemoveAsync(string recordId)
        {
            //_mchInfoRepository.Remove(recordId);
            //_mchInfoRepository.SaveChanges();
            var command = new RemoveMchInfoCommand() { MchNo = recordId };
            return Bus.SendCommand(command);
        }

        public async Task<bool> UpdateByIdAsync(MchInfoDto dto)
        {
            var entity = await _mchInfoRepository.GetByIdAsync(dto.MchNo);
            entity.UpdatedAt = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(dto.MchLevel))
            {
                entity.MchLevel = dto.MchLevel;
                _mchInfoRepository.Update(entity, e => new { e.MchLevel, e.UpdatedAt });
            }
            if (!string.IsNullOrWhiteSpace(dto.Sipw))
            {
                entity.Sipw = dto.Sipw;
                _mchInfoRepository.Update(entity, e => new { e.Sipw, e.UpdatedAt });
            }
            var (result, _) = await _mchInfoRepository.SaveChangesWithResultAsync();
            return result;
        }

        public Task ModifyAsync(MchInfoModifyDto dto)
        {
            var command = _mapper.Map<ModifyMchInfoCommand>(dto);
            return Bus.SendCommand(command);
        }

        public IEnumerable<MchInfoDto> GetByMchNos(List<string> mchNos)
        {
            var records = _mchInfoRepository.GetAllAsNoTracking().Where(w => mchNos.Contains(w.MchNo));
            return _mapper.Map<IEnumerable<MchInfoDto>>(records);
        }

        public IEnumerable<MchInfoDto> GetByIsvNo(string isvNo)
        {
            var records = _mchInfoRepository.GetAllAsNoTracking().Where(w => w.IsvNo.Equals(isvNo));
            return _mapper.Map<IEnumerable<MchInfoDto>>(records);
        }

        public Task<PaginatedList<MchInfoDto>> GetPaginatedDataAsync(MchInfoQueryDto dto)
        {
            var query = _mchInfoRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (string.IsNullOrWhiteSpace(dto.MchName) || w.MchName.Contains(dto.MchName) || w.MchShortName.Contains(dto.MchName))
                && (dto.Type.Equals(null) || w.Type.Equals(dto.Type))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                && (dto.CreatedUid.Equals(null) || w.CreatedUid.Equals(dto.CreatedUid)))
                .OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<MchInfo>.CreateAsync<MchInfoDto>(query, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
