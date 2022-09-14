using AGooday.AgPay.Application.ViewModels;
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
    /// 领域模型 -> 视图模型的映射，是 读 命令
    /// </summary>
    public class DomainToViewModelMappingProfile : Profile
    {
        /// <summary>
        /// 配置构造函数，用来创建关系映射
        /// </summary>
        public DomainToViewModelMappingProfile()
        {
            CreateMap<IsvInfo, IsvInfoVM>();
            CreateMap<MchApp, MchAppVM>();
            CreateMap<MchDivisionReceiverGroup, MchDivisionReceiverGroupVM>();
            CreateMap<MchDivisionReceiver, MchDivisionReceiverVM>();
            CreateMap<MchInfo, MchInfoVM>();
            CreateMap<MchNotifyRecord, MchNotifyRecordVM>();
            CreateMap<MchPayPassage, MchPayPassageVM>();
            CreateMap<PayInterfaceConfig, PayInterfaceConfigVM>();
            CreateMap<PayInterfaceDefine, PayInterfaceDefineVM>();
            CreateMap<PayOrderDivisionRecord, PayOrderDivisionRecordVM>();
            CreateMap<PayOrder, PayOrderVM>();
            CreateMap<PayWay, PayWayVM>();
            CreateMap<RefundOrder, RefundOrderVM>();
            CreateMap<SysConfig, SysConfigVM>();
            CreateMap<SysEntitlement, SysEntitlementVM>();
            CreateMap<SysLog, SysLogVM>();
            CreateMap<SysRoleEntRela, SysRoleEntRelaVM>();
            CreateMap<SysUserAuth, SysUserAuthVM>();
            CreateMap<SysUserRoleRela, SysUserRoleRelaVM>();

            CreateMap<SysUser, SysUserVM>();
            CreateMap<SysUser, SysUserCreatedEvent>();

            CreateMap<TransferOrder, TransferOrderVM>();
        }
    }
}
