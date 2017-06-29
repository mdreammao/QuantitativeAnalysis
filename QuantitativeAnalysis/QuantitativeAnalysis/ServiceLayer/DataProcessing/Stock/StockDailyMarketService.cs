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
using System.Data;
using QuantitativeAnalysis.DataAccessLayer.DateToMSSQL.stock;

namespace QuantitativeAnalysis.ServiceLayer.DataProcessing.Stock
{
    class StockDailyMarketService : StockDailyFactorService<StockDailyMarket>
    {
        protected override List<StockDailyMarket> readFromLocalCSVOnly(string code, DateTime startDate, DateTime endDate,string tag = null)
        {
            return Platforms.container.Resolve<StockDailyMarketFromLocalCSVRepository>().ReadFromLocalCSVForDays(code,startDate,endDate);
        }

        protected override List<StockDailyMarket> readFromSQLServerOnly(string code, DateTime startDate, DateTime endDate, string sourceServer = "local", string tag = null)
        {
            throw new NotImplementedException();
        }

        protected override List<StockDailyMarket> readFromWindOnly(string code, DateTime startDate, DateTime endDate, string tag = null, IDictionary<string, object> options = null)
        {
            return Platforms.container.Resolve<StockDailyMarketFromWindRepository>().readFromWind(code, startDate, endDate);
        }

        protected override void saveToLocalCSV(IList<StockDailyMarket> data, string tag = null, bool appendMode = false, bool canSaveToday = false)
        {
            Platforms.container.Resolve<StockDailyMarketToLocalCSVRepository>().saveToLocalCsv(data);
        }

        protected override void saveToSQLServer(string targetServer, string dataBase, string tableName, DataTable data, Dictionary<string, string> pair = null)
        {
            Platforms.container.Resolve<StockDailyDataToMSSQLRepository>().saveToSQLServer(targetServer,dataBase,tableName,data,pair);
        }
    }
}
