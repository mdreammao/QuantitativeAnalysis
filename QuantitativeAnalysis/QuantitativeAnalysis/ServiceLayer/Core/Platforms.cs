using Autofac;
using NLog;
using QuantitativeAnalysis.DataAccessLayer.DataFromLocalCSV.Common;
using QuantitativeAnalysis.DataAccessLayer.DataFromWind.Common;
using QuantitativeAnalysis.DataAccessLayer.DataFromWind.Futures;
using QuantitativeAnalysis.ModelLayer.Common;
using QuantitativeAnalysis.ModelLayer.Futures;
using QuantitativeAnalysis.ServiceLayer.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WAPIWrapperCSharp;

namespace QuantitativeAnalysis.ServiceLayer.Core    
{
    /// <summary>
    /// 提供一个全局可访问的类，存放各种重要变量
    /// </summary>
    public class Platforms
    {
        //Autofac容器
        public static IContainer container;
        static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 整个应用的全局初始化
        /// </summary>
        public static void Initialize()
        {
            log.Info("平台正在初始化中...");
            //初始化Autofac核心 - container
            ContainerBuilder builder = new ContainerBuilder();

            //Autofac中注册所有组件
            _RegisterComponents(builder);
            container = builder.Build();

            Initialization.__Initialize(container);

            log.Info("------ Platform初始化完成. ------");
        }
        /// <summary>
        /// 整个应用的终止
        /// </summary>
        public static void ShutDown()
        {
            if (_windAPI != null)
            {
                log.Debug("正在关闭Wind API...");
                if (_windAPI.isconnected())
                {
                    _windAPI.stop();
                }
            }

            log.Info("------ Platform已关闭. ------");
        }


        private static WindAPI _windAPI;
        /// <summary>
        /// 获取可立即使用的WindAPI,如果处于未连接状态自动开启
        /// </summary>
        /// <returns></returns>
        public static WindAPI GetWindAPI()
        {
            if (_windAPI == null)
            {
                _windAPI = new WindAPI();
            }
            if (!_windAPI.isconnected())
            {
                _windAPI.start();
            }
            return _windAPI;
        }

        /// <summary>
        /// 为container注册各种接口，实现依赖注入的核心功能
        /// 具体参见： https://autofac.org/
        /// </summary>
        /// <param name="builder"></param>
        private static void _RegisterComponents(ContainerBuilder cb)
        {
            //cb.RegisterInstance(new KLinesDataRepositoryFromWind()).As<KLinesDataRepository>();

            //cb.RegisterInstance(new ASharesInfoRepositoryFromWind()).As<ASharesInfoRepository>();

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
