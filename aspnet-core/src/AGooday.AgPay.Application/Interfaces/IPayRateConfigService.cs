﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;
using Newtonsoft.Json.Linq;
using static AGooday.AgPay.Application.DataTransfer.PayRateConfigSaveDto;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayRateConfigService : IAgPayService<PayRateConfigDto, long>
    {
        Task<PaginatedList<PayWayDto>> GetPayWaysByInfoIdAsync(PayWayUsableQueryDto dto);
        JObject GetByInfoIdAndIfCodeJson(string configMode, string infoId, string ifCode);
        Task<PayRateConfigItem> GetPayRateConfigItemAsync(string configType, string infoType, string infoId, string ifCode, string wayCode);
        bool SaveOrUpdate(PayRateConfigSaveDto dto);
        Task<List<PayRateConfigInfoDto>> GetPayRateConfigInfosAsync(string mchNo, string ifCode, string wayCode, long amount, string bankCardType = null);
    }
}
