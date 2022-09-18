using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Application.ViewModels;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Nodes;

namespace AGooday.AgPay.Manager.Api.Controllers
{
    [Route("/api/sysEnt")]
    [ApiController]
    public class SysEntController : ControllerBase
    {
        private readonly ILogger<SysEntController> _logger;
        private readonly ISysEntitlementService _sysEntService;

        public SysEntController(ILogger<SysEntController> logger, ISysEntitlementService sysEntService)
        {
            _logger = logger;
            _sysEntService = sysEntService;
        }

        [HttpGet]
        [Route("bySysType")]
        public ApiRes BySystem(string sysType, string entId)
        {
            var sysEnts = _sysEntService.GetBySysType(sysType, entId);
            return ApiRes.Ok(sysEnts);
        }

        [HttpPut]
        [Route("update/{entId}")]
        public ApiRes Update(SysEntitlementVM vm)
        {
            _sysEntService.Update(vm);
            return ApiRes.Ok();
        }

        [HttpGet]
        [Route("showTree")]
        public ActionResult ShowTree(string sysType)
        {
            //查询全部数据
            var sysEnt = _sysEntService.GetBySysType(sysType, null);

            //递归转换为树状结构
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var jsonArray = JArray.FromObject(sysEnt);
            var leftMenuTree = new TreeDataBuilder(jsonArray, "entId", "pid", "children", "entSort", true).BuildTreeObject();
            var json = JsonConvert.SerializeObject(ApiRes.Ok(leftMenuTree));
            return Content(json, "application/json");
        }
    }
}
