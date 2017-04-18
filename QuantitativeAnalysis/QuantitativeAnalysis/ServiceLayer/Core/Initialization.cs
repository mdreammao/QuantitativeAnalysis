using Autofac;
using NLog;
using QuantitativeAnalysis.ServiceLayer.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ServiceLayer.Core
{
    public class Initialization
    {
        //Autofac容器
        //public static IContainer container;
        static Logger log = LogManager.GetCurrentClassLogger();
        public static void __Initialize(IContainer container)
        {
            //配置NLog日志模块
            MyNLogConfig.Apply();

            //初始化CacheData文件夹
            var cdPath = ConfigurationManager.AppSettings["RootPath"]+ConfigurationManager.AppSettings["CacheData.RootPath"];
            if (!Directory.Exists(cdPath)) Directory.CreateDirectory(cdPath);

            //初始化交易日数据           
            TradeDaysService tradeDaysService = container.Resolve<TradeDaysService>();
            tradeDaysService.fetchFromLocalCsvOrWindAndSaveAndCache();
        }

        private static void _RegisterComponents(ContainerBuilder cb)
        {

            //cb.RegisterInstance(new FuturesDailyFromWind()).As<DataFromWind<FuturesDaily>>();
            //cb.RegisterInstance(new FuturesMinuteFromWind()).As<DataFromWind<FuturesMinute>>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            //自动扫描注册
            cb.RegisterAssemblyTypes(assemblies).Where(t => t.Name.EndsWith("Repository")).AsSelf();
            cb.RegisterAssemblyTypes(assemblies).Where(t => t.Name.EndsWith("Service")).AsSelf();
            //cb.RegisterAssemblyTypes(assemblies)
            //       .Where(t => t.Name.EndsWith("Service"))
            //       .AsImplementedInterfaces().AsSelf();

        }
    }
}
