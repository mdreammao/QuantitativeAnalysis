using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using ZedGraph;
using QuantitativeAnalysis.Utilities.Common;

namespace QuantitativeAnalysis.DataAccessLayer.DataToLocalCSV.Common
{
    public class RecordUtil
    {
        public static void recordToCsv<T>(IList<T> data, string tag, string type, string parameters = "", string performance = "")
        {
            var fullPath = ConfigurationManager.AppSettings["RootPath"] +ConfigurationManager.AppSettings["CacheData.ResultPath"] + ConfigurationManager.AppSettings["CacheData.StrategyPath"];
            var dateStr = Kit.ToInt_yyyyMMdd(DateTime.Now).ToString();
            fullPath = ResultPathUtil.GetLocalPath(fullPath, tag, dateStr, type, parameters, performance);
            var dt = DataTableUtils.ToDataTable(data);
            CsvFileUtils.WriteToCsvFile(fullPath, dt);
        }

        public static void recordToPng(ZedGraphControl zedG, string path = "")
        {
            var fullPath = path;
            zedG.GetImage().Save(fullPath);
        }
    }
}
