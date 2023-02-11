using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
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

        /// <summary>
        /// 是否启用缓存
        /// true: 表示将使用内存缓存， 将部分系统配置项 或 商户应用/服务商信息进行缓存并读取
        /// false: 直接查询DB
        /// </summary>
        public static bool IS_USE_CACHE = false;

        /// <summary>
        /// 数据库application配置参数
        /// </summary>
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

        /// <summary>
        /// 根据分组查询，并返回键值对
        /// </summary>
        /// <param name="groupKey"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetKeyValueByGroupKey(string groupKey)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            _sysConfigRepository.GetAll().Where(w => w.GroupKey.Equals(groupKey))
                .Select(s => new { s.ConfigKey, s.ConfigVal }).ToList().ForEach((c) =>
                {
                    keyValuePairs.Add(c.ConfigKey, c.ConfigVal);
                });
            return keyValuePairs;
        }

        /// <summary>
        /// 根据分组查询，并返回JSON对象格式的数据
        /// </summary>
        /// <param name="groupKey"></param>
        /// <returns></returns>
        public string SelectByGroupKey(string groupKey)
        {
            return JsonConvert.SerializeObject(GetKeyValueByGroupKey(groupKey));
        }

        /// <summary>
        /// 获取实际的数据
        /// </summary>
        /// <returns></returns>
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
            foreach (KeyValuePair<string, string> config in configs)
            {
                var sysConfig = _sysConfigRepository.GetById(config.Key);
                if (sysConfig == null)
                {
                    sysConfig = new SysConfig();
                }
                sysConfig.ConfigKey = config.Key;
                sysConfig.ConfigVal = config.Value;
                _sysConfigRepository.SaveOrUpdate(sysConfig, sysConfig.ConfigKey);
            }
            return _sysConfigRepository.SaveChanges();
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
            var config = _mapper.Map<SysConfig>(source: dto);
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
