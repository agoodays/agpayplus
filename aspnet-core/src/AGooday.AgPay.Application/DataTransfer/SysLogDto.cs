namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 系统操作日志表
    /// </summary>
    public class SysLogDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public long? SysLogId { get; set; }

        /// <summary>
        /// 系统用户ID
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 浏览器
        /// </summary>
        public string Browser { get; set; }

        /// <summary>
        /// 操作系统
        /// </summary>
        public string Os { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        /// 浏览器信息
        /// </summary>
        public string BrowserInfo { get; set; }

        /// <summary>
        /// 用户IP
        /// </summary>
        public string UserIp { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, AGENT-代理商中心, MCH-商户中心
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
        /// 请求方法
        /// </summary>
        public string ReqMethod { get; set; }

        /// <summary>
        /// 操作请求参数
        /// </summary>
        public string OptReqParam { get; set; }

        /// <summary>
        /// 操作响应结果
        /// </summary>
        public string OptResInfo { get; set; }

        /// <summary>
        /// 耗时（毫秒）
        /// </summary>
        public long? ElapsedMs { get; set; }

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
