using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 商户门店表
    /// </summary>
    public class MchStoreDto : BaseModel
    {
        /// <summary>
        /// 门店ID
        /// </summary>
        public long? StoreId { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }

        /// <summary>
        /// 代理商号
        /// </summary>
        public string AgentNo { get; set; }

        /// <summary>
        /// 服务商号
        /// </summary>
        public string IsvNo { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        public string ContactPhone { get; set; }

        /// <summary>
        /// 门店LOGO
        /// </summary>
        public string StoreLogo { get; set; }

        /// <summary>
        /// 门头照
        /// </summary>
        public string StoreOuterImg { get; set; }

        /// <summary>
        /// 门店内景照
        /// </summary>
        public string StoreInnerImg { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 省代码
        /// </summary>
        public string ProvinceCode { get; set; }

        /// <summary>
        /// 市代码
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        /// 区代码
        /// </summary>
        public string DistrictCode { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public string Lng { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public string Lat { get; set; }

        /// <summary>
        /// 是否默认: 0-否, 1-是
        /// </summary>
        public byte? DefaultFlag { get; set; }

        /// <summary>
        /// 绑定AppId
        /// </summary>
        public string BindAppId { get; set; }

        /// <summary>
        /// 创建者用户ID
        /// </summary>
        public long? CreatedUid { get; set; }

        /// <summary>
        /// 创建者姓名
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
