using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysConfigService : IDisposable
    {
        void InitDBConfig(string groupKey);
        DBApplicationConfig GetDBApplicationConfig();
        DBNoticeConfig GetDBNoticeConfig();
        DBOssConfig GetDBOssConfig();
        DBOcrConfig GetDBOcrConfig();
        DBSmsConfig GetDBSmsConfig();
        int UpdateByConfigKey(Dictionary<string, string> configs, string groupKey, string sysType, string belongInfoId);
        IEnumerable<SysConfigDto> GetByGroupKey(string groupKey, string sysType, string belongInfoId);
        Dictionary<string, string> GetKeyValueByGroupKey(string groupKey, string sysType, string belongInfoId);
        void Add(SysConfigDto dto);
        void Remove(string recordId);
        void Update(SysConfigDto dto);
        bool SaveOrUpdate(SysConfigDto dto);
        SysConfigDto GetById(string recordId);
        SysConfigDto GetByKey(string configKey, string sysType, string belongInfoId);
        IEnumerable<SysConfigDto> GetAll();
    }
}
