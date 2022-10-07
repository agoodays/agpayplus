using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Domain.Commands.SysUsers;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Application.Services
{
    public class MchPayPassageService : IMchPayPassageService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IMchPayPassageRepository _mchPayPassageRepository;
        private readonly IPayInterfaceDefineRepository _payInterfaceDefineRepository;
        private readonly IPayInterfaceConfigRepository _payInterfaceConfigRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public MchPayPassageService(IMapper mapper, IMediatorHandler bus,
            IMchPayPassageRepository mchPayPassageRepository,
            IPayInterfaceDefineRepository payInterfaceDefineRepository,
            IPayInterfaceConfigRepository payInterfaceConfigRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _mchPayPassageRepository = mchPayPassageRepository;
            _payInterfaceDefineRepository = payInterfaceDefineRepository;
            _payInterfaceConfigRepository = payInterfaceConfigRepository;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(MchPayPassageDto dto)
        {
            var m = _mapper.Map<MchPayPassage>(dto);
            _mchPayPassageRepository.Add(m);
            _mchPayPassageRepository.SaveChanges();
        }

        public void Remove(long recordId)
        {
            _mchPayPassageRepository.Remove(recordId);
            _mchPayPassageRepository.SaveChanges();
        }

        public void Update(MchPayPassageDto dto)
        {
            var m = _mapper.Map<MchPayPassage>(dto);
            _mchPayPassageRepository.Update(m);
            _mchPayPassageRepository.SaveChanges();
        }

        public MchPayPassageDto GetById(long recordId)
        {
            var entity = _mchPayPassageRepository.GetById(recordId);
            var dto = _mapper.Map<MchPayPassageDto>(entity);
            return dto;
        }

        public IEnumerable<MchPayPassageDto> GetMchPayPassageByAppId(string mchNo, string appId)
        {
            var mchPayPassages = _mchPayPassageRepository.GetAll()
                .Where(w => w.MchNo.Equals(mchNo) && w.AppId.Equals(appId) && w.State.Equals(CS.PUB_USABLE));
            return _mapper.Map<IEnumerable<MchPayPassageDto>>(mchPayPassages);
        }

        public IEnumerable<MchPayPassageDto> GetAll()
        {
            var mchPayPassages = _mchPayPassageRepository.GetAll();
            return _mapper.Map<IEnumerable<MchPayPassageDto>>(mchPayPassages);
        }

        public IEnumerable<MchPayPassageDto> GetAll(string appId, List<string> wayCodes)
        {
            var mchPayPassages = _mchPayPassageRepository.GetAll().Where(w => w.AppId.Equals(appId)
            && (wayCodes.Count.Equals(0) || wayCodes.Contains(w.WayCode)));
            return _mapper.Map<IEnumerable<MchPayPassageDto>>(mchPayPassages);
        }

        /// <summary>
        /// 根据支付方式查询可用的支付接口列表
        /// </summary>
        /// <param name="wayCode"></param>
        /// <param name="appId"></param>
        /// <param name="infoType"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<AvailablePayInterfaceDto> SelectAvailablePayInterfaceList(string wayCode, string appId, byte infoType, byte type)
        {
            var result = _payInterfaceDefineRepository.GetAll().Join(_payInterfaceConfigRepository.GetAll(),
                pid => pid.IfCode, pic => pic.IfCode,
                (pid, pic) => new { pid, pic })
                .Where(w => w.pid.State.Equals(CS.YES) && w.pic.State.Equals(CS.YES)
                && w.pid.WayCodes.Contains(wayCode) && w.pic.InfoType.Equals(infoType) && w.pic.InfoId.Equals(appId)
                && !string.IsNullOrWhiteSpace(w.pic.IfParams.Trim()))
                .Select(s => new AvailablePayInterfaceDto()
                {
                    IfCode = s.pid.IfCode,
                    IfName = s.pid.IfName,
                    ConfigPageType = s.pid.ConfigPageType,
                    Icon = s.pid.Icon,
                    BgColor = s.pid.BgColor,
                    IfParams = s.pic.IfParams,
                    IfRate = s.pic.IfRate * 100,
                });
            if (result != null)
            {
                var mchPayPassages = _mchPayPassageRepository.GetAll().Where(w => w.AppId.Equals(appId) && w.WayCode.Equals(wayCode));
                foreach (var item in result)
                {
                    var payPassage = mchPayPassages.Where(w => w.IfCode.Equals(item.IfCode)).FirstOrDefault();
                    if (payPassage != null)
                    {
                        item.PassageId = payPassage.Id;
                        item.State = payPassage.State;
                        item.Rate = payPassage.Rate * 100;
                    }
                }
            }
            return result;
        }

        public void SaveOrUpdateBatchSelf(List<MchPayPassageDto> mchPayPassages, string mchNo)
        {
            foreach (var payPassage in mchPayPassages)
            {
                if (payPassage.State == CS.NO && payPassage.Id == null)
                {
                    continue;
                }
                // 商户系统配置通道，添加商户号参数
                if (!string.IsNullOrWhiteSpace(mchNo))
                {
                    payPassage.MchNo = mchNo;
                }
                payPassage.Rate = payPassage.Rate / 100;

                var m = _mapper.Map<MchPayPassage>(payPassage);
                _mchPayPassageRepository.SaveOrUpdate(m, payPassage.Id);
            }
            _mchPayPassageRepository.SaveChanges();
        }

        /// <summary>
        /// 根据应用ID 和 支付方式， 查询出商户可用的支付接口
        /// </summary>
        /// <param name="mchNo"></param>
        /// <param name="appId"></param>
        /// <param name="wayCode"></param>
        /// <returns></returns>
        public MchPayPassageDto FindMchPayPassage(string mchNo, string appId, string wayCode)
        {
            var entity = _mchPayPassageRepository.GetAll().Where(w => w.State.Equals(CS.YES)
            && w.MchNo.Equals(mchNo)
            && w.AppId.Equals(appId)
            && w.WayCode.Equals(wayCode)).FirstOrDefault();
            var dto = _mapper.Map<MchPayPassageDto>(entity);
            return dto;
        }

        public bool IsExistMchPayPassageUseWayCode(string wayCode)
        {
            return _mchPayPassageRepository.IsExistMchPayPassageUseWayCode(wayCode);
        }
    }
}
