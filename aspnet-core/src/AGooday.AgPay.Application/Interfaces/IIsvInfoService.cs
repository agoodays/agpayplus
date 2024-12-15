﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IIsvInfoService : IAgPayService<IsvInfoDto>
    {
        Task<bool> IsExistIsvNoAsync(string isvNo);
        Task<PaginatedList<IsvInfoDto>> GetPaginatedDataAsync(IsvInfoQueryDto dto);
    }
}
