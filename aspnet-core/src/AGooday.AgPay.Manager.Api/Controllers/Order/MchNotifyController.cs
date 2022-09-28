using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Manager.Api.Controllers.Merchant;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Order
{
    /// <summary>
    /// 商户通知类
    /// </summary>
    [Route("/api/mchNotify")]
    [ApiController]
    public class MchNotifyController : ControllerBase
    {
        private readonly ILogger<MchNotifyController> _logger;
        private readonly IMchNotifyRecordService _mchNotifyService;

        public MchNotifyController(IMchNotifyRecordService mchNotifyService, ILogger<MchNotifyController> logger)
        {
            _mchNotifyService = mchNotifyService;
            _logger = logger;
        }

        /// <summary>
        /// 商户通知列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        public ApiRes List([FromQuery] MchNotifyQueryDto dto)
        {
            var data = _mchNotifyService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
        }

        /// <summary>
        /// 商户通知信息
        /// </summary>
        /// <param name="notifyId"></param>
        /// <returns></returns>
        [HttpGet, Route("{notifyId}")]
        public ApiRes Detail(long notifyId)
        {
            var mchNotify = _mchNotifyService.GetById(notifyId);
            if (mchNotify == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(mchNotify);
        }

        /// <summary>
        /// 商户通知重发操作
        /// </summary>
        /// <param name="notifyId"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost, Route("{notifyId}")]
        public ApiRes Resend(long notifyId)
        {
            var mchNotify = _mchNotifyService.GetById(notifyId);
            if (mchNotify == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            if (mchNotify.State != (byte)MchNotifyRecordState.STATE_FAIL)
            {
                throw new BizException("请选择失败的通知记录");
            }

            //更新通知中
            _mchNotifyService.UpdateIngAndAddNotifyCountLimit(notifyId);

            //调起MQ重发

            return ApiRes.Ok(mchNotify);
        }
    }
}
