using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog.Config;
using NLog.Targets;

namespace SportsStore.WebUI.Logging
{
    public class LoggingSettings
    {
        //LoggingConfiguration config = new NLog.Config.LoggingConfiguration();

        //FileTarget logfile = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };
        //ConsoleTarget logconsole = new NLog.Targets.ConsoleTarget("logconsole");

        //config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
        //config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            
        //NLog.LogManager.Configuration = config;
    }
}