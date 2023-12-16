using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IStatisticService
    {
        PaginatedList<StatisticResultDto> Statistics(string agentNo, StatisticQueryDto dto);
        JObject Total(string agentNo, StatisticQueryDto dto);
    }
}
