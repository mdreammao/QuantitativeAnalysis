using Autofac;
using NLog;
using QuantitativeAnalysis.ModelLayer.Stock.MultiFactor.Market;
using QuantitativeAnalysis.ServiceLayer.DataProcessing.Option;
using QuantitativeAnalysis.ServiceLayer.DataProcessing.Stock;
using QuantitativeAnalysis.ServiceLayer.TradeDays;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAPIWrapperCSharp;

namespace QuantitativeAnalysis.ServiceLayer.MyCore
{   
    public class Initialization
    {
        //Autofac容器
        //public static IContainer container;
        static Logger log = LogManager.GetCurrentClassLogger();
        public static void __Initialize(IContainer container)
        {
            //配置NLog日志模块
            if (ConfigurationManager.AppSettings["ConsoleLog"] =="on")
            {
                MyNLogConfig.ApplyWithConsole();
            }
            else
            {
                MyNLogConfig.Apply();
            }

            //初始化CacheData文件夹
            var cdPath = ConfigurationManager.AppSettings["RootPath"]+ConfigurationManager.AppSettings["CacheData.RootPath"];
            if (!Directory.Exists(cdPath)) Directory.CreateDirectory(cdPath);


            //初始化wind连接
            try
            {
                WindAPI wapi = Platforms.GetWindAPI();


            }
            catch (Exception e)
            {
                log.Error(e, "Wind未连接！");
            }
            


            //初始化交易日数据           
            TradeDaysService tradeDaysService = container.Resolve<TradeDaysService>();
            if (Caches.WindConnection==true)
            {
                tradeDaysService.fetchFromLocalCsvOrWindAndSaveAndCache();
            }
            else
            {
                tradeDaysService.fetchFromLocalCsvOnly();
            }

            //初始化交易费用
            switch (ConfigurationManager.AppSettings["Setting"])
            {
                case "common":
                    MySettings.CommonSettings();
                    break;
                default:
                    break;

            }

            switch (ConfigurationManager.AppSettings["DisplayNetWorth"])
            {
                case "on":
                    Caches.DisplayNetWorth = true;
                    break;
                default:
                    break;

            }

            switch (ConfigurationManager.AppSettings["50ETFOptionInfoRecord"])
            {
                case "on":
                    OptionInfoService optionInfoService = container.Resolve<OptionInfoService>();
                    optionInfoService.fetchFromLocalCsvOrWindAndSaveAndCache(tag:"OptionInfo",code:"510050.SH");
                    break;
                default:
                    break;
            }

            switch (ConfigurationManager.AppSettings["StockBasicInfoRecord"])
            {
                case "on":
                    StockBasicInfoService stockInfoService = container.Resolve<StockBasicInfoService>();
                    stockInfoService.fetchFromLocalCsvOrWindAndSaveAndCache(localCsvExpiration: 0, tag: "StockBasicInfo", code: "allStocks");
                    break;
                default:
                    break;
            }
            
            //switch (ConfigurationManager.AppSettings["CommodityOptionInfoRecord"])
            //{
            //    case "on":
            //        OptionInfoDailyService optionInfoService = container.Resolve<OptionInfoDailyService>();  
            //        optionInfoService.fetchFromLocalCsvOrWindAndSave(tag: null, code: "M1707.DCE",date: new DateTime(2017, 05, 15));
            //        break;
            //    default:
            //        break;
            //}

            StockDailyMarketService test = container.Resolve<StockDailyMarketService>();
            var a=test.fetchFromLocalCsvOrWindAndSave("600000.SH", new DateTime(2007, 1, 1), DateTime.Today);
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
