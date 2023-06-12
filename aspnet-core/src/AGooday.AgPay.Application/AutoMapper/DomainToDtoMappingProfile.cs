using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Domain.Events.AgentInfos;
using AGooday.AgPay.Domain.Events.MchInfos;
using AGooday.AgPay.Domain.Events.SysUsers;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
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
            CreateMap<AgentInfo, AgentInfoDto>();
            CreateMap<AgentInfo, AgentInfoDetailDto>();
            CreateMap<AgentInfo, AgentInfoCreatedEvent>();

            CreateMap<IsvInfo, IsvInfoDto>();
            CreateMap<PayRateLevelConfig, PayRateLevelConfigDto>();
            CreateMap<MchApp, MchAppDto>()
                .ForMember(d => d.AppSignType, o => o.MapFrom(s => JArray.Parse(s.AppSignType)));
            CreateMap<MchStore, MchStoreDto>();
            CreateMap<MchStore, MchStoreListDto>();
            CreateMap<MchDivisionReceiverGroup, MchDivisionReceiverGroupDto>();
            CreateMap<MchDivisionReceiver, MchDivisionReceiverDto>();

            CreateMap<MchInfo, MchInfoDto>()
                .ForMember(d => d.RefundMode, o => o.MapFrom(s => JArray.Parse(s.RefundMode)));
            CreateMap<MchInfo, MchInfoDetailDto>()
                .ForMember(d => d.RefundMode, o => o.MapFrom(s => JArray.Parse(s.RefundMode)));
            CreateMap<MchInfo, MchInfoCreatedEvent>()
                .ForMember(d => d.RefundMode, o => o.MapFrom(s => JArray.Parse(s.RefundMode)));

            CreateMap<MchNotifyRecord, MchNotifyRecordDto>();
            CreateMap<MchPayPassage, MchPayPassageDto>();
            CreateMap<PayInterfaceConfig, PayInterfaceConfigDto>();
            CreateMap<PayInterfaceDefine, PayInterfaceDefineDto>()
                .ForMember(d => d.WayCodes, o => o.MapFrom(s => JArray.Parse(s.WayCodes)));
            CreateMap<PayOrderDivisionRecord, PayOrderDivisionRecordDto>();
            CreateMap<PayOrder, PayOrderDto>();
            CreateMap<PayRateConfig, PayRateConfigDto>();

            CreateMap<PayWay, PayWayDto>();
            CreateMap<PayWay, MchPayPassagePayWayDto>();

            CreateMap<QrCode, QrCodeDto>();
            CreateMap<QrCodeShell, QrCodeShellDto>()
                .ForMember(d => d.ConfigInfo, o => o.MapFrom(s => JObject.Parse(s.ConfigInfo)));

            CreateMap<RefundOrder, RefundOrderDto>();
            CreateMap<SysArticle, SysArticleDto>()
                .ForMember(d => d.ArticleRange, o => o.MapFrom(s => JArray.Parse(s.ArticleRange)));
            CreateMap<SysConfig, SysConfigDto>()
                .ForMember(dest => dest.SysType, opt => opt.Ignore())
                .ForMember(dest => dest.BelongInfoId, opt => opt.Ignore());
            CreateMap<SysEntitlement, SysEntitlementDto>();
            CreateMap<SysLog, SysLogDto>();
            CreateMap<SysRole, SysRoleDto>();
            CreateMap<SysRoleEntRela, SysRoleEntRelaDto>();
            CreateMap<SysUserAuth, SysUserAuthDto>();
            CreateMap<SysUserRoleRela, SysUserRoleRelaDto>();

            CreateMap<SysUser, SysUserDto>();
            CreateMap<SysUser, SysUserListDto>();
            CreateMap<SysUser, SysUserCreatedEvent>();

            CreateMap<SysUserTeam, SysUserTeamDto>();

            CreateMap<TransferOrder, TransferOrderDto>();
        }
    }
}
