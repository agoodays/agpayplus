using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Merchant.Api.Attributes;
using AGooday.AgPay.Merchant.Api.Authorization;
using AGooday.AgPay.Merchant.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Merchant.Api.Controllers.Merchant
{
    [Route("/api/mchConfig")]
    [ApiController, Authorize]
    public class MchConfigController : CommonController
    {
        private readonly IMQSender mqSender;
        private readonly ILogger<MchConfigController> _logger;
        private readonly IMchInfoService _mchInfoService;
        private readonly ISysConfigService _sysConfigService;

        public MchConfigController(IMQSender mqSender,
            ILogger<MchConfigController> logger,
            IMchInfoService mchInfoService,
            ISysConfigService sysConfigService, RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            this.mqSender = mqSender;
            _logger = logger;
            _mchInfoService = mchInfoService;
            _sysConfigService = sysConfigService;
        }

        /// <summary>
        /// 分组下的配置
        /// </summary>
        /// <param name="groupKey"></param>
        /// <returns></returns>
        [HttpGet, Route("{groupKey}"), NoLog]
        [PermissionAuth(PermCode.MCH.ENT_MCH_CONFIG)]
        public ApiRes GetConfigs(string groupKey)
        {
            var configList = _sysConfigService.GetByGroupKey(groupKey, CS.SYS_TYPE.MCH, GetCurrentMchNo());
            return ApiRes.Ok(configList);
        }

        /// <summary>
        /// 更新商户配置信息
        /// </summary>
        /// <param name="groupKey"></param>
        /// <param name="configs"></param>
        /// <returns></returns>
        [HttpPut, Route("{groupKey}"), MethodLog("更新商户配置信息")]
        [PermissionAuth(PermCode.MCH.ENT_MCH_CONFIG_EDIT)]
        public ApiRes Update(string groupKey, Dictionary<string, string> configs)
        {
            int update = _sysConfigService.UpdateByConfigKey(configs, groupKey, CS.SYS_TYPE.MCH, GetCurrentMchNo());
            if (update <= 0)
            {
                return ApiRes.Fail(ApiCode.SYSTEM_ERROR, "更新失败");
            }

            return ApiRes.Ok();
        }

        /// <summary>
        /// 更改商户级别
        /// </summary>
        /// <param name="groupKey"></param>
        /// <param name="configs"></param>
        /// <returns></returns>
        [HttpPut, Route("mchLevel"), MethodLog("更改商户级别")]
        [PermissionAuth(PermCode.MCH.ENT_MCH_CONFIG_EDIT)]
        public ApiRes SetMchLevel(ModifyMchLevel model)
        {
            MchInfoUpdateDto dto = new MchInfoUpdateDto();
            dto.MchNo = GetCurrentMchNo();
            dto.MchLevel = model.MchLevel;
            _mchInfoService.UpdateById(dto);
            return ApiRes.Ok();
        }

        /// <summary>
        /// 更改支付密码	
        /// </summary>
        /// <param name="groupKey"></param>
        /// <param name="configs"></param>
        /// <returns></returns>
        [HttpPut, Route("mchSipw"), MethodLog("更改支付密码")]
        [PermissionAuth(PermCode.MCH.ENT_MCH_CONFIG_EDIT)]
        public ApiRes SetMchSipw(ModifyMchSipw model)
        {
            var mchinfo = _mchInfoService.GetById(GetCurrentMchNo());
            string currentSipw = Base64Util.DecodeBase64(model.OriginalPwd);
            bool verified = BCryptUtil.VerifyHash(currentSipw, mchinfo.Sipw);
            //验证当前密码是否正确
            if (!verified)
            {
                throw new BizException("原支付密码验证失败！");
            }
            string opSipw = Base64Util.DecodeBase64(model.ConfirmPwd);
            // 验证原密码与新密码是否相同
            if (opSipw.Equals(currentSipw))
            {
                throw new BizException("新密码与原密码不能相同！");
            }
            MchInfoUpdateDto dto = new MchInfoUpdateDto();
            dto.MchNo = mchinfo.MchNo;
            dto.Sipw = opSipw;
            _mchInfoService.UpdateById(dto);
            return ApiRes.Ok();
        }
    }
}
