using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IStatisticService
    {
        Task<PaginatedResult<StatisticResultDto>> StatisticsAsync(string agentNo, StatisticQueryDto dto);
        Task<JObject> TotalAsync(string agentNo, StatisticQueryDto dto);
    }
}
