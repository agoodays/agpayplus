using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 码牌模板信息表
    /// </summary>
    [Comment("码牌模板信息表")]
    [Table("t_qr_code_shell")]
    public class QrCodeShell
    {
        /// <summary>
        /// 码牌模板ID
        /// </summary>
        [Comment("码牌模板ID")]
        [Key, Required, Column("id", TypeName = "int")]
        public int Id { get; set; }

        /// <summary>
        /// 样式代码: shellA, shellB
        /// </summary>
        [Comment("样式代码: shellA, shellB")]
        [Required, Column("style_code", TypeName = "varchar(20)")]
        public string StyleCode { get; set; }

        /// <summary>
        /// 模板别名
        /// </summary>
        [Comment("模板别名")]
        [Required, Column("shell_alias", TypeName = "varchar(20)")]
        public string ShellAlias { get; set; }

        /// <summary>
        /// 模板配置信息,json字符串
        /// </summary>
        [Comment("模板配置信息,json字符串")]
        [Required, Column("config_info", TypeName = "varchar(4096)")]
        public string ConfigInfo { get; set; }

        /// <summary>
        /// 状态: 0-停用, 1-启用
        /// </summary>
        [Comment("状态: 0-停用, 1-启用")]
        [Required, Column("state", TypeName = "tinyint(6)")]
        public byte State { get; set; }

        /// <summary>
        /// 所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心
        /// </summary>
        [Comment("所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心")]
        [Required, Column("sys_type", TypeName = "varchar(8)")]
        public string SysType { get; set; }

        /// <summary>
        /// 所属商户ID / 所属代理商ID / 0(平台)
        /// </summary>
        [Comment("所属商户ID / 所属代理商ID / 0(平台)")]
        [Required, Column("belong_info_id", TypeName = "varchar(64)")]
        public string BelongInfoId { get; set; }

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
