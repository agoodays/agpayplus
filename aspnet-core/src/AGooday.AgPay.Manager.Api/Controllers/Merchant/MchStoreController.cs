using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Merchant
{
    /// <summary>
    /// 商户门店管理类
    /// </summary>
    [Route("/api/mchStore")]
    [ApiController, Authorize]
    public class MchStoreController : ControllerBase
    {
        private readonly IMQSender mqSender;
        private readonly ILogger<MchStoreController> _logger;
        private readonly IMchStoreService _mchStoreService;
        private readonly IMchInfoService _mchInfoService;

        public MchStoreController(IMQSender mqSender, ILogger<MchStoreController> logger,
            IMchStoreService mchStoreService,
            IMchInfoService mchInfoService)
        {
            this.mqSender = mqSender;
            _logger = logger;
            _mchStoreService = mchStoreService;
            _mchInfoService = mchInfoService;
        }

        /// <summary>
        /// 门店列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_MCH_STORE_LIST)]
        public ApiRes List([FromQuery] MchStoreQueryDto dto)
        {
            var data = _mchStoreService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
        }

        /// <summary>
        /// 新建门店
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("新建门店")]
        [PermissionAuth(PermCode.MGR.ENT_MCH_STORE_ADD)]
        public ApiRes Add(MchStoreDto dto)
        {
            if (!_mchInfoService.IsExistMchNo(dto.MchNo))
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            var mchInfo = _mchInfoService.GetByMchNo(dto.MchNo);
            dto.AgentNo = mchInfo.AgentNo;
            dto.IsvNo = mchInfo.IsvNo;
            var result = _mchStoreService.Add(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_CREATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 删除门店
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpDelete, Route("{recordId}"), MethodLog("删除门店")]
        [PermissionAuth(PermCode.MGR.ENT_MCH_STORE_DEL)]
        public ApiRes Delete(long recordId)
        {
            var mchStore = _mchStoreService.GetById(recordId);
            _mchStoreService.Remove(recordId);

            //// 推送mq到目前节点进行更新数据
            //mqSender.Send(ResetIsvMchStoreInfoConfigMQ.Build(ResetIsvMchStoreInfoConfigMQ.RESET_TYPE_MCH_STORE, null, mchStore.MchNo, recordId));

            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新门店信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{recordId}"), MethodLog("更新门店信息")]
        [PermissionAuth(PermCode.MGR.ENT_MCH_STORE_EDIT)]
        public ApiRes Update(long recordId, MchStoreDto dto)
        {
            var result = _mchStoreService.Update(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_UPDATE);
            }
            //// 推送修改门店消息
            //mqSender.Send(ResetIsvMchStoreInfoConfigMQ.Build(ResetIsvMchStoreInfoConfigMQ.RESET_TYPE_MCH_STORE, null, dto.MchNo, dto.AppId));

            return ApiRes.Ok();
        }

        /// <summary>
        /// 门店详情
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpGet, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_MCH_STORE_VIEW, PermCode.MGR.ENT_MCH_STORE_EDIT)]
        public ApiRes Detail(long recordId)
        {
            var mchStore = _mchStoreService.GetById(recordId);
            if (mchStore == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(mchStore);
        }
    }
}
