using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Manager.Api.Models;
using Newtonsoft.Json;

namespace AGooday.AgPay.Manager.Api.Controllers
{
    [Route("/api/sysRole")]
    [ApiController]
    public class SysRoleController : CommonController
    {
        private readonly ILogger<SysRoleController> _logger;
        private readonly IDatabase _redis;
        private readonly ISysRoleService _sysRoleService;
        private readonly ISysRoleEntRelaService _sysRoleEntRelaService;
        private readonly ISysUserRoleRelaService _sysUserRoleRelaService;

        public SysRoleController(ILogger<SysRoleController> logger, RedisUtil client,
            ISysRoleService sysRoleService,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService, 
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            _logger = logger;
            _sysRoleService = sysRoleService;
            _sysRoleEntRelaService = sysRoleEntRelaService;
            _sysUserRoleRelaService = sysUserRoleRelaService;
        }

        [HttpGet]
        [Route("list")]
        public ApiRes List([FromBody] SysRoleDto dto, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var data = _sysRoleService.GetPaginatedData(dto, pageNumber, pageSize);
            return ApiRes.Ok(new { records = data.ToList(), total = data.TotalCount, current = data.PageIndex, hasNext = data.HasNext });
        }

        [HttpPost]
        [Route("add")]
        public ApiRes Add(SysRoleDto dto)
        {
            dto.RoleId = $"ROLE_{Guid.NewGuid().ToString("N").Substring(0, 6)}";
            dto.SysType = CS.SYS_TYPE.MGR;
            _sysRoleService.Add(dto);

            //如果包含： 可分配权限的权限 && entIdListStr 不为空
            if (GetCurrentUser().Authorities.Contains(PermCode.MGR.ENT_UR_ROLE_DIST)
                && !string.IsNullOrEmpty(dto.EntIdListStr))
            {
                var entIdList = JsonConvert.DeserializeObject<List<string>>(dto.EntIdListStr);
                _sysRoleEntRelaService.ResetRela(dto.RoleId, entIdList);
            }

            return ApiRes.Ok();
        }

        [HttpDelete]
        [Route("delete/{recordId}")]
        public ApiRes Delete(string recordId)
        {
            _sysRoleService.RemoveRole(recordId);
            return ApiRes.Ok();
        }

        [HttpPut]
        [Route("update/{recordId}")]
        public ApiRes Update(SysRoleDto dto)
        {
            _sysRoleService.Update(dto);
            if (GetCurrentUser().Authorities.Contains(PermCode.MGR.ENT_UR_ROLE_DIST)
                && !string.IsNullOrEmpty(dto.EntIdListStr))
            {
                var entIdList = JsonConvert.DeserializeObject<List<string>>(dto.EntIdListStr);
                _sysRoleEntRelaService.ResetRela(dto.RoleId, entIdList);

                //查询到该角色的人员， 将redis更新
                var sysUserIdList = _sysUserRoleRelaService.SelectRoleIdsByRoleId(dto.RoleId).ToList();
                RefAuthentication(sysUserIdList);
            }
            return ApiRes.Ok();
        }

        [HttpGet]
        [Route("detail/{recordId}")]
        public ApiRes Detail(string recordId)
        {
            var sysRole = _sysRoleService.GetById(recordId);
            if (sysRole == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(sysRole);
        }
    }
}
