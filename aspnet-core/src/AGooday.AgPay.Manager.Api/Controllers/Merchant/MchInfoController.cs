using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using AGooday.AgPay.Domain.Core.Notifications;

namespace AGooday.AgPay.Manager.Api.Controllers.Merchant
{
    /// <summary>
    /// 商户管理类
    /// </summary>
    [Route("/api/mchInfo")]
    [ApiController]
    public class MchInfoController : ControllerBase
    {
        private readonly ILogger<MchInfoController> _logger;
        private readonly IMchInfoService _mchInfoService;

        private readonly DomainNotificationHandler _notifications;

        public MchInfoController(ILogger<MchInfoController> logger, INotificationHandler<DomainNotification> notifications, 
            IMchInfoService mchInfoService)
        {
            _logger = logger;
            _mchInfoService = mchInfoService;
            _notifications = (DomainNotificationHandler)notifications;
        }

        /// <summary>
        /// 商户信息列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public ApiRes List([FromQuery] MchInfoQueryDto dto)
        {
            var data = _mchInfoService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
        }

        /// <summary>
        /// 新增商户信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public ApiRes Add(MchInfoCreateDto dto)
        {
            _mchInfoService.Create(dto);
            // 是否存在消息通知
            if (!_notifications.HasNotifications())
                return ApiRes.Ok();
            else
                return ApiRes.CustomFail(_notifications.GetNotifications().Select(s => s.Value).ToArray());
        }

        /// <summary>
        /// 删除商户信息
        /// </summary>
        /// <param name="mchNo"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{mchNo}")]
        public ApiRes Delete(string mchNo)
        {
            _mchInfoService.Remove(mchNo);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新商户信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{mchNo}")]
        public ApiRes Update(MchInfoModifyDto dto)
        {
            _mchInfoService.Modify(dto);
            // 是否存在消息通知
            if (!_notifications.HasNotifications())
                return ApiRes.Ok();
            else
                return ApiRes.CustomFail(_notifications.GetNotifications().Select(s => s.Value).ToArray());
        }

        /// <summary>
        /// 查询商户信息
        /// </summary>
        /// <param name="mchNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{mchNo}")]
        public ApiRes Detail(string mchNo)
        {
            var mchInfo = _mchInfoService.GetByMchNo(mchNo);
            if (mchInfo == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(mchInfo);
        }
    }
}
