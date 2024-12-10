using AGooday.AgPay.AopSdk;
using AGooday.AgPay.AopSdk.Exceptions;
using AGooday.AgPay.AopSdk.Models;
using AGooday.AgPay.AopSdk.Request;
using AGooday.AgPay.AopSdk.Response;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Division
{
    /// <summary>
    /// 商户分账接收者账号关系维护
    /// </summary>
    [Route("api/divisionReceivers")]
    [ApiController, Authorize]
    public class MchDivisionReceiverController : CommonController
    {
        private readonly IMchDivisionReceiverService _mchDivisionReceiverService;
        private readonly IMchDivisionReceiverGroupService _mchDivisionReceiverGroupService;
        private readonly IMchAppService _mchAppService;
        private readonly IMchInfoService _mchInfoService;
        private readonly ISysConfigService _sysConfigService;

        public MchDivisionReceiverController(ILogger<MchDivisionReceiverController> logger,
            IMchDivisionReceiverService mchDivisionReceiverService,
            IMchDivisionReceiverGroupService mchDivisionReceiverGroupService,
            IMchAppService mchAppService,
            IMchInfoService mchInfoService,
            ISysConfigService sysConfigService,
            RedisUtil client,
            IAuthService authService)
            : base(logger, client, authService)
        {
            _mchDivisionReceiverService = mchDivisionReceiverService;
            _mchDivisionReceiverGroupService = mchDivisionReceiverGroupService;
            _mchAppService = mchAppService;
            _mchInfoService = mchInfoService;
            _sysConfigService = sysConfigService;
        }

        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_DIVISION_RECEIVER_LIST)]
        public async Task<ApiPageRes<MchDivisionReceiverDto>> ListAsync([FromQuery] MchDivisionReceiverQueryDto dto)
        {
            var data = await _mchDivisionReceiverService.GetPaginatedDataAsync(dto);
            var mchNos = data.Select(s => s.MchNo).Distinct().ToList();
            var mchInfos = _mchInfoService.GetByMchNos(mchNos);
            foreach (var item in data)
            {
                item.AddExt("mchName", mchInfos?.FirstOrDefault(s => s.MchNo == item.MchNo)?.MchName);
            }
            return ApiPageRes<MchDivisionReceiverDto>.Pages(data);
        }

        [HttpGet, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_DIVISION_RECEIVER_VIEW)]
        public async Task<ApiRes> DetailAsync(long recordId)
        {
            var record = await _mchDivisionReceiverService.GetByIdAsync(recordId);
            if (record == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(record);
        }

        /// <summary>
        /// 新增分账接收账号
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost, Route(""), MethodLog("新增分账接收账号")]
        [PermissionAuth(PermCode.MGR.ENT_DIVISION_RECEIVER_ADD)]
        public async Task<ApiRes> AddAsync(DivisionReceiverBindReqModel dto)
        {
            if (!_mchInfoService.IsExistMchNo(dto.MchNo))
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            var mchApp = await _mchAppService.GetByIdAsync(dto.AppId);
            if (mchApp == null || mchApp.State != CS.PUB_USABLE)
            {
                throw new BizException("商户应用不存在或不可用");
            }
            DivisionReceiverBindRequest request = new DivisionReceiverBindRequest();
            request.SetBizModel(dto);
            dto.AppId = mchApp.AppId;
            dto.DivisionProfit = (Convert.ToDecimal(dto.DivisionProfit) / 100).ToString();

            var agPayClient = new AgPayClient(_sysConfigService.GetDBApplicationConfig().PaySiteUrl, mchApp.AppSecret);

            try
            {
                DivisionReceiverBindResponse response = await agPayClient.ExecuteAsync(request);
                if (response.Code != 0)
                {
                    throw new BizException(response.Msg);
                }
                return ApiRes.Ok(response.Get());
            }
            catch (AgPayException e)
            {
                throw new BizException(e.Message);
            }
        }

        /// <summary>
        /// 更新分账接收账号
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPut, Route("{recordId}"), MethodLog("更新分账接收账号")]
        [PermissionAuth(PermCode.MGR.ENT_DIVISION_RECEIVER_EDIT)]
        public ApiRes Update(long recordId, MchDivisionReceiverDto dto)
        {
            // 改为真实比例
            dto.DivisionProfit = dto.DivisionProfit / 100;
            if (dto.ReceiverGroupId != null)
            {
                var groupRecord = _mchDivisionReceiverGroupService.FindByIdAndMchNo(dto.ReceiverGroupId.Value, dto.MchNo);
                if (dto == null)
                {
                    throw new BizException("账号组不存在");
                }
                dto.ReceiverGroupId = groupRecord.ReceiverGroupId;
                dto.ReceiverGroupName = groupRecord.ReceiverGroupName;
            }
            var result = _mchDivisionReceiverService.Update(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_UPDATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 删除分账接收账号
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpDelete, Route("{recordId}"), MethodLog("删除分账接收账号")]
        [PermissionAuth(PermCode.MGR.ENT_DIVISION_RECEIVER_DELETE)]
        public async Task<ApiRes> DeleteAsync(long recordId)
        {
            var record = await _mchDivisionReceiverService.GetByIdAsync(recordId);
            if (record == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            _mchDivisionReceiverService.Remove(recordId);
            return ApiRes.Ok();
        }
    }
}
