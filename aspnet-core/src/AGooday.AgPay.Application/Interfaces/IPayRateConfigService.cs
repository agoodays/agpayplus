using AGooday.AgPay.Application.DataTransfer;
using Newtonsoft.Json.Linq;
using static AGooday.AgPay.Application.DataTransfer.PayRateConfigSaveDto;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayRateConfigService
    {
        PaginatedList<PayWayDto> GetPayWaysByInfoId(PayWayUsableQueryDto dto);
        JObject GetByInfoIdAndIfCodeJson(string configMode, string infoId, string ifCode);
        PayRateConfigItem GetPayRateConfigItem(string configType, string infoType, string infoId, string ifCode, string wayCode);
        bool SaveOrUpdate(PayRateConfigSaveDto dto);
    }
}
