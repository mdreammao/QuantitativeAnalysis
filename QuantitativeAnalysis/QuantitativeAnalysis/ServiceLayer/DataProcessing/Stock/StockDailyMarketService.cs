using Autofac;
using QuantitativeAnalysis.DataAccessLayer.DataFromLocalCSV.Stock;
using QuantitativeAnalysis.DataAccessLayer.DataFromWind.stock;
using QuantitativeAnalysis.DataAccessLayer.DataToLocalCSV.Stock;
using QuantitativeAnalysis.ModelLayer.Stock.MultiFactor.Market;
using QuantitativeAnalysis.ServiceLayer.DataProcessing.Common;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using QuantitativeAnalysis.Utilities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ServiceLayer.DataProcessing.Stock
{
    class StockDailyMarketService : SequentialByDayService<StockDailyMarket>
    {
        public override List<StockDailyMarket> readFromLocalCSVOnly(string code, DateTime date, string tag = null)
        {
            var path = _buildCacheDataFilePath(code, date, tag);
            return Platforms.container.Resolve<StockDailyMarketFromLocalCSVRepository>().readFromLocalCSV(path);
        }

        public override List<StockDailyMarket> readFromMSSQLOnly(string code, DateTime date)
        {
            throw new NotImplementedException();
        }

        public override List<StockDailyMarket> readFromWindOnly(string code, DateTime startDate, DateTime endDate, string tag = null, IDictionary<string, object> options = null)
        {
            return Platforms.container.Resolve<StockDailyMarketFromWindRepository>().readFromWind(code, startDate, endDate, tag, options);
        }

        public override void saveToLocalCSV(IList<StockDailyMarket> data, string code, DateTime date, string tag = null, bool appendMode = false, bool canSaveToday = false)
        {

            var path = _buildCacheDataFilePath(code, date, tag);
            Platforms.container.Resolve<StockDailyMarketToLocalCSVRepository>().saveToLocalCsv(path, data);
        }

        private static string _buildCacheDataFilePath(string code, DateTime date, string tag)
        {
            const string PATH_KEY = "CacheData.Path.StockFactor";
            if (tag == null) tag = typeof(StockDailyMarket).ToString();
            return FileUtils.GetCacheDataFilePath(PATH_KEY, new Dictionary<string, string>
            {
                ["{tag}"] = tag,
                ["{code}"] = code,
                ["{date}"] = date.ToString("yyyyMMdd")
            });
        }
    }
}
