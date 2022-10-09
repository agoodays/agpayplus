﻿using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Application.Params;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Models;

namespace AGooday.AgPay.Manager.Api.Controllers.Isv
{
    /// <summary>
    /// 服务商支付接口管理类
    /// </summary>
    [Route("/api/isv/payConfigs")]
    [ApiController]
    public class IsvPayInterfaceConfigController : CommonController
    {
        private readonly ILogger<IsvPayInterfaceConfigController> _logger;
        private readonly IPayInterfaceConfigService _payIfConfigService;

        public IsvPayInterfaceConfigController(ILogger<IsvPayInterfaceConfigController> logger, RedisUtil client,
            IPayInterfaceConfigService payIfConfigService,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
            _payIfConfigService = payIfConfigService;
        }

        /// <summary>
        /// 查询服务商支付接口配置列表
        /// </summary>
        /// <param name="isvNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public ApiRes List(string isvNo)
        {
            var data = _payIfConfigService.SelectAllPayIfConfigListByIsvNo(CS.INFO_TYPE_ISV, isvNo);
            return ApiRes.Ok(data);
        }

        /// <summary>
        /// 根据 服务商号、接口类型 获取商户参数配置
        /// </summary>
        /// <param name="isvNo"></param>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{isvNo}/{ifCode}")]
        public ApiRes GetByIsvNo(string isvNo, string ifCode)
        {
            var payInterfaceConfig = _payIfConfigService.GetByInfoIdAndIfCode(CS.INFO_TYPE_ISV, isvNo, ifCode);
            if (payInterfaceConfig != null) {
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
        [HttpPost]
        [Route("")]
        public ApiRes SaveOrUpdate(PayInterfaceConfigDto dto)
        {
            dto.InfoType = CS.INFO_TYPE_ISV;
            dto.IfRate = dto.IfRate / 100;// 存入真实费率
            //添加更新者信息
            long userId = GetCurrentUser().User.SysUserId;
            string realName = GetCurrentUser().User.Realname;
            dto.UpdatedUid = userId;
            dto.UpdatedBy = realName;

            //根据 服务商号、接口类型 获取商户参数配置
            var dbRecoed = _payIfConfigService.GetByInfoIdAndIfCode(CS.INFO_TYPE_ISV, dto.InfoId, dto.IfCode);
            //若配置存在，为saveOrUpdate添加ID，第一次配置添加创建者
            if (dbRecoed != null)
            {
                dto.Id = dbRecoed.Id;
            }
            else
            {
                dto.CreatedUid = userId;
                dto.CreatedBy = realName;
            }
            var result = _payIfConfigService.SaveOrUpdate(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYSTEM_ERROR, "配置失败");
            }

            // 推送mq到目前节点进行更新数据

            return ApiRes.Ok();
        }
    }
}