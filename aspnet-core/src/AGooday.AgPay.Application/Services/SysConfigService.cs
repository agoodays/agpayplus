﻿using AGooday.AgPay.Application.Config;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
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
    public class SysConfigService : AgPayService<SysConfigDto, SysConfig>, ISysConfigService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysConfigRepository _sysConfigRepository;

        public SysConfigService(IMapper mapper, IMediatorHandler bus,
            ISysConfigRepository sysConfigRepository)
            : base(mapper, bus, sysConfigRepository)
        {
            _sysConfigRepository = sysConfigRepository;
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

        private static Dictionary<string, DBOcrConfig> OCR_CONFIG = new Dictionary<string, DBOcrConfig> {
            { "ocrConfig", null }
        };

        private static Dictionary<string, DBSmsConfig> SMS_CONFIG = new Dictionary<string, DBSmsConfig> {
            { "smsConfig", null }
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

            if (OCR_CONFIG.First().Key.Equals(groupKey))
            {
                var dbConfig = JsonConvert.DeserializeObject<DBOcrConfig>(SelectByGroupKey(groupKey));
                OCR_CONFIG[OCR_CONFIG.First().Key] = dbConfig;
            }

            if (SMS_CONFIG.First().Key.Equals(groupKey))
            {
                var dbConfig = JsonConvert.DeserializeObject<DBSmsConfig>(SelectByGroupKey(groupKey));
                SMS_CONFIG[SMS_CONFIG.First().Key] = dbConfig;
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

            var sysConfigs = _sysConfigRepository.GetAllAsNoTracking()
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

        public DBNoticeConfig GetDBNoticeConfig()
        {
            return new DBNoticeConfig()
            {
                Notice = new DBNoticeConfig.NoticeConfig()
                {
                    Mail = new DBNoticeConfig.NoticeConfig.MailConfig()
                    {
                        Host = "smtp.qq.com",
                        Port = 465,
                        FromName = "xxx@foxmail.com",
                        FromAddress = "xxx@foxmail.com",
                        Password = "******",
                        ToAddress = new List<string>()
                        {
                            "123@qq.com"
                        }
                    }
                }
            };
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

        /// <summary>
        /// 获取实际的数据
        /// </summary>
        /// <returns></returns>
        public DBOcrConfig GetDBOcrConfig()
        {
            // 查询DB
            if (!IS_USE_CACHE)
            {
                return JsonConvert.DeserializeObject<DBOcrConfig>(SelectByGroupKey(OCR_CONFIG.First().Key));
            }

            // 缓存数据
            if (OCR_CONFIG.First().Value == null)
            {
                InitDBConfig(OCR_CONFIG.First().Key);
            }
            return OCR_CONFIG.First().Value;
        }

        /// <summary>
        /// 获取实际的数据
        /// </summary>
        /// <returns></returns>
        public DBSmsConfig GetDBSmsConfig()
        {
            // 查询DB
            if (!IS_USE_CACHE)
            {
                return JsonConvert.DeserializeObject<DBSmsConfig>(SelectByGroupKey(SMS_CONFIG.First().Key));
            }

            // 缓存数据
            if (SMS_CONFIG.First().Value == null)
            {
                InitDBConfig(SMS_CONFIG.First().Key);
            }
            return SMS_CONFIG.First().Value;
        }

        public async Task<int> UpdateByConfigKeyAsync(Dictionary<string, string> configs, string groupKey, string sysType, string belongInfoId)
        {
            foreach (KeyValuePair<string, string> config in configs)
            {
                var isAdd = false;
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
                    isAdd = true;
                    await _sysConfigRepository.AddAsync(sysConfig);
                }
                sysConfig.ConfigKey = config.Key;
                sysConfig.ConfigVal = config.Key switch
                {
                    "aliyunOssConfig" or
                    "agpaydxSmsConfig" or "aliyundySmsConfig" or
                    "tencentOcrConfig" or "aliOcrConfig" or "baiduOcrConfig" => StringUtil.Merge(sysConfig.ConfigVal, config.Value),
                    _ => config.Value,
                };
                sysConfig.UpdatedAt = DateTime.Now;
                if (isAdd)
                {
                    await _sysConfigRepository.AddAsync(sysConfig);
                }
                else
                {
                    _sysConfigRepository.Update(sysConfig);
                }
            }
            return await _sysConfigRepository.SaveChangesAsync();
        }

        public IEnumerable<SysConfigDto> GetByGroupKey(string groupKey, string sysType, string belongInfoId)
        {
            var sysConfigs = _sysConfigRepository.GetAllAsNoTracking()
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
            var sysConfigsTemp = _sysConfigRepository.GetAllAsNoTracking()
                .Where(w => w.GroupKey.Equals(groupKey) && w.SysType.Equals(sysType) && w.BelongInfoId.Equals(belongInfoId))
                .OrderBy(o => o.SortNum);
            var mergingList = sysConfigsTemp
                .Where(sc => !sysConfigs.Any(temp => temp.ConfigKey == sc.ConfigKey))
                .Select(sc => sc)
                .ToList();
            mergedList.AddRange(mergingList);
        }

        public SysConfigDto GetByKey(string configKey, string sysType, string belongInfoId)
        {
            var entity = _sysConfigRepository.GetByKey(configKey, sysType, belongInfoId);
            var dto = _mapper.Map<SysConfigDto>(entity);
            return dto;
        }
    }
}
