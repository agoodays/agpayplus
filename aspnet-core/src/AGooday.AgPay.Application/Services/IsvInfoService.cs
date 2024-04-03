using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 服务商信息表 服务实现类
    /// </summary>
    public class IsvInfoService : AgPayService<IsvInfoDto, IsvInfo>, IIsvInfoService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IIsvInfoRepository _isvInfoRepository;

        public IsvInfoService(IMapper mapper, IMediatorHandler bus,
            IIsvInfoRepository isvInfoRepository)
            : base(mapper, bus, isvInfoRepository)
        {
            _isvInfoRepository = isvInfoRepository;
        }

        public override bool Add(IsvInfoDto dto)
        {
            do
            {
                dto.IsvNo = SeqUtil.GenIsvNo();
            } while (IsExistIsvNo(dto.IsvNo));
            var m = _mapper.Map<IsvInfo>(dto);
            _isvInfoRepository.Add(m);
            return _isvInfoRepository.SaveChanges(out int _);
        }

        public override bool Update(IsvInfoDto dto)
        {
            var entity = _mapper.Map<IsvInfo>(dto);
            entity.UpdatedAt = DateTime.Now;
            _isvInfoRepository.Update(entity);
            return _isvInfoRepository.SaveChanges(out int _);
        }

        public bool IsExistIsvNo(string isvNo)
        {
            return _isvInfoRepository.IsExistIsvNo(isvNo);
        }

        public PaginatedList<IsvInfoDto> GetPaginatedData(IsvInfoQueryDto dto)
        {
            var isvInfos = _isvInfoRepository.GetAllAsNoTracking()
                .Where(w => (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (string.IsNullOrWhiteSpace(dto.IsvName) || w.IsvName.Contains(dto.IsvName) || w.IsvShortName.Contains(dto.IsvName))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<IsvInfo>.Create<IsvInfoDto>(isvInfos, _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }
    }
}
