using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysConfigService : IDisposable
    {
        DBApplicationConfig GetDBApplicationConfig();
        DBOssConfig GetDBOssConfig();
        int UpdateByConfigKey(Dictionary<string, string> configs);
        void Add(SysConfigDto dto);
        void Remove(string recordId);
        void Update(SysConfigDto dto);
        bool SaveOrUpdate(SysConfigDto dto);
        SysConfigDto GetById(string recordId);
        IEnumerable<SysConfigDto> GetAll();
        Dictionary<string, string> GetKeyValueByGroupKey(string groupKey);
        void InitDBConfig(string groupKey);
    }
}
