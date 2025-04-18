﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Params;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Cache.Services;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Isv
{
    /// <summary>
    /// 服务商支付接口管理类
    /// </summary>
    [Route("api/isv/payConfigs")]
    [ApiController, Authorize]
    public class IsvPayInterfaceConfigController : CommonController
    {
        private readonly IMQSender _mqSender;
        private readonly IPayInterfaceConfigService _payIfConfigService;

        public IsvPayInterfaceConfigController(ILogger<IsvPayInterfaceConfigController> logger,
            ICacheService cacheService,
            IAuthService authService,
            IMQSender mqSender,
            IPayInterfaceConfigService payIfConfigService)
            : base(logger, cacheService, authService)
        {
            _mqSender = mqSender;
            _payIfConfigService = payIfConfigService;
        }

        /// <summary>
        /// 查询服务商支付接口配置列表
        /// </summary>
        /// <param name="isvNo"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ISV_PAY_CONFIG_LIST)]
        public async Task<ApiRes> ListAsync(string isvNo)
        {
            var data = await _payIfConfigService.SelectAllPayIfConfigListByIsvNoAsync(CS.INFO_TYPE.ISV, isvNo);
            return ApiRes.Ok(data);
        }

        /// <summary>
        /// 根据 服务商号、接口类型 获取商户参数配置
        /// </summary>
        /// <param name="isvNo"></param>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        [HttpGet, Route("{isvNo}/{ifCode}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ISV_PAY_CONFIG_VIEW)]
        public async Task<ApiRes> GetByIsvNoAsync(string isvNo, string ifCode)
        {
            var payInterfaceConfig = await _payIfConfigService.GetByInfoIdAndIfCodeAsync(CS.INFO_TYPE.ISV, isvNo, ifCode);
            if (payInterfaceConfig != null)
            {
                // 费率转换为百分比数值
                payInterfaceConfig.IfRate = payInterfaceConfig.IfRate * 100;
                if (!string.IsNullOrWhiteSpace(payInterfaceConfig.IfParams))
                {
                    var isvParams = IsvParams.Factory(payInterfaceConfig.IfCode, payInterfaceConfig.IfParams);
                    if (isvParams != null)
                    {
                        payInterfaceConfig.IfParams = isvParams.DeSenData();
                    }
                }
            }
            return ApiRes.Ok(payInterfaceConfig);
        }

        /// <summary>
        /// 服务商支付接口参数配置
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("更新服务商支付参数")]
        [PermissionAuth(PermCode.MGR.ENT_ISV_PAY_CONFIG_ADD)]
        public async Task<ApiRes> SaveOrUpdateAsync(PayInterfaceConfigDto dto)
        {
            dto.InfoType = CS.INFO_TYPE.ISV;
            dto.IfRate = dto.IfRate / 100;// 存入真实费率
            //添加更新者信息
            long userId = await GetCurrentUserIdAsync();
            string realName = (await GetCurrentUserAsync()).SysUser.Realname;
            dto.UpdatedUid = userId;
            dto.UpdatedBy = realName;
            dto.UpdatedAt = DateTime.Now;

            //根据 服务商号、接口类型 获取商户参数配置
            var dbRecoed = await _payIfConfigService.GetByInfoIdAndIfCodeAsync(CS.INFO_TYPE.ISV, dto.InfoId, dto.IfCode);
            //若配置存在，为saveOrUpdate添加ID，第一次配置添加创建者
            if (dbRecoed != null)
            {
                dto.Id = dbRecoed.Id;
                dto.CreatedUid = dbRecoed.CreatedUid;
                dto.CreatedBy = dbRecoed.CreatedBy;
                dto.CreatedAt = dbRecoed.CreatedAt;
                // 合并支付参数
                dto.IfParams = StringUtil.Merge(dbRecoed.IfParams, dto.IfParams);
            }
            else
            {
                dto.CreatedUid = userId;
                dto.CreatedBy = realName;
                dto.CreatedAt = DateTime.Now;
            }
            var result = await _payIfConfigService.SaveOrUpdateAsync(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYSTEM_ERROR, "配置失败");
            }

            // 推送mq到目前节点进行更新数据
            await _mqSender.SendAsync(ResetIsvAgentMchAppInfoConfigMQ.Build(ResetIsvAgentMchAppInfoConfigMQ.RESET_TYPE_ISV_INFO, dto.InfoId, null, null, null));

            return ApiRes.Ok();
        }
    }
}
