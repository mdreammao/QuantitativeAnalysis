using Autofac;
using QuantitativeAnalysis.DataAccessLayer.DataFromLocalCSV.Option;
using QuantitativeAnalysis.DataAccessLayer.DataFromWind.Option;
using QuantitativeAnalysisQuantitativeAnalysis.Utilities.DataApplication;
using QuantitativeAnalysis.ModelLayer.Option;
using QuantitativeAnalysis.ServiceLayer.DataProcessing.Common;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using QuantitativeAnalysis.Utilities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantitativeAnalysis.ServiceLayer.DataProcessing.Option
{
    class OptionDailyInfoService : InformationByDayService<OptionInfo>
    {
        const string PATH_KEY = "CacheData.Path.SequentialByDay";

        public override List<OptionInfo> readFromLocalCSVOnly(string code, DateTime date, string tag = null)
        {
            string path = _buildCacheDataFilePath(code, date, tag);
            return Platforms.container.Resolve<OptionInfoFromLocalCSVRepository>().readFromLocalCSV(path);
        }

        public override List<OptionInfo> readFromMSSQLOnly(string code, DateTime date)
        {
            throw new NotImplementedException();
        }

        public override List<OptionInfo> readFromWindOnly(string code, DateTime date, string tag = null, IDictionary<string, object> options = null)
        {
            return Platforms.container.Resolve<OptionInfoFromWindRepository>().readFromWind(code, date, date, tag, options);
        }

        public override void saveToLocalCSV(IList<OptionInfo> data, string code, DateTime date, string tag = null, bool appendMode = false, bool canSaveToday = false)
        {
            string path = _buildCacheDataFilePath(code, date, tag);
            Platforms.container.Resolve<OptionInfoToLocalCSVRepository>().saveToLocalCsv(path,data,appendMode);
        }


        private static string _buildCacheDataFilePath(string code, DateTime date, string tag)
        {
            if (tag == null) tag = typeof(OptionInfo).ToString();
            return FileUtils.GetCacheDataFilePath(PATH_KEY, new Dictionary<string, string>
            {
                ["{tag}"] = tag,
                ["{code}"] = code,
                ["{date}"] = date.ToString("yyyyMMdd")
            });
        }

    }
}
