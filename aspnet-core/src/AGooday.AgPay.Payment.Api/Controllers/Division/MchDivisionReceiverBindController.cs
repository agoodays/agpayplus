using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel;
using AGooday.AgPay.Payment.Api.Channel.AliPay;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Division;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.Division
{
    [ApiController]
    public class MchDivisionReceiverBindController : ControllerBase
    {
        private readonly ILogger<MchDivisionReceiverBindController> _logger;
        private readonly Func<string, IDivisionService> _divisionServiceFactory;
        private readonly ConfigContextQueryService _configContextQueryService;
        private readonly IPayInterfaceConfigService _payInterfaceConfigService;
        private readonly IMchDivisionReceiverService _mchDivisionReceiverService;
        private readonly IMchDivisionReceiverGroupService _mchDivisionReceiverGroupService;

        public MchDivisionReceiverBindController(ILogger<MchDivisionReceiverBindController> logger,
            //IEnumerable<IDivisionService> divisionServices,
            //IServiceProvider serviceProvider,
            Func<string, IDivisionService> divisionServiceFactory,
            ConfigContextQueryService configContextQueryService,
            IPayInterfaceConfigService payInterfaceConfigService,
            IMchDivisionReceiverService mchDivisionReceiverService,
            IMchDivisionReceiverGroupService mchDivisionReceiverGroupService)
        {
            _logger = logger;
            _divisionServiceFactory = divisionServiceFactory;
            _configContextQueryService = configContextQueryService;
            _payInterfaceConfigService = payInterfaceConfigService;
            _mchDivisionReceiverService = mchDivisionReceiverService;
            _mchDivisionReceiverGroupService = mchDivisionReceiverGroupService;

            //var wxpayDivisionService = _divisionServiceFactory("wxpay");
            //var divisions = serviceProvider.GetServices<IDivisionService>();
            //var alipay = divisionServices.FirstOrDefault(f => f.GetType().Equals(typeof(AliPayDivisionService)));
            //var wxpay = divisionServices.FirstOrDefault(f => f.GetType().Equals(typeof(AliPayDivisionService)));
        }

        [HttpPost, Route("api/division/receiver/bind")]
        public ApiRes Bind(DivisionReceiverBindRQ bizRQ)
        {
            try
            {
                //检查商户应用是否存在该接口
                string ifCode = bizRQ.IfCode;

                // 商户配置信息
                MchAppConfigContext mchAppConfigContext = _configContextQueryService.QueryMchInfoAndAppInfo(bizRQ.MchNo, bizRQ.AppId);
                if (mchAppConfigContext == null)
                {
                    throw new BizException("获取商户应用信息失败");
                }
                var mchInfo = mchAppConfigContext.MchInfo;

                if (!_payInterfaceConfigService.MchAppHasAvailableIfCode(bizRQ.AppId, ifCode))
                {
                    throw new BizException("商户应用的支付配置不存在或已关闭");
                }
                var group = _mchDivisionReceiverGroupService.FindByIdAndMchNo(bizRQ.ReceiverGroupId, bizRQ.MchNo);
                if (group == null)
                {
                    throw new BizException("商户分账账号组不存在，请检查或进入商户平台进行创建操作");
                }
                var divisionProfit = Convert.ToDecimal(bizRQ.DivisionProfit);
                if (divisionProfit.CompareTo(Decimal.Zero) <= 0 || divisionProfit.CompareTo(Decimal.One) > 1)
                {
                    throw new BizException("账号分账比例有误, 配置值为[0.0001~1.0000]");
                }

                //生成数据库对象信息 （数据不完成， 暂时不可入库操作）
                var receiver = GenRecord(bizRQ, group, mchInfo, divisionProfit);
                IDivisionService divisionService = _divisionServiceFactory(ifCode);
                if (divisionService == null)
                {
                    throw new BizException("系统不支持该分账接口");
                }

                ChannelRetMsg retMsg = divisionService.Bind(receiver, mchAppConfigContext);
                if (retMsg.ChannelState == ChannelState.CONFIRM_SUCCESS)
                {
                    receiver.State = CS.YES;
                    receiver.BindSuccessTime = DateTime.Now;
                    _mchDivisionReceiverService.Add(receiver);

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

                return ApiRes.OkWithSign(bizRes, mchAppConfigContext.MchApp.AppSecret);
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

        private MchDivisionReceiverDto GenRecord(DivisionReceiverBindRQ bizRQ, MchDivisionReceiverGroupDto group, MchInfoDto mchInfo, decimal divisionProfit)
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

            receiver.RelationTypeName = GetRelationTypeName(bizRQ.RelationType); //关系名称

            if (receiver.RelationTypeName == null)
            {
                receiver.RelationTypeName = bizRQ.RelationTypeName;
            }

            receiver.DivisionProfit = divisionProfit; //分账比例
            receiver.ChannelExtInfo = bizRQ.ChannelExtInfo; //渠道信息

            return receiver;
        }

        private string GetRelationTypeName(string relationType)
        {

            if ("PARTNER".Equals(relationType))
            {
                return "合作伙伴";
            }
            else if ("SERVICE_PROVIDER".Equals(relationType))
            {
                return "服务商";
            }
            else if ("STORE".Equals(relationType))
            {
                return "门店";
            }
            else if ("STAFF".Equals(relationType))
            {
                return "员工";
            }
            else if ("STORE_OWNER".Equals(relationType))
            {
                return "店主";
            }
            else if ("HEADQUARTER".Equals(relationType))
            {
                return "总部";
            }
            else if ("BRAND".Equals(relationType))
            {
                return "品牌方";
            }
            else if ("DISTRIBUTOR".Equals(relationType))
            {
                return "分销商";
            }
            else if ("USER".Equals(relationType))
            {
                return "用户";
            }
            else if ("SUPPLIER".Equals(relationType))
            {
                return "供应商";
            }
            return null;
        }
    }
}
