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
    /// 系统用户认证表
    /// </summary>
    [Table("t_sys_user_auth")]
    public class SysUserAuth
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key, Required, Column("auth_id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long AuthId { get; set; }

        /**
         * user_id
         */
        public long UserId { get; set; }

        /**
         * 登录类型  1-昵称 2-手机号 3-邮箱  10-微信  11-QQ 12-支付宝 13-微博
         */
        public byte IdentityType { get; set; }

        /**
         * 认证标识 ( 用户名 | open_id )
         */
        public string Identifier { get; set; }

        /**
         * 密码凭证
         */
        public string Credential { get; set; }

        /**
         * salt
         */
        public string Salt { get; set; }

        /**
         * 所属系统： MGR-运营平台, MCH-商户中心
         */
        public string SysType { get; set; }
    }
}
