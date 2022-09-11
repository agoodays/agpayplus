using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.ViewModels
{
    /// <summary>
    /// 系统操作日志表
    /// </summary>
    public class SysLogVM
    {
        /// <summary>
        /// ID
        /// </summary>
        public int SysLogId { get; set; }

        /// <summary>
        /// 系统用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户IP
        /// </summary>
        public string UserIp { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, MCH-商户中心
        /// </summary>
        public string SysType { get; set; }

        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 方法描述
        /// </summary>
        public string MethodRemark { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string ReqUrl { get; set; }

        /// <summary>
        /// 操作请求参数
        /// </summary>
        public string OptReqParam { get; set; }

        /// <summary>
        /// 操作响应结果
        /// </summary>
        public string OptResInfo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
