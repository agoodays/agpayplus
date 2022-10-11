using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Common.Utils
{
    public static class LogUtil
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        private static readonly ILog Log = null;

        static LogUtil()
        {
            Log = LogManager.GetLogger("");
        }

        public static void Info(string message)
        {
            if (Log.IsInfoEnabled)
            {
                Log.Info(message);
            }
        }

        public static void Info(Exception e)
        {
            if (Log.IsInfoEnabled)
            {
                Log.Info(e);
            }
        }

        public static void Info(string message, Exception e)
        {
            if (Log.IsInfoEnabled)
            {
                Log.Info(message, e);
            }
        }

        public static void Warn(string message)
        {
            if (Log.IsWarnEnabled)
            {
                Log.Warn(message);
            }
        }

        public static void Warn(Exception e)
        {
            if (Log.IsWarnEnabled)
            {
                Log.Warn(e);
            }
        }

        public static void Warn(string message, Exception e)
        {
            if (Log.IsWarnEnabled)
            {
                Log.Warn(message, e);
            }
        }

        public static void Debug(string message)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug(message);
            }
        }

        public static void Debug(Exception e)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug(e);
            }
        }

        public static void Debug(string message, Exception e)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug(message, e);
            }
        }

        public static void Error(string message)
        {
            if (Log.IsErrorEnabled)
            {
                Log.Error(message);
            }
        }

        public static void Error(Exception e)
        {
            if (Log.IsErrorEnabled)
            {
                Log.Error(e);
            }
        }

        public static void Error(string message, Exception e)
        {
            if (Log.IsErrorEnabled)
            {
                Log.Error(message, e);
            }
        }
    }
}
