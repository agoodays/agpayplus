using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;

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
        IEnumerable<SysConfigDto> GetByKey(string groupKey, string sysType = CS.SYS_TYPE.MGR, string belongInfoId = "0");
        Dictionary<string, string> GetKeyValueByGroupKey(string groupKey, string sysType = CS.SYS_TYPE.MGR, string belongInfoId = "0");
        void InitDBConfig(string groupKey);
    }
}
