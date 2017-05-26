using Autofac;
using QuantitativeAnalysis.DataAccessLayer.DataFromLocalCSV.Option;
using QuantitativeAnalysis.DataAccessLayer.DataFromWind.Option;
using QuantitativeAnalysisQuantitativeAnalysis.Utilities.DataApplication;
using QuantitativeAnalysis.ModelLayer.Option;
using QuantitativeAnalysis.ServiceLayer.MyCore;
using QuantitativeAnalysis.ServiceLayer.DataProcessing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantitativeAnalysis.Utilities.Common;

namespace QuantitativeAnalysis.ServiceLayer.DataProcessing.Option
{
    class OptionInfoService : BasicDataService<OptionInfo>
    {
        protected override List<OptionInfo> readFromLocalCsv(string path)
        {
            return Platforms.container.Resolve<OptionInfoFromLocalCSVRepository>().readFromLocalCSV(path);
        }

        protected override List<OptionInfo> readFromWind(string code,DateTime startDate,DateTime endDate)
        {
            return Platforms.container.Resolve<OptionInfoFromWindRepository>().readFromWindEntirely(code,null,null);
        }

        protected override void saveToLocalCsvFile(IList<OptionInfo> data, string path, bool appendMode = false, string tag = null)
        {
            Platforms.container.Resolve<OptionInfoToLocalCSVRepository>().saveToLocalCsv(path,data,appendMode);
        }
    }
}
