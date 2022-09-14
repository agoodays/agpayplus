using AGooday.AgPay.Application.ViewModels;
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
    public class ViewModelToDomainMappingProfile : Profile
    {
        /// <summary>
        /// 配置构造函数，用来创建关系映射
        /// </summary>
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<IsvInfoVM, IsvInfo>();
            CreateMap<MchAppVM, MchApp>();
            CreateMap<MchDivisionReceiverGroupVM, MchDivisionReceiverGroup>();
            CreateMap<MchDivisionReceiverVM, MchDivisionReceiver>();
            CreateMap<MchInfoVM, MchInfo>();
            CreateMap<MchNotifyRecordVM, MchNotifyRecord>();
            CreateMap<MchPayPassageVM, MchPayPassage>();
            CreateMap<PayInterfaceConfigVM, PayInterfaceConfig>();
            CreateMap<PayInterfaceDefineVM, PayInterfaceDefine>();
            CreateMap<PayOrderDivisionRecordVM, PayOrderDivisionRecord>();
            CreateMap<PayOrderVM, PayOrder>();
            CreateMap<PayWayVM, PayWay>();
            CreateMap<RefundOrderVM, RefundOrder>();
            CreateMap<SysConfigVM, SysConfig>();
            CreateMap<SysEntitlementVM, SysEntitlement>();
            CreateMap<SysLogVM, SysLog>();
            CreateMap<SysRoleEntRelaVM, SysRoleEntRela>();
            CreateMap<SysUserAuthVM, SysUserAuth>();
            CreateMap<SysUserRoleRelaVM, SysUserRoleRela>();

            CreateMap<SysUserVM, SysUser>();
            CreateMap<SysUserVM, SysUserCommand>();
            CreateMap<SysUserCommand, SysUser>();

            CreateMap<TransferOrder, TransferOrderVM>();
        }
    }
}
