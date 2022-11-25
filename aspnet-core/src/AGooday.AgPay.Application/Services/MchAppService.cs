using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Application.Services
{
    public class MchAppService : IMchAppService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IMchAppRepository _mchAppRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public MchAppService(IMchAppRepository mchAppRepository, IMapper mapper, IMediatorHandler bus)
        {
            _mchAppRepository = mchAppRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool Add(MchAppDto dto)
        {
            var m = _mapper.Map<MchApp>(dto);
            _mchAppRepository.Add(m);
            return _mchAppRepository.SaveChanges(out int _);
        }

        public bool Remove(string recordId)
        {
            _mchAppRepository.Remove(recordId);
            return _mchAppRepository.SaveChanges(out _);
        }

        public bool Update(MchAppDto dto)
        {
            var renew = _mapper.Map<MchApp>(dto);
            var old = _mchAppRepository.GetById(dto.AppId);
            renew.UpdatedAt = DateTime.Now;
            renew.AppSecret = string.IsNullOrWhiteSpace(renew.AppSecret) ? old.AppSecret : renew.AppSecret;
            _mchAppRepository.Update(renew);
            return _mchAppRepository.SaveChanges(out int _);
        }

        public MchAppDto GetById(string recordId)
        {
            var entity = _mchAppRepository.GetById(recordId);
            var dto = _mapper.Map<MchAppDto>(entity);
            return dto;
        }
        public MchAppDto GetById(string recordId, string mchNo)
        {
            var entity = _mchAppRepository.GetAll().Where(w => w.MchNo.Equals(mchNo) && w.AppId.Equals(recordId)).FirstOrDefault();
            return _mapper.Map<MchAppDto>(entity);
        }
        public IEnumerable<MchAppDto> GetAll()
        {
            var mchApps = _mchAppRepository.GetAll();
            return _mapper.Map<IEnumerable<MchAppDto>>(mchApps);
        }

        public PaginatedList<MchAppDto> GetPaginatedData(MchAppQueryDto dto)
        {
            var mchApps = _mchAppRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (string.IsNullOrWhiteSpace(dto.AppName) || w.AppName.Contains(dto.AppName))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<MchApp>.Create<MchAppDto>(mchApps.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
