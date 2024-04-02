using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface ISysConfigRepository : IAgPayRepository<SysConfig>
    {
        bool IsExistSysConfig(string configKey);
        SysConfig GetByKey(string configKey, string sysType, string belongInfoId);
    }
}
