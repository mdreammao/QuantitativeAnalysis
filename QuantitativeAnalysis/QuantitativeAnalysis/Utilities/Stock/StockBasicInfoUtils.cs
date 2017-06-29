using NLog;
using QuantitativeAnalysis.ModelLayer.Stock.BasicInfo;
using QuantitativeAnalysis.ServiceLayer.DataProcessing.Stock;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using System;
using System.Collections.Generic;
using Autofac;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Core;

namespace QuantitativeAnalysis.Utilities.Stock
{
    public class StockBasicInfoUtils
    {
        static Logger log = LogManager.GetCurrentClassLogger();

        private static List<StockBasicInfo> _stockBasicInfo;
        
        /// <summary>
        /// 获得股票基本数据的构造函数
        /// </summary>
        /// <returns>股票数据列表</returns>
        private static List<StockBasicInfo> getStockBasicInfo()
        {
            if (_stockBasicInfo==null)
            {
                try
                {
                    _stockBasicInfo = Caches.get<List<StockBasicInfo>>("StockBasicInfo_allStocks");
                }
                catch (Exception e)
                {
                    log.Error(e,"未将股票基本信息数据存入内存,需重新获取数据！{0}");
                }
                if (_stockBasicInfo==null)
                {
                    StockBasicInfoService stockInfoService = Platforms.container.Resolve<StockBasicInfoService>();
                    _stockBasicInfo=stockInfoService.fetchFromLocalCsvOrWindAndSaveAndCache(localCsvExpiration: 180, tag: "StockBasicInfo", code: "allStocks");
                }
            }
            return _stockBasicInfo;
        }

        public static List<StockBasicInfo> getAllStockList()
        {
            return getStockBasicInfo();
        }
        /// <summary>
        /// 根据给定日期，获取当日存续的股票
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static List<StockBasicInfo> getStockListByDate(DateTime date)
        {
            List<StockBasicInfo> list = new List<StockBasicInfo>();
            foreach (var stock in getStockBasicInfo())
            {
                if (stock.listDate<=date && stock.delistDate>date)
                {
                    list.Add(stock);
                }
            }
            return list;
        }

        /// <summary>
        /// 根据代码获取上市时间
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static DateTime getStockListDate(string code)
        {
            return getStockBasicInfo().Find(x => x.code == code).listDate;
        }

        /// <summary>
        /// 根据代码获取退市时间
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static DateTime getStockDelistDate(string code)
        {
            return getStockBasicInfo().Find(x => x.code == code).delistDate;
        }
    }
}
