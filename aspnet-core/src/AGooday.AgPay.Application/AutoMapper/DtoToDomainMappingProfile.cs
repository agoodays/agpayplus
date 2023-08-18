using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Domain.Commands.AgentInfos;
using AGooday.AgPay.Domain.Commands.MchInfos;
using AGooday.AgPay.Domain.Commands.SysUsers;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Domain.Queries.SysUsers;
using AutoMapper;
using Newtonsoft.Json;

namespace AGooday.AgPay.Application.AutoMapper
{
    /// <summary>
    /// 视图模型 -> 领域模式的映射，是 写 命令
    /// </summary>
    public class DtoToDomainMappingProfile : Profile
    {
        /// <summary>
        /// 配置构造函数，用来创建关系映射
        /// </summary>
        public DtoToDomainMappingProfile()
        {
            CreateMap<AgentInfoDto, AgentInfo>();
            CreateMap<AgentInfoCreateDto, CreateAgentInfoCommand>();
            CreateMap<CreateAgentInfoCommand, AgentInfo>();
            CreateMap<AgentInfoModifyDto, ModifyAgentInfoCommand>();
            CreateMap<ModifyAgentInfoCommand, AgentInfo>();

            CreateMap<IsvInfoDto, IsvInfo>();
            CreateMap<PayRateLevelConfigDto, PayRateLevelConfig>();
            CreateMap<MchAppDto, MchApp>()
                .ForMember(d => d.AppSignType, o => o.MapFrom(s => JsonConvert.SerializeObject(s.AppSignType)));
            CreateMap<MchStoreDto, MchStore>();
            CreateMap<MchDivisionReceiverGroupDto, MchDivisionReceiverGroup>();
            CreateMap<MchDivisionReceiverDto, MchDivisionReceiver>();

            CreateMap<MchInfoDto, MchInfo>()
                .ForMember(d => d.RefundMode, o => o.MapFrom(s => JsonConvert.SerializeObject(s.RefundMode)));
            CreateMap<MchInfoCreateDto, CreateMchInfoCommand>();
            CreateMap<CreateMchInfoCommand, MchInfo>()
                .ForMember(d => d.RefundMode, o => o.MapFrom(s => JsonConvert.SerializeObject(s.RefundMode)));
            CreateMap<MchInfoModifyDto, ModifyMchInfoCommand>();
            CreateMap<ModifyMchInfoCommand, MchInfo>()
                .ForMember(d => d.RefundMode, o => o.MapFrom(s => JsonConvert.SerializeObject(s.RefundMode)));

            CreateMap<MchNotifyRecordDto, MchNotifyRecord>();
            CreateMap<MchPayPassageDto, MchPayPassage>();
            CreateMap<PayInterfaceConfigDto, PayInterfaceConfig>();
            CreateMap<PayInterfaceDefineAddOrEditDto, PayInterfaceDefine>();
            CreateMap<PayInterfaceDefineDto, PayInterfaceDefine>()
                .ForMember(d => d.WayCodes, o => o.MapFrom(s => JsonConvert.SerializeObject(s.WayCodes)));
            CreateMap<PayOrderDivisionRecordDto, PayOrderDivisionRecord>();
            CreateMap<PayOrderDto, PayOrder>();
            CreateMap<PayRateConfigDto, PayRateConfig>();

            CreateMap<PayWayDto, PayWay>();

            CreateMap<QrCodeDto, QrCode>();
            CreateMap<QrCodeShellDto, QrCodeShell>()
                .ForMember(d => d.ConfigInfo, o => o.MapFrom(s => JsonConvert.SerializeObject(s.ConfigInfo)));

            CreateMap<RefundOrderDto, RefundOrder>();
            CreateMap<SysArticleDto, SysArticle>()
                .ForMember(d => d.ArticleRange, o => o.MapFrom(s => JsonConvert.SerializeObject(s.ArticleRange)));
            CreateMap<SysConfigDto, SysConfig>();
            CreateMap<SysEntitlementDto, SysEntitlement>();
            CreateMap<SysEntModifyDto, SysEntitlement>();
            CreateMap<SysLogDto, SysLog>();

            CreateMap<SysRoleDto, SysRole>();
            CreateMap<SysRoleCreateDto, SysRole>();
            CreateMap<SysRoleModifyDto, SysRole>();

            CreateMap<SysRoleEntRelaDto, SysRoleEntRela>();

            CreateMap<SysUserAuthDto, SysUserAuth>();
            CreateMap<SysUserRoleRelaDto, SysUserRoleRela>();

            CreateMap<SysUserDto, SysUser>();
            CreateMap<SysUserCreateDto, CreateSysUserCommand>();
            CreateMap<CreateSysUserCommand, SysUser>();
            CreateMap<SysUserModifyDto, ModifySysUserCommand>();
            CreateMap<ModifySysUserCommand, SysUser>();
            CreateMap<SysUserQueryDto, SysUserQuery>();

            CreateMap<SysUserTeamDto, SysUserTeam>();

            CreateMap<TransferOrder, TransferOrderDto>();
        }
    }
}
