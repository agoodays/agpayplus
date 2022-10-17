using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysConfigService : IDisposable
    {
        DBApplicationConfig GetDBApplicationConfig();
        int UpdateByConfigKey(Dictionary<string, string> configs);
        void Add(SysConfigDto dto);
        void Remove(string recordId);
        void Update(SysConfigDto dto);
        bool SaveOrUpdate(SysConfigDto dto);
        SysConfigDto GetById(string recordId);
        IEnumerable<SysConfigDto> GetAll();
        void InitDBConfig(string groupKey);
    }
}
