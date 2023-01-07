using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 商户门店表
    /// </summary>
    [Table("t_mch_store")]
    [Comment("商户门店表")]
    public class MchStore
    {
        /// <summary>
        /// 门店ID
        /// </summary>
        [Comment("门店ID")]
        [Key, Required, Column("store_id", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自增列
        public long StoreId { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        [Comment("门店名称")]
        [Required, Column("store_name", TypeName = "varchar(64)")]
        public string StoreName { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [Comment("商户号")]
        [Required, Column("mch_no", TypeName = "varchar(64)")]
        public string MchNo { get; set; }

        /// <summary>
        /// 代理商号
        /// </summary>
        [Comment("代理商号")]
        [Column("agent_no", TypeName = "varchar(64)")]
        public string AgentNo { get; set; }

        /// <summary>
        /// 服务商号
        /// </summary>
        [Comment("服务商号")]
        [Column("isv_no", TypeName = "varchar(64)")]
        public string IsvNo { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        [Comment("联系人电话")]
        [Required, Column("contact_phone", TypeName = "varchar(32)")]
        public string ContactPhone { get; set; }

        /// <summary>
        /// 门店LOGO
        /// </summary>
        [Comment("门店LOGO")]
        [Required, Column("store_logo", TypeName = "varchar(64)")]
        public string StoreLogo { get; set; }

        /// <summary>
        /// 门头照
        /// </summary>
        [Comment("门店LOGO")]
        [Required, Column("store_outer_img", TypeName = "varchar(64)")]
        public string StoreOuterImg { get; set; }

        /// <summary>
        /// 门店内景照
        /// </summary>
        [Comment("门店LOGO")]
        [Required, Column("store_inner_img", TypeName = "varchar(64)")]
        public string StoreInnerImg { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Comment("备注")]
        [Column("remark", TypeName = "varchar(128)")]
        public string Remark { get; set; }

        /// <summary>
        /// 省代码
        /// </summary>
        [Comment("省代码")]
        [Required, Column("province_code", TypeName = "varchar(32)")]
        public string ProvinceCode { get; set; }

        /// <summary>
        /// 市代码
        /// </summary>
        [Comment("市代码")]
        [Required, Column("city_code", TypeName = "varchar(32)")]
        public string CityCode { get; set; }

        /// <summary>
        /// 区代码
        /// </summary>
        [Comment("区代码")]
        [Required, Column("area_code", TypeName = "varchar(32)")]
        public string AreaCode { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [Comment("详细地址")]
        [Required, Column("address", TypeName = "varchar(32)")]
        public string Address { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [Comment("经度")]
        [Required, Column("lng", TypeName = "varchar(32)")]
        public string Lng { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        [Comment("纬度")]
        [Required, Column("lat", TypeName = "varchar(32)")]
        public string Lat { get; set; }

        /// <summary>
        /// 是否默认: 0-否, 1-是
        /// </summary>
        [Comment("是否默认: 0-否, 1-是")]
        [Required, Column("default_flag", TypeName = "tinyint(6)")]
        public byte DefaultFlag { get; set; }

        /// <summary>
        /// 绑定AppId
        /// </summary>
        [Column("bind_app_id", TypeName = "varchar(64)")]
        public string BindAppId { get; set; }

        /// <summary>
        /// 创建者用户ID
        /// </summary>
        [Comment("创建者用户ID")]
        [Column("created_uid", TypeName = "bigint")]
        public long? CreatedUid { get; set; }

        /// <summary>
        /// 创建者姓名
        /// </summary>
        [Comment("创建者姓名")]
        [Column("created_by", TypeName = "varchar(64)")]
        public string CreatedBy { get; set; }

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
