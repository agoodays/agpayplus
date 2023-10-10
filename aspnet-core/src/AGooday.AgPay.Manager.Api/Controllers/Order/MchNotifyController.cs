using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Order
{
    /// <summary>
    /// 商户通知类
    /// </summary>
    [Route("/api/mchNotify")]
    [ApiController, Authorize, NoLog]
    public class MchNotifyController : ControllerBase
    {
        private readonly IMQSender mqSender;
        private readonly ILogger<MchNotifyController> _logger;
        private readonly IMchNotifyRecordService _mchNotifyService;

        public MchNotifyController(IMQSender mqSender, IMchNotifyRecordService mchNotifyService, ILogger<MchNotifyController> logger)
        {
            this.mqSender = mqSender;
            _logger = logger;
            _mchNotifyService = mchNotifyService;
        }

        /// <summary>
        /// 商户通知列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("")]
        [PermissionAuth(PermCode.MGR.ENT_NOTIFY_LIST)]
        public ApiPageRes<MchNotifyRecordDto> List([FromQuery] MchNotifyQueryDto dto)
        {
            dto.BindDateRange();
            var data = _mchNotifyService.GetPaginatedData(dto);
            return ApiPageRes<MchNotifyRecordDto>.Pages(data);
        }

        /// <summary>
        /// 商户通知信息
        /// </summary>
        /// <param name="notifyId"></param>
        /// <returns></returns>
        [HttpGet, Route("{notifyId}")]
        [PermissionAuth(PermCode.MGR.ENT_MCH_NOTIFY_VIEW)]
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
        [PermissionAuth(PermCode.MGR.ENT_MCH_NOTIFY_RESEND)]
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
            mqSender.Send(PayOrderMchNotifyMQ.Build(notifyId));

            return ApiRes.Ok(mchNotify);
        }
    }
}
