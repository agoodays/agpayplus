using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IStatisticService
    {
        PaginatedList<StatisticResultDto> Statistics(StatisticQueryDto dto);
        JObject Total(StatisticQueryDto dto);
    }
}
