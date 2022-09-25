using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.DataTransfer
{
    public class SysEntModifyDto
    {
        /// <summary>
        /// 权限ID[ENT_功能模块_子模块_操作], eg: ENT_ROLE_LIST_ADD
        /// </summary>
        public string EntId { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string EntName { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string MenuIcon { get; set; }

        /// <summary>
        /// 菜单uri/路由地址
        /// </summary>
        public string MenuUri { get; set; }

        /// <summary>
        /// 组件Name（前后端分离使用）
        /// </summary>
        public string ComponentName { get; set; }

        /// <summary>
        /// 权限类型 ML-左侧显示菜单, MO-其他菜单, PB-页面/按钮
        /// </summary>
        public string EntType { get; set; }

        /// <summary>
        /// 快速开始菜单 0-否, 1-是
        /// </summary>
        public byte QuickJump { get; set; }

        /// <summary>
        /// 状态 0-停用, 1-启用
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// 父ID
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 排序字段, 规则：正序
        /// </summary>
        public int EntSort { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, MCH-商户中心
        /// </summary>
        public string SysType { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
