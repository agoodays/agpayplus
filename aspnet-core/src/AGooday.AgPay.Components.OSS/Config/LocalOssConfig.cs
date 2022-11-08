using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Components.OSS.Config
{
    public class LocalOssConfig
    {
        ////指定该属性为嵌套值, 否则默认为简单值导致对象为空（外部类不存在该问题， 内部static需明确指定）
        public static OssConfig Oss { get; set; } = new OssConfig();

        /// <summary>
        /// 系统oss配置信息
        /// </summary>
        public class OssConfig
        {
            /// <summary>
            /// 存储根路径
            /// </summary>
            public string FileRootPath { get; set; }

            /// <summary>
            /// 公共读取块
            /// </summary>
            public string FilePublicPath { get; set; }

            /// <summary>
            /// 私有读取块
            /// </summary>
            public string FilePrivatePath { get; set; }

            /// <summary>
            /// oss类型
            /// </summary>
            public string ServiceType { get; set; }
        }
    }
}
