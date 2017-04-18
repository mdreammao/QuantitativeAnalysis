
using NLog;
using NLog.Conditions;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ServiceLayer.Core
{
    public class MyNLogConfig
    {
        static string rootDir = ConfigurationManager.AppSettings["RootPath"]+ConfigurationManager.AppSettings["Log.RootPath"];
        const string conLayout = @"${date:format=yyyy-MM-dd HH\:mm\:ss} [${pad:padding=5:inner=${level:uppercase=true}}] ${message}${exception:format=toString}";
        const string conLayout1 = @"${date:format=yyyy-MM-dd HH\:mm\:ss} [${pad:padding=5:inner=${level:uppercase=true}}] ${logger:shortName=false}: ${message}";
        const string fileLayout = @"${date:format=yyyy-MM-dd HH\:mm\:ss} [${pad:padding=5:inner=${level:uppercase=true}}] ${logger:shortName=true}: ${message}${exception:format=toString}";

        public static object Condition { get; private set; }

        public static void Apply()
        {
            // Step 1. Create configuration object 
            var config = new LoggingConfiguration();

            if (rootDir == null) rootDir = "${basedir}";
            // Step 2. Create targets and add them to the configuration 
            var con = new ColoredConsoleTarget();   //控制台
            var f1 = new FileTarget();      //当天日志文件（所有消息）
            var f2 = new FileTarget();      //当天日志文件（错误消息）

            //con.WordHighlightingRules.Add(new ConsoleWordHighlightingRule("------",ConsoleOutputColor.Green, ConsoleOutputColor.Green));
            con.RowHighlightingRules.Add(new ConsoleRowHighlightingRule(
                ConditionParser.ParseExpression("level == LogLevel.Debug"),
                ConsoleOutputColor.DarkGray, ConsoleOutputColor.NoChange));

            con.RowHighlightingRules.Add(new ConsoleRowHighlightingRule(
               ConditionParser.ParseExpression("level == LogLevel.Info"),
               ConsoleOutputColor.Gray, ConsoleOutputColor.NoChange));

            con.RowHighlightingRules.Add(new ConsoleRowHighlightingRule(
                ConditionParser.ParseExpression("level == LogLevel.Warn"),
                ConsoleOutputColor.DarkYellow, ConsoleOutputColor.NoChange));

            con.RowHighlightingRules.Add(new ConsoleRowHighlightingRule(
                ConditionParser.ParseExpression("level == LogLevel.Error"),
                ConsoleOutputColor.Red, ConsoleOutputColor.NoChange));

            //不显示在console上  
            config.AddTarget("console", con);
            config.AddTarget("f1", f1);
            config.AddTarget("f2", f2);

            // Step 3. Set target properties 

            con.Layout = conLayout;
            f1.FileName = rootDir + "/all.${shortdate}.log";
            f1.Layout = fileLayout;
            f2.FileName = rootDir + "/error.${shortdate}.log";
            f2.Layout = fileLayout;

            // Step 4. Define rules     
            //不显示在console上  .

            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Warn, con));
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, f1));
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Error, f2));


            // Step 5. Activate the configuration
            LogManager.Configuration = config;

        }
    }
}
