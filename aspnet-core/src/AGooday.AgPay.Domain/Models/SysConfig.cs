using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AGooday.AgPay.Domain.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 系统配置表
    /// </summary>
    [Comment("系统配置表")]
    [Table("t_sys_config")]
    public class SysConfig : AbstractTrackableTimestamps
    {
        /// <summary>
        /// 配置KEY
        /// </summary>
        [Comment("配置KEY")]
        [Required, Column("config_key", TypeName = "varchar(50)")]
        public string ConfigKey { get; set; }

        /// <summary>
        /// 配置名称
        /// </summary>
        [Comment("配置名称")]
        [Required, Column("config_name", TypeName = "varchar(50)")]
        public string ConfigName { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        [Comment("描述信息")]
        [Required, Column("config_desc", TypeName = "varchar(200)")]
        public string ConfigDesc { get; set; }

        /// <summary>
        /// 分组key
        /// </summary>
        [Comment("分组key")]
        [Required, Column("group_key", TypeName = "varchar(50)")]
        public string GroupKey { get; set; }

        /// <summary>
        /// 分组名称
        /// </summary>
        [Comment("分组名称")]
        [Required, Column("group_name", TypeName = "varchar(50)")]
        public string GroupName { get; set; }

        /// <summary>
        /// 配置内容项
        /// </summary>
        [Comment("配置内容项")]
        [Required, Column("config_val", TypeName = "text")]
        public string ConfigVal { get; set; }

        /// <summary>
        /// 类型: text-输入框, textarea-多行文本, uploadImg-上传图片, switch-开关
        /// </summary>
        [Comment("类型: text-输入框, textarea-多行文本, uploadImg-上传图片, switch-开关")]
        [Required, Column("type", TypeName = "varchar(20)")]
        public string Type { get; set; }

        /// <summary>
        /// 所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心
        /// </summary>
        [Comment("所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心")]
        [Required, Column("sys_type", TypeName = "varchar(8)")]
        public string SysType { get; set; }

        /// <summary>
        /// 所属商户ID / 代理商ID / 0(平台)
        /// </summary>
        [Comment("所属商户ID / 代理商ID / 0(平台)")]
        [Required, Column("belong_info_id", TypeName = "varchar(64)")]
        public string BelongInfoId { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        [Comment("显示顺序")]
        [Required, Column("sort_num", TypeName = "bigint(20)")]
        public long SortNum { get; set; }
    }
}
