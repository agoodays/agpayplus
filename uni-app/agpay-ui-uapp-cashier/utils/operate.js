// operate.js
export default {
  api: function() {
    // H5 环境处理
    // #ifdef H5
    const env = process.env.NODE_ENV;
    switch (env) {
      case 'development':
        return "https://localhost:9819"; // 开发环境地址
      case 'staging':
        return "https://staging.example.com"; // 预发布环境地址
      case 'production':
      default:
        return ""; // 生产环境地址或默认地址
    }
    // #endif

    // 小程序环境处理 (原有逻辑)
    // #ifdef MP-WEIXIN
    let version = uni.getAccountInfoSync().miniProgram.envVersion;
    switch (version) {
      case "develop": //开发预览版
        return "https://localhost:9819";
      case 'trial': //体验版
        return "http://xxx.xxx.xxx.xxx:xxx";
      case 'release': //正式版
        return "http://xxx.xxx.xxx.xxx:xxx";
      default: //未知,默认调用正式版
        return "http://xxx.xxx.xxx.xxx:xxx";
    }
    // #endif
  }
}