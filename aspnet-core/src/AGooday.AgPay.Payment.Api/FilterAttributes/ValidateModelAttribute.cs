using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AGooday.AgPay.Payment.Api.FilterAttributes
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 如果其他过滤器已经设置了结果，则跳过验证
            if (context.Result != null) return;

            // 如果验证通过，跳过后面的动作
            if (context.ModelState.IsValid) return;

            // 获取失败的验证信息列表
            var errors = new Dictionary<string, object>();
            context.ModelState
                .Where(s => s.Value != null && s.Value.ValidationState == ModelValidationState.Invalid)
                .ToList().ForEach((item) =>
                {
                    errors.Add(item.Key, item.Value!.Errors.Select(e => e.ErrorMessage).ToList());
                });

            // 统一返回格式
            var result = ApiRes.Fail(ApiCode.PARAMS_ERROR, errors);

            // 设置结果
            context.Result = new OkObjectResult(result);
        }
    }
}
