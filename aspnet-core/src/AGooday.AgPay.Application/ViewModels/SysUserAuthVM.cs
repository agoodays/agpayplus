using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.ViewModels
{
    /// <summary>
    /// 系统用户认证表
    /// </summary>
    public class SysUserAuth
    {
        /// <summary>
        /// ID
        /// </summary>
        public long AuthId { get; set; }

        /// <summary>
        /// user_id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 登录类型  1-昵称 2-手机号 3-邮箱  10-微信  11-QQ 12-支付宝 13-微博
        /// </summary>
        public byte IdentityType { get; set; }

        /// <summary>
        /// 认证标识 ( 用户名 | open_id )
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// 密码凭证
        /// </summary>
        public string Credential { get; set; }

        /// <summary>
        /// salt
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, MCH-商户中心
        /// </summary>
        public string SysType { get; set; }
    }
}
