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
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Services
{
    public class SysConfigService : ISysConfigService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysConfigRepository _sysConfigRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public SysConfigService(ISysConfigRepository sysConfigRepository, IMapper mapper, IMediatorHandler bus)
        {
            _sysConfigRepository = sysConfigRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /** 是否启用缓存
         * true: 表示将使用内存缓存， 将部分系统配置项 或 商户应用/服务商信息进行缓存并读取
         * false: 直接查询DB
         * **/
        public static bool IS_USE_CACHE = false;

        /** 数据库application配置参数 **/
        private static Dictionary<string, DBApplicationConfig> APPLICATION_CONFIG = new Dictionary<string, DBApplicationConfig> {
            {"applicationConfig", null }
        };

        public void InitDBConfig(string groupKey)
        {

            // 若当前系统不缓存，则直接返回
            if (!IS_USE_CACHE)
            {
                return;
            }

            if (APPLICATION_CONFIG.First().Key.Equals(groupKey))
            {
                var dbConfig = JsonConvert.DeserializeObject<DBApplicationConfig>(SelectByGroupKey(groupKey));
                APPLICATION_CONFIG[APPLICATION_CONFIG.First().Key] = dbConfig;
            }
        }

        /** 根据分组查询，并返回JSON对象格式的数据 **/
        public string SelectByGroupKey(string groupKey)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            _sysConfigRepository.GetAll().Where(w => w.GroupKey.Equals(groupKey))
                .Select(s => new { s.ConfigKey, s.ConfigVal }).ToList().ForEach((c) =>
                {
                    keyValuePairs.Add(c.ConfigKey, c.ConfigVal);
                });
            return JsonConvert.SerializeObject(keyValuePairs);
        }

        /** 获取实际的数据 **/
        public DBApplicationConfig GetDBApplicationConfig()
        {
            // 查询DB
            if (!IS_USE_CACHE)
            {
                return JsonConvert.DeserializeObject<DBApplicationConfig>(SelectByGroupKey(APPLICATION_CONFIG.First().Key));
            }

            // 缓存数据
            if (APPLICATION_CONFIG.First().Value == null)
            {
                InitDBConfig(APPLICATION_CONFIG.First().Key);
            }
            return APPLICATION_CONFIG.First().Value;
        }

        public int UpdateByConfigKey(Dictionary<string, string> configs)
        {
            int count = 0;
            foreach (KeyValuePair<string, string> config in configs)
            {
                var sysConfig = new SysConfigDto();
                sysConfig.ConfigKey = config.Key;
                sysConfig.ConfigVal = config.Value;
                bool update = this.SaveOrUpdate(sysConfig);
                if (update)
                {
                    count++;
                }
            }
            return count;
        }

        public void Add(SysConfigDto dto)
        {
            var m = _mapper.Map<SysConfig>(dto);
            _sysConfigRepository.Add(m);
            _sysConfigRepository.SaveChanges();
        }

        public void Remove(string recordId)
        {
            _sysConfigRepository.Remove(recordId);
            _sysConfigRepository.SaveChanges();
        }

        public void Update(SysConfigDto dto)
        {
            var m = _mapper.Map<SysConfig>(dto);
            _sysConfigRepository.Update(m);
            _sysConfigRepository.SaveChanges();
        }

        public bool SaveOrUpdate(SysConfigDto dto)
        {
            var config = _mapper.Map<SysConfig>(dto);
            _sysConfigRepository.SaveOrUpdate(config, dto.ConfigKey);
            return _sysConfigRepository.SaveChanges() > 0;
        }

        public SysConfigDto GetById(string recordId)
        {
            var entity = _sysConfigRepository.GetById(recordId);
            var dto = _mapper.Map<SysConfigDto>(entity);
            return dto;
        }

        public IEnumerable<SysConfigDto> GetAll()
        {
            var sysConfigs = _sysConfigRepository.GetAll();
            return _mapper.Map<IEnumerable<SysConfigDto>>(sysConfigs);
        }
    }
}
