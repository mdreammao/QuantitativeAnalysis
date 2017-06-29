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

namespace QuantitativeAnalysis.ServiceLayer.DataProcessing.Option
{
    class OptionTickDataDailyService : SequentialByDayService<OptionTickFromMssql>
    {
        protected override List<OptionTickFromMssql> readFromLocalCSVOnly(string code, DateTime date, string tag = null)
        {
            throw new NotImplementedException();
        }

        protected override List<OptionTickFromMssql> readFromMSSQLOnly(string code, DateTime date,string sourceServer)
        {
            var connName = "local";
            var yyyyMM = date.ToString("yyyyMM");
            var yyyyMMdd = date.ToString("yyyyMMdd");
            var codeStr = code.Replace('.', '_');
            var SqlString = String.Format(@"
            SELECT * FROM [OptionTickData].[dbo].[MarketData_{0}] where tdate={1} and ttime>=91500000 order by tdate,ttime
            ", codeStr, yyyyMMdd);
            return Platforms.container.Resolve<OptionDataFromLocalServerRepository>().readFromMSSQLDaily(code, date, connName, SqlString);
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
            throw new NotImplementedException();
        }
    }
}
