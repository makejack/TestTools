using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

[assembly: XmlConfigurator(ConfigFile = "Log4.config", Watch = true)]
namespace Bll
{
    public class Log4Helper
    {
        private static ILog m_Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void LogInfo(string message)
        {
            m_Log.Info(message);
        }

        public static void LogInfo(string message, Exception ex)
        {
            m_Log.Info(message, ex);
        }

        public static void ErrorInfo(string message)
        {
            m_Log.Error(message);
        }

        public static void ErrorInfo(string message, Exception ex)
        {
            m_Log.Error(message, ex);
        }

        public static void LogWarn(string message)
        {
            m_Log.Warn(message);
        }

        public static void LogWarn(string message, Exception ex)
        {
            m_Log.Warn(message, ex);
        }

        public static void LogFatal(string message)
        {
            m_Log.Fatal(message);
        }

        public static void LogFatal(string message, Exception ex)
        {
            m_Log.Fatal(message, ex);
        }
    }
}
