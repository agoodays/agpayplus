namespace AGooday.AgPay.Common.Constants
{
    /// <summary>
    /// Constants 常量对象
    /// </summary>
    public class CS
    {
        /// <summary>
        /// 登录图形验证码缓存时间，单位：s
        /// </summary>
        public const int VERCODE_CACHE_TIME = 60;

        /** 系统类型定义 **/
        public interface SYS_TYPE
        {
            public const string MCH = "MCH";
            public const string AGENT = "AGENT";
            public const string MGR = "MGR";
        }

        public interface BASE_BELONG_INFO_ID
        {
            public const string MGR = "0";
            public const string AGENT = "A0000000000";
            public const string MCH = "M0000000000";
        }

        /** yes or no **/
        public const byte NO = 0;
        public const byte YES = 1;

        #region 通用 可用 / 禁用 
        /// <summary>
        /// 正常
        /// </summary>
        public const int PUB_USABLE = 1;
        /// <summary>
        /// 停用
        /// </summary>
        public const int PUB_DISABLE = 0;
        #endregion

        #region 账号类型:1-服务商 2-商户 3-商户应用
        /// <summary>
        /// 服务商
        /// </summary>
        public const byte INFO_TYPE_ISV = 1;
        /// <summary>
        /// 商户
        /// </summary>
        public const byte INFO_TYPE_MCH = 2;
        /// <summary>
        /// 商户应用
        /// </summary>
        public const byte INFO_TYPE_MCH_APP = 3;
        #endregion

        #region 商户类型:1-普通商户 2-特约商户
        /// <summary>
        /// 普通商户
        /// </summary>
        public const byte MCH_TYPE_NORMAL = 1;
        /// <summary>
        /// 特约商户
        /// </summary>
        public const byte MCH_TYPE_ISVSUB = 2;
        #endregion

        #region 代理商类型:1-普通商户 2-特约商户
        /// <summary>
        /// 普通代理商
        /// </summary>
        public const byte AGENT_TYPE_NORMAL = 1;
        /// <summary>
        /// 特约代理商
        /// </summary>
        public const byte AGENT_TYPE_ISVSUB = 2;
        #endregion

        #region 性别 1- 男， 2-女
        /// <summary>
        /// 未知
        /// </summary>
        public const byte SEX_UNKNOWN = 0;
        /// <summary>
        /// 男
        /// </summary>
        public const byte SEX_MALE = 1;
        /// <summary>
        /// 女
        /// </summary>
        public const byte SEX_FEMALE = 2;
        #endregion

        /// <summary>
        /// 默认密码
        /// </summary>
        public const string DEFAULT_PWD = "agpay666";

        /// <summary>
        /// 允许上传的的图片文件格式，需要与 WebSecurityConfig对应
        /// </summary>
        public static List<string> ALLOW_UPLOAD_IMG_SUFFIX = new List<string>() { "jpg" ,"png" ,"jpeg" ,"gif" ,"mp4" };

        public const int TOKEN_TIME = 60 * 60 * 2; //单位：s,  两小时

        /// <summary>
        /// access_token 名称
        /// </summary>
        public const string ACCESS_TOKEN_NAME = "authorization";

        #region 缓存Key
        /** ！！不同系统请放置不同的redis库 ！！ **/
        /// <summary>
        /// 缓存key: 当前用户所有用户的token集合  example: TOKEN_1001_HcNheNDqHzhTIrT0lUXikm7xU5XY4Q
        /// </summary>
        public const string CACHE_KEY_TOKEN = "Token:TOKEN_{0}_{1}";
        public static string GetCacheKeyToken(long sysUserId, string uuid)
        {
            return string.Format(CACHE_KEY_TOKEN, sysUserId, uuid);
        }

        /// <summary>
        /// 图片验证码 缓存key
        /// </summary>
        private const string CACHE_KEY_IMG_CODE = "ImgCode:img_code_{0}";
        public static string GetCacheKeyImgCode(string imgToken)
        {
            return string.Format(CACHE_KEY_IMG_CODE, imgToken);
        }

        /// <summary>
        /// 短信验证码 缓存key
        /// </summary>
        private const string CACHE_KEY_SMS_CODE = "SmsCode:sms_code_{0}";
        public static string GetCacheKeySmsCode(string smsToken)
        {
            return string.Format(CACHE_KEY_SMS_CODE, smsToken);
        }
        #endregion

        /// <summary>
        /// 登录认证类型
        /// </summary>
        public interface AUTH_TYPE
        {
            public const byte LOGIN_USER_NAME = 1; //登录用户名
            public const byte TELPHONE = 2; //手机号
            public const byte EMAIL = 3; //邮箱

            public const byte WX_UNION_ID = 10; //微信unionId
            public const byte WX_MINI = 11; //微信小程序
            public const byte WX_MP = 12; //微信公众号

            public const byte QQ = 20; //QQ
        }

        /// <summary>
        /// 菜单类型
        /// </summary>
        public interface ENT_TYPE
        {
            public const string MENU_LEFT = "ML";  //左侧显示菜单
            public const string MENU_OTHER = "MO";  //其他菜单
            public const string PAGE_OR_BTN = "PB";  //页面 or 按钮
        }

        /// <summary>
        /// 接口类型
        /// </summary>
        public interface IF_CODE
        {
            public const string ALIPAY = "alipay";   // 支付宝官方支付
            public const string WXPAY = "wxpay";     // 微信官方支付
            public const string YSFPAY = "ysfpay";   // 云闪付开放平台
            public const string XXPAY = "xxpay";     // 小新支付
            public const string PPPAY = "pppay";     // Paypal 支付
        }

        /// <summary>
        /// 支付方式代码
        /// </summary>
        public interface PAY_WAY_CODE
        {
            // 特殊支付方式
            public const string QR_CASHIER = "QR_CASHIER"; //  ( 通过二维码跳转到收银台完成支付， 已集成获取用户ID的实现。  )
            public const string AUTO_BAR = "AUTO_BAR"; // 条码聚合支付（自动分类条码类型）

            public const string ALI_BAR = "ALI_BAR";  //支付宝条码支付
            public const string ALI_JSAPI = "ALI_JSAPI";  //支付宝服务窗支付
            public const string ALI_APP = "ALI_APP";  //支付宝 app支付
            public const string ALI_PC = "ALI_PC";  //支付宝 电脑网站支付
            public const string ALI_WAP = "ALI_WAP";  //支付宝 wap支付
            public const string ALI_QR = "ALI_QR";  //支付宝 二维码付款

            public const string YSF_BAR = "YSF_BAR";  //云闪付条码支付
            public const string YSF_JSAPI = "YSF_JSAPI";  //云闪付服务窗支付

            public const string WX_JSAPI = "WX_JSAPI";  //微信jsapi支付
            public const string WX_LITE = "WX_LITE";  //微信小程序支付
            public const string WX_BAR = "WX_BAR";  //微信条码支付
            public const string WX_H5 = "WX_H5";  //微信H5支付
            public const string WX_NATIVE = "WX_NATIVE";  //微信扫码支付

            public const string PP_PC = "PP_PC"; // Paypal 支付
        }

        /// <summary>
        /// 支付数据包 类型
        /// </summary>
        public interface PAY_DATA_TYPE
        {
            public const string PAY_URL = "payurl";  //跳转链接的方式  redirectUrl
            public const string FORM = "form";  //表单提交
            public const string WX_APP = "wxapp";  //微信app参数
            public const string ALI_APP = "aliapp";  //支付宝app参数
            public const string YSF_APP = "ysfapp";  //云闪付app参数
            public const string CODE_URL = "codeUrl";  //二维码URL
            public const string CODE_IMG_URL = "codeImgUrl";  //二维码图片显示URL
            public const string NONE = "none";  //无参数
            //public const string QR_CONTENT = "qrContent";  //二维码实际内容
        }

        /// <summary>
        /// 接口版本
        /// </summary>
        public interface PAY_IF_VERSION
        {
            public const string WX_V2 = "V2";  //微信接口版本V2
            public const string WX_V3 = "V3";  //微信接口版本V3
        }
    }
}
