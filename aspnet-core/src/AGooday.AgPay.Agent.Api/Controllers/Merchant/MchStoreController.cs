﻿using AGooday.AgPay.Agent.Api.Attributes;
using AGooday.AgPay.Agent.Api.Authorization;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Components.MQ.Vender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Agent.Api.Controllers.Merchant
{
    /// <summary>
    /// 商户门店管理类
    /// </summary>
    [Route("api/mchStore")]
    [ApiController, Authorize]
    public class MchStoreController : CommonController
    {
        private readonly IMQSender _mqSender;
        private readonly IMchStoreService _mchStoreService;
        private readonly IMchInfoService _mchInfoService;
        private readonly ISysConfigService _sysConfigService;

        public MchStoreController(ILogger<MchStoreController> logger,
            ICacheService cacheService,
            IAuthService authService,
            IMQSender mqSender,
            IMchStoreService mchStoreService,
            IMchInfoService mchInfoService,
            ISysConfigService sysConfigService)
            : base(logger, cacheService, authService)
        {
            _mqSender = mqSender;
            _mchStoreService = mchStoreService;
            _mchInfoService = mchInfoService;
            _sysConfigService = sysConfigService;
        }

        /// <summary>
        /// 门店列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.AGENT.ENT_MCH_STORE_LIST)]
        public async Task<ApiPageRes<MchStoreListDto>> ListAsync([FromQuery] MchStoreQueryDto dto)
        {
            dto.AgentNo = await GetCurrentAgentNoAsync();
            var data = await _mchStoreService.GetPaginatedDataAsync(dto);
            return ApiPageRes<MchStoreListDto>.Pages(data);
        }

        /// <summary>
        /// 新建门店
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("新建门店")]
        [PermissionAuth(PermCode.AGENT.ENT_MCH_STORE_ADD)]
        public async Task<ApiRes> AddAsync(MchStoreDto dto)
        {
            if (!await _mchInfoService.IsExistMchNoAsync(dto.MchNo))
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            var mchInfo = await _mchInfoService.GetByIdAsync(dto.MchNo);
            var sysUser = (await GetCurrentUserAsync()).SysUser;
            dto.CreatedBy = sysUser.Realname;
            dto.CreatedUid = sysUser.SysUserId;
            dto.AgentNo = mchInfo.AgentNo;
            dto.IsvNo = mchInfo.IsvNo;
            var result = await _mchStoreService.AddAsync(dto);
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
        [PermissionAuth(PermCode.AGENT.ENT_MCH_STORE_DEL)]
        public async Task<ApiRes> DeleteAsync(long recordId)
        {
            await _mchStoreService.RemoveAsync(recordId);

            //// 推送mq到目前节点进行更新数据
            //var mchStore = await _mchStoreService.GetByIdAsync(recordId);
            //await mqSender.SendAsync(ResetIsvAgentMchAppInfoConfigMQ.Build(ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_MCH_STORE, null, null, mchStore.MchNo, null, recordId));

            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新门店信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{recordId}"), MethodLog("更新门店信息")]
        [PermissionAuth(PermCode.AGENT.ENT_MCH_STORE_EDIT)]
        public async Task<ApiRes> UpdateAsync(long recordId, MchStoreDto dto)
        {
            if (!dto.StoreId.HasValue || dto.StoreId.Value <= 0) // 应用分配
            {
                var sysUser = await _mchStoreService.GetByIdAsNoTrackingAsync(recordId);
                sysUser.BindAppId = dto.BindAppId;
                CopyUtil.CopyProperties(sysUser, dto);
            }
            var result = await _mchStoreService.UpdateAsync(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_UPDATE);
            }

            //// 推送修改门店消息
            //mqSender.Send(ResetIsvAgentMchAppInfoConfigMQ.Build(ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_MCH_STORE, null, null, dto.MchNo, dto.AppId));

            return ApiRes.Ok();
        }

        /// <summary>
        /// 门店详情
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpGet, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.AGENT.ENT_MCH_STORE_VIEW, PermCode.AGENT.ENT_MCH_STORE_EDIT)]
        public async Task<ApiRes> DetailAsync(long recordId)
        {
            var mchStore = await _mchStoreService.GetByIdAsNoTrackingAsync(recordId);
            if (mchStore == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(mchStore);
        }

        /// <summary>
        /// 获取地图配置
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("mapConfig"), AllowAnonymous, NoLog]
        public ApiRes MapConfig()
        {
            //var apiMapWebKey = "4056aa1b76794cdb8df1d5ee0000bdb8
            //var apiMapWebKey = "73c97ee762590de79509117207e170ab";
            //var apiMapWebSecret = "7fec782d86662766f46d8d92e4651154";

            var configList = _sysConfigService.GetKeyValueByGroupKey("apiMapConfig", CS.SYS_TYPE.MGR, CS.BASE_BELONG_INFO_ID.MGR);
            return ApiRes.Ok(configList);
        }
    }
}
