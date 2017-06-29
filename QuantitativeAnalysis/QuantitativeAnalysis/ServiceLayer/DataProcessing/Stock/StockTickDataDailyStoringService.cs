using Autofac;
using QuantitativeAnalysis.DataAccessLayer.DataFromMSSQL.Stock;
using QuantitativeAnalysis.ModelLayer.Stock.Tick;
using QuantitativeAnalysis.ServiceLayer.DataProcessing.Common;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using QuantitativeAnalysis.Utilities.DataApplication;
using QuantitativeAnalysis.DataAccessLayer.DateToMSSQL;
using QuantitativeAnalysis.Utilities.Common;
using QuantitativeAnalysis.DataAccessLayer.DateToMSSQL.stock;

namespace QuantitativeAnalysis.ServiceLayer.DataProcessing.Stock
{
    class StockTickDataDailyStoringService : SequentialByDayService<StockTickFromMssql>
    {
        protected override List<StockTickFromMssql> readFromLocalCSVOnly(string code, DateTime date, string tag = null)
        {
            throw new NotImplementedException();
        }

        protected override List<StockTickFromMssql> readFromMSSQLOnly(string code, DateTime date,string sourceServer="corp170")
        {
            var connName = sourceServer;
            var yyyyMM = date.ToString("yyyyMM");
            var yyyyMMdd = date.ToString("yyyyMMdd");
            var codeStr = code.Replace('.', '_');
            var SqlString = String.Format(@"
            SELECT * FROM [WindFullMarket{0}].[dbo].[MarketData_{1}] where tdate={2} and ((ttime>=91500000 and ttime%10000000<=5959999 and ttime%100000<=59999) or (ttime<240000 and ttime>0)) order by tdate,ttime
            ", yyyyMM, codeStr, yyyyMMdd);
            if (date>=new DateTime(2011,8,1))
            {
                return Platforms.container.Resolve<StockDataFromTDBStyleServerRepository>().readFromMSSQLDaily(connName, SqlString);

            }
            else
            {
                return Platforms.container.Resolve<StockDataFromMDBStyleServerRepository>().readFromMSSQLDaily(connName, SqlString);

            }
        }

        protected override List<StockTickFromMssql> readFromWindOnly(string code, DateTime startDate, DateTime endDate, string tag = null, IDictionary<string, object> options = null)
        {
            throw new NotImplementedException();
        }

        protected override void saveToLocalCSV(IList<StockTickFromMssql> data, string code, DateTime date, string tag = null, bool appendMode = false, bool canSaveToday = false)
        {
            throw new NotImplementedException();
        }

        protected override void saveToMSSQLOnly(string targetServer, string dataBase, string tableName, DataTable data, Dictionary<string, string> pair = null)
        {
            Platforms.container.Resolve<StockTickDataToMSSQLRepository>().saveToSQLServer(targetServer, dataBase, tableName, data, pair);
        }

        public void fetchDataFromSQLandModifiedandSaveToSQL(string code, DateTime date,string sourceServer, string targetServer, string dataBase, string tableName, Dictionary<string, string> pair = null)
        {
            var data = DataTableUtils.ToDataTable(TickDataUtils<StockTickFromMssql>.filteringTickData(readFromMSSQLOnly(code, date, sourceServer)));
            saveToMSSQLOnly(targetServer, dataBase, tableName, data, pair);
        }
    }
}
