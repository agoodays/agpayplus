using AGooday.AgPay.Common.Models;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 码牌模板信息表
    /// </summary>
    public class QrCodeShellDto
    {
        /// <summary>
        /// 码牌模板ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 样式代码: shellA, shellB
        /// </summary>
        public string StyleCode { get; set; }

        /// <summary>
        /// 模板别名
        /// </summary>
        public string ShellAlias { get; set; }

        /// <summary>
        /// 模板配置信息,json字符串
        /// </summary>
        public JObject ConfigInfo { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, AGENT-代理商中心, MCH-商户中心
        /// </summary>
        public string SysType { get; set; }

        /// <summary>
        /// 所属商户ID / 所属代理商ID / 0(平台)
        /// </summary>
        public string BelongInfoId { get; set; }

        /// <summary>
        /// 模板预览图Url
        /// </summary>
        public string ShellImgViewUrl { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

    public class QrCodeConfigInfo
    {
        public List<QrCodePayType> PayTypeList { get; set; }
        public bool ShowIdFlag { get; set; }
        public string CustomBgColor { get; set; }
        public string BgColor { get; set; }
        public string QrInnerImgUrl { get; set; }
        public string LogoImgUrl { get; set; }
    }
}
