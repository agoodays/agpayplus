using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Merchant
{
    /// <summary>
    /// 商户应用管理类
    /// </summary>
    [Route("/api/mchApps")]
    [ApiController, Authorize]
    public class MchAppController : ControllerBase
    {
        private readonly IMQSender mqSender;
        private readonly ILogger<MchAppController> _logger;
        private readonly IMchAppService _mchAppService;
        private readonly IMchInfoService _mchInfoService;

        public MchAppController(IMQSender mqSender, ILogger<MchAppController> logger,
            IMchAppService mchAppService,
            IMchInfoService mchInfoService)
        {
            this.mqSender = mqSender;
            _logger = logger;
            _mchAppService = mchAppService;
            _mchInfoService = mchInfoService;
        }

        /// <summary>
        /// 应用列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_MCH_APP_LIST)]
        public ApiRes List([FromQuery] MchAppQueryDto dto)
        {
            var data = _mchAppService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
        }

        /// <summary>
        /// 新建应用
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("新建应用")]
        [PermissionAuth(PermCode.MGR.ENT_MCH_APP_ADD)]
        public ApiRes Add(MchAppDto dto)
        {
            dto.AppId = SeqUtil.GenAppId();
            if (!_mchInfoService.IsExistMchNo(dto.MchNo))
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }

            var result = _mchAppService.Add(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_CREATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 删除应用
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpDelete, Route("{appId}"), MethodLog("删除应用")]
        [PermissionAuth(PermCode.MGR.ENT_MCH_APP_VIEW, PermCode.MGR.ENT_MCH_APP_EDIT)]
        public ApiRes Delete(string appId)
        {
            var mchApp = _mchAppService.GetById(appId);
            _mchAppService.Remove(appId);

            // 推送mq到目前节点进行更新数据
            mqSender.Send(ResetIsvMchAppInfoConfigMQ.Build(ResetIsvMchAppInfoConfigMQ.RESET_TYPE_MCH_APP, null, mchApp.MchNo, appId));

            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新应用信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{appId}"), MethodLog("更新应用信息")]
        [PermissionAuth(PermCode.MGR.ENT_MCH_APP_EDIT)]
        public ApiRes Update(string appId, MchAppDto dto)
        {
            var result = _mchAppService.Update(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_UPDATE);
            }
            // 推送修改应用消息
            mqSender.Send(ResetIsvMchAppInfoConfigMQ.Build(ResetIsvMchAppInfoConfigMQ.RESET_TYPE_MCH_APP, null, dto.MchNo, dto.AppId));

            return ApiRes.Ok();
        }

        /// <summary>
        /// 应用详情
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet, Route("{appId}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_MCH_APP_DEL)]
        public ApiRes Detail(string appId)
        {
            var mchApp = _mchAppService.GetById(appId);
            if (mchApp == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(mchApp);
        }
    }
}
