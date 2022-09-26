using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Domain.Commands.MchInfos;
using AGooday.AgPay.Domain.Commands.SysUsers;
using AGooday.AgPay.Domain.Events.SysUsers;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            CreateMap<IsvInfoDto, IsvInfo>();
            CreateMap<MchAppDto, MchApp>();
            CreateMap<MchDivisionReceiverGroupDto, MchDivisionReceiverGroup>();
            CreateMap<MchDivisionReceiverDto, MchDivisionReceiver>();

            CreateMap<MchInfoDto, MchInfo>();
            CreateMap<MchInfoCreateDto, CreateMchInfoCommand>();
            CreateMap<CreateMchInfoCommand, MchInfo>();
            CreateMap<MchInfoModifyDto, ModifyMchInfoCommand>();
            CreateMap<ModifyMchInfoCommand, MchInfo>();

            CreateMap<MchNotifyRecordDto, MchNotifyRecord>();
            CreateMap<MchPayPassageDto, MchPayPassage>();
            CreateMap<PayInterfaceConfigDto, PayInterfaceConfig>();
            CreateMap<PayInterfaceDefineDto, PayInterfaceDefine>();
            CreateMap<PayOrderDivisionRecordDto, PayOrderDivisionRecord>();
            CreateMap<PayOrderDto, PayOrder>();
            CreateMap<PayWayDto, PayWay>();
            CreateMap<RefundOrderDto, RefundOrder>();
            CreateMap<SysConfigDto, SysConfig>();
            CreateMap<SysEntitlementDto, SysEntitlement>();
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

            CreateMap<TransferOrder, TransferOrderDto>();
        }
    }
}
