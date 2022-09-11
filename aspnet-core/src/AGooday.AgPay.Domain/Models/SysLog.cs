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
    /// 系统操作日志表
    /// </summary>
    [Table("t_sys_log")]
    public class SysLog
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key, Required, Column("role_id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long SysLogId { get; set; }

        /// <summary>
        /// 系统用户ID
        /// </summary>
        [Required, Column("sys_user_id", TypeName = "bigint")]
        public long UserId { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [Required, Column("user_name", TypeName = "varchar(32)")]
        public string UserName { get; set; }

        /// <summary>
        /// 用户IP
        /// </summary>
        [Required, Column("user_ip", TypeName = "varchar(128)")]
        public string UserIp { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, MCH-商户中心
        /// </summary>
        [Required, Column("sys_type", TypeName = "varchar(8)")]
        public string SysType { get; set; }

        /// <summary>
        /// 方法名
        /// </summary>
        [Required, Column("method_name", TypeName = "varchar(128)")]
        public string MethodName { get; set; }

        /// <summary>
        /// 方法描述
        /// </summary>
        [Required, Column("method_remark", TypeName = "varchar(128)")]
        public string MethodRemark { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        [Required, Column("req_url", TypeName = "varchar(256)")]
        public string ReqUrl { get; set; }

        /// <summary>
        /// 操作请求参数
        /// </summary>
        [Required, Column("opt_req_param", TypeName = "varchar(2048)")]
        public string OptReqParam { get; set; }

        /// <summary>
        /// 操作响应结果
        /// </summary>
        [Required, Column("opt_res_info", TypeName = "varchar(2048)")]
        public string OptResInfo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required, Column("created_at", TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; }
    }
}
