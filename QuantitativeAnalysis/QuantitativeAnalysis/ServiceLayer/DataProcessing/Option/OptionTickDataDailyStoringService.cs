using Autofac;
using QuantitativeAnalysis.DataAccessLayer.DataFromMSSQL.Option;
using QuantitativeAnalysis.ModelLayer.Option;
using QuantitativeAnalysis.ServiceLayer.DataProcessing.Common;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using QuantitativeAnalysis.DataAccessLayer.DateToMSSQL.stock;
using QuantitativeAnalysis.Utilities.Common;
using QuantitativeAnalysis.Utilities.DataApplication;

namespace QuantitativeAnalysis.ServiceLayer.DataProcessing.Option
{
    class OptionTickDataDailyStoringService : SequentialByDayService<OptionTickFromMssql>
    {
        protected override List<OptionTickFromMssql> readFromLocalCSVOnly(string code, DateTime date, string tag = null)
        {
            throw new NotImplementedException();
        }

        protected override List<OptionTickFromMssql> readFromMSSQLOnly(string code, DateTime date,string sourceServer="corp170")
        {
            var connName = "corp170";
            var yyyyMM = date.ToString("yyyyMM");
            var yyyyMMdd = date.ToString("yyyyMMdd");
            var codeStr = code.Replace('.', '_');
            var SqlString = String.Format(@"
            SELECT * FROM [WindFullMarket{0}].[dbo].[MarketData_{1}] where tdate={2} and ttime>=91500000 and ttime%10000000<=5959999 and ttime%100000<=59999 order by tdate,ttime
            ", yyyyMM, codeStr, yyyyMMdd);
            if (Convert.ToInt32(yyyyMM) <= 201512)
            {
                connName = "corp217";
                SqlString = String.Format(@"
            SELECT * FROM [TradeMarket{0}].[dbo].[MarketData_{1}] where tdate={2} and ttime>=91500000 and ttime%10000000<=5959999 and ttime%100000<=59999 order by tdate,ttime
            ", yyyyMM, codeStr, yyyyMMdd);
            }
            return Platforms.container.Resolve<OptionDataFrom170ServerRepository>().readFromMSSQLDaily(code, date, connName, SqlString);
        }

        protected override List<OptionTickFromMssql> readFromWindOnly(string code, DateTime startDate, DateTime endDate, string tag = null, IDictionary<string, object> options = null)
        {
            throw new NotImplementedException();
        }

        protected override void saveToLocalCSV(IList<OptionTickFromMssql> data, string code, DateTime date, string tag = null, bool appendMode = false, bool canSaveToday = false)
        {
            throw new NotImplementedException();
        }

        protected override void saveToMSSQLOnly(string targetServer, string dataBase, string tableName, DataTable data, Dictionary<string, string> pair = null)
        {
            Platforms.container.Resolve<StockTickDataToMSSQLRepository>().saveToSQLServer(targetServer, dataBase, tableName, data, pair);
        }

        public void fetchDataFromSQLandModifiedandSaveToSQL(string code, DateTime date, string sourceServer, string targetServer, string dataBase, string tableName, Dictionary<string, string> pair = null)
        {
            var data = DataTableUtils.ToDataTable(TickDataUtils<OptionTickFromMssql>.filteringTickData(readFromMSSQLOnly(code, date, sourceServer)));
            saveToMSSQLOnly(targetServer, dataBase, tableName, data, pair);
        }
    }
}
