using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 系统操作日志表
    /// </summary>
    [Comment("系统操作日志表")]
    [Table("t_sys_log")]
    public class SysLog
    {
        /// <summary>
        /// ID
        /// </summary>
        [Comment("ID")]
        [Key, Required, Column("sys_log_id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long SysLogId { get; set; }

        /// <summary>
        /// 系统用户ID
        /// </summary>
        [Comment("系统用户ID")]
        [Column("user_id", TypeName = "bigint")]
        public long? UserId { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [Comment("用户姓名")]
        [Column("user_name", TypeName = "varchar(32)")]
        public string UserName { get; set; }

        /// <summary>
        /// 浏览器
        /// </summary>
        [Comment("浏览器")]
        [Required, Column("browser", TypeName = "varchar(60)")]
        public string Browser { get; set; }

        /// <summary>
        /// 操作系统
        /// </summary>
        [Comment("操作系统")]
        [Required, Column("os", TypeName = "varchar(60)")]
        public string Os { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        [Comment("设备")]
        [Required, Column("device", TypeName = "varchar(60)")]
        public string Device { get; set; }

        /// <summary>
        /// 浏览器信息
        /// </summary>
        [Comment("浏览器信息")]
        [Required, Column("browser_info", TypeName = "varchar(200)")]
        public string BrowserInfo { get; set; }

        /// <summary>
        /// 用户IP
        /// </summary>
        [Comment("用户IP")]
        [Required, Column("user_ip", TypeName = "varchar(128)")]
        public string UserIp { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, AGENT-代理商中心, MCH-商户中心
        /// </summary>
        [Comment("所属系统： MGR-运营平台, AGENT-代理商中心, MCH-商户中心")]
        [Required, Column("sys_type", TypeName = "varchar(8)")]
        public string SysType { get; set; }

        /// <summary>
        /// 方法名
        /// </summary>
        [Comment("方法名")]
        [Required, Column("method_name", TypeName = "varchar(128)")]
        public string MethodName { get; set; }

        /// <summary>
        /// 方法描述
        /// </summary>
        [Comment("方法描述")]
        [Required, Column("method_remark", TypeName = "varchar(128)")]
        public string MethodRemark { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        [Comment("请求地址")]
        [Required, Column("req_url", TypeName = "varchar(256)")]
        public string ReqUrl { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        [Comment("请求方法")]
        [Required, Column("req_method", TypeName = "varchar(10)")]
        public string ReqMethod { get; set; }

        /// <summary>
        /// 操作请求参数
        /// </summary>
        [Comment("操作请求参数")]
        [Required, Column("opt_req_param", TypeName = "varchar(2048)")]
        public string OptReqParam { get; set; }

        /// <summary>
        /// 操作响应结果
        /// </summary>
        [Comment("操作响应结果")]
        [Required, Column("opt_res_info", TypeName = "varchar(2048)")]
        public string OptResInfo { get; set; }

        /// <summary>
        /// 耗时（毫秒）
        /// </summary>
        [Comment("耗时（毫秒）")]
        [Column("elapsed_ms", TypeName = "bigint")]
        public long? ElapsedMs { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Comment("创建时间")]
        [Required, Column("created_at", TypeName = "timestamp(6)")]
        public DateTime CreatedAt { get; set; }
    }
}
