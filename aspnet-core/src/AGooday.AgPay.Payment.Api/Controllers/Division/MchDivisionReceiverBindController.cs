﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.Third.Channel;
using AGooday.AgPay.Components.Third.Models;
using AGooday.AgPay.Components.Third.RQRS.Division;
using AGooday.AgPay.Components.Third.RQRS.Msg;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using AGooday.AgPay.Payment.Api.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.Division
{
    /// <summary>
    /// 分账账号绑定
    /// </summary>
    [ApiController]
    public class MchDivisionReceiverBindController : ApiControllerBase
    {
        private readonly ILogger<MchDivisionReceiverBindController> _logger;
        private readonly IChannelServiceFactory<IDivisionService> _divisionServiceFactory;
        private readonly IPayInterfaceConfigService _payInterfaceConfigService;
        private readonly IMchDivisionReceiverService _mchDivisionReceiverService;
        private readonly IMchDivisionReceiverGroupService _mchDivisionReceiverGroupService;

        public MchDivisionReceiverBindController(ILogger<MchDivisionReceiverBindController> logger,
            //IEnumerable<IDivisionService> divisionServices,
            //IServiceProvider serviceProvider,
            IChannelServiceFactory<IDivisionService> divisionServiceFactory,
            IPayInterfaceConfigService payInterfaceConfigService,
            IMchDivisionReceiverService mchDivisionReceiverService,
            IMchDivisionReceiverGroupService mchDivisionReceiverGroupService,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(requestKit, configContextQueryService)
        {
            _logger = logger;
            _divisionServiceFactory = divisionServiceFactory;
            _payInterfaceConfigService = payInterfaceConfigService;
            _mchDivisionReceiverService = mchDivisionReceiverService;
            _mchDivisionReceiverGroupService = mchDivisionReceiverGroupService;

            //var wxpayDivisionService = _divisionServiceFactory("wxpay");
            //var divisions = serviceProvider.GetServices<IDivisionService>();
            //var alipay = divisionServices.FirstOrDefault(f => f.GetType().Equals(typeof(AliPayDivisionService)));
            //var wxpay = divisionServices.FirstOrDefault(f => f.GetType().Equals(typeof(AliPayDivisionService)));
        }

        /// <summary>
        /// 分账账号绑定
        /// </summary>
        /// <param name="bizRQ"></param>
        /// <returns></returns>
        [HttpPost, Route("api/division/receiver/bind")]
        [PermissionAuth(PermCode.PAY.API_DIVISION_BIND)]
        public async Task<ApiRes> BindAsync()
        {
            //获取参数 & 验签
            DivisionReceiverBindRQ bizRQ = await this.GetRQByWithMchSignAsync<DivisionReceiverBindRQ>();

            try
            {
                //检查商户应用是否存在该接口
                string ifCode = bizRQ.IfCode;

                // 商户配置信息
                MchAppConfigContext mchAppConfigContext = await _configContextQueryService.QueryMchInfoAndAppInfoAsync(bizRQ.MchNo, bizRQ.AppId);
                if (mchAppConfigContext == null)
                {
                    throw new BizException("获取商户应用信息失败");
                }
                var mchInfo = mchAppConfigContext.MchInfo;

                if (!await _payInterfaceConfigService.MchAppHasAvailableIfCodeAsync(bizRQ.AppId, ifCode))
                {
                    throw new BizException("商户应用的支付配置不存在或已关闭");
                }
                var group = await _mchDivisionReceiverGroupService.FindByIdAndMchNoAsync(bizRQ.ReceiverGroupId, bizRQ.MchNo);
                if (group == null)
                {
                    throw new BizException("商户分账账号组不存在，请检查或进入商户平台进行创建操作");
                }
                var divisionProfit = Convert.ToDecimal(bizRQ.DivisionProfit);
                if (divisionProfit.CompareTo(Decimal.Zero) <= 0 || divisionProfit.CompareTo(Decimal.One) > 0)
                {
                    throw new BizException("账号分账比例有误, 配置值为[0.0001~1.0000]");
                }

                //生成数据库对象信息 （数据不完成， 暂时不可入库操作）
                var receiver = GenRecord(bizRQ, group, mchInfo, divisionProfit);
                IDivisionService divisionService = _divisionServiceFactory.GetService(ifCode);
                if (divisionService == null)
                {
                    throw new BizException("系统不支持该分账接口");
                }

                ChannelRetMsg retMsg = await divisionService.BindAsync(receiver, mchAppConfigContext);
                if (retMsg.ChannelState == ChannelState.CONFIRM_SUCCESS)
                {
                    receiver.State = CS.YES;
                    receiver.BindSuccessTime = DateTime.Now;
                    await _mchDivisionReceiverService.AddAsync(receiver);
                }
                else
                {
                    receiver.State = CS.NO;
                    receiver.ChannelBindResult = retMsg.ChannelErrMsg;
                }

                DivisionReceiverBindRS bizRes = DivisionReceiverBindRS.BuildByRecord(receiver);

                if (retMsg.ChannelState == ChannelState.CONFIRM_SUCCESS)
                {
                    bizRes.BindState = CS.YES;
                }
                else
                {
                    bizRes.BindState = CS.NO;
                    bizRes.ErrCode = retMsg.ChannelErrCode;
                    bizRes.ErrMsg = retMsg.ChannelErrMsg;
                }

                return ApiRes.OkWithSign(bizRes, bizRQ.SignType, mchAppConfigContext.MchApp.AppSecret);
            }
            catch (BizException e)
            {
                return ApiRes.CustomFail(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"系统异常：{e.Message}");
                return ApiRes.CustomFail("系统异常");
            }
        }

        private static MchDivisionReceiverDto GenRecord(DivisionReceiverBindRQ bizRQ, MchDivisionReceiverGroupDto group, MchInfoDto mchInfo, decimal divisionProfit)
        {
            MchDivisionReceiverDto receiver = new MchDivisionReceiverDto();
            receiver.ReceiverAlias = string.IsNullOrWhiteSpace(bizRQ.ReceiverAlias) ? bizRQ.AccNo : bizRQ.ReceiverAlias; //别名
            receiver.ReceiverGroupId = bizRQ.ReceiverGroupId; //分组ID
            receiver.ReceiverGroupName = group.ReceiverGroupName; //组名称
            receiver.MchNo = bizRQ.MchNo; //商户号
            receiver.IsvNo = mchInfo.IsvNo; //isvNo
            receiver.AppId = bizRQ.AppId; //appId
            receiver.IfCode = bizRQ.IfCode; //接口代码
            receiver.AccType = bizRQ.AccType; //账号类型
            receiver.AccNo = bizRQ.AccNo; //账号
            receiver.AccName = bizRQ.AccName; //账号名称
            receiver.RelationType = bizRQ.RelationType; //关系
            receiver.RelationTypeName = GetRelationTypeName(bizRQ.RelationType) ?? bizRQ.RelationTypeName; //关系名称
            receiver.DivisionProfit = divisionProfit; //分账比例
            receiver.ChannelExtInfo = bizRQ.ChannelExtInfo; //渠道信息

            return receiver;
        }

        private static string GetRelationTypeName(string relationType)
        {
            return relationType switch
            {
                "PARTNER" => "合作伙伴",
                "SERVICE_PROVIDER" => "服务商",
                "STORE" => "门店",
                "STAFF" => "员工",
                "STORE_OWNER" => "店主",
                "HEADQUARTER" => "总部",
                "BRAND" => "品牌方",
                "DISTRIBUTOR" => "分销商",
                "USER" => "用户",
                "SUPPLIER" => "供应商",
                _ => null,
            };
        }
    }
}
