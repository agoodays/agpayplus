﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Domain.Events.AgentInfos;
using AGooday.AgPay.Domain.Events.MchInfos;
using AGooday.AgPay.Domain.Events.SysUsers;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Application.AutoMapper
{
    /// <summary>
    /// 领域模型 -> 视图模型的映射，是 读 命令
    /// </summary>
    public class DomainToDtoMappingProfile : Profile
    {
        /// <summary>
        /// 配置构造函数，用来创建关系映射
        /// </summary>
        public DomainToDtoMappingProfile()
        {
            CreateMap<AccountBill, AccountBillDto>();
            CreateMap<AgentInfo, AgentInfoDto>();
            CreateMap<AgentInfo, AgentInfoCreatedEvent>();

            CreateMap<IsvInfo, IsvInfoDto>();
            CreateMap<PayRateLevelConfig, PayRateLevelConfigDto>();
            CreateMap<MchApp, MchAppDto>()
                .ForMember(d => d.AppSignType, o =>
                {
                    o.PreCondition(s => !string.IsNullOrWhiteSpace(s.AppSignType));
                    o.MapFrom(s => JArray.Parse(s.AppSignType));
                });
            CreateMap<MchStore, MchStoreDto>();
            CreateMap<MchStore, MchStoreListDto>();
            CreateMap<MchDivisionReceiverGroup, MchDivisionReceiverGroupDto>();
            CreateMap<MchDivisionReceiver, MchDivisionReceiverDto>();

            CreateMap<MchInfo, MchInfoDto>()
                .ForMember(d => d.RefundMode, o =>
                {
                    o.PreCondition(s => !string.IsNullOrWhiteSpace(s.RefundMode));
                    o.MapFrom(s => JArray.Parse(s.RefundMode));
                });
            CreateMap<MchInfo, MchInfoCreatedEvent>()
                .ForMember(d => d.RefundMode, o =>
                {
                    o.PreCondition(s => !string.IsNullOrWhiteSpace(s.RefundMode));
                    o.MapFrom(s => JArray.Parse(s.RefundMode));
                });

            CreateMap<MchNotifyRecord, MchNotifyRecordDto>();
            CreateMap<MchPayPassage, MchPayPassageDto>();
            CreateMap<PayInterfaceConfig, PayInterfaceConfigDto>();
            CreateMap<PayInterfaceDefine, PayInterfaceDefineDto>()
                .ForMember(d => d.WayCodes, o =>
                {
                    o.PreCondition(s => !string.IsNullOrWhiteSpace(s.WayCodes));
                    o.MapFrom(s => JArray.Parse(s.WayCodes));
                });
            CreateMap<PayOrderDivisionRecord, PayOrderDivisionRecordDto>();
            CreateMap<PayOrder, PayOrderDto>();
            CreateMap<PayOrderProfit, PayOrderProfitDto>();
            CreateMap<PayRateConfig, PayRateConfigDto>();
            CreateMap<PayRateConfig, PayRateConfigInfoDto>();

            CreateMap<PayWay, PayWayDto>();
            CreateMap<PayWay, MchPayPassagePayWayDto>();

            CreateMap<QrCode, QrCodeDto>();
            CreateMap<QrCodeShell, QrCodeShellDto>()
                .ForMember(d => d.ConfigInfo, o =>
                {
                    o.PreCondition(s => !string.IsNullOrWhiteSpace(s.ConfigInfo));
                    o.MapFrom(s => JObject.Parse(s.ConfigInfo));
                });

            CreateMap<RefundOrder, RefundOrderDto>();
            CreateMap<SysArticle, SysArticleDto>()
                .ForMember(d => d.ArticleRange, o =>
                {
                    o.PreCondition(s => !string.IsNullOrWhiteSpace(s.ArticleRange));
                    o.MapFrom(s => JArray.Parse(s.ArticleRange));
                });
            CreateMap<SysConfig, SysConfigDto>()
                .ForMember(dest => dest.SysType, opt => opt.Ignore())
                .ForMember(dest => dest.BelongInfoId, opt => opt.Ignore());
            CreateMap<SysEntitlement, SysEntitlementDto>()
                .ForMember(d => d.MatchRule, o =>
                {
                    o.PreCondition(s => !string.IsNullOrWhiteSpace(s.MatchRule));
                    o.MapFrom(s => JsonConvert.DeserializeObject<SysEntitlementDto.EntMatchRule>(s.MatchRule));
                });
            CreateMap<SysLog, SysLogDto>();
            CreateMap<SysRole, SysRoleDto>();
            CreateMap<SysRoleEntRela, SysRoleEntRelaDto>();
            CreateMap<SysUserAuth, SysUserAuthDto>();
            CreateMap<SysUserLoginAttempt, SysUserLoginAttemptDto>();
            CreateMap<SysUserRoleRela, SysUserRoleRelaDto>();

            CreateMap<SysUser, SysUserDto>()
                .ForMember(d => d.EntRules, o =>
                {
                    o.PreCondition(s => !string.IsNullOrWhiteSpace(s.EntRules));
                    o.MapFrom(s => JsonConvert.DeserializeObject<List<string>>(s.EntRules));
                })
                .ForMember(d => d.BindStoreIds, o =>
                {
                    o.PreCondition(s => !string.IsNullOrWhiteSpace(s.BindStoreIds));
                    o.MapFrom(s => JsonConvert.DeserializeObject<List<string>>(s.BindStoreIds));
                });
            CreateMap<SysUser, SysUserListDto>()
                .ForMember(d => d.EntRules, o =>
                {
                    o.PreCondition(s => !string.IsNullOrWhiteSpace(s.EntRules));
                    o.MapFrom(s => JsonConvert.DeserializeObject<List<string>>(s.EntRules));
                })
                .ForMember(d => d.BindStoreIds, o =>
                {
                    o.PreCondition(s => !string.IsNullOrWhiteSpace(s.BindStoreIds));
                    o.MapFrom(s => JsonConvert.DeserializeObject<List<string>>(s.BindStoreIds));
                });
            CreateMap<SysUser, SysUserCreatedEvent>()
                .ForMember(d => d.EntRules, o =>
                {
                    o.PreCondition(s => !string.IsNullOrWhiteSpace(s.EntRules));
                    o.MapFrom(s => JsonConvert.DeserializeObject<List<string>>(s.EntRules));
                })
                .ForMember(d => d.BindStoreIds, o =>
                {
                    o.PreCondition(s => !string.IsNullOrWhiteSpace(s.BindStoreIds));
                    o.MapFrom(s => JsonConvert.DeserializeObject<List<string>>(s.BindStoreIds));
                });

            CreateMap<SysUserTeam, SysUserTeamDto>();

            CreateMap<TransferOrder, TransferOrderDto>();
        }
    }
}
