using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Commands.MchInfos;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Extensions;
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

        public Task<List<MchInfoDto>> GetByMchNosAsNoTrackingAsync(List<string> mchNos)
        {
            var query = _mchInfoRepository.GetAllAsNoTracking().Where(w => mchNos.Contains(w.MchNo));
            return query.ToListProjectToAsync<MchInfo, MchInfoDto>(_mapper);
        }

        public Task<List<MchInfoDto>> GetByIsvNoAsNoTrackingAsync(string isvNo)
        {
            var query = _mchInfoRepository.GetAllAsNoTracking().Where(w => w.IsvNo.Equals(isvNo));
            return query.ToListProjectToAsync<MchInfo, MchInfoDto>(_mapper);
        }

        public Task<PaginatedResult<MchInfoDto>> GetPaginatedDataAsync(MchInfoQueryDto dto)
        {
            var query = _mchInfoRepository.GetAllAsNoTracking()
                .WhereIfNotEmpty(dto.MchNo, w => w.MchNo.Equals(dto.MchNo))
                .WhereIfNotEmpty(dto.AgentNo, w => w.AgentNo.Equals(dto.AgentNo))
                .WhereIfNotEmpty(dto.IsvNo, w => w.IsvNo.Equals(dto.IsvNo))
                .WhereIfNotEmpty(dto.MchName, w => w.MchName.Contains(dto.MchName) || w.MchShortName.Contains(dto.MchName))
                .WhereIfNotNull(dto.Type, w => w.Type.Equals(dto.Type))
                .WhereIfNotNull(dto.State, w => w.State.Equals(dto.State))
                .WhereIfNotNull(dto.CreatedUid, w => w.CreatedUid.Equals(dto.CreatedUid))
                .OrderByDescending(o => o.CreatedAt);
            return query.ToPaginatedResultAsync<MchInfo, MchInfoDto>(_mapper, dto.PageNumber, dto.PageSize);
        }
    }
}
