using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 系统配置表
    /// </summary>
    public class SysConfigDto
    {
        /// <summary>
        /// 配置KEY
        /// </summary>
        public string ConfigKey { get; set; }

        /// <summary>
        /// 配置名称
        /// </summary>
        public string ConfigName { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string ConfigDesc { get; set; }

        /// <summary>
        /// 分组key
        /// </summary>
        public string GroupKey { get; set; }

        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 配置内容项
        /// </summary>
        public string ConfigVal { get; set; }

        /// <summary>
        /// 类型: text-输入框, textarea-多行文本, uploadImg-上传图片, switch-开关
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public long SortNum { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
