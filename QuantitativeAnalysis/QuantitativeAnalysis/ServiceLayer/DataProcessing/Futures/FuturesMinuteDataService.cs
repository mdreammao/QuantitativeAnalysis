using Autofac;
using QuantitativeAnalysis.DataAccessLayer.DataFromLocalCSV.Futures;
using QuantitativeAnalysis.DataAccessLayer.DataFromWind.Futures;
using QuantitativeAnalysis.DataAccessLayer.DataToLocalCSV.Futures;
using QuantitativeAnalysis.ModelLayer.Futures;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using QuantitativeAnalysis.ServiceLayer.DataProcessing.Common;
using QuantitativeAnalysis.Utilities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ServiceLayer.DataProcessing.Futures
{
    class FuturesMinuteDataService : SequentialByDayService<FuturesMinute>
    {
        const string PATH_KEY = "CacheData.Path.SequentialByDay";
        protected override List<FuturesMinute> readFromLocalCSVOnly(string code, DateTime date, string tag = null)
        {
            var path = _buildCacheDataFilePath(code, date, tag);
            return Platforms.container.Resolve<FuturesMinuteFromLocalCSVRepository>().readFromLocalCSV(path);
        }

        protected override List<FuturesMinute> readFromWindOnly(string code, DateTime dateStart, DateTime dateEnd, string tag = null, IDictionary<string, object> options = null)
        {
            return Platforms.container.Resolve<FuturesMinuteFromWindRepository>().readFromWind(code, dateStart, dateEnd, tag, options);
        }

        protected override List<FuturesMinute> readFromMSSQLOnly(string code, DateTime date)
        {
            throw new NotImplementedException();
        }

        private static string _buildCacheDataFilePath(string code, DateTime date, string tag)
        {
            if (tag == null) tag = typeof(FuturesMinute).ToString();
            return FileUtils.GetCacheDataFilePath(PATH_KEY, new Dictionary<string, string>
            {
                ["{tag}"] = tag,
                ["{code}"] = code,
                ["{date}"] = date.ToString("yyyyMMdd")
            });
        }

        protected override void saveToLocalCSV(IList<FuturesMinute> data, string code, DateTime date, string tag = null, bool appendMode = false, bool canSaveToday = false)
        {
            Platforms.container.Resolve<FuturesMinuteToLocalCSVRepository>().saveToLocalCsv(data, code, date, tag);
        }
    }
}
