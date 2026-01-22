using Serilog;

namespace AGooday.AgPay.Common.Utils
{
    public static class LogUtil<T> where T : class
    {
        private static readonly ILogger Logger = Log.ForContext<T>();

        public static void Info(string message)
            => Logger.Information(message);

        public static void Info(string message, Exception exception)
            => Logger.Information(exception, message);

        public static void Warn(string message)
            => Logger.Warning(message);

        public static void Warn(string message, Exception exception)
            => Logger.Warning(exception, message);

        public static void Debug(string message)
            => Logger.Debug(message);

        public static void Debug(string message, Exception exception)
            => Logger.Debug(exception, message);

        public static void Error(string message)
            => Logger.Error(message);

        public static void Error(string message, Exception exception)
            => Logger.Error(exception, message);

        // 可选：单独记录异常（带默认消息）
        public static void Error(Exception exception)
            => Logger.Error(exception, "An error occurred.");
    }
}
