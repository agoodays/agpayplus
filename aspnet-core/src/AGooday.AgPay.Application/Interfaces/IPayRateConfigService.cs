using AGooday.AgPay.Application.DataTransfer;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayRateConfigService
    {
        PaginatedList<PayWayDto> GetPayWaysByInfoId(PayWayUsableQueryDto dto);
        JObject GetByInfoIdAndIfCodeJson(string configMode, string infoId, string ifCode);
        bool SaveOrUpdate(PayRateConfigSaveDto dto);
    }
}
