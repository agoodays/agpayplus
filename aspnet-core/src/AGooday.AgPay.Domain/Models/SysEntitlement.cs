using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 系统权限表
    /// </summary>
    [Comment("系统权限表")]
    [Table("t_sys_entitlement")]
    public class SysEntitlement
    {
        /// <summary>
        /// 权限ID[ENT_功能模块_子模块_操作], eg: ENT_ROLE_LIST_ADD
        /// </summary>
        [Comment("权限ID[ENT_功能模块_子模块_操作], eg: ENT_ROLE_LIST_ADD")]
        [Key, Required, Column("ent_id", TypeName = "varchar(64)")]
        public string EntId { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        [Comment("权限名称")]
        [Required, Column("ent_name", TypeName = "varchar(32)")]
        public string EntName { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        [Comment("菜单图标")]
        [Column("menu_icon", TypeName = "varchar(32)")]
        public string MenuIcon { get; set; }

        /// <summary>
        /// 菜单uri/路由地址
        /// </summary>
        [Comment("菜单uri/路由地址")]
        [Column("menu_uri", TypeName = "varchar(128)")]
        public string MenuUri { get; set; }

        /// <summary>
        /// 组件Name（前后端分离使用）
        /// </summary>
        [Comment("组件Name（前后端分离使用）")]
        [Column("component_name", TypeName = "varchar(32)")]
        public string ComponentName { get; set; }

        /// <summary>
        /// 权限类型 ML-左侧显示菜单, MO-其他菜单, PB-页面/按钮
        /// </summary>
        [Comment("权限类型 ML-左侧显示菜单, MO-其他菜单, PB-页面/按钮")]
        [Required, Column("ent_type", TypeName = "char(2)")]
        public string EntType { get; set; }

        /// <summary>
        /// 快速开始菜单 0-否, 1-是
        /// </summary>
        [Comment("快速开始菜单 0-否, 1-是")]
        [Required, Column("quick_jump", TypeName = "tinyint(6)")]
        public byte QuickJump { get; set; }

        /// <summary>
        /// 状态 0-停用, 1-启用
        /// </summary>
        [Comment("状态 0-停用, 1-启用")]
        [Required, Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }

        /// <summary>
        /// 父ID
        /// </summary>
        [Comment("父ID")]
        [Required, Column("pid", TypeName = "varchar(32)")]
        public string Pid { get; set; }

        /// <summary>
        /// 排序字段, 规则：正序
        /// </summary>
        [Comment("排序字段, 规则：正序")]
        [Required, Column("ent_sort", TypeName = "int(11)")]
        public int EntSort { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, MCH-商户中心
        /// </summary>
        [Comment("所属系统： MGR-运营平台, MCH-商户中心")]
        [Required, Column("sys_type", TypeName = "varchar(8)")]
        public string SysType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Comment("创建时间")]
        [Required, Column("created_at", TypeName = "timestamp(6)")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Comment("更新时间")]
        [Required, Column("updated_at", TypeName = "timestamp(6)")]
        public DateTime UpdatedAt { get; set; }
    }
}
