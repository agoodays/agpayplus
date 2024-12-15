using AGooday.AgPay.Application.Config;
using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysConfigService : IAgPayService<SysConfigDto>
    {
        void InitDBConfig(string groupKey);
        DBApplicationConfig GetDBApplicationConfig();
        DBNoticeConfig GetDBNoticeConfig();
        DBOssConfig GetDBOssConfig();
        DBOcrConfig GetDBOcrConfig();
        DBSmsConfig GetDBSmsConfig();
        Task<int> UpdateByConfigKeyAsync(Dictionary<string, string> configs, string groupKey, string sysType, string belongInfoId);
        IEnumerable<SysConfigDto> GetByGroupKey(string groupKey, string sysType, string belongInfoId);
        Dictionary<string, string> GetKeyValueByGroupKey(string groupKey, string sysType, string belongInfoId);
        SysConfigDto GetByKey(string configKey, string sysType, string belongInfoId);
    }
}
