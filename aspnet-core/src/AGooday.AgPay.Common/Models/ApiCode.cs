using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Common.Models
{
    public class ApiCode
    {
        public static ApiCode SUCCESS => Init(0, "SUCCESS");//请求成功
        public static ApiCode CUSTOM_FAIL => Init(9999, "自定义业务异常");//自定义业务异常

        public static ApiCode SYSTEM_ERROR => Init(10, "系统异常{0}");
        public static ApiCode PARAMS_ERROR => Init(11, "参数有误{0}");
        public static ApiCode DB_ERROR => Init(12, "数据库服务异常");

        public static ApiCode SYS_OPERATION_FAIL_CREATE => Init(5000, "新增失败");
        public static ApiCode SYS_OPERATION_FAIL_DELETE => Init(5001, "删除失败");
        public static ApiCode SYS_OPERATION_FAIL_UPDATE => Init(5002, "修改失败");
        public static ApiCode SYS_OPERATION_FAIL_SELETE => Init(5003, "记录不存在");
        public static ApiCode SYS_PERMISSION_ERROR => Init(5004, "权限错误，当前用户不支持此操作");

        private int code;

        private string msg;

        public ApiCode(int code, string msg)
        {
            this.code = code;
            this.msg = msg;
        }

        private static ApiCode Init(int code, string msg) => new ApiCode(code, msg);

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
