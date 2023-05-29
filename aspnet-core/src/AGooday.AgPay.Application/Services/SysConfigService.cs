using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 系统配置表 服务实现类
    /// </summary>
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
            { "applicationConfig", null }
        };

        private static Dictionary<string, DBOssConfig> OSS_CONFIG = new Dictionary<string, DBOssConfig> {
            { "ossConfig", null }
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

            if (OSS_CONFIG.First().Key.Equals(groupKey))
            {
                var dbConfig = JsonConvert.DeserializeObject<DBOssConfig>(SelectByGroupKey(groupKey));
                OSS_CONFIG[OSS_CONFIG.First().Key] = dbConfig;
            }
        }

        /// <summary>
        /// 根据分组查询，并返回键值对
        /// </summary>
        /// <param name="groupKey"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetKeyValueByGroupKey(string groupKey, string sysType = CS.SYS_TYPE.MGR, string belongInfoId = CS.BASE_BELONG_INFO_ID.MGR)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            var sysConfigs = _sysConfigRepository.GetAll()
                .Where(w => w.GroupKey.Equals(groupKey) && w.SysType.Equals(sysType) && w.BelongInfoId.Equals(belongInfoId));
            sysConfigs.Select(s => new { s.ConfigKey, s.ConfigVal }).ToList().ForEach((c) =>
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
        public string SelectByGroupKey(string groupKey, string sysType = CS.SYS_TYPE.MGR, string belongInfoId = CS.BASE_BELONG_INFO_ID.MGR)
        {
            return JsonConvert.SerializeObject(GetKeyValueByGroupKey(groupKey, sysType, belongInfoId));
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

        /// <summary>
        /// 获取实际的数据
        /// </summary>
        /// <returns></returns>
        public DBOssConfig GetDBOssConfig()
        {
            // 查询DB
            if (!IS_USE_CACHE)
            {
                return JsonConvert.DeserializeObject<DBOssConfig>(SelectByGroupKey(OSS_CONFIG.First().Key));
            }

            // 缓存数据
            if (OSS_CONFIG.First().Value == null)
            {
                InitDBConfig(OSS_CONFIG.First().Key);
            }
            return OSS_CONFIG.First().Value;
        }

        public int UpdateByConfigKey(Dictionary<string, string> configs, string groupKey, string sysType, string belongInfoId)
        {
            foreach (KeyValuePair<string, string> config in configs)
            {
                var sysConfig = _sysConfigRepository.GetByKey(config.Key, sysType, belongInfoId);
                if (sysConfig == null)
                {
                    switch (sysType)
                    {
                        case CS.SYS_TYPE.MCH:
                            sysConfig = _sysConfigRepository.GetByKey(config.Key, sysType, CS.BASE_BELONG_INFO_ID.MCH);
                            break;
                        case CS.SYS_TYPE.AGENT:
                            sysConfig = _sysConfigRepository.GetByKey(config.Key, sysType, CS.BASE_BELONG_INFO_ID.AGENT);
                            break;
                        case CS.SYS_TYPE.MGR:
                        default:
                            break;
                    }

                    sysConfig = sysConfig ?? new SysConfig()
                    {
                        SysType = sysType,
                        BelongInfoId = belongInfoId,
                        ConfigKey = config.Key,
                        ConfigVal = config.Value,
                        GroupKey = groupKey,
                        UpdatedAt = DateTime.Now,
                    };
                    sysConfig.BelongInfoId = belongInfoId;
                    sysConfig.UpdatedAt = DateTime.Now;
                    _sysConfigRepository.Add(sysConfig);
                }
                else
                {
                    sysConfig.ConfigKey = config.Key;
                    sysConfig.ConfigVal = config.Value;
                    sysConfig.UpdatedAt = DateTime.Now;
                    _sysConfigRepository.Update(sysConfig);
                }
            }
            return _sysConfigRepository.SaveChanges();
        }

        public IEnumerable<SysConfigDto> GetByGroupKey(string groupKey, string sysType, string belongInfoId)
        {
            var sysConfigs = _sysConfigRepository.GetAll()
                .Where(w => w.GroupKey.Equals(groupKey) && w.SysType.Equals(sysType) && w.BelongInfoId.Equals(belongInfoId))
                .OrderBy(o => o.SortNum);
            List<SysConfig> mergedList = new List<SysConfig>(sysConfigs);
            switch (sysType)
            {
                case CS.SYS_TYPE.MCH:
                    MergedSysConfig(groupKey, sysType, sysConfigs, mergedList, CS.BASE_BELONG_INFO_ID.MCH);
                    break;
                case CS.SYS_TYPE.AGENT:
                    MergedSysConfig(groupKey, sysType, sysConfigs, mergedList, CS.BASE_BELONG_INFO_ID.AGENT);
                    break;
                case CS.SYS_TYPE.MGR:
                default:
                    break;
            }
            return _mapper.Map<IEnumerable<SysConfigDto>>(mergedList);
        }

        private void MergedSysConfig(string groupKey, string sysType, IOrderedQueryable<SysConfig> sysConfigs, List<SysConfig> mergedList, string belongInfoId)
        {
            var sysConfigsTemp = _sysConfigRepository.GetAll()
                .Where(w => w.GroupKey.Equals(groupKey) && w.SysType.Equals(sysType) && w.BelongInfoId.Equals(belongInfoId))
                .OrderBy(o => o.SortNum);
            var mergingList = sysConfigsTemp
                .Where(sc => !sysConfigs.Any(temp => temp.ConfigKey == sc.ConfigKey))
                .Select(sc => sc)
                .ToList();
            mergedList.AddRange(mergingList);
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

        public SysConfigDto GetByKey(string configKey, string sysType, string belongInfoId)
        {
            var entity = _sysConfigRepository.GetByKey(configKey, sysType, belongInfoId);
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
