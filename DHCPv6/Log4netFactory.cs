using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.IO;
using log4net.Config;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace DHCPv6
{
    public class Log4netFactory
    {
        public static ILog Create(Type type)
        {
           return LogManager.GetLogger(type);
        }
    }
}
