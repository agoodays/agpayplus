using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.OSS.Constants;

namespace AGooday.AgPay.Components.OSS.Models
{
    /// <summary>
    /// 定义文件上传的配置信息
    /// </summary>
    public class OssFileConfig
    {
        /// <summary>
        /// 业务类型
        /// </summary>
        interface BIZ_TYPE
        {
            /// <summary>
            /// 用户头像
            /// </summary>
            public const string AVATAR = "avatar";
            /// <summary>
            /// 接口类型卡片背景图片
            /// </summary>
            public const string IF_BG = "ifBG";
            /// <summary>
            /// 接口参数
            /// </summary>
            public const string CERT = "cert";
            /// <summary>
            /// 表单参数
            /// </summary>
            public const string FORM = "form";
        }

        /// <summary>
        /// 图片类型后缀格式
        /// </summary>
        public static List<string> IMG_SUFFIX = new List<string>() { "jpg", "png", "jpeg", "gif" };

        /// <summary>
        /// 全部后缀格式的文件标识符
        /// </summary>
        public static string ALL_SUFFIX_FLAG = "*";

        /// <summary>
        /// 不校验文件大小标识符
        /// </summary>
        public static long ALL_MAX_SIZE = -1L;

        /// <summary>
        /// 允许上传的最大文件大小的默认值
        /// </summary>
        public static long DEFAULT_MAX_SIZE = 5 * 1024 * 1024L;

        private static Dictionary<string, OssFileConfig> ALL_BIZ_TYPE_MAP = new Dictionary<string, OssFileConfig>()
        {
            { BIZ_TYPE.AVATAR, new OssFileConfig(OssSavePlaceEnum.PUBLIC, IMG_SUFFIX) },
            { BIZ_TYPE.IF_BG, new OssFileConfig(OssSavePlaceEnum.PUBLIC, IMG_SUFFIX) },
            { BIZ_TYPE.CERT, new OssFileConfig(OssSavePlaceEnum.PRIVATE, new List<string>(){ ALL_SUFFIX_FLAG})},
            { BIZ_TYPE.FORM, new OssFileConfig(OssSavePlaceEnum.PUBLIC, IMG_SUFFIX) },
        };

        /// <summary>
        /// 存储位置
        /// </summary>
        public OssSavePlaceEnum OssSavePlaceEnum { get; private set; }

        /// <summary>
        /// 允许的文件后缀, 默认全部类型
        /// </summary>
        public List<string> AllowFileSuffix { get; private set; } = new List<string>() { ALL_SUFFIX_FLAG };

        /// <summary>
        /// 允许的文件大小, 单位： Byte
        /// </summary>
        public long MaxSize { get; private set; } = DEFAULT_MAX_SIZE;

        public OssFileConfig(OssSavePlaceEnum OssSavePlaceEnum, List<string> list)
        {
            this.OssSavePlaceEnum = OssSavePlaceEnum;
            this.AllowFileSuffix = list;
        }

        public OssFileConfig(OssSavePlaceEnum ossSavePlaceEnum, List<string> allowFileSuffix, long maxSize)
        {
            this.OssSavePlaceEnum = ossSavePlaceEnum;
            this.AllowFileSuffix = allowFileSuffix;
            this.MaxSize = maxSize;
        }


        /// <summary>
        /// 是否在允许的文件类型后缀内
        /// </summary>
        /// <param name="fixSuffix"></param>
        /// <returns></returns>
        public bool IsAllowFileSuffix(string fixSuffix)
        {
            if (this.AllowFileSuffix.Contains(ALL_SUFFIX_FLAG))
            {
                //允许全部
                return true;
            }

            return this.AllowFileSuffix.Contains(StringUtil.DefaultIfEmpty(fixSuffix, "").ToLower());
        }

        /// <summary>
        /// 是否在允许的大小范围内
        /// </summary>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        public bool IsMaxSizeLimit(long? fileSize)
        {
            if (ALL_MAX_SIZE.Equals(this.MaxSize))
            { //允许全部大小
                return true;
            }

            return this.MaxSize >= (fileSize == null ? 0L : fileSize);
        }

        public static OssFileConfig GetOssFileConfigByBizType(string bizType)
        {
            ALL_BIZ_TYPE_MAP.TryGetValue(bizType, out OssFileConfig ossFileConfig);
            return ossFileConfig;
        }
    }
}
