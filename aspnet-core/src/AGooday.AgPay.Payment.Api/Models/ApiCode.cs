namespace AGooday.AgPay.Payment.Api.Models
{
    public class ApiCode
    {
        public static ApiCode SUCCESS() => new ApiCode(0, "SUCCESS");//请求成功
        public static ApiCode CUSTOM_FAIL() => new ApiCode(9999, "自定义业务异常");//自定义业务异常

        public static ApiCode SYSTEM_ERROR() => new ApiCode(10, "系统异常{0}");
        public static ApiCode PARAMS_ERROR() => new ApiCode(11, "参数有误{0}");
        public static ApiCode DB_ERROR() => new ApiCode(12, "数据库服务异常");

        public static ApiCode SYS_OPERATION_FAIL_CREATE() => new ApiCode(5000, "新增失败");
        public static ApiCode SYS_OPERATION_FAIL_DELETE() => new ApiCode(5001, "删除失败");
        public static ApiCode SYS_OPERATION_FAIL_UPDATE() => new ApiCode(5002, "修改失败");
        public static ApiCode SYS_OPERATION_FAIL_SELETE() => new ApiCode(5003, "记录不存在");
        public static ApiCode SYS_PERMISSION_ERROR() => new ApiCode(5004, "权限错误，当前用户不支持此操作");

        private int code;

        private string msg;

        public ApiCode(int code, string msg)
        {
            this.code = code;
            this.msg = msg;
        }

        public int GetCode()
        {
            return this.code;
        }

        public string GetMsg()
        {
            return this.msg;
        }
    }
}
