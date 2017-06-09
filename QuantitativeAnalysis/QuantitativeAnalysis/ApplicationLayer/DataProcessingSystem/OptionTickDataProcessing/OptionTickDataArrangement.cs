using Autofac;
using QuantitativeAnalysis.ServiceLayer.DataProcessing.Option;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using QuantitativeAnalysis.Utilities.Common;
using QuantitativeAnalysis.Utilities.DataApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ApplicationLayer.DataProcessingSystem.OptionTickDataProcessing
{
    public class OptionTickDataArrangement
    {
        public OptionTickDataArrangement()
        {
            var test=Platforms.container.Resolve<OptionTickDataDailyService>().fetchFromMssql("10000001.SH", Kit.ToDate(20150209));
            test = OptionTickDataUtils.filteringTickData(test);

            //var test2 = Platforms.container.Resolve<OptionTickDataDailyService>().fetchFromMssql("10000730.SH", Kit.ToDate(20170209));
            string database = "test";
            string tablename = "test0";
            string connectString = "server=(local);database=;uid =sa;pwd=maoheng0;";
            string todayConnectString = "server=(local);database=" + database + ";uid =sa;pwd=maoheng0;";
            string connStr= "server=(local);database=" + database + "table="+tablename+";uid =sa;pwd=maoheng0;";
            // MSSQLUtils.CreateDataBase(database, connectString);
            MSSQLUtils.CreateTable(tablename, todayConnectString);
            MSSQLUtils.BulkToDB(todayConnectString,DataTableUtils.ToDataTable(test),tablename);
        }
    }
}
