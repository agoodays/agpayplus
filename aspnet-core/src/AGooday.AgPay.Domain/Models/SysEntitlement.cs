using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 系统权限表
    /// </summary>
    public class SysEntitlement
    {
        /**
         * 权限ID[ENT_功能模块_子模块_操作], eg: ENT_ROLE_LIST_ADD
         */
        public string EntId{ get; set; }

        /**
         * 权限名称
         */
        public string EntName{ get; set; }

        /**
         * 菜单图标
         */
        public string MenuIcon{ get; set; }

        /**
         * 菜单uri/路由地址
         */
        public string MenuUri{ get; set; }

        /**
         * 组件name（前后端分离使用）
         */
        public string ComponentName{ get; set; }

        /**
         * 权限类型 ML-左侧显示菜单, MO-其他菜单, PB-页面/按钮
         */
        public string EntType{ get; set; }

        /**
         * 快速开始菜单 0-否, 1-是
         */
        public byte QuickJump{ get; set; }

        /**
         * 状态 0-停用, 1-启用
         */
        public byte State{ get; set; }

        /**
         * 父ID
         */
        public string Pid{ get; set; }

        /**
         * 排序字段, 规则：正序
         */
        public int EntSort{ get; set; }

        /**
         * 所属系统： MGR-运营平台, MCH-商户中心
         */
        public string SysType{ get; set; }

        /**
         * 创建时间
         */
        public DateTime CreatedAt{ get; set; }

        /**
         * 更新时间
         */
        public DateTime UpdatedAt{ get; set; }
    }
}
