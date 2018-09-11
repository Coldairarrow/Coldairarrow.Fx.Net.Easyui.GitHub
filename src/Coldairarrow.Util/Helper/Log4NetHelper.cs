using log4net;
using System;
using System.IO;
using System.Web;

namespace Coldairarrow.Util
{
    /// <summary>
    /// 日志帮助类(使用log4net)
    /// </summary>
    public class Log4NetHelper
    {
        static Log4NetHelper()
        {
            FileInfo configFile = new FileInfo(HttpContext.Current.Server.MapPath("/Config/log4net.config"));
            log4net.Config.XmlConfigurator.Configure(configFile);
        }

        /// <summary>
        /// 获取日志操作接口
        /// </summary>
        /// <param name="type">当前类型</param>
        /// <returns></returns>
        public static ILog GetLogger(Type type)
        {
            return LogManager.GetLogger(type);
        }

        /// <summary>
        /// 获取日志操作接口
        /// </summary>
        /// <param name="str">当前位置</param>
        /// <returns></returns>
        public static ILog GetLogger(string str)
        {
            return LogManager.GetLogger(str);
        }
    }
}
