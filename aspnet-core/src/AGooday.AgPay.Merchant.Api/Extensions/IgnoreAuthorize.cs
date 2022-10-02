using Microsoft.AspNetCore.Mvc.Filters;

namespace AGooday.AgPay.Merchant.Api.Extensions
{
    public class IgnoreAuthorize : Attribute, IFilterMetadata
    {
    }
}
