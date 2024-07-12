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
            CreateMap<AccountBillDto, AccountBill>();
            CreateMap<AgentInfoDto, AgentInfo>();
            CreateMap<AgentInfoCreateDto, CreateAgentInfoCommand>();
            CreateMap<CreateAgentInfoCommand, AgentInfo>();
            CreateMap<AgentInfoModifyDto, ModifyAgentInfoCommand>();
            CreateMap<ModifyAgentInfoCommand, AgentInfo>();

            CreateMap<IsvInfoDto, IsvInfo>();
            CreateMap<PayRateLevelConfigDto, PayRateLevelConfig>();
            CreateMap<MchAppDto, MchApp>()
                .ForMember(d => d.AppSignType, o =>
                {
                    o.PreCondition(s => s.AppSignType != null);
                    o.MapFrom(s => JsonConvert.SerializeObject(s.AppSignType));
                });
            CreateMap<MchStoreDto, MchStore>();
            CreateMap<MchDivisionReceiverGroupDto, MchDivisionReceiverGroup>();
            CreateMap<MchDivisionReceiverDto, MchDivisionReceiver>();

            CreateMap<MchInfoDto, MchInfo>()
                .ForMember(d => d.RefundMode, o =>
                {
                    o.PreCondition(s => s.RefundMode != null);
                    o.MapFrom(s => JsonConvert.SerializeObject(s.RefundMode));
                });
            CreateMap<MchInfoCreateDto, CreateMchInfoCommand>();
            CreateMap<CreateMchInfoCommand, MchInfo>()
                .ForMember(d => d.RefundMode, o =>
                {
                    o.PreCondition(s => s.RefundMode != null);
                    o.MapFrom(s => JsonConvert.SerializeObject(s.RefundMode));
                });
            CreateMap<MchInfoModifyDto, ModifyMchInfoCommand>();
            CreateMap<ModifyMchInfoCommand, MchInfo>()
                .ForMember(d => d.RefundMode, o =>
                {
                    o.PreCondition(s => s.RefundMode != null);
                    o.MapFrom(s => JsonConvert.SerializeObject(s.RefundMode));
                });

            CreateMap<MchNotifyRecordDto, MchNotifyRecord>();
            CreateMap<MchPayPassageDto, MchPayPassage>();
            CreateMap<PayInterfaceConfigDto, PayInterfaceConfig>();
            CreateMap<PayInterfaceDefineAddOrEditDto, PayInterfaceDefine>();
            CreateMap<PayInterfaceDefineDto, PayInterfaceDefine>()
                .ForMember(d => d.WayCodes, o =>
                {
                    o.PreCondition(s => s.WayCodes != null);
                    o.MapFrom(s => JsonConvert.SerializeObject(s.WayCodes));
                });
            CreateMap<PayOrderDivisionRecordDto, PayOrderDivisionRecord>();
            CreateMap<PayOrderDto, PayOrder>();
            CreateMap<PayOrderProfitDto, PayOrderProfit>();
            CreateMap<PayRateConfigDto, PayRateConfig>();
            CreateMap<PayRateConfigInfoDto, PayRateConfig>();

            CreateMap<PayWayDto, PayWay>();

            CreateMap<QrCodeDto, QrCode>();
            CreateMap<QrCodeShellDto, QrCodeShell>()
                .ForMember(d => d.ConfigInfo, o =>
                {
                    o.PreCondition(s => s.ConfigInfo != null);
                    o.MapFrom(s => JsonConvert.SerializeObject(s.ConfigInfo));
                });

            CreateMap<RefundOrderDto, RefundOrder>();
            CreateMap<SysArticleDto, SysArticle>()
                .ForMember(d => d.ArticleRange, o =>
                {
                    o.PreCondition(s => s.ArticleRange != null);
                    o.MapFrom(s => JsonConvert.SerializeObject(s.ArticleRange));
                });
            CreateMap<SysConfigDto, SysConfig>();
            CreateMap<SysEntitlementDto, SysEntitlement>()
                .ForMember(d => d.MatchRule, o =>
                {
                    o.PreCondition(s => s.MatchRule != null);
                    o.MapFrom(s => JsonConvert.SerializeObject(s.MatchRule));
                });
            CreateMap<SysLogDto, SysLog>();

            CreateMap<SysRoleDto, SysRole>();
            CreateMap<SysRoleCreateDto, SysRole>();
            CreateMap<SysRoleModifyDto, SysRole>();

            CreateMap<SysRoleEntRelaDto, SysRoleEntRela>();

            CreateMap<SysUserAuthDto, SysUserAuth>();
            CreateMap<SysUserLoginAttemptDto, SysUserLoginAttempt>();
            CreateMap<SysUserRoleRelaDto, SysUserRoleRela>();

            CreateMap<SysUserDto, SysUser>()
                .ForMember(d => d.EntRules, o =>
                {
                    o.PreCondition(s => s.EntRules != null);
                    o.MapFrom(s => JsonConvert.SerializeObject(s.EntRules));
                })
                .ForMember(d => d.BindStoreIds, o =>
                {
                    o.PreCondition(s => s.BindStoreIds != null);
                    o.MapFrom(s => JsonConvert.SerializeObject(s.BindStoreIds));
                });
            CreateMap<SysUserCreateDto, CreateSysUserCommand>()
                .ForMember(d => d.EntRules, o =>
                {
                    o.PreCondition(s => s.EntRules != null);
                    o.MapFrom(s => JsonConvert.SerializeObject(s.EntRules));
                })
                .ForMember(d => d.BindStoreIds, o =>
                {
                    o.PreCondition(s => s.BindStoreIds != null);
                    o.MapFrom(s => JsonConvert.SerializeObject(s.BindStoreIds));
                });
            CreateMap<SysUserModifyDto, ModifySysUserCommand>()
                .ForMember(d => d.EntRules, o =>
                {
                    o.PreCondition(s => s.EntRules != null);
                    o.MapFrom(s => JsonConvert.SerializeObject(s.EntRules));
                })
                .ForMember(d => d.BindStoreIds, o =>
                {
                    o.PreCondition(s => s.BindStoreIds != null);
                    o.MapFrom(s => JsonConvert.SerializeObject(s.BindStoreIds));
                });
            CreateMap<CreateSysUserCommand, SysUser>();
            CreateMap<ModifySysUserCommand, SysUser>();
            CreateMap<SysUserQueryDto, SysUserQuery>();

            CreateMap<SysUserTeamDto, SysUserTeam>();

            CreateMap<TransferOrder, TransferOrderDto>();
        }
    }
}
