using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface ISysConfigRepository : IRepository<SysConfig>
    {
        bool IsExistSysConfig(string configKey);
        SysConfig GetByKey(string configKey, string sysType, string belongInfoId);
    }
}
