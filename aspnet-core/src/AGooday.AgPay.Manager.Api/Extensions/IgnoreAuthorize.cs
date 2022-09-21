using Microsoft.AspNetCore.Mvc.Filters;

namespace AGooday.AgPay.Manager.Api.Extensions
{
    public class IgnoreAuthorize : Attribute, IFilterMetadata
    {
    }
}
