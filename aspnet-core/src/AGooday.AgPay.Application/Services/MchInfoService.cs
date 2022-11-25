using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Domain.Commands.MchInfos;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Application.Services
{
    public class MchInfoService : IMchInfoService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IMchInfoRepository _mchInfoRepository;
        private readonly ISysUserRepository _sysUserRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public MchInfoService(IMapper mapper, IMediatorHandler bus,
            IMchInfoRepository mchInfoRepository,
            ISysUserRepository sysUserRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _mchInfoRepository = mchInfoRepository;
            _sysUserRepository = sysUserRepository;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool IsExistMchNo(string mchNo)
        {
            return _mchInfoRepository.IsExistMchNo(mchNo);
        }

        public bool Add(MchInfoDto dto)
        {
            var m = _mapper.Map<MchInfo>(dto);
            _mchInfoRepository.Add(m);
            return _mchInfoRepository.SaveChanges(out int _);
        }

        public void Create(MchInfoCreateDto dto)
        {
            var command = _mapper.Map<CreateMchInfoCommand>(dto);
            Bus.SendCommand(command);
        }

        public void Remove(string recordId)
        {
            //_mchInfoRepository.Remove(recordId);
            //_mchInfoRepository.SaveChanges();
            var command = new RemoveMchInfoCommand() { MchNo = recordId };
            Bus.SendCommand(command);
        }

        public bool Update(MchInfoDto dto)
        {
            var m = _mapper.Map<MchInfo>(dto);
            _mchInfoRepository.Update(m);
            return _mchInfoRepository.SaveChanges(out int _);
        }

        public async void Modify(MchInfoModifyDto dto)
        {
            var command = _mapper.Map<ModifyMchInfoCommand>(dto);
            await Bus.SendCommand(command);
        }

        public MchInfoDto GetById(string recordId)
        {
            var entity = _mchInfoRepository.GetById(recordId);
            var dto = _mapper.Map<MchInfoDto>(entity);
            return dto;
        }

        public MchInfoDetailDto GetByMchNo(string mchNo)
        {
            var mchInfo = _mchInfoRepository.GetById(mchNo);
            var dto = _mapper.Map<MchInfoDetailDto>(mchInfo);
            var sysUser = _sysUserRepository.GetById(mchInfo.InitUserId.Value);
            dto.LoginUsername = sysUser.LoginUsername;
            return dto;
        }

        public IEnumerable<MchInfoDto> GetAll()
        {
            var mchInfos = _mchInfoRepository.GetAll();
            return _mapper.Map<IEnumerable<MchInfoDto>>(mchInfos);
        }

        public PaginatedList<MchInfoDto> GetPaginatedData(MchInfoQueryDto dto)
        {
            var mchInfos = _mchInfoRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (string.IsNullOrWhiteSpace(dto.MchName) || w.MchName.Contains(dto.MchName) || w.MchShortName.Contains(dto.MchName))
                && (dto.Type.Equals(0) || w.Type.Equals(dto.Type))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<MchInfo>.Create<MchInfoDto>(mchInfos.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
