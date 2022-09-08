using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 系统用户表
    /// </summary>
    public class SysUser
    {
        /**
         * 系统用户ID
         */
        public long SysUserId { get; set; }

        /**
         * 登录用户名
         */
        public string LoginUsername { get; set; }

        /**
         * 真实姓名
         */
        public string Realname { get; set; }

        /**
         * 手机号
         */
        public string Telphone { get; set; }

        /**
         * 性别 0-未知, 1-男, 2-女
         */
        public byte Sex { get; set; }

        /**
         * 头像地址
         */
        public string AvatarUrl { get; set; }

        /**
         * 员工编号
         */
        public string UserNo { get; set; }

        /**
         * 是否超管（超管拥有全部权限） 0-否 1-是
         */
        public byte IsAdmin { get; set; }

        /**
         * 状态 0-停用 1-启用
         */
        public byte State { get; set; }

        /**
         * 所属系统： MGR-运营平台, MCH-商户中心
         */
        public string SysType { get; set; }

        /**
         * 所属商户ID / 0(平台)
         */
        public string BelongInfoId { get; set; }

        /**
         * 创建时间
         */
        public DateTime CreatedAt { get; set; }

        /**
         * 更新时间
         */
        public DateTime UpdatedAt { get; set; }
    }
}
