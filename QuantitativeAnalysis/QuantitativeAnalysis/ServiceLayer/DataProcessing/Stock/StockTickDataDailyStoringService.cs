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

namespace QuantitativeAnalysis.ServiceLayer.DataProcessing.Stock
{
    class StockTickDataDailyStoringService : SequentialByDayService<StockTickFromMssql>
    {
        protected override List<StockTickFromMssql> readFromLocalCSVOnly(string code, DateTime date, string tag = null)
        {
            throw new NotImplementedException();
        }

        protected override List<StockTickFromMssql> readFromMSSQLOnly(string code, DateTime date)
        {
            var connName = "corp170";
            var yyyyMM = date.ToString("yyyyMM");
            var yyyyMMdd = date.ToString("yyyyMMdd");
            var codeStr = code.Replace('.', '_');
            var SqlString = String.Format(@"
            SELECT * FROM [WindFullMarket{0}].[dbo].[MarketData_{1}] where tdate={2} and ttime>=91500000 and ttime%10000000<=5959999 and ttime%100000<=59999 order by tdate,ttime
            ", yyyyMM, codeStr, yyyyMMdd);
            connName = "corp217";
            SqlString = String.Format(@"
            SELECT * FROM [TradeMarket{0}].[dbo].[MarketData_{1}] where tdate={2} and ttime>=91500000 and ttime%10000000<=5959999 and ttime%100000<=59999 order by tdate,ttime
            ", yyyyMM, codeStr, yyyyMMdd);
            return Platforms.container.Resolve<StockDataFrom170ServerRepository>().readFromMSSQLDaily(code, date, connName, SqlString);
        }

        protected override List<StockTickFromMssql> readFromWindOnly(string code, DateTime startDate, DateTime endDate, string tag = null, IDictionary<string, object> options = null)
        {
            throw new NotImplementedException();
        }

        protected override void saveToLocalCSV(IList<StockTickFromMssql> data, string code, DateTime date, string tag = null, bool appendMode = false, bool canSaveToday = false)
        {
            throw new NotImplementedException();
        }
    }
}
