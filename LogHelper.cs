using NLog;
using NLog.Config;
using NLog.Targets;

namespace Pg2Couch
{
    public class LogHelper
    {
        public static void ConfigureLogging()
        {
            // We set this up programmatically, to avoid having a dependency on an nlog.config
            var config = new LoggingConfiguration();

            var consoleTarget = new ColoredConsoleTarget("console");
            consoleTarget.Layout = @"${level:uppercase=true} ${date:format=HH\:mm\:ss} ${logger}: ${message}";
            config.AddTarget(consoleTarget);

            var loggingRule = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(loggingRule);
            LogManager.Configuration = config;
        }
    }
}
